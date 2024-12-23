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

using CommunityToolkit.WinUI;
using EasePass.Extensions;
using EasePass.Helper;
using Microsoft.UI.Xaml.Controls;
using System;

namespace EasePass.Dialogs
{
    internal class InfoMessages
    {
        public static void CantDeleteDatabase() => new InfoBar().Show("InfoMessages_DatabaseDeletionFailed", InfoBarSeverity.Error, 5);
        public static void CantDeleteLoadedDatabase() => new InfoBar().Show("Can't delete Database!".Localized("InfoMessages_DatabaseDeletionFailed/Headline"), "You can't delete a loaded database!".Localized("InfoMessages_DatabaseDeleteLoaded/Text"), InfoBarSeverity.Error, 5);
        public static void DatabaseDeleted() => new InfoBar().Show("InfoMessages_DatabaseDeleted", InfoBarSeverity.Success, 5);
        public static void DatabaseLoaded() => new InfoBar().Show("InfoMessages_DatabaseLoaded", InfoBarSeverity.Success, 5);
        public static void EnteredWrongPassword(int attempts) => new InfoBar().Show(
            "Wrong password".Localized("InfoMessages_EnteredWrongPW/Headline"),
            $"You entered the wrong password.\nPlease try again\n({attempts}/3)".Localized("InfoMessages_EnteredWrongPW/Text").Replace("{attempts}", attempts.ToString()),
            InfoBarSeverity.Error
        );
        public static void TooManyPasswordAttempts() => new InfoBar().Show(
            "InfoMessages_TooManyPasswordAttempts",
            InfoBarSeverity.Error
        );
        public static void ImportDBWrongPassword(Panel parent = null) => new InfoBar().Show(
            "InfoMessages_ImportDBWrongPassword",
            InfoBarSeverity.Error,
            8,
            parent
        );
        public static void ImportDBSuccess() => new InfoBar().Show(
            "InfoMessages_ImportDBSuccess",
            InfoBarSeverity.Success
        );
        public static void ExportDBSuccess() => new InfoBar().Show(
            "InfoMessages_ExportDBSuccess",
            InfoBarSeverity.Success
        );
        public static void PasswordTooShort() => new InfoBar().Show(
            "InfoMessages_PasswordTooShort",
            InfoBarSeverity.Error
        );
        public static void PasswordTooShort(Panel parent = null) => new InfoBar().Show(
            "InfoMessages_PasswordTooShort",
            InfoBarSeverity.Error,
            8,
            parent
        );
        public static void PasswordAlreadyUsed() => new InfoBar().Show(
            "InfoMessages_PasswordAlreadyUsed",
            InfoBarSeverity.Warning, 10
        );
        public static void ChangePasswordWrong() => new InfoBar().Show(
            "InfoMessages_ChangePasswordWrong",
            InfoBarSeverity.Error
        );
        public static void ChangePasswordWrong(Panel parent = null) => new InfoBar().Show(
            "InfoMessages_ChangePasswordWrong",
            InfoBarSeverity.Error,
            8,
            parent
        );
        public static void PasswordsDoNotMatch() => new InfoBar().Show(
            "InfoMessages_PasswordsDoNotMatch",
            InfoBarSeverity.Error
        );
        public static void PasswordsDoNotMatch(Panel parent = null) => new InfoBar().Show(
            "InfoMessages_PasswordsDoNotMatch",
            InfoBarSeverity.Error,
            8,
            parent
        );
        public static void InvalidDatabasePath(Panel parent = null) => new InfoBar().Show(
            "InfoMessages_InvalidDatabasePath",
            InfoBarSeverity.Error,
            8,
            parent
        );
        public static void SuccessfullyChangedPassword() => new InfoBar().Show(
            "InfoMessages_SuccessfullyChangedPassword",
            InfoBarSeverity.Success
        );
        public static void AutomaticallyLoggedOut() => new InfoBar().ShowUntilLogin(
            "InfoMessages_AutomaticallyLoggedOut",
            InfoBarSeverity.Informational
        );
        public static void NoAccessToPathDatabaseNotSaved(string path) => new InfoBar().Show(
            "Database could not be saved".Localized("InfoMessages_NoAccessToPathDatabaseNotSaved/Headline"),
            $"Database could not be saved. No access to the path {path}".Localized("InfoMessages_NoAccessToPathDatabaseNotSaved/Text").Replace("{path}", path),
            InfoBarSeverity.Error, 15
        );
        public static void NoAccessToPathDatabaseNotLoaded(string path) => new InfoBar().Show(
            "Database could not be loaded".Localized("InfoMessages_NoAccessToPathDatabaseNotLoaded/Headline"),
            $"Database could not be loaded. No access to the path {path}".Localized("InfoMessages_NoAccessToPathDatabaseNotLoaded/Text").Replace("{path}", path),
            InfoBarSeverity.Error, 15
        );
        public static void DatabaseSaveToFileError(string path) => new InfoBar().Show(
            "Error while saving database".Localized("InfoMessages_DatabaseSaveToFileError/Headline"),
            $"Data save error at {path}. Verify connection or device storage, then retry.".Localized("InfoMessages_DatabaseSaveToFileError/Text").Replace("{path}", path),
            InfoBarSeverity.Error, 15
        );
        public static void DatabaseFileNotFoundAt(string path) => new InfoBar().Show(
            "Database not found".Localized("InfoMessages_DatabaseFileNotFoundAt/Headline"),
            $"Your database with the stored passwords could not be found at path {path}".Localized("InfoMessages_DatabaseFileNotFoundAt/Text").Replace("{path}", path),
            InfoBarSeverity.Error, 15
        );
        public static void DatabaseInvalidData() => new InfoBar().Show(
            "InfoMessages_DatabaseInvalidData",
            InfoBarSeverity.Error, 15
        );
        public static void PrinterNotSelected() => new InfoBar().Show(
            "InfoMessages_PrinterNotSelected",
            InfoBarSeverity.Error, 15
        );
        public static void PrinterItemSkipped(string name) => new InfoBar().Show(
            "Password skipped".Localized("InfoMessages_PrinterItemSkipped/Headline"),
            $"The service \"{name}\" was skipped, because it doesn't fit into the page.".Localized("InfoMessages_PrinterItemSkipped/Text").Replace("{name}", name),
            InfoBarSeverity.Error, 15
        );
        public static void CouldNotGetExtensions(string exception) => new InfoBar().Show(
            "Could not get Plugins".Localized("InfoMessages_CouldNotGetExtensions/Headline"),
            "Could not get the plugins from the Server:\n".Localized("InfoMessages_CouldNotGetExtensions/Text") + exception,
            InfoBarSeverity.Error, 10
        );
        public static void FileIsNotAnExtensions() => new InfoBar().Show(
            "InfoMessages_FileIsNotAnExtensions",
            InfoBarSeverity.Error, 10
        );
        public static void ExtensionAlreadyInstalled() => new InfoBar().Show(
            "InfoMessages_ExtensionAlreadyInstalled",
            InfoBarSeverity.Error, 10
        );
        public static void Invalid2FA() => new InfoBar().Show(
            "InfoMessages_Invalid2FA",
            InfoBarSeverity.Error, 10
        );
        public static void Error() => new InfoBar().Show(
            "InfoMessages_Error",
            InfoBarSeverity.Error, 5
        );
        public static void CouldNotCreateDatabaseBackupFolder(Exception ex) => new InfoBar().Show(
            "Could not create database backup folder".Localized("InfoMessages_CouldNotCreateDBBackupFolder/Headline"),
            ex.Message,
            InfoBarSeverity.Error, 10
            );
        public static void PluginMovedWhileInstallingLocal() => new InfoBar().Show(
            "InfoMessages_PluginMovedWhileCopy",
            InfoBarSeverity.Error, 10
            );

        public static void UnhandledException(Exception ex) => new InfoBar().Show("Unhandled Exception:".Localized("InfoMessages_UnhandledException/Headline"), ex.Message, InfoBarSeverity.Error);
        public static void NewVersionInfo(string version)
        {
            var btn = new Button { Content = "Changelog".Localized("InfoMessages_NewVersion/Content") };
            btn.Click += delegate
            {
                NavigationHelper.ToAboutPage(0);
            };

            new InfoBar().Show(
                "New version".GetLocalized("InfoMessages_NewVersion/Headline"),
                $"Welcome to Ease Pass version {version}".Localized("InfoMessages_NewVersion/Text").Replace("{version}", version),
                btn,
                InfoBarSeverity.Success
                );
        }

        public static InfoBar DownloadingPluginInfo()
        {
            var progressbar = new ProgressBar { ShowPaused = false, ShowError = false, IsIndeterminate = true };

            var infobar = new InfoBar();
            infobar.ShowInfobar("Downloading plugin in progress".Localized("InfoMessages_DownloadPluginProgress/Headline"), "", progressbar, InfoBarSeverity.Informational);
            return infobar;
        }

    }
}
