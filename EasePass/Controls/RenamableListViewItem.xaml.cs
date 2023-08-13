using EasePass.Models;
using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;
using System.Diagnostics;

namespace EasePass.Controls
{
    public sealed partial class RenamableListViewItem : UserControl, INotifyPropertyChanged
    {
        public RenamableListViewItem()
        {
            this.InitializeComponent();
        }

        private PasswordManagerItem _PWItem;
        public PasswordManagerItem PWItem { get => _PWItem; set { _PWItem = value; Text = _PWItem.DisplayName; } }

        public string Text
        {
            get { return PWItem?.DisplayName; }
            set
            {
                if(value != PWItem.DisplayName && IsRenaming)
                    RenamedEvent.Invoke(this, value);
                PWItem.DisplayName = value;
                NotifyPropertyChanged("Text");
            }
        }

        public bool _IsRenaming = false;
        public bool IsRenaming
        {
            get => _IsRenaming;
            set
            {
                _IsRenaming = value;
                NotifyPropertyChanged("IsRenaming");
            }
        }

        public delegate void RenamedEventHandler(RenamableListViewItem sender, string newName);
        public event RenamedEventHandler RenamedEvent;

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void UserControl_LostFocus(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            IsRenaming = false;
        }
    }
}
