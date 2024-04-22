using EasePass.Settings;
using Microsoft.UI.Xaml;
using System;

namespace EasePass.Helper
{
    public class InactivityHelper
    {
        DispatcherTimer inactivityTimer = new DispatcherTimer();
        public bool PreventAutologout { get; set; } = false;

        public InactivityHelper()
        {
            inactivityTimer.Tick += InactivityTimer_Tick;
        }

        private void InactivityTimer_Tick(object sender, object e)
        {
            if (PreventAutologout)
            {
                inactivityTimer.Start();
                return;
            }

            InactivityStarted?.Invoke();
            inactivityTimer.Stop();
        }

        public void WindowDeactivated()
        {
            inactivityTimer.Interval = new TimeSpan(0, AppSettings.GetSettingsAsInt(AppSettingsValues.inactivityLogoutTime, DefaultSettingsValues.inactivityLogoutTime), 0);

            inactivityTimer.Stop();
            inactivityTimer.Start();
        }
        public void WindowActivated()
        {
            inactivityTimer.Stop();
        }

        public delegate void InactivityStartedEvent();
        public event InactivityStartedEvent InactivityStarted;
    }
}
