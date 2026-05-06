using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasePass.Core.Database;
using EasePass.Dialogs;
using EasePass.Helper;
using EasePass.Helper.Database;
using EasePass.Helper.FileSystem;
using EasePass.Helper.Security;
using EasePass.Models;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Security;
using System.Threading.Tasks;

namespace EasePass.ViewModels;

public partial class ManageDatabaseViewModel : ViewModelBase
{
    [ObservableProperty] private ObservableCollection<DatabaseItem> _databases = new();
    [ObservableProperty] private DatabaseItem _selectedDatabase;
    [ObservableProperty] private bool _isEditPanelVisible;

    public ManageDatabaseViewModel()
    {
        LoadDatabases();
    }

    private void LoadDatabases()
    {
        var dbs = Database.GetAllUnloadedDatabases();
        Databases.Clear();
        foreach (var db in dbs)
        {
            if (Database.LoadedInstance != null &&
                (db.DatabaseSource == Database.LoadedInstance.DatabaseSource ||
                 (db.DatabaseSource is NativeDatabaseSource nds1 &&
                  Database.LoadedInstance.DatabaseSource is NativeDatabaseSource nds2 &&
                  nds1.Path == nds2.Path)))
            {
                Databases.Add(Database.LoadedInstance);
            }
            else
            {
                Databases.Add(db);
            }
        }
    }

    partial void OnSelectedDatabaseChanged(DatabaseItem value)
    {
        IsEditPanelVisible = value != null;
    }

    [RelayCommand]
    private async Task SelectDatabase(DatabaseItem db)
    {
        if (db == null) return;

        // If database is not loaded, prompt for password
        if (db.MasterPassword == null && db != Database.LoadedInstance)
        {
            SecureString password = (await new EnterPasswordDialog().ShowAsync()).Password;
            if (password == null)
            {
                SelectedDatabase = null;
                return;
            }

            var res = await db.Unlock(password, true, false);
            if (!res)
            {
                SelectedDatabase = null;
                return;
            }
        }

        SelectedDatabase = db;
    }

    [RelayCommand]
    private async Task CreateDatabase()
    {
        DatabaseItem newDB = await ManageDatabaseHelper.CreateDatabase();
        if (newDB != null)
        {
            Databases.Add(newDB);
            SelectedDatabase = newDB;
        }
    }

    [RelayCommand]
    private async Task ImportDatabase()
    {
        DatabaseItem res = await ManageDatabaseHelper.ImportDatabase();
        if (res != null)
        {
            Databases.Add(res);
        }
    }

    [RelayCommand]
    private async Task ImportIntoDatabase()
    {
        if (SelectedDatabase == null) return;
        await ManageDatabaseHelper.ImportIntoDatabase();
    }

    [RelayCommand]
    private async Task DeleteDatabase()
    {
        if (SelectedDatabase == null) return;

        if (!await new ConfirmDeleteDatabaseDialog().ShowAsync(SelectedDatabase))
            return;

        if (Database.GetAllUnloadedDatabases().Length == 1)
        {
            InfoMessages.CantDeleteDatabase();
            return;
        }

        if (Database.LoadedInstance == SelectedDatabase)
        {
            InfoMessages.CantDeleteLoadedDatabase();
            return;
        }

        if (SelectedDatabase.DatabaseSource is not NativeDatabaseSource nds)
        {
            InfoMessages.CantDeleteRemoteDatabase();
            return;
        }

        File.Delete(nds.Path);
        Database.RemoveDatabasePath(nds.Path);
        Databases.Remove(SelectedDatabase);
        SelectedDatabase = null;
    }

    [RelayCommand]
    private async Task ExportDatabase()
    {
        if (SelectedDatabase == null) return;
        await ExportPasswordsHelper.Export(SelectedDatabase, Database.LoadedInstance.Items);
    }

    [RelayCommand]
    private async Task ExportDatabaseDiffPW()
    {
        if (SelectedDatabase == null) return;

        var dialog = await new EnterPasswordDialog().ShowAsync();
        if (dialog.Password == null) return;

        await ExportPasswordsHelper.Export(SelectedDatabase, Database.LoadedInstance.Items, dialog.Password);
    }

    [RelayCommand]
    private void LoadDatabase()
    {
        if (SelectedDatabase == null) return;
        if (SelectedDatabase == Database.LoadedInstance) return;

        Database.LoadedInstance.Dispose();
        Database.LoadedInstance = SelectedDatabase;
        Database.LoadedInstance.DatabaseSource.Login();
        InfoMessages.DatabaseLoaded();
    }

    [RelayCommand]
    private async Task ChangePassword()
    {
        if (SelectedDatabase == null) return;
        await new ChangePasswordDialog().ShowAsync(SelectedDatabase);
    }

    [RelayCommand]
    private async Task CopyDatabasePath()
    {
        if (SelectedDatabase?.DatabaseSource is NativeDatabaseSource nds)
            await ClipboardHelper.CopyAsync(nds.Path);
    }

    [RelayCommand]
    private async Task ManageSecondFactor()
    {
        if (SelectedDatabase == null) return;

        var sfPage = await new ManageSecondFactorDialog().ShowAsync(
            SelectedDatabase.Name, SelectedDatabase.Settings, SelectedDatabase.SecondFactor);

        if (sfPage.Result)
        {
            if (!SelectedDatabase.Settings.UseSecondFactor &&
                sfPage.Settings.UseSecondFactor &&
                !await new AddSecondFactorConfirmationDialog().ShowAsync(SelectedDatabase.Name))
            {
                return;
            }
            SelectedDatabase.Settings = sfPage.Settings;
            SelectedDatabase.SecondFactor = sfPage.Token;
            await SelectedDatabase.ForceSaveAsync();
        }
    }
}
