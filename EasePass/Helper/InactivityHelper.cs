using EasePass.Settings;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasePass.Helper
{
    internal class InactivityHelper
    {
        DispatcherTimer inactivityTimer = new DispatcherTimer();

        public InactivityHelper()
        {
            inactivityTimer.Tick += InactivityTimer_Tick;
        }

        private void InactivityTimer_Tick(object sender, object e)
        {
            inactivityTimer.Stop();
            InactivityStarted.Invoke();
        }

        public void WindowDeactivated()
        {
            inactivityTimer.Interval += new TimeSpan(0, AppSettings.GetSettingsAsInt(AppSettingsValues.inactivityLogoutTime, DefaultSettingsValues.inactivityLogoutTime), 0);

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
