using EasePassExtensibility;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Collections.Generic;
using System.Text;

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
            List<string> items = new List<string>();
            for (int i = 0; i < Interfaces.Length; i++)
            {
                if (Interfaces[i] is IPasswordImporter) items.Add("• register new passwords");
                if (Interfaces[i] is IPasswordGenerator) items.Add("• generate new passwords");
                if (Interfaces[i] is IDatabasePaths) items.Add("• retrieve database paths");
                // fill up with other interfaces
            }
            List<string> itemsFinal = new List<string>();
            for (int i = 0; i < items.Count; i++)
                if (!itemsFinal.Contains(items[i]))
                    itemsFinal.Add(items[i]);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Authorizations requested by plugin:");
            for (int i = 0; i < itemsFinal.Count; i++)
                sb.AppendLine(itemsFinal[i]);
            return sb.ToString();
        }
    }
}
