using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasePass.Controls;
using EasePass.Helper;
using EasePass.Helper.Security.Generator;
using EasePass.Settings;
using System.Threading.Tasks;

namespace EasePass.ViewModels.Dialog;

public partial class GenPasswordViewModel : ObservableObject
{
    [ObservableProperty] private string _generatedPassword = "";
    [ObservableProperty] private bool _isGenerating;

    public GenPasswordViewModel()
    {
        _ = GeneratePasswordAsync();
    }

    [RelayCommand]
    private async Task GeneratePasswordAsync()
    {
        IsGenerating = true;
        GeneratedPassword = await PasswordHelper.GeneratePassword();
        IsGenerating = false;
    }
}
