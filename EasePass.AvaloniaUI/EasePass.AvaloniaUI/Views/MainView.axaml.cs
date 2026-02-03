using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using EasePass.AvaloniaUI.Core;
using EasePass.AvaloniaUI.ViewModels;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

namespace EasePass.AvaloniaUI.Views
{
    public partial class MainView : UserControl
    {

        public MainView()
        {
            InitializeComponent();
        }

        private static Window GetMainWindow()
        {
            return (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow!;
        }
        public async Task<string?> ShowOpenFileDialogAsync(string title, string[] filters)
        {
            var options = new FilePickerOpenOptions
            {
                Title = title,
                AllowMultiple = false,
                FileTypeFilter = filters.Select(f => new FilePickerFileType(f)).ToList()
            };

            var window = GetMainWindow();
            var files = await window.StorageProvider.OpenFilePickerAsync(options);
            return files.FirstOrDefault()?.Path.LocalPath;
        }

        private async void Login_Click(object? sender, RoutedEventArgs e)
        {
            SecureString pw = PasswordBox.Text?.ConvertToSecureString();
            if (pw == null)
                return;

            string path = await ShowOpenFileDialogAsync("pick db", ["*.epdb"]);
            if (path == null)
                return;

            var db = new DatabaseItem(path);

            var res = await db.CheckPasswordCorrect(pw);
            if (res.result == PasswordValidationResult.Success)
            {
                db.Load(pw, res.database);
                Debug.WriteLine("Found: " + db.Items.Count);
                if (DataContext is MainViewModel vm)
                {
                    vm.PasswordItems = db.Items;
                }
            }


        }

        private void PasswordBox_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Login_Click(null, null);
        }

    }
}