using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.Media;
using Avalonia.Interactivity;
using EasePass.Core.Database;
using System.ComponentModel;
using System.Text;
using EasePass.Helper.Security.Generator;
using EasePass.Settings;
using EasePass.Helper.Security;
using Avalonia.Controls.Shapes;
using EasePass.Extensions;
using System.Collections.Generic;

namespace EasePass.Controls
{
    public partial class PasswordSafetyChart : UserControl, INotifyPropertyChanged
    {
        private List<string> _Messages = new List<string>
        {
            "Lower case letters".Localized("PW_SafetyChart_LowerCaseLetters/Text"),
            "Upper case letters".Localized("PW_SafetyChart_UpperCaseLetters/Text"),
            "Password length".Localized("PW_SafetyChart_PWLength/Text"),
            "Leaked or exploited".Localized("PW_SafetyChart_LeakedExploited/Text"),
            "Punctuation".Localized("PW_SafetyChart_SpecialChars/Text"),
            "Digits".Localized("PW_SafetyChart_Digits/Text"),
            "Predictability".Localized("PW_SafetyChart_Predictability/Text"),
            "Seen before".Localized("PW_SafetyChart_Seenbefore/Text")
        };
        public List<string> Messages { get => _Messages; }

        private static List<string> _Infos = new List<string>
        {
            "Contains Lower case:".Localized("PW_SafetyChart_Info_LowerCase/Text"),
            "Contains Upper case:".Localized("PW_SafetyChart_Info_UpperCase/Text"),
            "Passwordlength:".Localized("PW_SafetyChart_Info_Passwordlength/Text"),
            "Leaked or exploited:".Localized("PW_SafetyChart_Info_LeakedExploited/Text"),
            "Contains Punctuation:".Localized("PW_SafetyChart_Info_SpecialChars/Text"),
            "Contains Digits:".Localized("PW_SafetyChart_Info_Digits/Text"),
            "Predictability:".Localized("PW_SafetyChart_Info_Predictability/Text"),
            "Seen before:".Localized("PW_SafetyChart_Info_SeenBefore/Text")
        };
        public List<string> Infos => _Infos;

        private static string[][] suffixes = new string[][]
        {
            //true, false, null
            new string[]{ "True".Localized("PW_SafetyChart_Info_LowerCase_Positive/Text"), "False".Localized("PW_SafetyChart_Info_LowerCase_Negative/Text") },
            new string[]{ "True".Localized("PW_SafetyChart_Info_UpperCase_Positive/Text"), "False".Localized("PW_SafetyChart_Info_UpperCase_Negative/Text") },
            new string[]{ "Meets Minimum".Localized("PW_SafetyChart_Info_Passwordlength_Positive/Text"), "Doesn't meet Minimum".Localized("PW_SafetyChart_Info_Passwordlength_Negative/Text") },
            new string[]{ "False".Localized("PW_SafetyChart_Info_LeakedExploited_Positive/Text"), "True".Localized("PW_SafetyChart_Info_LeakedExploited_Negative/Text") },
            new string[]{ "True".Localized("PW_SafetyChart_Info_SpecialChars_Positive/Text"), "False".Localized("PW_SafetyChart_Info_SpecialChars_Negative/Text") },
            new string[]{ "True".Localized("PW_SafetyChart_Info_Digits_Positive/Text"), "False".Localized("PW_SafetyChart_Info_Digits_Negative/Text") },
            new string[]{ "Low".Localized("PW_SafetyChart_Info_Predictability_Positive/Text"), "High".Localized("PW_SafetyChart_Info_Predictability_Negative/Text") },
            new string[]{ "False".Localized("PW_SafetyChart_Info_SeenBefore_Positive/Text"), "True".Localized("PW_SafetyChart_Info_SeenBefore_Negative/Text") },
        };
        private static string unknown = "False".Localized("PW_SafetyChart_Info_Unknown/Text");


        private double _chartScale = 10;
        public double ChartScale 
        {
            get => _chartScale;
            set { _chartScale = value; RaisePropertyChanged(nameof(ChartScale)); }
        }

        private Path[] paths = new Path[8];
        private bool?[] checks = new bool?[8];
        
        private bool _showInfo = true;
        public bool ShowInfo
        {
            get => _showInfo;
            set { _showInfo = value; RaisePropertyChanged(nameof(ShowInfo)); }
        }
        
        private bool _singleHitbox = false;
        public bool SingleHitbox
        {
            get => _singleHitbox;
            set { _singleHitbox = value; RaisePropertyChanged(nameof(SingleHitbox)); }
        }

        public string ChartTooltip => this.ToString();

        public new event PropertyChangedEventHandler? PropertyChanged;

        public PasswordSafetyChart()
        {
            this.InitializeComponent();
            this.DataContext = this;
            
            paths[0] = this.FindControl<Path>("path1");
            paths[1] = this.FindControl<Path>("path2");
            paths[2] = this.FindControl<Path>("path3");
            paths[3] = this.FindControl<Path>("path4");
            paths[4] = this.FindControl<Path>("path5");
            paths[5] = this.FindControl<Path>("path6");
            paths[6] = this.FindControl<Path>("path7");
            paths[7] = this.FindControl<Path>("path8");
        }

        private void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void SetHeight(double chartHeight)
        {
            if (double.IsNaN(chartHeight)) return;

            ChartScale = chartHeight / 10;
            
            var pathGrid = this.FindControl<Grid>("pathGrid");
            var info_left = this.FindControl<Grid>("info_left");
            var info_right = this.FindControl<Grid>("info_right");

            if (pathGrid != null) { pathGrid.Height = chartHeight; pathGrid.Width = chartHeight; }
            if (info_left != null) info_left.Height = chartHeight;
            if (info_right != null) info_right.Height = chartHeight;
        }

        private bool CheckPasswordAlreadyUsed(string password, bool existingSingleTime)
        {
            if (Database.LoadedInstance?.Items == null)
                return false;

            int amount = 0;
            for (int i = 0; i < Database.LoadedInstance.Items.Count; i++)
            {
                if (Database.LoadedInstance.Items[i].Password == password) amount++;
            }
            return amount < (existingSingleTime ? 2 : 1);
        }

        public void EvaluatePassword(string password, bool existingSingleTime = false)
        {
            if (password == null || password.Length == 0)
            {
                for (int i = 0; i < checks.Length; i++)
                {
                    checks[i] = null;
                    SetChartEntry(i);
                }
                RaisePropertyChanged(nameof(ChartTooltip));
                return;
            }
            
            bool[] res = PasswordHelper.EvaluatePassword(password);
            checks[0] = res[0];
            checks[1] = res[1];
            checks[2] = res[2];
            checks[3] = AppSettings.DisableLeakedPasswords ? null : IsPwnedHelper.IsPwned(password) == PwnedResult.NotLeaked; //todo check == enum (ai generated)
            checks[4] = res[3];
            checks[5] = res[4];
            checks[6] = res[5];
            checks[7] = CheckPasswordAlreadyUsed(password, existingSingleTime);

            for (int i = 0; i < checks.Length; i++)
            {
                SetChartEntry(i);
            }
            RaisePropertyChanged(nameof(ChartTooltip));
        }

        private void SetChartEntry(int index)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (paths[index] == null) return;

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
                int index = ToIndex(checks[i]);
                string suffix = index == 2 ? unknown : suffixes[i][index];
                sb.AppendLine(_Infos[i] + " " + suffix);
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
            SetHeight(this.Bounds.Height);
        }
    }
}
