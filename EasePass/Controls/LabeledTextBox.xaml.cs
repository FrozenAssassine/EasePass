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

using Microsoft.UI.Xaml.Controls;

namespace EasePass.Controls
{
    public sealed partial class LabeledTextBox : TextBox
    {
        TextBlock infoLabel = null;

        public LabeledTextBox()
        {
            this.InitializeComponent();
        }

        public string InfoLabel { get => infoLabel.Text; set { if (infoLabel != null) infoLabel.Text = value; } }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            infoLabel = GetTemplateChild("infoLabel") as TextBlock;
            infoLabel.Text = "";
        }
    }
}
