using EasePass.Dialogs;
using EasePass.Models;
using EasePassExtensibility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasePass.Helper
{
    internal static class PasswordImportManager
    {
        public async static Task<(PasswordManagerItem[] Items, bool Override)> ManageImport(IPasswordImporter importer)
        {
            ImportDialog dlg = new ImportDialog();
            try
            {
                PasswordItem[] items = null;
                Task.Run(new Action(() =>
                {
                    items = importer.ImportPasswords();
                })).GetAwaiter().OnCompleted(new Action(() =>
                {
                    if(items != null)
                    {
                        if (items.Length > 0)
                        {
                            PasswordManagerItem[] pwItems = new PasswordManagerItem[items.Length];
                            for (int i = 0; i < items.Length; i++)
                                pwItems[i] = ToPMI(items[i]);
                            dlg.SetPagePasswords(pwItems);
                        }
                        else
                        {
                            dlg.SetPageMessage(ImportDialog.MsgType.NoPasswords);
                        }
                    }
                    else
                    {
                        dlg.SetPageMessage(ImportDialog.MsgType.Error);
                    }
                }));
            }
            catch
            {
                dlg.SetPageMessage(ImportDialog.MsgType.Error);
            }
            return await dlg.ShowAsync();
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
            try
            {
                switch (item.TOTPAlgorithm)
                {
                    case PasswordItem.Algorithm.SHA1:
                        pmi.Algorithm = "SHA1";
                        break;
                    case PasswordItem.Algorithm.SHA256:
                        pmi.Algorithm = "SHA256";
                        break;
                    case PasswordItem.Algorithm.SHA512:
                        pmi.Algorithm = "SHA512";
                        break;
                }
            }
            catch { }
            return pmi;
        }
    }
}
