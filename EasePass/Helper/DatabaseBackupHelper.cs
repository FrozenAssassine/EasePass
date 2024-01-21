using EasePass.Core;
using EasePass.Dialogs;
using EasePass.Models;
using EasePass.Settings;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Security;
using System.Threading.Tasks;
using Windows.Storage;

/*This is not the AutomaticBackupHelper. This is for a database backup which WILL happen. You can not turn it off.*/

namespace EasePass.Helper;

public enum BackupCycle
{
    Weekly, Daily
}

public class DatabaseBackupHelper
{
    private readonly PasswordItemsManager pwItemsManager;
    private SecureString password;
    private BackupCycle backupCycle;

    public DatabaseBackupHelper(PasswordItemsManager pwItemsManager, SecureString pw, BackupCycle backupCycle)
    {
        this.pwItemsManager = pwItemsManager;
        this.password = pw;
        this.backupCycle = backupCycle;
    }

    private int CurrentDay => DateTime.Now.DayOfYear;
    private async Task<StorageFolder> GetBackupFolder()
    {
        try
        {
            return await ApplicationData.Current.LocalFolder.CreateFolderAsync("Backups", CreationCollisionOption.OpenIfExists);
        }
        catch (Exception ex)
        {
            InfoMessages.CouldNotCreateDatabaseBackupFolder(ex);
            return null;
        }
    }


    public async Task<bool> CheckAndDoBackup()
    {
        int lastBackupDay = AppSettings.GetSettingsAsInt(AppSettingsValues.lastBackupDay, 0);

        //still same day or same week no backup needed:
        if ((backupCycle == BackupCycle.Weekly && lastBackupDay + 7 != CurrentDay) ||
            (backupCycle == BackupCycle.Daily && lastBackupDay == CurrentDay))
        {
            return true;
        }

        //create the backup with the current date in the name:
        StorageFolder folder = await GetBackupFolder();
        if (folder == null)
            return false;

        var backupPath = folder.Path + "\\" + DateTime.Now.ToString("dd_MM_yyyy") + "_Backup.epdb";
        DatabaseHelper.SaveDatabase(pwItemsManager, password, backupPath);
        AppSettings.SaveSettings(AppSettingsValues.lastBackupDay, CurrentDay);

        return true;
    }
    
    public async Task<string[]> GetAllBackupFiles()
    {
        var backupFolder = await GetBackupFolder();
        try
        {
            return Directory.GetFiles(backupFolder.Path, "*.epdb");
        }
        catch
        {
            //TODO: better error handling
            return null;
        }
    }

    public async Task<bool> LoadBackupFromFile(string path)
    {
        var loadedItems = DatabaseHelper.LoadDatabase(password, path);
        if (loadedItems == null)
            return false;

        ImportPasswordsDialog dialog = new ImportPasswordsDialog();
        dialog.SetPagePasswords(loadedItems);

        var res = await dialog.ShowAsync(false);
        if (res.Items == null)
            return false;
        
        if (res.Override)
        {
            pwItemsManager.PasswordItems.Clear();
        }

        pwItemsManager.AddRange(res.Items);
        return true;
    }
}
