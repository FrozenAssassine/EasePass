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

namespace EasePass.Views.DialogPages
{
    public sealed partial class EnterSecondFactorPage : Page
    {
        public EnterSecondFactorPage()
        {
            this.InitializeComponent();
        }

        public string GetPassword()
        {
            return tokenBox.Password;
        }
    }
}
