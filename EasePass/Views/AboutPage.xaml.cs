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
