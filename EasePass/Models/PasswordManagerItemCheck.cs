using System.ComponentModel;

namespace EasePass.Models;

internal class PasswordManagerItemCheck : INotifyPropertyChanged
{
    private bool check { get; set; } = true;
    public bool Checked
    {
        get => check;
        set
        {
            check = value;
            PropertyChanged(this, new PropertyChangedEventArgs("Checked"));
        }
    }
    public PasswordManagerItem Item = null;

    public event PropertyChangedEventHandler PropertyChanged;

    public PasswordManagerItemCheck(PasswordManagerItem item)
    {
        this.Item = item;
    }
}
