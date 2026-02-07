using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace EasePass.Controls
{
    public partial class InfoBarTemplate : UserControl
    {
        public InfoBarTemplate()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
