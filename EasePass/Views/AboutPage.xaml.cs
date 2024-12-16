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

using EasePass.Helper;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using Windows.ApplicationModel;

namespace EasePass.Views
{
    public sealed partial class AboutPage : Page
    {
        public string AppVersion => AppVersionHelper.GetAppVersion();

        public AboutPage()
        {
            this.InitializeComponent();
            App.m_window.ShowBackArrow = true;

            SetChangelog();
            
        }

        private async void NavigateToLink_Click(Controls.SettingsControl sender)
        {
            if (sender.Tag == null)
                return;

            await Windows.System.Launcher.LaunchUriAsync(new Uri(sender.Tag.ToString()));
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            App.m_window.ShowBackArrow = false;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //send any parameter opens the changelog
            if (e.Parameter != null)
            {
                changelogExpander.IsExpanded = true;
            }
            base.OnNavigatedTo(e);
        }

        private void SetChangelog()
        {
            //Simple parser to make headlines bigger and add paragraphs
            string filePath = Path.Combine(Package.Current.InstalledLocation.Path, "Assets", "changelog.txt");
            var data = File.ReadAllLines(filePath);
            List<Paragraph> paragraphs = new List<Paragraph> { new Paragraph() };
            for (int i = 0; i < data.Length; i++)
            {
                string currentLine = data[i];
                Run line = new Run();

                //Headline:
                if (currentLine.StartsWith("#"))
                {
                    currentLine = currentLine.Remove(0, 1);
                    line.FontSize = 24;
                }
                //Paragraph
                else if (currentLine.StartsWith("---"))
                {
                    currentLine = currentLine.Remove(0, 3);
                    paragraphs.Add(new Paragraph());
                }

                line.Text = currentLine + "\n";
                paragraphs[paragraphs.Count - 1].Inlines.Add(line);
            }
            foreach (var paragraph in paragraphs)
            {
                ChangelogDisplay.Blocks.Add(paragraph);
            }
        }

    }
}
