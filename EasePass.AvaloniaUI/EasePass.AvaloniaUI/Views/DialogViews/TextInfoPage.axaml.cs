using Avalonia.Controls;

namespace EasePass.Views.DialogViews;

public partial class TextInfoPage : UserControl
{
    public TextInfoPage()
    {
        InitializeComponent();
    }

    public TextInfoPage(string text) : this()
    {
        infoText.Text = text;
    }
}
