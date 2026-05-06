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
using EasePass.Services;
using EasePass.ViewModels.Dialog;
using EasePass.Views;
using EasePass.Views.DialogViews;
using System.Threading.Tasks;

namespace EasePass.Dialogs;

internal class GenPasswordDialog
{
    private GenPasswordViewModel _vm;
    private GenPasswordPage _page;

    public async Task<bool> ShowAsync()
    {
        _vm = new GenPasswordViewModel();
        _page = new GenPasswordPage { DataContext = _vm };

        var dialog = new Helper.Logout.AutoLogoutContentDialog
        {
            Title = "Password generator".Localized("Dialog_PWGenerator_New/Text"),
            PrimaryButtonText = "New".Localized("Dialog_Button_New/Text"),
            CloseButtonText = "Done".Localized("Dialog_Button_Done/Text"),
            Content = _page
        };
        dialog.Closing += Dialog_Closing;
        return await dialog.ShowOnMainView() == DialogResult.Secondary;
    }

    private void Dialog_Closing(object? sender, BaseDialogClosingArgs args)
    {
        if (args.Result == DialogResult.Primary)
        {
            // "New" button pressed - generate a new password and keep dialog open
            _vm.GeneratePasswordCommand.Execute(null);
            args.Cancel = true;
        }
    }
}
