using System.ComponentModel;

namespace EasePass.Models
{
    public class PasswordManagerItem : INotifyPropertyChanged
    {
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public string Password { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Notes { get; set; }
        public string Secret { get; set; }
        public string Digits { get; set; } = "6";
        public string Interval { get; set; } = "30";
        public string Algorithm { get; set; } = "SHA1";
        private string _DisplayName;
        public string DisplayName
        {
            get => _DisplayName;
            set
            {
                _DisplayName = value;
                NotifyPropertyChanged("DisplayName");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
