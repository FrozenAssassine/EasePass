using Microsoft.UI.Xaml.Controls;

namespace EasePass.Controls;

public sealed partial class IconButton : Button
{
    public string Glyph { get; set; }
    public new string Content { get; set; }
    public IconButton()
    {
        this.InitializeComponent();
    }
}
