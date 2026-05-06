using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasePass.Helper;
using EasePass.Helper.Security.Generator;
using EasePass.Models;
using EasePass.Views;
using System.Threading.Tasks;
using static EasePassExtensibility.PasswordItem;

namespace EasePass.ViewModels.Dialog
{
    public partial class AddItemPageViewModel : ObservableObject
    {
        // This is the copy we bind to in the UI
        [ObservableProperty]
        private PasswordManagerItem _itemCopy;

        [ObservableProperty] private bool _show2FAInputs;
        [ObservableProperty] private bool _isGenerating;
        [ObservableProperty] private Bitmap? _qrCodeSource;
        [ObservableProperty] private string _tagsString = "";

        private readonly PasswordManagerItem? _originalItem;
        private readonly PasswordsPage.PasswordExists _pwExistsDelegate;

        public AddItemPageViewModel(PasswordsPage.PasswordExists pwExistsDelegate, PasswordManagerItem? inputItem = null)
        {
            _pwExistsDelegate = pwExistsDelegate;
            _originalItem = inputItem;

            //cloning the item, because we need one item to edit and one to keep original values if canceled
            ItemCopy = inputItem != null ? inputItem.CloneItem() : new PasswordManagerItem();

            if (ItemCopy.Tags != null)
                TagsString = string.Join(" ", ItemCopy.Tags);
            Show2FAInputs = !string.IsNullOrEmpty(ItemCopy.Secret);
        }

        [RelayCommand]
        private async Task GeneratePassword()
        {
            IsGenerating = true;
            ItemCopy.Password = await PasswordHelper.GeneratePassword();
            IsGenerating = false;
        }

        [RelayCommand]
        private void Remove2FA()
        {
            ItemCopy.Secret = string.Empty;
            Show2FAInputs = false;
            QrCodeSource = null;
        }

        public PasswordManagerItem GetResult()
        {
            if (_originalItem != null)
            {
                _originalItem.Password = ItemCopy.Password;
                _originalItem.Username = ItemCopy.Username;
                _originalItem.Email = ItemCopy.Email;
                _originalItem.Algorithm = ItemCopy.Algorithm;
                _originalItem.Notes = ItemCopy.Notes;
                _originalItem.DisplayName = ItemCopy.DisplayName;
                _originalItem.Website = ItemCopy.Website;

                PasswordItemTagHelper.ParseToTags(_tagsString, _originalItem);

                _originalItem.Secret = ItemCopy.Secret;
                _originalItem.Digits = ItemCopy.Digits;
                _originalItem.Interval = ItemCopy.Interval;

                return _originalItem;
            }
            return ItemCopy;
        }


        [RelayCommand]
        private void Export2FA()
        {
            var uri = TOTP.EncodeUrl(ItemCopy.DisplayName, ItemCopy.Username, ItemCopy.Email, ItemCopy.Secret,
                TOTP.StringToHashMode(ItemCopy.Algorithm), int.Parse(ItemCopy.Digits), int.Parse(ItemCopy.Interval));

            QrCodeSource = QRCodeScanner.GenerateAvaloniaBitmap(uri);
        }
    }
}
