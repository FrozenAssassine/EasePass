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
using EasePass.Models;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;

namespace EasePass.Dialogs
{
    internal class ConfirmDeleteDatabaseDialog
    {
        public async Task<bool> ShowAsync(Database database)
        {
            var dialog = new AutoLogoutContentDialog
            {
                Title = "Confirm delete Database".Localized("Dialog_ConfirmDeleteDatabase_Title/Text"),
                PrimaryButtonText = "Delete".Localized("Dialog_Button_Delete/Text"),
                CloseButtonText = "Close".Localized("Dialog_Button_Close/Text"),
                XamlRoot = App.m_window.Content.XamlRoot,
                Content = "Confirm to delete Database:".Localized("Dialog_ConfirmDeleteDatabase_Title/Content") + "\n" + database.Name + "\n" + database.Path,
            };
            return await dialog.ShowAsync() == ContentDialogResult.Primary;
        }
    }
}
