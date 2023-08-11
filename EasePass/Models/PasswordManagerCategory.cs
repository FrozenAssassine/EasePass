using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasePass.Models
{
    internal class PasswordManagerCategory
    {
        public string CategoryName { get; set; }
        public string IconId { get; set; }
        public List<PasswordManagerItem> Items { get; set; }
    }
}
