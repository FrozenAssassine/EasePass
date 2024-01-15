using EasePass.Helper;
using EasePass.Models;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace EasePass.Views
{
    public sealed partial class GenPasswordPage : Page
    {
        public GenPasswordPage(ObservableCollection<PasswordManagerItem> items)
        {
            this.InitializeComponent();
            safetyChart.SetPasswordItems(items);
        }

        public async void GeneratePassword()
        {
            await _GeneratePassword();
            safetyChart.EvaluatePassword(passwordTB.Password);
            progressBar.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
        }

        public async Task _GeneratePassword()
        {
            passwordTB.Password = await PasswordHelper.GeneratePassword();
        }
    }
}
