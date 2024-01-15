
using Microsoft.UI.Xaml.Controls;

namespace EasePass.Controls
{
    public sealed partial class SettingsItemSeparator : UserControl
    {
        public SettingsItemSeparator()
        {
            this.InitializeComponent();
        }

        public string Header { get; set; }
    }
}
