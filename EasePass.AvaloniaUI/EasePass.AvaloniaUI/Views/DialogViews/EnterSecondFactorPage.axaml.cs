using Avalonia.Controls;

namespace EasePass.Views.DialogViews;

public partial class EnterSecondFactorPage : UserControl
{
    public EnterSecondFactorPage()
    {
        InitializeComponent();
    }
    public string GetPassword()
    {
        return tokenBox.Text ?? string.Empty;
    }
}