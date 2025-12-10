using EasePass.Extensions;
using EasePass.Helper;
using EasePass.Views.DialogPages;
using System;
using System.Threading.Tasks;

namespace EasePass.Dialogs
{
    internal class ShowSecondFactorDialog
    {
        public async Task<bool> ShowAsync(string token)
        {
            ShowSecondFactorPage showSecondFactorPage = new ShowSecondFactorPage(token);
            AutoLogoutContentDialog dialog = new AutoLogoutContentDialog
            {
                Title = "The new SecondFactor Token for the Database".Localized("Dialogs_ShowSF_Title/Text"),
                PrimaryButtonText = "Done".Localized("Dialog_Button_Done/Text"),
                CloseButtonText = "Done".Localized("Dialog_Button_Done/Text"),
                XamlRoot = App.m_window.Content.XamlRoot,
                Content = showSecondFactorPage
            };
            await dialog.ShowAsync();
            return true;
        }
    }
}
