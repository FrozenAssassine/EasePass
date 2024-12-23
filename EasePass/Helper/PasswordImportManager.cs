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

using EasePass.Dialogs;
using EasePass.Models;
using EasePassExtensibility;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EasePass.Helper
{
    internal static class PasswordImportManager
    {
        public async static Task<(PasswordManagerItem[] Items, bool Override)> ManageImport(IPasswordImporter importer)
        {
            ImportPasswordsDialog dlg = new ImportPasswordsDialog();
            dlg.ShowProgressBar();
            var dlgRes = await dlg.ShowAsync(true);

            PasswordItem[] items = importer.ImportPasswords();
            dlg.SetPagePasswords(items.Select(x => ToPMI(x)).ToArray());

            return dlgRes;
        }

        private static PasswordManagerItem ToPMI(PasswordItem item)
        {
            PasswordManagerItem pmi = new PasswordManagerItem();
            pmi.DisplayName = item.DisplayName;
            pmi.Email = item.EMail;
            pmi.Username = item.UserName;
            pmi.Password = item.Password;
            pmi.Website = item.Website;
            pmi.Notes = item.Notes;
            pmi.Secret = item.TOTPSecret;
            pmi.Interval = Convert.ToString(item.TOTPInterval);
            pmi.Digits = Convert.ToString(item.TOTPDigits);
            pmi.Algorithm = item.TOTPAlgorithm.ToString();

            return pmi;
        }
    }
}
