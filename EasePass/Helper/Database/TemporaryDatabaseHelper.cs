using EasePass.Core.Database;
using EasePass.Models;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace EasePass.Helper.Database;

internal class TemporaryDatabaseHelper
{
    //handle databases opend by file association

    private static DatabaseItem CreateTempDatabase(string path)
    {
        DatabaseItem db = new DatabaseItem(new NativeDatabaseSource(path));
        db.IsTemporaryDatabase = true;
        return db;
    }

    public static void HandleImportTempDatabase(NavigationEventArgs e, ComboBox databasebox)
    {
        if (e.Parameter is string path)
        {
            if (path == null || path.Length == 0)
                return;

            var tempDB = CreateTempDatabase(path);
            databasebox.Items.Add(tempDB);
            databasebox.SelectedItem = tempDB;
        }
    }

    public static void LoadImportedDatabase()
    {
        Core.Database.Database.LoadedInstance.IsTemporaryDatabase = false;
        if (Core.Database.Database.LoadedInstance.DatabaseSource is NativeDatabaseSource nds)
            Core.Database.Database.AddDatabasePath(nds.Path);
    }

    public static void ShowTempDBButton(Button button)
    {
        button.Visibility = ConvertHelper.BoolToVisibility(Core.Database.Database.LoadedInstance.IsTemporaryDatabase);
    }
}
