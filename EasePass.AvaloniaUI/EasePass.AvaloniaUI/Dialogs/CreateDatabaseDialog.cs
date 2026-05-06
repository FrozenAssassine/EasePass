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

using EasePass.Core.Database;
using EasePass.Extensions;
using EasePass.Services;
using EasePass.ViewModels.Dialog;
using EasePass.Views;
using EasePass.Views.DialogViews;
using System.Threading.Tasks;

namespace EasePass.Dialogs
{
    internal class CreateDatabaseDialog
    {
        private CreateDatabaseViewModel _vm;

        public async Task<DatabaseItem> ShowAsync()
        {
            _vm = new CreateDatabaseViewModel();
            var page = new CreateDatabasePage { DataContext = _vm };

            var dialog = new Helper.Logout.AutoLogoutContentDialog
            {
                Title = "Create Database".Localized("Dialog_CreateDB_Headline/Text"),
                PrimaryButtonText = "Create".Localized("Dialog_Button_Create/Text"),
                CloseButtonText = "Close".Localized("Dialog_Button_Close/Text"),
                Content = page
            };

            dialog.Closing += Dialog_Closing;

            var res = await dialog.ShowOnMainView();
            if (res == DialogResult.Primary)
            {
                var eval = _vm.Evaluate();
                return await Database.CreateNewDatabase(eval.path, eval.masterPassword);
            }
            return null;
        }

        private void Dialog_Closing(object? sender, BaseDialogClosingArgs args)
        {
            if (args.Result != DialogResult.Primary)
                return;

            if (!_vm.PasswordsMatch)
            {
                InfoMessages.PasswordsDoNotMatch();
                args.Cancel = true;
                return;
            }

            if (!_vm.PathValid)
            {
                InfoMessages.InvalidDatabasePath();
                args.Cancel = true;
                return;
            }

            if (!_vm.PasswordLengthCorrect)
            {
                InfoMessages.PasswordTooShort();
                args.Cancel = true;
                return;
            }

            if (_vm.PathValid && _vm.PathAlreadyExists)
            {
                InfoMessages.DatabaseWithThatNameAlreadyExists();
                args.Cancel = true;
                return;
            }
        }
    }
}
