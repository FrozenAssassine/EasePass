using EasePass.Core.Database.Format.Serialization;
using EasePass.Dialogs;
using EasePass.Helper.Database;
using EasePass.Models;
using EasePass.Settings;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

namespace EasePass.Core.Database
{
    public class DatabaseItem : IDisposable, INotifyPropertyChanged
    {
        #region Properties
        /// <summary>
        /// The Name of the Database
        /// </summary>
        public string Name => MakeDatabaseName();
        /// <summary>
        /// The Path to the Database
        /// </summary>
        public string Path { get; set; } = "";
        /// <summary>
        /// The Password of the Database
        /// </summary>
        public SecureString MasterPassword { get; set; } = null;
        /// <summary>
        /// The SecondFactor Token of the Database
        /// </summary>
        public SecureString SecondFactor { get; set; } = null;
        /// <summary>
        /// The Settings of the Database
        /// </summary>
        public DatabaseSettings Settings { get; set; } = null;
        /// <summary>
        /// The Passworditems of the Database
        /// </summary>
        public ObservableCollection<PasswordManagerItem> Items { get; set; } = null;
        /// <summary>
        /// The Last <see cref="DateTime"/> the Database was accessed by someone
        /// </summary>
        public DateTime LastModified => File.GetLastWriteTime(Path);
        /// <summary>
        /// The Name of the Database ?
        /// </summary>
        public string LastModifiedStr => Name; //LastModified.ToString("D");
        /// <summary>
        /// Specifies if the Database is a temporary Database
        /// </summary>
        public bool IsTemporaryDatabase { get; set; } = false;

        public event PropertyChangedEventHandler PropertyChanged;

        public DeferredSaveHelper deferredSaver { get; } = new DeferredSaveHelper(new TimeSpan(0, 0, DefaultSettingsValues.databaseDeferredSaveTime_Sec));
        #endregion

        #region Constructor
        public DatabaseItem(string path)
        {
            Path = path;
            Items = new ObservableCollection<PasswordManagerItem>();
            Items.CollectionChanged += Items_CollectionChanged;

            CallPropertyChanged("Name");
            CallPropertyChanged("Path");
            CallPropertyChanged("MasterPassword");
            CallPropertyChanged("Items");
        }
        #endregion

        #region AddItem
        public void AddItem(PasswordManagerItem item)
        {
            Items.Add(item);
            CallPropertyChanged("Items");
        }
        #endregion

        #region AddRange
        public void AddRange(PasswordManagerItem[] items)
        {
            foreach (PasswordManagerItem item in items)
            {
                Items.Add(item);
            }

            CallPropertyChanged("Items");
        }

        public void AddRange(ObservableCollection<PasswordManagerItem> items)
        {
            foreach (PasswordManagerItem item in items)
            {
                Items.Add(item);
            }

            CallPropertyChanged("Items");
        }
        #endregion

        #region CallPropertyChanged
        private void CallPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion

        #region CheckPasswordCorrect
        public async Task<(PasswordValidationResult result, DatabaseFile database)> CheckPasswordCorrect(SecureString enteredPassword, bool showWrongPasswordError = false)
        {
            if (enteredPassword == null)
                return (PasswordValidationResult.WrongPassword, null);

            if (!File.Exists(Path))
                return (PasswordValidationResult.DatabaseNotFound, null);

            var result = await DatabaseFormatHelper.Load(Path, enteredPassword, showWrongPasswordError);
            return result;
        }
        #endregion

        #region ClearOldClicksCache
        public void ClearOldClicksCache()
        {
            if (Items == null)
                return;

            for (int i = 0; i < Items.Count; i++)
            {
                for (int j = 0; j < Items[i].Clicks.Count; j++)
                {
                    string[] splitted = Items[i].Clicks[j].Split('.');
                    if (splitted.Length != 3) continue;
                    DateTime date = new DateTime(Convert.ToInt32(splitted[2]), Convert.ToInt32(splitted[1]), Convert.ToInt32(splitted[0]));
                    if (DateTime.Now - date > TimeSpan.FromDays(365))
                    {
                        Items[i].Clicks.RemoveAt(j);
                        j--;
                    }
                }
            }
            CallPropertyChanged("Items");
        }
        #endregion

