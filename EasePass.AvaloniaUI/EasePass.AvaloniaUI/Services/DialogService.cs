using Avalonia.Controls;
using EasePass.AvaloniaUI;
using EasePass.Dialogs;
using System.Threading.Tasks;

namespace EasePass.Services;

public enum DialogResult { Primary, Secondary, Cancel }

public static class DialogService
{
    public static TopLevel topLevel { get; set; }

    public static async Task<DialogResult> ShowAsyncWithPreventAutoLogout(this BaseDialog dialog)
    {
        App.MainVM.InactivityHelper.PreventAutologout = true;
        var result = await dialog.ShowAsync(topLevel);
        App.MainVM.InactivityHelper.PreventAutologout = false;
        return result;
    }
    public static async Task<DialogResult> ShowOnMainView(this BaseDialog dialog)
    {
        var result = await dialog.ShowAsync(topLevel);
        return result;
    }

}
