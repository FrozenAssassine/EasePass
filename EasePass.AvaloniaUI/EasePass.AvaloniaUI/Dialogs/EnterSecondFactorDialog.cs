using EasePass.Dialogs;
using EasePass.Extensions;
using EasePass.Services;
using EasePass.Views;
using System.Security;
using System.Threading.Tasks;

namespace EasePass.Dialogs;

internal class EnterSecondFactorDialog
{
    private BaseDialog dialog;
    //private EnterSecondFactorPage page;
    public SecureString? Token { get; private set; }

    public async Task<EnterSecondFactorDialog> ShowAsync()
    {
        //page = new EnterSecondFactorPage();

        dialog = new BaseDialog
        {
            Title = "Enter the SecondFactor Token of the Database".Localized("Dialogs_EnterSF_Title/Text"),
            PrimaryButtonText = "Done".Localized("Dialog_Button_Done/Text"),
            CloseButtonText = "Cancel".Localized("Dialog_Button_Cancel/Text"),
            Width = 400,
            Height = 200
        };

        var result = await dialog.ShowOnMainWindow();

        if (result == DialogResult.Primary && Token == null)
        {
            //Token = page.GetPassword().ConvertToSecureString();
        }

        return this;
    }
}