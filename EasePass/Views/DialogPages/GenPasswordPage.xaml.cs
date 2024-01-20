using EasePass.Core;
using EasePass.Helper;
using Microsoft.UI.Xaml.Controls;
using System.Threading.Tasks;

namespace EasePass.Views
{
    public sealed partial class GenPasswordPage : Page
    {
        public GenPasswordPage(PasswordItemsManager pwItemsManager)
        {
            this.InitializeComponent();
            safetyChart.SetPasswordItems(pwItemsManager);
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
