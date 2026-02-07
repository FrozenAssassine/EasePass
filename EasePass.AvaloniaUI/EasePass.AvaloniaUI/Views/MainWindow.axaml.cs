using Avalonia.Controls;
using Avalonia.Controls.Chrome;
using Avalonia.Interactivity;
using EasePass.Core;
using EasePass.Core.Database;
using EasePass.Dialogs;
using EasePass.Helper.App;
using EasePass.Helper.Logout;
using EasePass.Helper.Security.Generator;
using EasePass.Manager;
using EasePass.Models.Logger;
using EasePass.Settings;
using EasePass.ViewModels;
using EasePassExtensibility;
using System.Threading.Tasks;

namespace EasePass.Views
{
    public partial class MainWindow : Window
    {
        public static MainWindow current;

        public static StackPanel InfoMessagesPanel;

        public InactivityManager inactivityHelper = new InactivityManager();
        public bool ShowBackArrow { get; set; } //todo implement { get => navigateBackButton.Visibility == Visibility.Visible; set => navigateBackButton.Visibility = value ? Visibility.Visible : Visibility.Collapsed; }

        public static MainWindow CurrentInstance = null;

        public static LocalizationManager localizationHelper = new LocalizationManager();

        public readonly RestoreWindowManager restoreWindowManager;
        public readonly WindowStateManager windowStateManager;
        public readonly ExtensionManager extensionManager;

        public MainWindow()
        {
            this.InitializeComponent();

            current = this;
            InfoMessagesPanel = infoMessagesPanel;

            LoggingManager.Logger = new MultiLogger(new FileLogger(), new DebugLineLogger()); // To disable, use "new NoLogger()"
            LoggingManager.InitializeCurrentLogger();

            CurrentInstance = this;

            windowStateManager = new WindowStateManager(this);
            restoreWindowManager = new RestoreWindowManager(this, windowStateManager);
            extensionManager = new ExtensionManager();
            extensionManager.Init();

            restoreWindowManager.RestoreSettings();

            localizationHelper.Initialize();

            Title = "Ease Pass";

            inactivityHelper.InactivityStarted += InactivityHelper_InactivityStarted;

            PasswordHelper.Init();

            InfoMessagesPanel = infoMessagesPanel;
            ShowBackArrow = false;

            this.Closing += MainWindow_Closing;

            //start wiht login page todo: show register page if needed
            NavigationHelper.ToLoginPage();
        }

        private bool _isClosingForcefully = false;

        public async Task<bool> DoMasterSaveWithProgress()
        {
            //databaseSavingProgressRing.Visibility = Visibility.Visible;

            bool saveRes = await Task.Run(async () => await Database.LoadedInstance.ForceSaveAsync());

            //databaseSavingProgressRing.Visibility = Visibility.Collapsed;
            return saveRes;
        }

        private async void MainWindow_Closing(object? sender, WindowClosingEventArgs e)
        {
            if (_isClosingForcefully) return;

            if (Database.LoadedInstance != null && Database.LoadedInstance.deferredSaver.SaveScheduled)
            {
                e.Cancel = true;

                if (!await DoMasterSaveWithProgress())
                    return;

                _isClosingForcefully = true;
                this.Close();
            }

            Database.LoadedInstance?.Dispose();

            LoggingManager.Logger.Flush();
        }

        private void InactivityHelper_InactivityStarted()
        {
            //todo inactivity
            //if (this.navigationFrame.CurrentSourcePageType != typeof(LoginPage) &&
            //    this.navigationFrame.CurrentSourcePageType != typeof(RegisterPage))
            //{
            //    //do not trigger auto logout, when there is an important dialog open e.g. edit or add item dialog
            //    if (!AutoLogoutContentDialog.InactivityStarted())
            //        return;

            //    LogoutHelper.Logout();
            //    InfoMessages.AutomaticallyLoggedOut();
            //    Database.LoadedInstance.Dispose();
            //}
        }

        private void NavigateBack_Click(object sender, RoutedEventArgs e)
        {
            //navigationFrame.GoBack();
        }

        //private void Window_Activated(object sender, WindowActivatedEventArgs args)
        //{
        //    if (args.WindowActivationState == WindowActivationState.Deactivated)
        //        inactivityHelper.WindowDeactivated();
        //    else
        //        inactivityHelper.WindowActivated();
        //}
    }
}

