using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasePassExtensibility
{
    public interface IAboutPlugin : IExtensionInterface
    {
        string PluginName { get; }
        string PluginDescription { get; }
        string PluginAuthor { get; }
        string PluginAuthorURL { get; }
        ImageSource PluginIcon { get; }
    }
}
