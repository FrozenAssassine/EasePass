using EasePass.Views;
using System;
using System.Collections.ObjectModel;

namespace EasePass.Models
{
    internal class SettingsNavigationParameters
    {
        public ObservableCollection<PasswordManagerItem> PwItems { get; set; }
        public PasswordsPage PasswordPage { get; set; }
        public Action SavePwItems { get; set; }
    }
}
