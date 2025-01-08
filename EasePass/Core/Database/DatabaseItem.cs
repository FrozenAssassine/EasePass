using EasePass.Dialogs;
using EasePass.Helper;
using EasePass.Models;
using EasePass.Settings;
using Microsoft.UI.Xaml.Documents;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace EasePass.Core.Database
{
    public class DatabaseItem : IDisposable, INotifyPropertyChanged
    {
        public string Name => MakeDatabaseName();
        public string Path = "";
        public SecureString MasterPassword = null;
        public ObservableCollection<PasswordManagerItem> Items = null;
        public DateTime LastModified => File.GetLastWriteTime(Path);
        public string LastModifiedStr => Name; //LastModified.ToString("D");

        public bool IsTemporaryDatabase { get; set; } = false;

        public event PropertyChangedEventHandler PropertyChanged;

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

        private string MakeDatabaseName()
        {
            var name = System.IO.Path.GetFileNameWithoutExtension(Path);

            if (IsTemporaryDatabase)
                name += " (Temp)";

            return name;
        }

        private void CallPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }


        public void LoadedInstanceChanged()
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Path"));
                PropertyChanged(this, new PropertyChangedEventArgs("Name"));
                PropertyChanged(this, new PropertyChangedEventArgs("MasterPassword"));
            }
        }

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            CallPropertyChanged("Items");
        }


        public bool Unlock(SecureString password, bool showWrongPasswordError = true)
        {
            var res = CheckPasswordCorrect(password, showWrongPasswordError);

            if (res.result == PasswordValidationResult.DatabaseNotFound)
            {
                InfoMessages.DatabaseFileNotFoundAt(Path);
                return false;
            }

            if (res.result == PasswordValidationResult.WrongPassword)
                return false;

            MasterPassword = password;

            OldDatabaseImporter.CheckAndFixFile(this);

            Items = Database.LoadItems(res.decryptedData);
            ClearOldClicksCache();

            CallPropertyChanged("Items");
            CallPropertyChanged("MasterPassword");

            return true;
        }

        public bool Load(SecureString password, bool showWrongPasswordError = true)
        {
            Unlock(password, showWrongPasswordError);

            Database.LoadedInstance = this;
            return true;
        }

        public (PasswordValidationResult result, string decryptedData) CheckPasswordCorrect(SecureString enteredPassword, bool showWrongPasswordError = false)
        {
            if (enteredPassword == null)
                return (PasswordValidationResult.WrongPassword, null);

            if (!File.Exists(Path))
                return (PasswordValidationResult.DatabaseNotFound, null);

            var oldImporterRes = OldDatabaseImporter.CheckValidPassword(this.Path, enteredPassword, showWrongPasswordError);
            if (oldImporterRes.result == PasswordValidationResult.Success)
                return oldImporterRes;

            var (data, success) = Database.ReadFile(Path, enteredPassword, showWrongPasswordError);
            if (success)
                return (PasswordValidationResult.Success, data);
            
            return (PasswordValidationResult.WrongPassword, null);
        }

        public void Save(string path = null)
        {
            var data = Database.CreateJsonstring(Items);
            Database.WriteFile(path ?? Path, data, MasterPassword);
        }

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

        public void DeleteItem(PasswordManagerItem item)
        {
            Items.Remove(item);
            CallPropertyChanged("Items");
        }

        public void AddItem(PasswordManagerItem item)
        {
            Items.Add(item);
            CallPropertyChanged("Items");
        }

        public int PasswordAlreadyExists(string password)
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

        public void AddRange(PasswordManagerItem[] items)
        {
            foreach (var item in items)
            {
                Items.Add(item);
            }

            CallPropertyChanged("Items");
        }

        public void AddRange(ObservableCollection<PasswordManagerItem> items)
        {
            foreach (var item in items)
            {
                Items.Add(item);
            }

            CallPropertyChanged("Items");
        }

        public void SetNewPasswords(ObservableCollection<PasswordManagerItem> items)
        {
            Items.Clear();

            foreach (var item in items)
            {
                Items.Add(item);
            }

            CallPropertyChanged("Items");
        }
        public void SetNewPasswords(PasswordManagerItem[] items)
        {
            if (items == null)
                return;

            Items.Clear();

            foreach (var item in items)
            {
                Items.Add(item);
            }

            CallPropertyChanged("Items");
        }

    }
}
