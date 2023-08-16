using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System;
using Microsoft.UI.Xaml.Media;

namespace EasePass.Extensions
{
    internal static class InfobarExtension
    {
        public static void ShowWithoutTimer(this InfoBar infobar, string title, string message, InfoBarSeverity severity)
        {
            ShowInfobar(infobar, title, message, null, severity);
        }
        public static void Show(this InfoBar infobar, string title, string message, InfoBarSeverity severity, int showSeconds = 5)
        {
            Show(infobar, title, message, null, severity, showSeconds);
        }
        public static void Show(this InfoBar infobar, string title, string message, ButtonBase actionButton, InfoBarSeverity severity, int showSeconds = 5)
        {
            ShowInfobar(infobar, title, message, actionButton, severity);
            
            DispatcherTimer autoCloseTimer = new DispatcherTimer();
            autoCloseTimer.Interval = new TimeSpan(0, 0, showSeconds);
            autoCloseTimer.Start();
            autoCloseTimer.Tick += delegate
            {
                infobar.IsOpen = false;
                autoCloseTimer.Stop();
            };
        }

        private static void ShowInfobar(this InfoBar infobar, string title, string message, ButtonBase actionButton, InfoBarSeverity severity)
        {
            infobar.Title = title;
            infobar.Message = message;
            infobar.ActionButton = actionButton;
            infobar.Severity = severity;
            infobar.IsOpen = true;
            infobar.Background = Application.Current.Resources["SolidBackgroundFillColorBaseAltBrush"] as Brush;
            MainWindow.InfoMessagesPanel.Children.Add(infobar);
        }
    }
}
