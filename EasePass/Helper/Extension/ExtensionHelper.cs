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
using EasePass.Dialogs;
using EasePass.Helper.FileSystem;
using EasePass.Models;
using EasePass.Settings;
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

    public static Task Init()
    {
        return Task.Run(new Action(async () =>
        {
            if (!Directory.Exists(ApplicationData.Current.LocalFolder.Path + "\\extensions\\"))
                return;

            // Delete extensions that are marked for deletion
            if (File.Exists(ApplicationData.Current.LocalFolder.Path + "\\delete_extensions.dat"))
            {
                foreach (string extensionID in File.ReadLines(ApplicationData.Current.LocalFolder.Path + "\\delete_extensions.dat"))
                {
                    if (File.Exists(ApplicationData.Current.LocalFolder.Path + "\\extensions\\" + extensionID + ".dll"))
                    {
                        await (await (await ApplicationData.Current.LocalFolder.GetFolderAsync("extensions")).GetFileAsync(extensionID + ".dll")).DeleteAsync();
                        if (AppSettings.RemovePluginSettingsOnUninstall)
                            PluginStorageHelper.Clean(extensionID);
                    }
                }
            }
            File.WriteAllText(ApplicationData.Current.LocalFolder.Path + "\\delete_extensions.dat", "");

            Extensions.Clear();
            DatabaseSources.Clear();

            // Load all extensions
            string[] extensionPaths = Directory.GetFiles(ApplicationData.Current.LocalFolder.Path + "\\extensions\\");
            for (int i = 0; i < extensionPaths.Length; i++)
                if (FileHelper.HasExtension(extensionPaths[i], ".dll"))
                    LoadExtension(extensionPaths[i]);
        }));
    }

    public static Models.Extension LoadExtension(string path)
    {
        string id = Path.GetFileNameWithoutExtension(path);
        var interfaces = ReflectionHelper.GetAllExternalInstances(path);
        if (interfaces.Length == 0)
            return null;

        // Do not compress into single loop, because injection must be done before initialization
        foreach (var extInterface in interfaces)
            Inject(extInterface, id);

        foreach (var extInterface in interfaces)
            Init(extInterface, id);

        var extension = new Models.Extension(interfaces, id);
        Extensions.Add(extension);

        return extension;
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

    private static void Inject(IExtensionInterface obj, string pluginID)
    {
        if (obj is IFilePickerInjectable filePicker)
        {
            try
            {
                filePicker.FilePicker = FilePicker;
            }
            catch { }
        }
        if (obj is IStorageInjectable storageInjectable)
        {
            try
            {
                PluginStorageHelper.Initialize(storageInjectable, pluginID);
            }
            catch { }
        }
    }

    private static void Init(IExtensionInterface obj, string pluginID)
    {
        if (obj is IInitializer initializer)
        {
            try
            {
                initializer.Init();
            }
            catch { }
        }
        if (obj is IDatabasePaths p)
        {
            try
            {
                p.Init(Core.Database.Database.GetAllDatabasePaths());
            }
            catch { }
        }
        if(obj is IDatabaseProvider dbProv)
        {
            try
            {
                dbProv.GetDatabases().ToList().ForEach((item) =>
                {
                    DatabaseSources.Add(new DatabaseSourceErrorHandlingWrapper(item));
                });
            }
            catch
            {
                UIThreadInvoker.Invoke(() => InfoMessages.DatabaseProviderLoadingFailed(dbProv.SourceName));
            }
        }
    }

    // For IFilePickerInjectable
    private static string FilePicker(string[] extensions)
    {
        var res = Task.Run(async () => await FilePickerHelper.PickOpenFile(extensions)).Result;
        if (res.success)
            return res.path;
        return "";
    }
}
