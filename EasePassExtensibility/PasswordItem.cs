using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasePassExtensibility
{
    /// <summary>
    /// IPasswordImporter uses this class to store a password.
    /// </summary>
    public class PasswordItem
    {
        public string DisplayName { get; set; }
        public string UserName { get; set; }
        public string EMail { get; set; }
        public string Website { get; set; }
        public string Password { get; set; }
        public string Notes { get; set; }
        public string TOTPSecret { get; set; }
        public int TOTPDigits { get; set; }
        public int TOTPInterval { get; set; }
        public Algorithm TOTPAlgorithm { get; set; }

        public enum Algorithm
        {
            SHA1,
            SHA256,
            SHA512
        }
    }
}
