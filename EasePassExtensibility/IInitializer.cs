using System;
using System.Collections.Generic;
using System.Text;

namespace EasePassExtensibility
{
    public interface IInitializer : IExtensionInterface
    {
        void Init();
    }
}
