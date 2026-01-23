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
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace EasePass.Controls
{
    public sealed partial class CopyPasswordbox : TextBox
    {
        private Button showPasswordButton = null;
        private PasswordSafetyChart pwSafetyChart = new PasswordSafetyChart
        {
            Margin = new Thickness(6, 2, 5, 2),
            Height = 30,
            ShowInfo = false,
            SingleHitbox = true,
            HorizontalAlignment = HorizontalAlignment.Right
        };

        public CopyPasswordbox()
        {
            this.InitializeComponent();
            this.IsReadOnly = true;
        }

        private void ToggleShowPassword(bool show)
        {
            base.Text = show ? _Password : new string('•', _Password.Length);

            if (showPasswordButton != null)
                showPasswordButton.Content = ShowPassword ? '\uED1A' : '\uF78D';
        }

        private string _Password;
        public string Password
        {
            get => _Password;
            set
            {
                _Password = value;
                ToggleShowPassword(ShowPassword);

                pwSafetyChart.EvaluatePassword(_Password, true);
            }
        }

        private bool _ShowPassword = false;
        public bool ShowPassword { get => _ShowPassword; set { _ShowPassword = value; ToggleShowPassword(value); } }

        private void CopyText() => ClipboardHelper.Copy(this.Password, true);

        private void TextBox_DoubleTapped(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            if (AppSettings.DoubleTapToCopy)
                CopyText();
        }
        private void CopyText_Click(object sender, RoutedEventArgs e)
        {
            CopyText();
        }

        private void showPW_Toggled(object sender, RoutedEventArgs e)
        {
            ShowPassword = !ShowPassword;
        }

        private void TextBox_PreviewKeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (KeyHelper.IsKeyPressed(Windows.System.VirtualKey.Control) && !KeyHelper.IsKeyPressed(Windows.System.VirtualKey.Menu))
            {
                if (e.Key == Windows.System.VirtualKey.A)
                    this.SelectAll();
                else if (e.Key == Windows.System.VirtualKey.C)
                    CopyText();

                e.Handled = true;
                return;
            }
        }
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var sp = GetTemplateChild("pwSafetyChartParentSP") as StackPanel;

            if (!sp.Children.Contains(pwSafetyChart))
                sp.Children.Add(pwSafetyChart);

            showPasswordButton = GetTemplateChild("showPasswordButton") as Button;

            ToggleShowPassword(false);
        }
    }
}
