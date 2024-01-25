using EasePass.Helper;
using EasePass.Models;
using Microsoft.UI.Xaml.Controls;
using System.Threading.Tasks;

namespace EasePass.Views
{
    public sealed partial class GenPasswordPage : Page
    {
        public GenPasswordPage(Database database)
        {
            this.InitializeComponent();
            safetyChart.SetPasswordItems(database);
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
