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

using Avalonia.Threading;
using EasePass.AvaloniaUI;
using EasePass.Controls;
using EasePass.ViewModels;
using System;
using System.Diagnostics;

namespace EasePass.Extensions;

public enum InfobarClearCondition
{
    Timer,
    Login,
    Manual,
}


public enum InfoBarSeverity
{
    Informational,
    Warning,
    Success,
    Error,
}

public class InfobarExtension
{
    public static void ShowUntilLogin(string localizationKey, InfoBarSeverity severity)
    {
        //todo: CreateAndShow("".Localized(localizationKey + "/Headline"), "".Localized(localizationKey + "/Text"), null, severity, InfobarClearCondition.Login);
        CreateAndShow(localizationKey + "/Headline", localizationKey + "/Text", null, severity, InfobarClearCondition.Login);

    }

    public static void ShowUntilLogin(string title, string message, InfoBarSeverity severity)
    {
        CreateAndShow(title, message, null, severity, InfobarClearCondition.Login);
    }

    public static void Show(string localizationKey, InfoBarSeverity severity, int showSeconds = 8)
    {
        //todo localization
        CreateAndShow(localizationKey + "/Headline", localizationKey + "/Text", null, severity, InfobarClearCondition.Timer, showSeconds);
    }

    public static void Show(string title, string message, InfoBarSeverity severity, int showSeconds = 8)
    {
        CreateAndShow(title, message, null, severity, InfobarClearCondition.Timer, showSeconds);
    }

    public static void Show(string title, string message, object content, InfoBarSeverity severity, int showSeconds = 8)
    {
        CreateAndShow(title, message, content, severity, InfobarClearCondition.Timer, showSeconds);
    }


    private static void CreateAndShow(string title, string message, object actionContent, InfoBarSeverity severity, InfobarClearCondition clearCondition, int seconds = 8)
    {
        var note = new NotificationViewModel
        {
            Title = title,
            Message = message,
            ActionContent = actionContent,
            Severity = severity,
            ClearCondition = clearCondition
        };

        App.MainVM.Notifications.Add(note);

        if (clearCondition == InfobarClearCondition.Timer)
        {
            DispatcherTimer.RunOnce(() =>
            {
                note.IsOpen = false;
                App.MainVM.Notifications.Remove(note);
            }, TimeSpan.FromSeconds(seconds));
        }
    }

    public static void ClearAfterLogin()
    {
        for (int i = App.MainVM.Notifications.Count - 1; i >= 0; i--)
        {
            var note = App.MainVM.Notifications[i];
            if (note.ClearCondition == InfobarClearCondition.Login)
            {
                note.IsOpen = false;
                App.MainVM.Notifications.RemoveAt(i);
            }
        }
    }
}
