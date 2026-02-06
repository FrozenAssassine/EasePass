using Avalonia.Controls;
using Avalonia.Controls.Chrome;
using Avalonia.Interactivity;
using EasePass.Settings;
using EasePass.Core;
using EasePass.Core.Database;
using EasePass.Dialogs;
using EasePass.Helper.Logout;
using EasePass.Helper.Security.Generator;
using EasePass.Manager;
using EasePass.Models.Logger;
using System.Threading.Tasks;

namespace EasePass.Views
{
    public partial class MainWindow : Window
    {
        public static Window current;

        public static StackPanel InfoMessagesPanel;

        public InactivityManager inactivityHelper = new InactivityManager();
        public Frame MainFrame => navigationFrame;
        public bool ShowBackArrow { get => navigateBackButton.Visibility == Visibility.Visible; set => navigateBackButton.Visibility = value ? Visibility.Visible : Visibility.Collapsed; }

        public static MainWindow CurrentInstance = null;

        public static DispatcherQueue UIDispatcherQueue = null;
        public static XamlRoot XamlRoot = null;
        public static LocalizationManager localizationHelper = new LocalizationManager();

        public readonly RestoreWindowManager restoreWindowManager;
        public readonly WindowStateManager windowStateManager;
        public readonly ExtensionManager extensionManager;

        public MainWindow()
        {
            InitializeComponent();
            current = this;
            InfoMessagesPanel = infoMessagesPanel;

            ApplicationData.Initialize();
        }



        public MainWindow()
        {
            this.InitializeComponent();

            LoggingManager.Logger = new MultiLogger(new FileLogger(), new DebugLineLogger()); // To disable, use "new NoLogger()"
            LoggingManager.InitializeCurrentLogger();

            CurrentInstance = this;
            UIDispatcherQueue = DispatcherQueue.GetForCurrentThread();

            windowStateManager = new WindowStateManager(this);
            restoreWindowManager = new RestoreWindowManager(this, windowStateManager);
            extensionManager = new ExtensionManager();
            extensionManager.Init();

            restoreWindowManager.RestoreSettings();

            localizationHelper.Initialize();

            Title = Package.Current.DisplayName;
            this.AppWindow.SetIcon(Path.Combine(Package.Current.InstalledLocation.Path, "Assets\\AppIcon\\appicon.ico"));

            inactivityHelper.InactivityStarted += InactivityHelper_InactivityStarted;

            PasswordHelper.Init();

            InfoMessagesPanel = infoMessagesPanel;
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(titleBar);
            ShowBackArrow = false;

            this.AppWindow.Closing += AppWindow_Closing;
        }

        private bool _isClosingForcefully = false;

        public async Task<bool> DoMasterSaveWithProgress()
        {
            databaseSavingProgressRing.Visibility = Visibility.Visible;

            bool saveRes = await Task.Run(async () => await Database.LoadedInstance.ForceSaveAsync());

            databaseSavingProgressRing.Visibility = Visibility.Collapsed;
            return saveRes;
        }

        private async void AppWindow_Closing(Microsoft.UI.Windowing.AppWindow sender, Microsoft.UI.Windowing.AppWindowClosingEventArgs args)
        {
            if (_isClosingForcefully) return;

            if (Database.LoadedInstance != null && Database.LoadedInstance.deferredSaver.SaveScheduled)
            {
                args.Cancel = true;

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
            if (this.navigationFrame.CurrentSourcePageType != typeof(LoginPage) &&
                this.navigationFrame.CurrentSourcePageType != typeof(RegisterPage))
            {
                //do not trigger auto logout, when there is an important dialog open e.g. edit or add item dialog
                if (!AutoLogoutContentDialog.InactivityStarted())
                    return;

                LogoutHelper.Logout();
                InfoMessages.AutomaticallyLoggedOut();
                Database.LoadedInstance.Dispose();
            }
        }

        private void NavigateBack_Click(object sender, RoutedEventArgs e)
        {
            navigationFrame.GoBack();
        }

        private void Window_Activated(object sender, WindowActivatedEventArgs args)
        {
            if (args.WindowActivationState == WindowActivationState.Deactivated)
                inactivityHelper.WindowDeactivated();
            else
                inactivityHelper.WindowActivated();
        }
    }

