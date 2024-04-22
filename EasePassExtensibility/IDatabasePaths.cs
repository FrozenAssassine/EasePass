using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasePassExtensibility
{
    public interface IDatabasePaths : IExtensionInterface
    {
        public void Init(string[] databasePaths);
    }
}
