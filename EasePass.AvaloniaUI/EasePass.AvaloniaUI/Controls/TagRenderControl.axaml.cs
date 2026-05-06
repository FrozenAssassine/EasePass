using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;
using Avalonia.Layout;

namespace EasePass.Controls
{
    public partial class TagRenderControl : UserControl, INotifyPropertyChanged
    {
        public TagRenderControl()
        {
            InitializeComponent();
        }

        public static readonly StyledProperty<string[]> TagsProperty =
            AvaloniaProperty.Register<TagRenderControl, string[]>(nameof(Tags));

        public string[] Tags
        {
            get => GetValue(TagsProperty);
            set => SetValue(TagsProperty, value);
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
             base.OnPropertyChanged(change);
             if (change.Property == TagsProperty)
             {
                 UpdateTagsUI();
             }
        }
        
        public new event PropertyChangedEventHandler? PropertyChanged;
        private void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void UpdateTagsUI()
        {
            var tagRenderer = this.FindControl<StackPanel>("tagRenderer");
            if (tagRenderer == null) return;
            
            tagRenderer.Children.Clear();

            if (Tags == null || Tags.Length == 0)
                return;

            foreach (var tag in Tags)
            {
                using MD5 md5Hasher = MD5.Create();
                byte[] bytes = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(tag));
                var tagColor = new SolidColorBrush(Color.FromArgb(255, bytes[1], bytes[0], bytes[2]));

                var border = new Border
                {
                    BorderBrush = tagColor,
                    CornerRadius = new CornerRadius(5),
                    BorderThickness = new Thickness(1.5),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(3, 0, 3, 0),
                    Padding = new Thickness(4,1,4,1)
                };
                
                var tagElement = new TextBlock
                {
                    Text = tag,
                    Foreground = Brushes.White,
                    FontSize = 12,
                    VerticalAlignment = VerticalAlignment.Center
                };
                border.Child = tagElement;
                tagRenderer.Children.Add(border);
            }
        }
    }
}
