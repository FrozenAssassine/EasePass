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


using Avalonia.Platform.Storage;
using EasePass.Views;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasePass.Views;
using EasePass.AvaloniaUI;

namespace EasePass.Helper.FileSystem;

public class FilePickerHelper
{
    public static async Task<(string? path, bool success)> PickOpenFile(string[] extensions)
    {
        var options = new FilePickerOpenOptions
        {
            Title = "Select a file",
            AllowMultiple = false,
            FileTypeFilter = extensions.Select(f => new FilePickerFileType(f)).ToList()
        };

        var files = await App.StorageProvider.OpenFilePickerAsync(options);
        var file = files.FirstOrDefault();
        return file != null ? (file.Path.LocalPath, true) : (null, false);
    }

    public static async Task<(string? path, bool success)> PickSaveFile((string val, List<string> ext) extensions)
    {
        var options = new FilePickerSaveOptions
        {
            Title = "Save file",
            DefaultExtension = extensions.ext.FirstOrDefault() ?? "",
            FileTypeChoices = new List<FilePickerFileType>
        {
            new FilePickerFileType(extensions.val)
            {
                Patterns = extensions.ext.Select(e => "*." + e).ToList()
            }
        }
        };

        var file = await App.StorageProvider.SaveFilePickerAsync(options);
        return file != null ? (file.Path.LocalPath, true) : (null, false);
    }

    public static async Task<(string? path, bool success)> PickFolder()
    {
        var options = new FolderPickerOpenOptions
        {
            Title = "Select folder"
        };

        var folders = await App.StorageProvider.OpenFolderPickerAsync(options);
        var folder = folders.FirstOrDefault();
        return folder != null ? (folder.Path.LocalPath, true) : (null, false);
    }
}