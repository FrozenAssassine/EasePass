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

using EasePass.Extensions;
using EasePass.Helper;
using EasePass.Views;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;

namespace EasePass.Dialogs;

internal class GenPasswordDialog
{
    private GenPasswordPage page = new GenPasswordPage();
    
    public async Task<bool> ShowAsync()
    {
        page.GeneratePassword();
        var dialog = new AutoLogoutContentDialog
        {
            Title = "Password generator".Localized("Dialog_PWGenerator_New/Text"),
            PrimaryButtonText = "New".Localized("Dialog_Button_New/Text"),
            CloseButtonText = "Done".Localized("Dialog_Button_Done/Text"),
            XamlRoot = App.m_window.Content.XamlRoot,
            Content = page
        };
        dialog.Closing += Dialog_Closing;
        return await dialog.ShowAsync() == ContentDialogResult.Secondary;
    }

    private void Dialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
    {
        if(args.Result == ContentDialogResult.Primary)
        {
            page.GeneratePassword();
            args.Cancel = true;
        }
    }
}
