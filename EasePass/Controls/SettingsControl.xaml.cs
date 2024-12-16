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
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace EasePass.Controls
{
    public sealed partial class SettingsControl : UserControl
    {
        public delegate void ClickedEvent(SettingsControl sender);
        public event ClickedEvent Clicked;

        public SettingsControl()
        {
            this.InitializeComponent();
        }

        public bool Clickable { get; set; }

        private string _Glyph;
        public string Glyph { get => _Glyph; set { _Glyph = value; iconDisplay.Visibility = ConvertHelper.BoolToVisibility(value.Length > 0); } }
        public string Header { get; set; }
        public new UIElement Content
        {
            set
            {
                contentHost.Content = value;
            }
        }

        private void mainGrid_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            Clicked?.Invoke(this);
        }

        private void UserControl_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (!Clickable)
                return;

            EnterStoryboard.Begin();
        }

        private void UserControl_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (!Clickable)
                return;

            ExitStoryboard.Begin();
        }
    }
}
