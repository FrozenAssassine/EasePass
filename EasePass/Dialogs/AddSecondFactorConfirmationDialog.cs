using EasePass.Extensions;
using EasePass.Helper.Logout;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;

namespace EasePass.Dialogs
{
    internal class AddSecondFactorConfirmationDialog
    {
        /// <summary>
        /// Shows the Confirmation Dialog for adding the SecondFactor
        /// </summary>
        /// <param name="databaseName">The Name of the Database</param>
        /// <returns>Returns <see langword="true"/> if the User wants to add the SecondFactor to the Database</returns>
        public async Task<bool> ShowAsync(string databaseName)
        {
            AutoLogoutContentDialog dialog = new AutoLogoutContentDialog
            {
                Title = "Confirm enabling SecondFactor".Localized("Dialog_ConfirmAddSecondFactor_Headline/Text"),
                PrimaryButtonText = "Confirm".Localized("Dialog_Button_Add/Text"),
                CloseButtonText = "Cancel".Localized("Dialog_Button_Cancel/Text"),
                XamlRoot = App.m_window.Content.XamlRoot,
                Content = "Are you sure you want to enable a SecondFactor for the Database \"@@@Name@@@\"?\nIf you Forget your Password you will have no Access to your Database!".Localized("Dialog_ConfirmAddSecondFactor_Item/Text").Replace("@@@Name@@@", databaseName),
            };

            return await dialog.ShowAsync() == ContentDialogResult.Primary;
        }
    }
}
