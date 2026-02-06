using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using EasePass.ViewModels;

namespace EasePass.Views
{
    public partial class LoginPage : UserControl
    {
        public LoginPage()
        {
            InitializeComponent();
            DataContext = new LoginViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
            var pwBox = this.FindControl<TextBox>("passwordBox");
            if (pwBox != null)
            {
               pwBox.Focus();
            }
        }
    }
}
