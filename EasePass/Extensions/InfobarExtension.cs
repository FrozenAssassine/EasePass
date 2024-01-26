using EasePass.Helper;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using System;

namespace EasePass.Extensions
{
    enum InfobarClearCondition
    {
        Timer,
        Login,
        Manual,
    }

    internal static class InfobarExtension
    {
        public static void ShowUntilLogin(this InfoBar infobar, string localizationKey, InfoBarSeverity severity)
        {
            ShowInfobar(infobar, "".Localized(localizationKey + "/Headline"), "".Localized(localizationKey + "/Text"), null, severity, InfobarClearCondition.Login);
        }
        public static void ShowUntilLogin(this InfoBar infobar, string title, string message, InfoBarSeverity severity)
        {
            ShowInfobar(infobar, title, message, null, severity, InfobarClearCondition.Login);
        }
        public static void Show(this InfoBar infobar, string localizationKey, InfoBarSeverity severity, int showSeconds = 8)
        {
            Show(infobar, "".Localized(localizationKey + "/Headline"), "".Localized(localizationKey + "/Text"), null, severity, InfobarClearCondition.Timer, showSeconds);
        }
        public static void Show(this InfoBar infobar, string title, string message, InfoBarSeverity severity, int showSeconds = 8)
        {
            Show(infobar, title, message, null, severity, InfobarClearCondition.Timer, showSeconds);
        }
        public static void Show(this InfoBar infobar, string title, string message, ButtonBase actionButton, InfoBarSeverity severity, InfobarClearCondition clearCondition = InfobarClearCondition.Timer, int showSeconds = 5)
        {
            ShowInfobar(infobar, title, message, actionButton, severity, clearCondition);

            DispatcherTimer autoCloseTimer = new DispatcherTimer();
            autoCloseTimer.Interval = new TimeSpan(0, 0, showSeconds);
            autoCloseTimer.Start();
            autoCloseTimer.Tick += delegate
            {
                infobar.IsOpen = false;
                autoCloseTimer.Stop();
            };
        }

        private static void ShowInfobar(this InfoBar infobar, string title, string message, ButtonBase actionButton, InfoBarSeverity severity, InfobarClearCondition clearCondition)
        {
            infobar.Title = title;
            infobar.Message = message;
            infobar.ActionButton = actionButton;
            infobar.Severity = severity;
            infobar.Tag = clearCondition;
            infobar.IsOpen = true;
            infobar.Background = Application.Current.Resources["SolidBackgroundFillColorBaseAltBrush"] as Brush;
            MainWindow.InfoMessagesPanel.Children.Add(infobar);
        }
        public static void ShowInfobar(this InfoBar infobar, string title, string message, Control content, InfoBarSeverity severity)
        {
            infobar.Title = title;
            infobar.Message = message;
            infobar.Severity = severity;
            infobar.Content = content;
            infobar.Tag = InfobarClearCondition.Manual;
            infobar.IsOpen = true;
            infobar.Background = Application.Current.Resources["SolidBackgroundFillColorBaseAltBrush"] as Brush;
            MainWindow.InfoMessagesPanel.Children.Add(infobar);
        }

        public static void ClearInfobarsAfterLogin(StackPanel infobarDisplay)
        {
            foreach (InfoBar infobar in infobarDisplay.Children)
            {
                if (infobar == null || ConvertHelper.ToEnum(infobar.Tag, InfobarClearCondition.Timer) == InfobarClearCondition.Timer)
                    continue;

                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval += new TimeSpan(0, 0, 4);
                timer.Start();
                timer.Tick += (e, i) =>
                {
                    infobarDisplay.Children.Remove(infobar);
                };
            }
        }
    }
}
