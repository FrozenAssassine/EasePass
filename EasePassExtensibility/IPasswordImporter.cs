using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasePassExtensibility
{
    /// <summary>
    /// Implement this interface to create an element in the "Import passwords" section on the settings page. It's made to import passwords from other services or password storages.
    /// </summary>
    public interface IPasswordImporter : IExtensionInterface
    {
        /// <summary>
        /// Source from which you get the passwords.
        /// </summary>
        string SourceName { get; }
        /// <summary>
        /// Icon of the source
        /// </summary>
        Uri SourceIcon { get; }
        /// <summary>
        /// Ease Pass will call this function to get the passwords from this source.
        /// </summary>
        /// <returns>List of the passwords.</returns>
        PasswordItem[] ImportPasswords();
        /// <summary>
        /// Ease Pass will call this function to check if passwords are available. Ease Pass will not call this function every time.
        /// </summary>
        /// <returns></returns>
        bool PasswordsAvailable();
    }
}
