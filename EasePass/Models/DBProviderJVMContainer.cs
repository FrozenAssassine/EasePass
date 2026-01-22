using EasePass.Helper;
using EasePassExtensibility;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace EasePass.Models
{
    internal class DBProviderJVMContainer
    {
        public IDatabaseProvider Provider { get; private set; }

        public JsonNodeViewModel ViewModel { get; private set; }

        public ImageSource SourceIcon => new BitmapImage(Provider.SourceIcon);

        public DBProviderJVMContainer(IDatabaseProvider provider)
        {
            Provider = provider;
            ViewModel = JsonViewModelConverter.GetViewModelFromJson(Provider.GetConfigurationJSON());
        }

        public void SettingsInViewModelChanged()
        {
            Provider.SetConfigurationJSON(JsonViewModelConverter.GetJsonFromViewModel(ViewModel));
        }
    }
}
