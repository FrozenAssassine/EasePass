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
using System;

namespace EasePass.Controls
{
    public sealed partial class CopyTextbox : TextBox
    {
        public bool RemoveWhitespaceOnCopy { get; set; } = false;
        public bool IsUrlAction { get; set; } = false;

        public CopyTextbox()
        {
            this.InitializeComponent();
        }

        private async void CopyText_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            string txt = this.Text;
            if (!txt.ToLower().StartsWith("http")) txt = "http://" + txt;
            if (string.IsNullOrEmpty(txt))
                return;
            try
            {
                if (IsUrlAction)
                {
                    await Windows.System.Launcher.LaunchUriAsync(new Uri(txt));
                    return;
                }
            }
            catch (UriFormatException) { /*Invalid URL*/ return; }

            ClipboardHelper.Copy(RemoveWhitespaceOnCopy ? this.Text.Replace(" ", "") : this.Text);
        }

        private void TextBox_DoubleTapped(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            if (SettingsManager.GetSettingsAsBool(AppSettingsValues.doubleTapToCopy, DefaultSettingsValues.doubleTapToCopy))
                CopyText_Click(this, null);
        }
    }
}
