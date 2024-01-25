using EasePass.Extensions;
using Microsoft.UI.Xaml.Controls;

namespace EasePass.Views.DialogPages
{
    public sealed partial class EnterPasswordPage : Page
    {
        public EnterPasswordPage()
        {
            this.InitializeComponent();
        }

        public string GetPassword()
        {
            return passwordbox.Text;
        }
    }
}
