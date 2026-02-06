using Avalonia.Controls;
using EasePass.AvaloniaUI.Views;
using EasePass.Dialogs;
using EasePass.Extensions;
using System.Security;
using System.Threading.Tasks;

namespace EasePass.AvaloniaUI.Dialogs;

internal class EnterSecondFactorDialog
{
    private BaseDialog dialog;
    private EnterSecondFactorPage page;
    public SecureString? Token { get; private set; }

    public async Task<EnterSecondFactorDialog> ShowAsync()
    {
        page = new EnterSecondFactorPage();

        dialog = new BaseDialog
        {
            Title = "Enter the SecondFactor Token of the Database".Localized("Dialogs_EnterSF_Title/Text"),
            Content = page,
            Width = 400,
            Height = 200
        };

        var doneButton = new Button { Content = "Done".Localized("Dialog_Button_Done/Text") };
        dialog.SetPrimaryButton(doneButton);

        var cancelButton = new Button { Content = "Cancel".Localized("Dialog_Button_Cancel/Text") };
        dialog.SetSecondaryButton(cancelButton);

        doneButton.Click += (_, __) =>
        {
            Token = page.GetPassword().ConvertToSecureString();
        };

        var result = await dialog.ShowDialogAsync(MainWindow.current);

        if (result == DialogResult.Primary && Token == null)
        {
            Token = page.GetPassword().ConvertToSecureString();
        }

        return this;
    }
}