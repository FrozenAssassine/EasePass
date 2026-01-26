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

using EasePass.Helper;
using EasePass.Settings;
using Microsoft.UI.Xaml.Controls;

namespace EasePass.Controls
{
    public sealed partial class EditPasswordBox : UserControl
    {
        public EditPasswordBox()
        {
            this.InitializeComponent();
            this.passwordBox.PasswordRevealMode = PasswordRevealMode.Peek;
            this.passwordBox.IsPasswordRevealButtonEnabled = true;
        }

        public delegate void PasswordChangedEvent(string password);
        public event PasswordChangedEvent PasswordChanged;

        public string Password
        {
            get => passwordBox.Password;
            set => passwordBox.Password = value;
        }

        private void CopyText() => ClipboardHelper.Copy(this.Password, true);

        private void TextBox_DoubleTapped(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            if (AppSettings.DoubleTapToCopy)
                CopyText();
        }

        private void TextBox_PreviewKeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (KeyHelper.IsKeyPressed(Windows.System.VirtualKey.Control) && !KeyHelper.IsKeyPressed(Windows.System.VirtualKey.Menu))
            {
                if (e.Key == Windows.System.VirtualKey.A)
                    this.passwordBox.SelectAll();
                else if (e.Key == Windows.System.VirtualKey.C)
                    CopyText();
                else if (e.Key == Windows.System.VirtualKey.V)
                    this.passwordBox.PasteFromClipboard();

                e.Handled = true;
                return;
            }
        }

        private void RevealButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (this.passwordBox.PasswordRevealMode == PasswordRevealMode.Visible)
                this.passwordBox.PasswordRevealMode = PasswordRevealMode.Hidden;
            else
                this.passwordBox.PasswordRevealMode = PasswordRevealMode.Visible;
        }

        private void passwordBox_PasswordChanged(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            PasswordChanged?.Invoke(passwordBox.Password);
        }
    }
}
