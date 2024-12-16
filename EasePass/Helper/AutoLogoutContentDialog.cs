using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;

namespace EasePass.Helper
{
    internal class AutoLogoutContentDialog : ContentDialog
    {
        private static List<AutoLogoutContentDialog> Dialogs = new List<AutoLogoutContentDialog>();
        private static bool doPreventAutoLogoutDialog = false;

        public static bool InactivityStarted()
        {
            if (doPreventAutoLogoutDialog)
                return false;

            for (int i = 0; i < Dialogs.Count; i++)
            {
                Dialogs[i].Hide();
            }
            return true;
        }

        public AutoLogoutContentDialog(bool preventAutoLogout = false) : base()
        {
            doPreventAutoLogoutDialog = preventAutoLogout;
            Dialogs.Add(this);
            this.Closing += AutoLogoutContentDialog_Closing;
        }

        private void AutoLogoutContentDialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            if (sender is AutoLogoutContentDialog dialog && dialog != null)
            {
                Dialogs.Remove(dialog);
                doPreventAutoLogoutDialog = false;
            }
        }
    }
}
