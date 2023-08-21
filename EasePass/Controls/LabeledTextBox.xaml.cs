using EasePass.Helper;
using EasePass.Settings;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Diagnostics;

namespace EasePass.Controls
{
    public sealed partial class LabeledTextBox : TextBox
    {
        TextBlock infoLabel = null;

        public LabeledTextBox()
        {
            this.InitializeComponent();
        }

        public void SetInfo(string str)
        {
            if (infoLabel != null)
            {
                infoLabel.Text = str;
            }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            infoLabel = GetTemplateChild("infoLabel") as TextBlock;
            infoLabel.Text = "";
        }
    }
}
