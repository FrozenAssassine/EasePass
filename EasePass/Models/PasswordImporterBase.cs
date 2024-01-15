using EasePassExtensibility;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return _passwordImporter.ImportPasswords();
        }

        public bool PasswordsAvailable()
        {
            return _passwordImporter.PasswordsAvailable();
        }
    }
}
