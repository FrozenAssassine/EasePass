using EasePassExtensibility;
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

        public Extension(IExtensionInterface[] interfaces)
        {
            Interfaces = interfaces;
            for(int i = 0; i < interfaces.Length; i++)
            {
                if (interfaces[i] is IAboutPlugin)
                    AboutPlugin = (IAboutPlugin)interfaces[i];
            }
        }
    }
}
