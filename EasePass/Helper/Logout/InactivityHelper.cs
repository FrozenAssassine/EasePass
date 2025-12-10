/*
MIT License

Copyright (c) 2023 Julius Kirsch

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
*/

using EasePass.Settings;
using Microsoft.UI.Xaml;
using System;

namespace EasePass.Helper.Logout
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
            inactivityTimer.Interval = new TimeSpan(0, SettingsManager.GetSettingsAsInt(AppSettingsValues.inactivityLogoutTime, DefaultSettingsValues.inactivityLogoutTime), 0);

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
