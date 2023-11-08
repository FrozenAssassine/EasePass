using EasePass.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasePass.Helper
{
    public static class ExtensionHelper
    {
        public static List<Extension> Extensions = new List<Extension>();

        public static void Init()
        {
            Task.Run(new Action(() =>
            {
                string[] extensionPaths = new string[0]; // @FrozenAssassine load the array with the file paths of the extension dlls from appdata folder
                Extensions.Clear();
                for(int i = 0; i < extensionPaths.Length; i++)
                {
                    Extensions.Add(new Extension(ReflectionHelper.GetAllExternalInstances(extensionPaths[i])));
                }
            }));
        }
    }
}
