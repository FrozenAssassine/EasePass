using Avalonia.Controls;
using EasePass.Models;

namespace EasePass.Views.DialogViews;

public partial class Add2FAPage : UserControl
{
    private readonly PasswordManagerItem _item;

    public Add2FAPage()
    {
        InitializeComponent();
    }

    public Add2FAPage(PasswordManagerItem item) : this()
    {
        _item = item;

        if (!string.IsNullOrEmpty(item.Secret))
            secretBox.Text = item.Secret;
        if (!string.IsNullOrEmpty(item.Digits))
            digitsBox.Text = item.Digits;
        if (!string.IsNullOrEmpty(item.Interval))
            intervalBox.Text = item.Interval;
        if (!string.IsNullOrEmpty(item.Algorithm))
        {
            for (int i = 0; i < algorithmBox.Items.Count; i++)
            {
                if (algorithmBox.Items[i]?.ToString() == item.Algorithm)
                {
                    algorithmBox.SelectedIndex = i;
                    break;
                }
            }
        }
    }

    public void UpdateItem()
    {
        if (_item == null) return;
        _item.Secret = secretBox.Text ?? "";
        _item.Digits = string.IsNullOrEmpty(digitsBox.Text) ? "6" : digitsBox.Text;
        _item.Interval = string.IsNullOrEmpty(intervalBox.Text) ? "30" : intervalBox.Text;
        _item.Algorithm = algorithmBox.SelectedItem?.ToString() ?? "SHA1";
    }
}
