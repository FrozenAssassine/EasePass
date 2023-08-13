using EasePass.Extensions;
using Microsoft.UI.Xaml.Controls;


namespace EasePass.Dialogs
{
    internal class InfoMessages
    {
        public static void EnteredWrongPassword(int attempts) => new InfoBar().Show("Wrong password", $"You entered the wrong password.\nPlease try again\n({attempts}/3)", InfoBarSeverity.Error);
        public static void TooManyPasswordAttempts() => new InfoBar().Show("Too many attempts", "You entered the password wrong three times.", InfoBarSeverity.Error);
    }
}
