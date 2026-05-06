using EasePass.Models;
using EasePass.Settings;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using System.Threading;

namespace EasePass.Manager;

public class LocalizationManager
{
    private readonly ResourceManager _resourceManager;
    public List<LanguageItem> Languages { get; } = new();

    public LocalizationManager()
    {
        // Use your .resx base name here, e.g., "EasePass.Resources.Strings"
        _resourceManager = new ResourceManager("EasePass.Resources.Strings", typeof(LocalizationManager).Assembly);
    }

    public void Initialize()
    {
        RegisterLanguagesFromResource();

        // get system language
        string systemLanguage = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

        // get saved language from settings
        string settingsLanguage = AppSettings.Language;

        var language = Languages.Find(x => x.Tag == settingsLanguage)
                       ?? Languages.Find(x => x.Tag == systemLanguage)
                       ?? Languages[0]; // fallback

        SetLanguage(language);
    }

    public void SetLanguage(LanguageItem languageItem)
    {
        if (!Languages.Contains(languageItem))
            return;

        var culture = new CultureInfo(languageItem.Tag);
        Thread.CurrentThread.CurrentUICulture = culture;
        Thread.CurrentThread.CurrentCulture = culture;

        SettingsManager.SaveSettings(AppSettingsValues.language, languageItem.Tag);
    }

    private void RegisterLanguagesFromResource()
    {
        Languages.Clear();
        Languages.Add(new LanguageItem("en", "English"));
        Languages.Add(new LanguageItem("de", "Deutsch"));
    }

    public string GetString(string key)
    {
        try
        {
            return _resourceManager.GetString(key) ?? key;
        }
        catch
        {
            return key;
        }
    }
}