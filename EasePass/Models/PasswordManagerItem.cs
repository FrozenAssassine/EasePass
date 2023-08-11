using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasePass.Models
{
    internal class PasswordManagerItem
    {
        public PasswordManagerItem(string displayname, string password, string username, string email, string notes)
        {
            DisplayName = displayname;
            Password = password;
            Username = username;
            Email = email;
            Notes = notes;
        }

        public PasswordManagerItem(string displayname, string iconid, string password, string username, string email, string notes)
        {
            IconId = iconid;
            DisplayName = displayname;
            Password = password;
            Username = username;
            Email = email;
            Notes = notes;
        }

        public string Password { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Notes { get; set; }
        public string DisplayName { get; set; }
        public string IconId { get; set; }
    }
}
