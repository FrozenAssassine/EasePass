using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasePassExtensibility
{
    public delegate string FilePicker(string[] extensions);

    public interface IFilePickerInjectable : IExtensionInterface
    {
        FilePicker FilePicker { get; set; }
    }
}
