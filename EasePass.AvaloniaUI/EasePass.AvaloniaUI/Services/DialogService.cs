using Avalonia.Controls;
using EasePass.AvaloniaUI;
using EasePass.Dialogs;
using System.Threading.Tasks;

namespace EasePass.Services;

public static class DialogService
{
    public static Window MainWindow { get; set; }

    public static async Task<DialogResult> ShowAsyncWithPreventAutoLogout(this BaseDialog dialog)
    {
        App.MainVM.InactivityHelper.PreventAutologout = true;
        var result = await dialog.ShowDialogAsync(MainWindow);
        App.MainVM.InactivityHelper.PreventAutologout = false;
        return result;
    }
    public static async Task<DialogResult> ShowOnMainWindow(this BaseDialog dialog)
    {
        var result = await dialog.ShowDialogAsync(MainWindow);
        return result;
    }

}
