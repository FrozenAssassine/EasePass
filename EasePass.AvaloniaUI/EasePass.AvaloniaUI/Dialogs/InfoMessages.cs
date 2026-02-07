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

using Avalonia.Controls;
using EasePass.Controls;
using EasePass.Extensions;
using EasePass.Helper.App;
using System;

namespace EasePass.Dialogs
{
    internal class InfoMessages
    {
        public static void CantDeleteDatabase() => InfobarExtension.Show("InfoMessages_DatabaseDeletionFailed", InfoBarSeverity.Error, 5);
        public static void CantDeleteLoadedDatabase() => InfobarExtension.Show("Can't delete Database!".Localized("InfoMessages_DatabaseDeletionFailed/Headline"), "You can't delete a loaded database!".Localized("InfoMessages_DatabaseDeleteLoaded/Text"), InfoBarSeverity.Error, 5);
        public static void DatabaseDeleted() => InfobarExtension.Show("InfoMessages_DatabaseDeleted", InfoBarSeverity.Success, 5);
        public static void DatabaseLoaded() => InfobarExtension.Show("InfoMessages_DatabaseLoaded", InfoBarSeverity.Success, 5);
        public static void EnteredWrongPassword(int attempts) => InfobarExtension.Show(
            "Wrong password".Localized("InfoMessages_EnteredWrongPW/Headline"),
            $"You entered the wrong password.\nPlease try again\n({attempts}/3)".Localized("InfoMessages_EnteredWrongPW/Text").Replace("{attempts}", attempts.ToString()),
            InfoBarSeverity.Error
        );
        public static void TooManyPasswordAttempts() => InfobarExtension.Show(
            "InfoMessages_TooManyPasswordAttempts",
            InfoBarSeverity.Error
        );
        public static void ImportDBWrongPassword() => InfobarExtension.Show(
            "InfoMessages_ImportDBWrongPassword",
            InfoBarSeverity.Error,
            8
        );
        public static void ImportDBSuccess() => InfobarExtension.Show(
            "InfoMessages_ImportDBSuccess",
            InfoBarSeverity.Success
        );
        public static void ExportDBSuccess() => InfobarExtension.Show(
            "InfoMessages_ExportDBSuccess",
            InfoBarSeverity.Success
        );
        public static void ExportDBFailed() => InfobarExtension.Show(
            "InfoMessages_ExportDBFailed",
            InfoBarSeverity.Error
        );
        public static void PasswordTooShort() => InfobarExtension.Show(
            "InfoMessages_PasswordTooShort",
            InfoBarSeverity.Error
        );
        //public static void PasswordTooShort() => InfobarExtension.Show(
        //    "InfoMessages_PasswordTooShort",
        //    InfoBarSeverity.Error,
        //    8
        //);
        public static void PasswordAlreadyUsed() => InfobarExtension.Show(
            "InfoMessages_PasswordAlreadyUsed",
            InfoBarSeverity.Warning, 10
        );
        public static void ChangePasswordWrong() => InfobarExtension.Show(
            "InfoMessages_ChangePasswordWrong",
            InfoBarSeverity.Error
        );
        //public static void ChangePasswordWrong() => InfobarExtension.Show(
        //    "InfoMessages_ChangePasswordWrong",
        //    InfoBarSeverity.Error,
        //    8
        //);
        public static void PasswordsDoNotMatch() => InfobarExtension.Show(
            "InfoMessages_PasswordsDoNotMatch",
            InfoBarSeverity.Error
        );
        //public static void PasswordsDoNotMatch() => InfobarExtension.Show(
        //    "InfoMessages_PasswordsDoNotMatch",
        //    InfoBarSeverity.Error,
        //    8
        //);
        public static void InvalidDatabasePath() => InfobarExtension.Show(
            "InfoMessages_InvalidDatabasePath",
            InfoBarSeverity.Error,
            8
        );
        public static void SuccessfullyChangedPassword() => InfobarExtension.Show(
            "InfoMessages_SuccessfullyChangedPassword",
            InfoBarSeverity.Success
        );
        public static void AutomaticallyLoggedOut() => InfobarExtension.ShowUntilLogin(
            "InfoMessages_AutomaticallyLoggedOut",
            InfoBarSeverity.Informational
        );
        public static void NoAccessToPathDatabaseNotSaved(string path) => InfobarExtension.Show(
            "Database could not be saved".Localized("InfoMessages_NoAccessToPathDatabaseNotSaved/Headline"),
            $"Database could not be saved. No access to the path {path}".Localized("InfoMessages_NoAccessToPathDatabaseNotSaved/Text").Replace("{path}", path),
            InfoBarSeverity.Error, 15
        );
        public static void NoAccessToPathDatabaseNotLoaded(string path) => InfobarExtension.Show(
            "Database could not be loaded".Localized("InfoMessages_NoAccessToPathDatabaseNotLoaded/Headline"),
            $"Database could not be loaded. No access to the path {path}".Localized("InfoMessages_NoAccessToPathDatabaseNotLoaded/Text").Replace("{path}", path),
            InfoBarSeverity.Error, 15
        );
        public static void DatabaseSaveToFileError(string path) => InfobarExtension.Show(
            "Error while saving database".Localized("InfoMessages_DatabaseSaveToFileError/Headline"),
            $"Data save error at {path}. Verify connection or device storage, then retry.".Localized("InfoMessages_DatabaseSaveToFileError/Text").Replace("{path}", path),
            InfoBarSeverity.Error, 15
        );
        public static void DatabaseFileNotFoundAt(string path) => InfobarExtension.Show(
            "Database not found".Localized("InfoMessages_DatabaseFileNotFoundAt/Headline"),
            $"Your database with the stored passwords could not be found at path {path}".Localized("InfoMessages_DatabaseFileNotFoundAt/Text").Replace("{path}", path),
            InfoBarSeverity.Error, 15
        );
        public static void DatabaseInvalidData() => InfobarExtension.Show(
            "InfoMessages_DatabaseInvalidData",
            InfoBarSeverity.Error, 15
        );
        public static void PrinterNotSelected() => InfobarExtension.Show(
            "InfoMessages_PrinterNotSelected",
            InfoBarSeverity.Error, 15
        );
        public static void PrinterItemSkipped(string name) => InfobarExtension.Show(
            "Password skipped".Localized("InfoMessages_PrinterItemSkipped/Headline"),
            $"The service \"{name}\" was skipped, because it doesn't fit into the page.".Localized("InfoMessages_PrinterItemSkipped/Text").Replace("{name}", name),
            InfoBarSeverity.Error, 15
        );
        public static void CouldNotGetExtensions(string exception) => InfobarExtension.Show(
            "Could not get Plugins".Localized("InfoMessages_CouldNotGetExtensions/Headline"),
            "Could not get the plugins from the Server:\n".Localized("InfoMessages_CouldNotGetExtensions/Text") + exception,
            InfoBarSeverity.Error, 10
        );
        public static void FileIsNotAnExtensions() => InfobarExtension.Show(
            "InfoMessages_FileIsNotAnExtensions",
            InfoBarSeverity.Error, 10
        );
        public static void ExtensionAlreadyInstalled() => InfobarExtension.Show(
            "InfoMessages_ExtensionAlreadyInstalled",
            InfoBarSeverity.Error, 10
        );
        public static void Invalid2FA() => InfobarExtension.Show(
            "InfoMessages_Invalid2FA",
            InfoBarSeverity.Error, 10
        );
        public static void Error() => InfobarExtension.Show(
            "InfoMessages_Error",
            InfoBarSeverity.Error, 5
        );
        public static void CouldNotCreateDatabaseBackupFolder(Exception ex) => InfobarExtension.Show(
            "Could not create database backup folder".Localized("InfoMessages_CouldNotCreateDBBackupFolder/Headline"),
            ex.Message,
            InfoBarSeverity.Error, 10
            );
        public static void PluginMovedWhileInstallingLocal() => InfobarExtension.Show(
            "InfoMessages_PluginMovedWhileCopy",
            InfoBarSeverity.Error, 10
            );

