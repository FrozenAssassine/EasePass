using CommunityToolkit.Mvvm.ComponentModel;
using EasePass.Controls;
using EasePass.Extensions;
using System;

namespace EasePass.ViewModels;

public partial class NotificationViewModel : ObservableObject
{
    [ObservableProperty] private string _title;
    [ObservableProperty] private string _message;
    [ObservableProperty] private InfoBarSeverity _severity;
    [ObservableProperty] private bool _isOpen = true;
    [ObservableProperty] private object _actionContent;

    public InfobarClearCondition ClearCondition { get; set; }
    public DateTime CreatedAt { get; } = DateTime.Now;
}
