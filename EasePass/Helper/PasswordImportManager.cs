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
            try
            {
                PasswordItem[] items = null;
                Task.Run(new Action(() =>
                {
                    items = importer.ImportPasswords();
                })).GetAwaiter().OnCompleted(
                    new Action(() =>
                    {
                        if (items == null)
                            dlg.SetPageMessage(ImportPasswordsDialog.MsgType.Error);
                        else if(items.Length == 0)
                            dlg.SetPageMessage(ImportPasswordsDialog.MsgType.NoPasswords);
                        else
                            dlg.SetPagePasswords(items.Select(x => ToPMI(x)).ToArray());
                    }));
            }
            catch
            {
                dlg.SetPageMessage(ImportPasswordsDialog.MsgType.Error);
            }
            return await dlg.ShowAsync(true);
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
