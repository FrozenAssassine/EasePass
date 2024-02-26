using EasePass.Models;
using EasePassExtensibility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace EasePass.Helper;

public static class ExtensionHelper
{
    public static List<Extension> Extensions = new List<Extension>();

    public static void Init()
    {
        Task.Run(new Action(async () =>
        {
            if (Directory.Exists(ApplicationData.Current.LocalFolder.Path + "\\extensions\\"))
            {
                if (File.Exists(ApplicationData.Current.LocalFolder.Path + "\\delete_extensions.dat"))
                    foreach (string extensionID in File.ReadLines(ApplicationData.Current.LocalFolder.Path + "\\delete_extensions.dat"))
                        if (File.Exists(ApplicationData.Current.LocalFolder.Path + "\\extensions\\" + extensionID + ".dll"))
                            await (await (await ApplicationData.Current.LocalFolder.GetFolderAsync("extensions")).GetFileAsync(extensionID + ".dll")).DeleteAsync();
                File.WriteAllText(ApplicationData.Current.LocalFolder.Path + "\\delete_extensions.dat", "");
                string[] extensionPaths = Directory.GetFiles(ApplicationData.Current.LocalFolder.Path + "\\extensions\\");
                Extensions.Clear();
                for (int i = 0; i < extensionPaths.Length; i++)
                {
                    if (Path.GetExtension(extensionPaths[i]) == ".dll")
                        Extensions.Add(new Extension(ReflectionHelper.GetAllExternalInstances(extensionPaths[i]), Path.GetFileNameWithoutExtension(extensionPaths[i])));
                }
                for (int i = 0; i < Extensions.Count; i++)
                {
                    for (int j = 0; j < Extensions[i].Interfaces.Length; j++)
                    {
                        if(Extensions[i].Interfaces[j] is IDatabasePaths)
                        {
                            ((IDatabasePaths)Extensions[i].Interfaces[j]).Init(Database.GetAllDatabasePaths());
                        }
                    }
                }
            }
        }));
    }

    public static T[] GetAllClassesWithInterface<T>() where T : IExtensionInterface
    {
        List<T> result = new List<T>();
        for (int i = 0; i < Extensions.Count; i++)
        {
            for (int j = 0; j < Extensions[i].Interfaces.Length; j++)
            {
                if (Extensions[i].Interfaces[j] is T) result.Add((T)Extensions[i].Interfaces[j]);
            }
        }
        return result.ToArray();
    }

    public static async Task<List<FetchedExtension>> GetExtensionsFromServer()
    {
        string url = "https://github.com/FrozenAssassine/EasePass/raw/master/Plugins/PluginList.txt";

        var result = await RequestsHelper.MakeRequest(url);
        if (!result.success)
            return null;

        List<FetchedExtension> items = new();

        var splitted = result.result.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var plunginURL in splitted)
        {
            var pluginData = plunginURL.Split("|");
            if (pluginData.Length < 2)
                continue;

            items.Add(new FetchedExtension(pluginData[1], pluginData[0]));
        }
        return items;
    }
}
