using EasePassExtensibility;
using System;

namespace EasePass.Models;

internal class DummyAboutExtensionPage : IAboutPlugin
{
    public string PluginName => "Unknown name";

    public string PluginDescription => "Unknown description";

    public string PluginAuthor => "Unknown author";

    public string PluginAuthorURL => "";

    public Uri PluginIcon => new Uri("ms-appx:///Assets/AppIcon/Icon.png");
}
