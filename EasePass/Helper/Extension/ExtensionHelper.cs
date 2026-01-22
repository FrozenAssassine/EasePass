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

using EasePass.Core.Database;
using EasePass.Helper.FileSystem;
using EasePass.Models;
using EasePassExtensibility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace EasePass.Helper.Extension;

public static class ExtensionHelper
{
    public static List<Models.Extension> Extensions = new List<Models.Extension>();
    public static List<IDatabaseSource> DatabaseSources = new List<IDatabaseSource>();

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
                        {
                            await (await (await ApplicationData.Current.LocalFolder.GetFolderAsync("extensions")).GetFileAsync(extensionID + ".dll")).DeleteAsync();
                        }
                    }
                }

                File.WriteAllText(ApplicationData.Current.LocalFolder.Path + "\\delete_extensions.dat", "");
                string[] extensionPaths = Directory.GetFiles(ApplicationData.Current.LocalFolder.Path + "\\extensions\\");
                Extensions.Clear();
                for (int i = 0; i < extensionPaths.Length; i++)
                {
                    if (FileHelper.HasExtension(extensionPaths[i], ".dll"))
                    {
                        Extensions.Add(new Models.Extension(ReflectionHelper.GetAllExternalInstances(extensionPaths[i]), Path.GetFileNameWithoutExtension(extensionPaths[i])));
                    }
                }

                for (int i = 0; i < Extensions.Count; i++)
                {
                    int length = Extensions[i].Interfaces.Length;
                    for (int j = 0; j < length; j++)
                    {
                        if(Extensions[i].Interfaces[j] is IDatabasePaths)
                        {
                            ((IDatabasePaths)Extensions[i].Interfaces[j]).Init(Core.Database.Database.GetAllDatabasePaths());
                        }
                    }
                }

                foreach(IDatabaseProvider dbProv in GetAllClassesWithInterface<IDatabaseProvider>())
                {
                    dbProv.GetDatabases().ToList().ForEach((item) =>
                    {
                        DatabaseSources.Add(item);
                    });
                }
            }
        }));
    }

    public static T[] GetAllClassesWithInterface<T>() where T : IExtensionInterface
    {
        List<T> result = new List<T>();
        for (int i = 0; i < Extensions.Count; i++)
        {
            int length = Extensions[i].Interfaces.Length;
            for (int j = 0; j < length; j++)
            {
                if (Extensions[i].Interfaces[j] is T t)
                {
                    result.Add(t);
                }
            }
        }
        return result.ToArray();
    }

    public static List<FetchedExtension> GetExtensionsFromSources()
    {
        List<FetchedExtension> res = new List<FetchedExtension>();
        for(int i = 0; i < Extensions.Count; i++)
        {
            int length = Extensions[i].Interfaces.Length;
            for (int j = 0; j < length; j++)
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
