/*
MIT License

Copyright (c) 2023 Julius Kirsch

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
*/

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
            try
            {
                Dialogs.Remove(sender as AutoLogoutContentDialog);
            }
            catch { }
        }
    }
}
