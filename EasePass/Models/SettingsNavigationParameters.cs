using EasePass.Core;
using EasePass.Views;
using System;

namespace EasePass.Models
{
    internal class SettingsNavigationParameters
    {
        public PasswordItemsManager PwItemsManager { get; set; }
        public PasswordsPage PasswordPage { get; set; }
        public Action SavePwItems { get; set; }
    }
}
