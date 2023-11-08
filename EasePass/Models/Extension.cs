using EasePass.Helper;
using EasePassExtensibility;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasePass.Models
{
    public class Extension
    {
        public IExtensionInterface[] Interfaces;

        public IAboutPlugin AboutPlugin = null;

        public ImageSource IconSource
        {
            get
            {
                if (AboutPlugin != null) return new BitmapImage(AboutPlugin.PluginIcon);
                return null;
            }
        }

        public string ID = "";

        public Extension(IExtensionInterface[] interfaces, string id)
        {
            Interfaces = interfaces;
            for (int i = 0; i < interfaces.Length; i++)
            {
                if (interfaces[i] is IAboutPlugin)
                    AboutPlugin = (IAboutPlugin)interfaces[i];
            }
            if (AboutPlugin == null) AboutPlugin = new DummyAboutExtensionPage();
            ID = id;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Authorizations requested by plugin:");
            for(int i = 0; i < Interfaces.Length; i++)
            {
                if (Interfaces[i] is IPasswordImporter) sb.AppendLine("• register new passwords");
                // fill up with other interfaces
            }
            return sb.ToString();
        }
    }
}
