using System.Text.RegularExpressions;

namespace EasePass.Regexs
{
    public partial class DNS
    {
        [GeneratedRegex(@"^(?!\-)([a-zA-Z0-9\-]{1,63})(?<!\-)\.([a-zA-Z]{2,})$")]
        public static partial Regex ValidDNS();
    }
}
