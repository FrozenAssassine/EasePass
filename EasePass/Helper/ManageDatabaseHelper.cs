using EasePass.Dialogs;
using EasePass.Models;
using System.Threading.Tasks;

namespace EasePass.Helper;

internal class ManageDatabaseHelper
{
    public static async Task<Database> ImportDatabase()
    {
        var pickerResult = await FilePickerHelper.PickOpenFile(new string[] { ".epdb", ".eped" });
        if (!pickerResult.success)
            return null;

        Database db;
        var password = (await new EnterPasswordDialog().ShowAsync()).Password;
        if (password == null)
        {
            InfoMessages.ImportDBWrongPassword();
            return null;
        }

        db = new Database(pickerResult.path);
        if (db.ValidatePwAndLoadDB(password) != PasswordValidationResult.Success)
            return null;

        Database.AddDatabasePath(db.Path);
        return db;
    }

    public static async Task<bool> ImportIntoDatabase()
    {
        var pickerResult = await FilePickerHelper.PickOpenFile(new string[] { ".epdb", ".eped" });
        if (!pickerResult.success)
            return false;

        var password = (await new EnterPasswordDialog().ShowAsync()).Password;
        if (password == null)
        {
            InfoMessages.ImportDBWrongPassword();
            return false;
        }

        var importedItems = Database.GetItemsFromDatabase(pickerResult.path, password);

        var dialog = new ImportPasswordsDialog();
        dialog.SetPagePasswords(importedItems);

        var importItemsResult = await dialog.ShowAsync(false);
        if (importItemsResult.Items == null)
            return false;

        if (importItemsResult.Override)
        {
            Database.LoadedInstance.Items.Clear();
        }

        Database.LoadedInstance.AddRange(importItemsResult.Items);
        return true;
    }

    public static async Task<Database> CreateDatabase()
    {
        Database newDB = await new CreateDatabaseDialog().ShowAsync();
        if (newDB == null)
            return null;

        Database.AddDatabasePath(newDB.Path);
        return newDB;
    }
}