        #region DeleteItem
        public void DeleteItem(PasswordManagerItem item)
        {
            Items.Remove(item);
            CallPropertyChanged("Items");
        }
        #endregion

        #region Dispose
        public void Dispose()
        {
            if (this == Database.LoadedInstance)
            {
                Items.Clear();
            }
            else
            {
                Items = null;
            }
            MasterPassword = null;
            CallPropertyChanged("Items");
            CallPropertyChanged("MasterPassword");
        }
        #endregion

        #region FindItemsByName
        public ObservableCollection<PasswordManagerItem> FindItemsByName(string name)
        {
            return new ObservableCollection<PasswordManagerItem>(Items.Where(x => x.DisplayName.Contains(name, StringComparison.OrdinalIgnoreCase)));
        }
        public ObservableCollection<PasswordManagerItem> FindItemsByTag(string tag)
        {
            return new ObservableCollection<PasswordManagerItem>(
                Items.Where(x => x.Tags != null && x.Tags.Any(t => t.Contains(tag, StringComparison.OrdinalIgnoreCase)))
            );
        }
        #endregion

        #region Items_CollectionChanged
        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            CallPropertyChanged("Items");
        }
        #endregion

        #region Load & Unlock
        /// <summary>
        /// Loads the <paramref name="database"/> and sets it's <paramref name="password"/>
        /// </summary>
        /// <param name="password">The Password of the Database</param>
        /// <param name="database">The Database, which contains the Settings and the Passwords</param>
        /// <returns>Returns <see langword="true"/> if the <paramref name="database"/> could be loaded</returns>
        private bool LoadInternal(SecureString password, DatabaseFile database)
        {
            if (database == null || password == null)
                return false;

            MasterPassword = password;
            Items = database.Items;
            Settings = database.Settings;
            SecondFactor = database.SecondFactor;
            database.SecondFactor = null;

            ClearOldClicksCache();

            CallPropertyChanged("Items");
            CallPropertyChanged("MasterPassword");

            return true;
        }

        /// <summary>
        /// Loads the Database with the given <paramref name="password"/> and sets it as current shown Database
        /// </summary>
        /// <param name="password">The Password of the Database</param>
        /// <param name="showWrongPasswordError">Specifies if a Info Message should be shown if the <paramref name="password"/> is wrong</param>
        /// <returns>Returns <see langword="true"/> if the Database could be loaded, otherwise <see langword="false"/>.</returns>
        public async Task<bool> Load(SecureString password, bool showWrongPasswordError = true)
        {
            bool result = await Unlock(password, showWrongPasswordError);
            if (result)
            {
                // Set the LoadedInstance only if the Password was correct
                Database.LoadedInstance = this;
            }
            return result;
        }
        /// <summary>
        /// Sets the Database with the given <paramref name="database"/>, the <paramref name="password"/> and sets it as current shown Database
        /// </summary>
        /// <param name="password">The Password of the Database</param>
        /// <param name="database">The Database, which should be set</param>
        /// <returns>Returns <see langword="true"/> if the Database could be set, otherwise <see langword="false"/>.</returns>
        public bool Load(SecureString password, DatabaseFile database)
        {
            if (!LoadInternal(password, database))
                return false;

            // Set the LoadedInstance only if the Password was correct
            Database.LoadedInstance = this;

            return true;
        }

        /// <summary>
        /// Unlocks the Database with the given <paramref name="password"/>
        /// </summary>
        /// <param name="password">The Password of the Database</param>
        /// <param name="showWrongPasswordError">Specifies if a Info Messae should be shown if the <paramref name="password"/> is wrong</param>
        /// <returns>Returns <see langword="true"/> if the Database could be unlocked, otherwise <see langword="false"/>.</returns>
        public async Task<bool> Unlock(SecureString password, bool showWrongPasswordError = true)
        {
            var result = await CheckPasswordCorrect(password, showWrongPasswordError);
            if (result.result == PasswordValidationResult.DatabaseNotFound)
            {
                InfoMessages.DatabaseFileNotFoundAt(Path);
                return false;
            }

            return result.result != PasswordValidationResult.WrongPassword
                && result.result != PasswordValidationResult.WrongFormat
                && result.result != PasswordValidationResult.WrongPassword
                && LoadInternal(password, result.database);
        }
        #endregion

