using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace EasePass.Views.DialogViews;

public partial class EnterPasswordPage : UserControl
{
    public EnterPasswordPage()
    {
        InitializeComponent();
    }

    public string GetPassword()
    {
        return passwordbox.Text;
    }
}