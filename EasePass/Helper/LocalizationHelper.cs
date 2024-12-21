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
using EasePass.Settings;
using Microsoft.Windows.ApplicationModel.Resources;
using System.Collections.Generic;

namespace EasePass.Helper;

public class LocalizationHelper
{
    private ResourceManager resourceManager = new();
    public ResourceContext resourceContext;
    public ResourceMap resourceMap = null;

    public List<LanguageItem> languages = new();

    public LocalizationHelper()
    {
        resourceContext = resourceManager.CreateResourceContext();
        resourceMap = new ResourceManager().MainResourceMap;

    }

    public void SetLanguage(LanguageItem languageItem)
    {
        if (languages.Contains(languageItem))
        {
            Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = languageItem.Tag;

            SettingsManager.SaveSettings(AppSettingsValues.language, languageItem.Tag);
        }
    }

    public void Initialize()
    {
        RegisterLanguageFromResource();

        var languageTag = SettingsManager.GetSettings(AppSettingsValues.language, "en-US");
        var res = languages.Find(x => x.Tag == languageTag);
        if (res == null)
        {
            SetLanguage(languages.Find(x => x.Tag == "en-US"));
            return;
        }

        SetLanguage(res);
    }

    private void RegisterLanguageFromResource()
    {
        ResourceMap resourceMap = new ResourceManager().MainResourceMap.GetSubtree("LanguageList");
        uint length = resourceMap.ResourceCount;
        for (uint i = 0; i < length; i++)
        {
            var resource = resourceMap.GetValueByIndex(i);
            languages.Add(new LanguageItem(resource.Key, resource.Value.ValueAsString));
        }
    }
}
