/*
MIT License

Copyright (c) 2023 Julius Kirsch

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
*/

using EasePass.Models;
using EasePassExtensibility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                {
                    foreach (string extensionID in File.ReadLines(ApplicationData.Current.LocalFolder.Path + "\\delete_extensions.dat"))
                    {
                        if (File.Exists(ApplicationData.Current.LocalFolder.Path + "\\extensions\\" + extensionID + ".dll"))
                            await (await (await ApplicationData.Current.LocalFolder.GetFolderAsync("extensions")).GetFileAsync(extensionID + ".dll")).DeleteAsync();
                    }
                }

                File.WriteAllText(ApplicationData.Current.LocalFolder.Path + "\\delete_extensions.dat", "");
                string[] extensionPaths = Directory.GetFiles(ApplicationData.Current.LocalFolder.Path + "\\extensions\\");
                Extensions.Clear();
                int length = extensionPaths.Length;
                for (int i = 0; i < length; i++)
                {
                    if (Path.GetExtension(extensionPaths[i]) == ".dll")
                        Extensions.Add(new Extension(ReflectionHelper.GetAllExternalInstances(extensionPaths[i]), Path.GetFileNameWithoutExtension(extensionPaths[i])));
                }
                length = Extensions.Count;
                for (int i = 0; i < length; i++)
                {
                    int length2 = Extensions[i].Interfaces.Length;
                    for (int j = 0; j < length2; j++)
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
        int length = Extensions.Count;
        for (int i = 0; i < length; i++)
        {
            int length2 = Extensions[i].Interfaces.Length;
            for (int j = 0; j < length2; j++)
            {
                if (Extensions[i].Interfaces[j] is T t) result.Add(t);
            }
        }
        return result.ToArray();
    }

    public static List<FetchedExtension> GetExtensionsFromSources()
    {
        List<FetchedExtension> res = new List<FetchedExtension>();
        int length = Extensions.Count;
        for(int i = 0; i < length; i++)
        {
            int length2 = Extensions[i].Interfaces.Length;
            for (int j = 0; j < length2; j++)
            {
                if (Extensions[i].Interfaces[j] is IExtensionSource source)
                {
                    res.AddRange(source.GetExtensionSources().Select((item) => { return new FetchedExtension(item.Source, item.Name, source.SourceName); }));
                }
            }
        }
        return res;
    }
}
