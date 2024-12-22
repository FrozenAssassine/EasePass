using EasePass.Models;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.IO;

namespace EasePass.Helper;

internal class TemporaryDatabaseHelper
{
    //handle databases opend by file association

    private static Database CreateTempDatabase(string path)
    {
        File.WriteAllText("D:\\tempdb.txt", path);
        Database db = new Database(path);
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
}
