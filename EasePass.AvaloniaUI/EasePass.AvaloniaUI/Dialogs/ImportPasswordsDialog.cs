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
using EasePass.Models;
using EasePass.Services;
using EasePass.ViewModels.Dialog;
using EasePass.Views;
using EasePass.Views.DialogViews;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace EasePass.Dialogs
{
    public class ImportPasswordsDialog
    {
        private ImportPasswordsViewModel _vm;
        private ImportPasswordsPage _page;

        public ImportPasswordsDialog()
        {
            _vm = new ImportPasswordsViewModel();
            _page = new ImportPasswordsPage { DataContext = _vm };
        }

        public async Task<(PasswordManagerItem[] Items, bool Override)> ShowAsync(bool showProgressbar)
        {
            var dialog = new Helper.Logout.AutoLogoutContentDialog
            {
                Title = "Import passwords".Localized("Dialog_ImportPW_Headline/Text"),
                PrimaryButtonText = "Add".Localized("Dialog_Button_Add/Text"),
                SecondaryButtonText = "Override".Localized("Dialog_Button_Override/Text"),
                CloseButtonText = "Cancel".Localized("Dialog_Button_Cancel/Text"),
                Content = _page
            };

            dialog.Closing += Dialog_Closing;

            var res = await dialog.ShowOnMainView();
            PasswordManagerItem[] items = _vm.GetSelectedPasswords();

            if (res == DialogResult.Primary)
                return (items, false);
            if (res == DialogResult.Secondary)
                return (items, true);
            return (null, false);
        }

        private void Dialog_Closing(object? sender, BaseDialogClosingArgs args)
        {
            if (args.Result != DialogResult.Secondary)
                return;

            // Show confirmation for overwrite
            if (!_vm.ShowOverwriteConfirm)
            {
                _vm.ShowOverwriteConfirm = true;
                args.Cancel = true;
                return;
            }

            if (!_vm.ConfirmOverwrite)
            {
                args.Cancel = true;
                return;
            }
        }

        public void SetPagePasswords(PasswordManagerItem[] items)
        {
            _vm.SetPasswords(items);
        }

        public void SetPagePasswords(ObservableCollection<PasswordManagerItem> items)
        {
            _vm.SetPasswords(items);
        }

        public void ShowProgressBar()
        {
            // Progress indication handled by the dialog itself
        }
    }
}
