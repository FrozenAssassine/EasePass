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
using EasePass.Views;
using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace EasePass.Dialogs
{
    public class ImportPasswordsDialog
    {
        private readonly ImportPasswordsDialogPage importPage;

        public ImportPasswordsDialog()
        {
            importPage = new ImportPasswordsDialogPage();
        }

        public async Task<(PasswordManagerItem[] Items, bool Override)> ShowAsync(bool showProgressbar)
        {
            var dialog = new AutoLogoutContentDialog
            {
                Title = "Import passwords".Localized("Dialog_ImportPW_Headline/Text"),
                PrimaryButtonText = "Add".Localized("Dialog_Button_Add/Text"),
                SecondaryButtonText = "Override".Localized("Dialog_Button_Override/Text"),
                CloseButtonText = "Cancel".Localized("Dialog_Button_Cancel/Text"),
                XamlRoot = App.m_window.Content.XamlRoot,
                Content = importPage
            };

            //to add confirmation on overwrite passwords:
            dialog.Closing += Dialog_Closing;

            var res = await dialog.ShowAsync();
            PasswordManagerItem[] items = importPage.GetSelectedPasswords();

            if (res == ContentDialogResult.Primary)
                return (items, false);
            if (res == ContentDialogResult.Secondary)
                return (items, true);
            return (null, false);
        }

        private void Dialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            //ensure overwrite button was pressed:
            if (args.Result != ContentDialogResult.Secondary)
                return;

            importPage.ShowConfirmOverWriteDatabase();

            var overwriteState = importPage.GetConfirmOverwriteState();
            //allow overwrite
            if (overwriteState.result)
                return;

            overwriteState.confirmOverwriteCheckbox.BorderBrush = new SolidColorBrush(Colors.Red);
            args.Cancel = true;
        }

        public void SetPagePasswords(PasswordManagerItem[] items)
        {
            importPage.SetPasswords(items);
        }
        public void SetPagePasswords(ObservableCollection<PasswordManagerItem> items)
        {
            importPage.SetPasswords(items);
        }

        public void ShowProgressBar()
        {
            importPage.ShowProgressBar();
        }
    }
}
