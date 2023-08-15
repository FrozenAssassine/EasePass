using EasePass.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasePass.Models
{
    internal class SettingsNavigationParameters
    {
        public ObservableCollection<PasswordManagerItem> PwItems { get; set; }
        public PasswordsPage PasswordPage { get; set; }
    }
}
