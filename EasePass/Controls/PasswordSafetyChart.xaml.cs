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

using EasePass.Extensions;
using EasePass.Helper;
using EasePass.Models;
using EasePass.Settings;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace EasePass.Controls
{
    public sealed partial class PasswordSafetyChart : UserControl, INotifyPropertyChanged
    {
        private string[] messages = new string[]
        {
            "Lower case letters".Localized("PW_SafetyChart_LowerCaseLetters/Text"),
            "Upper case letters".Localized("PW_SafetyChart_UpperCaseLetters/Text"),
            "Password length".Localized("PW_SafetyChart_PWLength/Text"),
            "Leaked or exploited".Localized("PW_SafetyChart_LeakedExploited/Text"),
            "Punctuation".Localized("PW_SafetyChart_Punctation/Text"),
            "Digits".Localized("PW_SafetyChart_Digits/Text"),
            "Predictability".Localized("PW_SafetyChart_Predictability/Text"),
            "Seen before".Localized("PW_SafetyChart_Seenbefore/Text")
        };
        private string[][] prefixes = new string[][]
        {
            //true, false, null
            new string[]{ "Contains".Localized("PW_SafetyChart_Contains/Text"), "Missing".Localized("PW_SafetyChart_Missing/Text"), "Unknown whether it contains".Localized("PW_SafetyChart_UnknownContains/Text") },
            new string[]{ "Contains".Localized("PW_SafetyChart_Contains/Text"), "Missing".Localized("PW_SafetyChart_Missing/Text"), "Unknown whether it contains".Localized("PW_SafetyChart_UnknownContains/Text") },
            new string[]{ "Meets the minimum".Localized("PW_SafetyChart_MeetsMinimum/Text"), "Does not meet the minimum".Localized("PW_SafetyChart_DoesNotMeetMinimum/Text"), "Unknown whether it meets the minimum".Localized("PW_SafetyChart_UnknownMeetsMinimum/Text") },
            new string[]{ "Not".Localized("PW_SafetyChart_Not/Text"), "Is already".Localized("PW_SafetyChart_IsAlready/Text"), "Unknown whether it has already been".Localized("PW_SafetyChart_UnknownAlready/Text") },
            new string[]{ "Contains".Localized("PW_SafetyChart_Contains/Text"), "Missing".Localized("PW_SafetyChart_Missing/Text"), "Unknown whether it contains".Localized("PW_SafetyChart_UnknownContains/Text") },
            new string[]{ "Contains".Localized("PW_SafetyChart_Contains/Text"), "Missing".Localized("PW_SafetyChart_Missing/Text"), "Unknown whether it contains".Localized("PW_SafetyChart_UnknownContains/Text") },
            new string[]{ "Low".Localized("PW_SafetyChart_Low/Text"), "High".Localized("PW_SafetyChart_High/Text"), "Unknown ".Localized("PW_SafetyChart_Unknown/Text") },
            new string[]{ "Not yet".Localized("PW_SafetyChart_NotYet/Text"), "Already".Localized("PW_SafetyChart_Already/Text"), "Unknown whether".Localized("PW_SafetyChart_UnknownWhether/Text") },
        };
        private double ChartScale = 10;
        private Path[] paths = new Path[8];
        private bool?[] checks = new bool?[8];

        public bool ShowInfo { get; set; } = true;
        public bool SingleHitbox { get; set; } = false;
        public event PropertyChangedEventHandler PropertyChanged;


        public PasswordSafetyChart()
        {
            this.InitializeComponent();
            paths[0] = path1;
            paths[1] = path2;
            paths[2] = path3;
            paths[3] = path4;
            paths[4] = path5;
            paths[5] = path6;
            paths[6] = path7;
            paths[7] = path8;
        }

        private void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public void SetHeight(double chartHeight)
        {
            ChartScale = chartHeight / 10;
            RaisePropertyChanged("ChartScale");
            pathGrid.Height = chartHeight;
            pathGrid.Width = chartHeight;
            info_left.Height = chartHeight;
            info_right.Height = chartHeight;
        }

        public void EvaluatePassword(string password, bool existingSingleTime = false)
        {
            checks[3] = null;
            if (!AppSettings.GetSettingsAsBool(AppSettingsValues.disableLeakedPasswords, DefaultSettingsValues.disableLeakedPasswords))
            {
                Task<bool?> task = PasswordHelper.IsPwned(password);
                TaskAwaiter<bool?> taskAwaiter = task.GetAwaiter();
                taskAwaiter.OnCompleted(new Action(() =>
                {
                    checks[3] = !task.Result;
                    SetChartEntry(3);
                    RaisePropertyChanged("ToString");
                }));
            }

            bool[] res = PasswordHelper.EvaluatePassword(password);
            for (int i = 0; i < 6; i++)
            {
                checks[i >= 3 ? i + 1 : i] = res[i];
            }

            checks[7] = null;
            if (Database.LoadedInstance.Items != null)
            {
                int amount = 0;
                for (int i = 0; i < Database.LoadedInstance.Items.Count; i++)
                {
                    if (Database.LoadedInstance.Items[i].Password == password) amount++;
                }
                checks[7] = amount < (existingSingleTime ? 2 : 1);
            }


            for (int i = 0; i < checks.Length; i++)
            {
                SetChartEntry(i);
            }

            RaisePropertyChanged("ToString");
        }

        private void SetChartEntry(int index)
        {
            if (DispatcherQueue == null)
                return;

            this.DispatcherQueue.TryEnqueue(() =>
            {
                if (checks[index] == null) paths[index].Fill = new SolidColorBrush(Color.FromArgb(255, 100, 100, 100));
                else if (checks[index] == true) paths[index].Fill = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0));
                else if (checks[index] == false) paths[index].Fill = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
            });
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < checks.Length; i++)
            {
                sb.AppendLine(prefixes[i][ToIndex(checks[i])] + " " + messages[i].ToLower());
            }
            return sb.ToString();
        }

        private int ToIndex(bool? value)
        {
            return value switch
            {
                null => 2,
                true => 0,
                false => 1
            };
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            SetHeight(this.Height);
        }
    }
}
