
using Microsoft.UI.Xaml.Controls;

namespace EasePass.Controls
{
    public sealed partial class SettingsItemSeparator : UserControl
    {
        public SettingsItemSeparator()
        {
            this.InitializeComponent();
        }

        public new object Content { get; set; }

        public string Header { get; set; }
    }
}