        public static void DatabaseWithThatNameAlreadyExists() => InfobarExtension.Show(
            "InfoMessages_DatabaseWithThatNameAlreadyExists",
            InfoBarSeverity.Error, 10
            );
        public static void CantDeleteRemoteDatabase() => InfobarExtension.Show(
            "Could not delete remote database".Localized("InfoMessages_CantDeleteRemoteDatabase/Headline"),
            "Remote databases are provides by plugins. To remove them, remove the plugin.".Localized("InfoMessages_CantDeleteRemoteDatabase/Text"),
            InfoBarSeverity.Error, 10
            );
        public static void DatabaseLockedByOtherUser() => InfobarExtension.Show(
            "Database locked by other user".Localized("InfoMessages_DatabaseLockedByOtherUser/Headline"),
            "This database could not be opened, because another user/device is using it.".Localized("InfoMessages_DatabaseLockedByOtherUser/Text"),
            InfoBarSeverity.Error, 10
            );
        public static void DatabaseProviderLoadingFailed(string providername) => InfobarExtension.Show(
            "Database provider loading error".Localized("InfoMessages_DatabaseProviderLoadingFailed/Headline"),
            $"The database provider \"{providername}\" failed on loading databases.".Localized("InfoMessages_DatabaseProviderLoadingFailed/Text").Replace("{providername}", providername),
            InfoBarSeverity.Error, 10
            );
        public static void UnknownDatabaseSourceError(string sourcename) => InfobarExtension.Show(
            "Database source error".Localized("InfoMessages_UnknownDatabaseSourceError/Headline"),
            $"The database source \"{sourcename}\" failed on internal processes.".Localized("InfoMessages_UnknownDatabaseSourceError/Text").Replace("{sourcename}", sourcename),
            InfoBarSeverity.Error, 10
            );
        public static void RemoteDBSettingsCouldNotBeSaved() => InfobarExtension.Show(
            "InfoMessages_RemoteDBSettingsCouldNotBeSaved",
            InfoBarSeverity.Error, 10
            );
        public static void OpenExternalRemoteConfigEditorFailed() => InfobarExtension.Show(
            "InfoMessages_OpenExternalRemoteConfigEditorFailed",
            InfoBarSeverity.Error, 10
            );

        public static void UnhandledException(Exception ex) => InfobarExtension.Show("Unhandled Exception:".Localized("InfoMessages_UnhandledException/Headline"), ex.Message, InfoBarSeverity.Error);
        public static void NewVersionInfo(string version)
        {
            var btn = new Button { Content = "Changelog".Localized("InfoMessages_NewVersion/Content") };
            btn.Click += delegate
            {
                NavigationHelper.ToAboutPage(0);
            };

            InfobarExtension.Show(
                "New version".Localized("InfoMessages_NewVersion/Headline"),
                $"Welcome to Ease Pass version {version}".Localized("InfoMessages_NewVersion/Text").Replace("{version}", version),
                btn,
                InfoBarSeverity.Success
                );
        }

        //public static InfoBar DownloadingPluginInfo()
        //{
        //    var progressbar = new ProgressBar { IsIndeterminate = true };

        //    var infobar = new InfoBar();
        //    infobar.ShowInfobar("Downloading plugin in progress".Localized("InfoMessages_DownloadPluginProgress/Headline"), "", progressbar, InfoBarSeverity.Info);
        //    return infobar;
        //}

    }
}
