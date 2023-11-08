using EasePass.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace EasePass.Helper
{
    public static class ExtensionHelper
    {
        public static List<Extension> Extensions = new List<Extension>();

        public static void Init()
        {
            Task.Run(new Action(async () =>
            {
                if(Directory.Exists(ApplicationData.Current.LocalFolder.Path + "\\extensions\\"))
                {
                    if(File.Exists(ApplicationData.Current.LocalFolder.Path + "\\delete_extensions.dat"))
                        foreach (string extensionID in File.ReadLines(ApplicationData.Current.LocalFolder.Path + "\\delete_extensions.dat"))
                            if (File.Exists(ApplicationData.Current.LocalFolder.Path + "\\extensions\\" + extensionID + ".dll"))
                                await (await (await ApplicationData.Current.LocalFolder.GetFolderAsync("extensions")).GetFileAsync(extensionID + ".dll")).DeleteAsync();
                    string[] extensionPaths = Directory.GetFiles(ApplicationData.Current.LocalFolder.Path + "\\extensions\\");
                    Extensions.Clear();
                    for (int i = 0; i < extensionPaths.Length; i++)
                    {
                        Extensions.Add(new Extension(ReflectionHelper.GetAllExternalInstances(extensionPaths[i]), Path.GetFileNameWithoutExtension(extensionPaths[i])));
                    }
                }
            }));
        }
    }
}
