using EasePass.Helper;
using EasePass.Models;
using EasePass.Settings;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.ObjectModel;
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
            "Lower case letters",
            "Upper case letters",
            "Password length",
            "Leaked or exploited",
            "Punctuation",
            "Digits",
            "Predictability",
            "Seen before"
        };
        private string[] messagesShort = new string[]
        {
            "Lower case",
            "Upper case",
            "Password length",
            "Leaked (or not)",
            "Punctuation",
            "Digits",
            "Predictability",
            "Seen before"
        };
        private string[][] prefixes = new string[][]
        {
            //true, false, null
            new string[]{ "Contains", "Missing", "Unknown whether it contains" },
            new string[]{ "Contains", "Missing", "Unknown whether it contains" },
            new string[]{ "Meets the minimum", "Does not meet the minimum", "Unknown whether it meets the minimum" },
            new string[]{ "Not", "Is already", "Unknown whether it has already been" },
            new string[]{ "Contains", "Missing", "Unknown whether it contains" },
            new string[]{ "Contains", "Missing", "Unknown whether it contains" },
            new string[]{ "Low", "High", "Unknown " },
            new string[]{ "Not yet", "Already", "Unknown whether" },
        };
        private double ChartScale = 10;
        private Path[] paths = new Path[8];
        private bool?[] checks = new bool?[8];

        public bool ShowInfo { get; set; } = true;
        public bool SingleHitbox { get; set; } = false;
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<PasswordManagerItem> items;

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

        public void SetPasswordItems(ObservableCollection<PasswordManagerItem> items)
        {
            this.items = items;
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
        
        public void EvaluatePassword(string password)
        {
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

            bool?[] res = PasswordHelper.EvaluatePassword(password);
            for(int i = 0; i < 6; i++)
            {
                checks[i >= 3 ? i + 1 : i] = res[i];
            }

            checks[7] = null;
            if(items != null)
            {
                int amount = 0;
                for(int i = 0; i < items.Count; i++)
                {
                    if (items[i].Password == password) amount++;
                }
                checks[7] = amount < 2;
            }
            

            for(int i = 0; i < checks.Length; i++)
            {
                SetChartEntry(i);
            }

            RaisePropertyChanged("ToString");
        }
        
        private void SetChartEntry(int index)
        {
            if (checks[index] == null) paths[index].Fill = new SolidColorBrush(Color.FromArgb(255, 100, 100, 100));
            else if (checks[index] == true) paths[index].Fill = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0));
            else if (checks[index] == false) paths[index].Fill = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
        }
        
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for(int i = 0; i < checks.Length; i++)
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
