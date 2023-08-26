using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasePass.Helper
{
    internal class AutoLogoutContentDialog : ContentDialog
    {
        private static List<AutoLogoutContentDialog> Dialogs = new List<AutoLogoutContentDialog> ();

        public static void InactivityStarted()
        {
            for(int i = 0; i < Dialogs.Count;i++)
            {
                //dialog.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
                Dialogs[i].Hide();
            }
        }

        public static void InactivityStopped()
        {
            //foreach(AutoLogoutContentDialog dialog in Dialogs)
            //{
            //    dialog.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            //}
        }

        public AutoLogoutContentDialog() : base()
        {
            Dialogs.Add(this);
            this.Closing += AutoLogoutContentDialog_Closing;
        }

        private void AutoLogoutContentDialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            try
            {
                Dialogs.Remove(sender as AutoLogoutContentDialog);
            }
            catch { }
        }
    }
}
