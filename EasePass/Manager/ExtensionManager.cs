using EasePass.Dialogs;
using EasePass.Helper;
using EasePass.Helper.Extension;
using EasePass.Helper.FileSystem;
using EasePass.Models;
using EasePass.Settings;
using EasePassExtensibility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace EasePass.Manager;

public class ExtensionManager
{
    public delegate void ExtensionsInitializedEvent();
    public event ExtensionsInitializedEvent ExtensionsInitialized;
    public bool ExtensionsLoaded { get; private set; } = false;
    public List<Extension> Extensions { get; } = new ();
    public List<IDatabaseSource> DatabaseSources { get; } = new();

    //todo @finn use them everywhere
    public const string ExtensionsFolderName = "extensions"; 
    public const string ToDeletedExtensionFileName = "delete_extensions.dat"; 
    private string ToDeletedExtensionPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, ToDeletedExtensionFileName);
    private string ExtensionsFolderPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, ExtensionsFolderName); 


    public void Init()
    {
        if (!Directory.Exists(ExtensionsFolderPath))
        {
            HandleExtensionsLoaded();
            return;
        }

        DeleteMarkedExtensions();

        Extensions.Clear();
        DatabaseSources.Clear();

        string[] extensionPaths = Directory.GetFiles(ExtensionsFolderPath);
        for (int i = 0; i < extensionPaths.Length; i++)
        {
            if (FileHelper.HasExtension(extensionPaths[i], ".dll"))
            {
                var extension = LoadExtension(extensionPaths[i]);
            }
        }

        HandleExtensionsLoaded();
    }

    private void HandleExtensionsLoaded()
    {
        ExtensionsInitialized?.Invoke();
        ExtensionsLoaded = true;
    }

    private void DeleteMarkedExtensions()
    {
        // Delete extensions that are marked for deletion
        if (!File.Exists(ToDeletedExtensionPath))
            return;
        foreach (string extensionID in File.ReadLines(ToDeletedExtensionPath))
        {
            string extensionFile = Path.Combine(ExtensionsFolderPath, extensionID + ".dll");
            if (!File.Exists(extensionFile))
                continue;

            File.Delete(extensionFile);

            if (AppSettings.RemovePluginSettingsOnUninstall)
                PluginStorageHelper.Clean(extensionID);
        }
        
        File.WriteAllText(ToDeletedExtensionPath, "");
    }


    public T[] GetAllClassesWithInterface<T>() where T : IExtensionInterface
    {
        List<T> result = new List<T>();
        for (int i = 0; i < Extensions.Count; i++)
            for (int j = 0; j < Extensions[i].Interfaces.Length; j++)
                if (Extensions[i].Interfaces[j] is T t)
                    result.Add(t);
        return result.ToArray();
    }
    public List<FetchedExtension> GetExtensionsFromSources()
    {
        List<FetchedExtension> res = new List<FetchedExtension>();
        for (int i = 0; i < Extensions.Count; i++)
            for (int j = 0; j < Extensions[i].Interfaces.Length; j++)
                if (Extensions[i].Interfaces[j] is IExtensionSource source)
                    res.AddRange(source.GetExtensionSources().Select((item) => { return new FetchedExtension(item.Source, item.Name, source.SourceName); }));
        return res;
    }


    public Extension LoadExtension(string path)
    {
        string id = Path.GetFileNameWithoutExtension(path);
        var interfaces = ReflectionHelper.GetAllExternalInstances(path);
        if (interfaces.Length == 0)
            return null;

        // Do not compress into single loop, because injection must be done before initialization
        foreach (var extInterface in interfaces)
            Inject(extInterface, id);

        foreach (var extInterface in interfaces)
            InitExtension(extInterface, id);

        Extension e = new Extension(interfaces, id);
        if(e != null)
            Extensions.Add(e);

        return e;
    }

    private void InitExtension(IExtensionInterface obj, string pluginID)
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
        if (obj is IDatabaseProvider dbProv)
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

    private void Inject(IExtensionInterface obj, string pluginID)
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

        string FilePicker(string[] extensions)
        {
            var res = Task.Run(async () => await FilePickerHelper.PickOpenFile(extensions)).Result;
            if (res.success)
                return res.path;
            return "";
        }
    }
}