        #region LoadedInstanceChanged
        public void LoadedInstanceChanged()
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Path"));
                PropertyChanged(this, new PropertyChangedEventArgs("Name"));
                PropertyChanged(this, new PropertyChangedEventArgs("MasterPassword"));
            }
        }
        #endregion

        #region MakeDatabaseName
        /// <summary>
        /// Creates the Name for the Database
        /// </summary>
        /// <returns>Returns the new created Name for the Database</returns>
        private string MakeDatabaseName()
        {
            string name = System.IO.Path.GetFileNameWithoutExtension(Path);
            if (IsTemporaryDatabase)
            {
                name += " (Temp)";
            }
            return name;
        }
        #endregion

        #region Password Occurence & Already Exist 
        /// <summary>
        /// Checks if a given <paramref name="password"/> already exist in the Database
        /// </summary>
        /// <param name="password">The Password, which should be checked</param>
        /// <returns>Returns <see langword="true"/> if the <paramref name="password"/> already exist, otherwise <see langword="false"/> will be returned</returns>
        public bool PasswordAlreadyExists(string password)
        {
            int length = Items.Count;
            for (int i = 0; i < length; i++)
            {
                if (Items[i].Password == password)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Gets the amount of Occurences of the given <paramref name="password"/> in the Database
        /// </summary>
        /// <param name="password">The Password, which occurence should be checked</param>
        /// <returns>Returns the amount of occurences in the Database</returns>
        public int GetPasswordOccurence(string password)
        {
            int count = 0;
            int length = Items.Count;
            for (int i = 0; i < length; i++)
            {
                if (Items[i].Password == password)
                {
                    count++;
                }
            }
            return count;
        }
        #endregion

        #region Save
        /// <summary>
        /// Saves the Database to the <paramref name="path"/>
        /// </summary>
        /// <param name="path">The Path of the Database. If the Path is equal to <see langword="null"/> the <see cref="Path"/> will be used</param>
        /// <returns>Returns <see langword="true"/> if the Database was saved successfully, otherwise <see langword="false"/> will be returned</returns>
        public async Task SaveAsync(string path = null)
        {
            await deferredSaver.RequestSaveAsync(() => ForceSave(path));
        }

        /// <summary>
        /// Immediately executes a save and cancels any pending scheduled saves.
        /// Useful for App Shutdown and some more cases
        /// </summary>
        public async Task<bool> ForceSaveAsync(string path = null)
        {
            deferredSaver.CancelPending();
            return await Task.Run(() => ForceSave(path));
        }

        /// <summary>
        /// Synchronous version of ForceSaveAsync. 
        /// </summary>
        public bool ForceSave(string path = null)
        {
            deferredSaver.CancelPending();
            return SaveDatabase(path);
        }

        private bool SaveDatabase(string path = null)
        {
            path ??= Path;
            if (string.IsNullOrWhiteSpace(path))
                return false;

            return DatabaseFormatHelper.Save(path, MasterPassword, SecondFactor, Settings, Items);
        }
        #endregion

        #region SetNewPasswords
        /// <summary>
        /// Deletes all Passwords in the <see cref="Items"/> and adds the <paramref name="items"/> to the <see cref="Items"/>
        /// </summary>
        /// <param name="items"></param>
        public void SetNewPasswords(ObservableCollection<PasswordManagerItem> items)
        {
            if (items == null)
                return;

            Items.Clear();
            foreach (PasswordManagerItem item in items)
            {
                if (item != null)
                {
                    Items.Add(item);
                }
            }

            CallPropertyChanged("Items");
        }

        /// <summary>
        /// Deletes all Passwords in the <see cref="Items"/> and adds the <paramref name="items"/> to the <see cref="Items"/>
        /// </summary>
        /// <param name="items"></param>
        public void SetNewPasswords(PasswordManagerItem[] items)
        {
            if (items == null)
                return;

            Items.Clear();

            foreach (PasswordManagerItem item in items)
            {
                if (item != null)
                {
                    Items.Add(item);
                }
            }

            CallPropertyChanged("Items");
        }
        #endregion
    }
}