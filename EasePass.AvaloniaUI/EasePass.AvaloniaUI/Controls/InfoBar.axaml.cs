using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using EasePass.Extensions;

namespace EasePass.Controls;

public enum InfoBarSeverity
{
    Info,
    Warning,
    Error,
    Success,
}

public enum InfobarClearCondition
{
    Timer,
    Login,
    Manual,
}

public class InfoBar : TemplatedControl
{
    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<InfoBar, string>(nameof(Title));

    public static readonly StyledProperty<string> MessageProperty =
        AvaloniaProperty.Register<InfoBar, string>(nameof(Message));

    public static readonly StyledProperty<Control> ActionContentProperty =
        AvaloniaProperty.Register<InfoBar, Control>(nameof(ActionContent));

    public static readonly StyledProperty<InfoBarSeverity> SeverityProperty =
        AvaloniaProperty.Register<InfoBar, InfoBarSeverity>(nameof(Severity));

    public static readonly StyledProperty<bool> IsOpenProperty =
        AvaloniaProperty.Register<InfoBar, bool>(nameof(IsOpen));

    public static readonly StyledProperty<InfobarClearCondition> ClearConditionProperty =
        AvaloniaProperty.Register<InfoBar, InfobarClearCondition>(nameof(ClearCondition));

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Message
    {
        get => GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    public Control ActionContent
    {
        get => GetValue(ActionContentProperty);
        set => SetValue(ActionContentProperty, value);
    }

    public InfoBarSeverity Severity
    {
        get => GetValue(SeverityProperty);
        set => SetValue(SeverityProperty, value);
    }

    public bool IsOpen
    {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    public InfobarClearCondition ClearCondition
    {
        get => GetValue(ClearConditionProperty);
        set => SetValue(ClearConditionProperty, value);
    }
}