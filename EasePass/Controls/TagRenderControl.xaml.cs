using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;
using Windows.UI;

namespace EasePass.Controls;

public sealed partial class TagRenderControl : UserControl, INotifyPropertyChanged
{
    public TagRenderControl()
    {
        this.InitializeComponent();
    }

    private string[] _Tags;
    public string[] Tags
    {
        get => _Tags;
        set
        {
            _Tags = value;
            RaisePropertyChanged(nameof(Tags));
            UpdateTagsUI();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    private void RaisePropertyChanged(string name)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    private void UpdateTagsUI()
    {
        tagRenderer.Children.Clear();

        if (Tags == null || Tags.Length == 0)
            return;

        foreach (var tag in Tags)
        {
            MD5 md5Hasher = MD5.Create();
            byte[] bytes = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(tag));
            var tagColor = new SolidColorBrush(Color.FromArgb(255, bytes[1], bytes[0], bytes[2]));

            var grid = new Grid
            {
                BorderBrush = tagColor,
                CornerRadius = new CornerRadius(5),
                BorderThickness = new Thickness(1.5),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(3, 0, 3, 0),
            };
            var tagElement = new TextBlock
            {
                Text = tag,
                Padding = new Thickness(2,1,2,1),
                Foreground = new SolidColorBrush(Microsoft.UI.Colors.White),
            };
            grid.Children.Add(tagElement);
            tagRenderer.Children.Add(grid);
        }
    }
}
