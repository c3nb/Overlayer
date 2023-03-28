using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overlayer.Scripting
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ApiAttribute : Attribute
    {
        public ApiAttribute(string desc) => Description = desc;
        public string Description { get; }
        public ScriptType SupportScript = ScriptType.All;
    }
}
