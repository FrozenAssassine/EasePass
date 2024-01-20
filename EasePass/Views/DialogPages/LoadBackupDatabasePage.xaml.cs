using EasePass.Models;
using Microsoft.UI.Xaml.Controls;

namespace EasePass.Views
{
    public sealed partial class LoadBackupDatabasePage : Page
    {
        private ContentDialog pageHost;
        public DatabaseBackupFile selectedFile = null;
        public LoadBackupDatabasePage(ContentDialog dialog)
        {
            this.InitializeComponent();
            LoadBackupsFromFile();

            this.pageHost = dialog;
        }

        private async void LoadBackupsFromFile()
        {
            var backupFiles = await MainWindow.databaseBackupHelper.GetAllBackupFiles();

            foreach (var backupFile in backupFiles)
            {
                backupDisplay.Items.Add(new DatabaseBackupFile(backupFile));
            }
        }

        private void backupDisplay_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (backupDisplay.SelectedItem == null)
            {
                selectedFile = null;
                pageHost.PrimaryButtonText = "";
                return;
            }

            selectedFile = backupDisplay.SelectedItem as DatabaseBackupFile;
            pageHost.PrimaryButtonText = "Load";
        }
    }
}
