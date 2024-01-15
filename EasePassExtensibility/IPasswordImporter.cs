using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasePassExtensibility
{
    public interface IPasswordImporter : IExtensionInterface
    {
        string SourceName { get; }
        Uri SourceIcon { get; }
        PasswordItem[] ImportPasswords();
        bool PasswordsAvailable();
    }
}
