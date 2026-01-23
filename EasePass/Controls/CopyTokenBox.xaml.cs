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
    /// <summary>
    /// A Box, which allows to have a copy and show functionality for a Token
    /// </summary>
    public sealed partial class CopyTokenBox : TextBox
    {
        #region Fields & Properties
        /// <summary>
        /// The Token in the TokenBox
        /// </summary>
        private string _token;
        /// <summary>
        /// The Token in the TokenBox
        /// </summary>
        public string Token
        {
            get => _token;
            set
            {
                _token = value;
                ToggleShowToken(ShowToken);
            }
        }

        /// <summary>
        /// Specifies if the <see cref="Token"/> should be shown
        /// </summary>
        private bool _showToken = false;

        /// <summary>
        /// Specifies if the <see cref="Token"/> should be shown
        /// </summary>
        public bool ShowToken
        {
            get => _showToken;
            set
            {
                _showToken = value;
                ToggleShowToken(value);
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes the <see cref="CopyTokenBox"/>
        /// </summary>
        public CopyTokenBox()
        {
            this.InitializeComponent();
            this.IsReadOnly = true;
        }
        #endregion

        #region Copy
        /// <summary>
        /// Copies the Text in the Tokenbox
        /// </summary>
        private void CopyText() => ClipboardHelper.Copy(Token, true);
        private void CopyText_Click(object sender, RoutedEventArgs e) => CopyText();
        #endregion

        #region Toggle
        /// <summary>
        /// Toggles the <see cref="ShowToken"/> Property to it's opposite value
        /// </summary>
        /// <param name="sender">The Sender of the Event</param>
        /// <param name="e">The Event Arguments</param>
        private void ShowToken_Toggled(object sender, RoutedEventArgs e)
        {
            ShowToken = !ShowToken;
        }
        /// <summary>
        /// Shows the Tokem or sets the Token to "*"
        /// </summary>
        /// <param name="show">Specifies if the Token should be shown or the mask of the Token</param>
        private void ToggleShowToken(bool show)
        {
            Text = show ? _token : new string('•', _token.Length);
        }
        #endregion

        #region DoubleTapped
        private void TextBox_DoubleTapped(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            if (AppSettings.DoubleTapToCopy)
            {
                CopyText();
            }
        }
        #endregion

        #region PreviewKeyDown
        private void TextBox_PreviewKeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (KeyHelper.IsKeyPressed(Windows.System.VirtualKey.Control) && !KeyHelper.IsKeyPressed(Windows.System.VirtualKey.Menu))
            {
                if (e.Key == Windows.System.VirtualKey.A)
                {
                    this.SelectAll();
                }
                else if (e.Key == Windows.System.VirtualKey.C)
                {
                    CopyText();
                }
                e.Handled = true;
                return;
            }
        }
        #endregion
    }
}