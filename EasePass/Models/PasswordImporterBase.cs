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

using EasePassExtensibility;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;

namespace EasePass.Models
{
    public class PasswordImporterBase : IPasswordImporter
    {
        public string SourceName => _passwordImporter.SourceName;

        public Uri SourceIcon => _passwordImporter.SourceIcon;

        public ImageSource IconSource => new BitmapImage(SourceIcon);

        public IPasswordImporter PasswordImporter { get { return _passwordImporter; } }

        private IPasswordImporter _passwordImporter;
        public PasswordImporterBase(IPasswordImporter passwordImporter)
        {
            _passwordImporter = passwordImporter;
        }

        public PasswordItem[] ImportPasswords()
        {
            try
            {
                return _passwordImporter.ImportPasswords();
            }
            catch
            {
                return Array.Empty<PasswordItem>();
            }
        }

        public bool PasswordsAvailable()
        {
            try
            {
                return _passwordImporter.PasswordsAvailable();
            }
            catch
            {
                return false;
            }
        }
    }
}
