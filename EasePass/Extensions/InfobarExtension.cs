using EasePass.Dialogs;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasePass.Extensions
{
    internal static class InfobarExtension
    {
        public static void Show(this InfoBar infobar, string title, string message, InfoBarSeverity severity, int showSeconds = 5)
        {
            Show(infobar, title, message, null, severity, showSeconds);
        }
        public static void Show(this InfoBar infobar, string title, string message, ButtonBase actionButton, InfoBarSeverity severity, int showSeconds = 5)
        {
            infobar.Title = title;
            infobar.Message = message;
            infobar.ActionButton = actionButton;
            infobar.Severity = severity;
            infobar.IsOpen = true;
            MainWindow.InfoMessagesPanel.Children.Add(infobar);

            DispatcherTimer autoCloseTimer = new DispatcherTimer();
            autoCloseTimer.Interval = new TimeSpan(0, 0, showSeconds);
            autoCloseTimer.Start();
            autoCloseTimer.Tick += delegate
            {
                infobar.IsOpen = false;
                autoCloseTimer.Stop();
            };
        }
    }
}
