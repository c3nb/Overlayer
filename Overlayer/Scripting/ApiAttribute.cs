using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overlayer.Scripting
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ApiAttribute : Attribute
    {
        public ApiAttribute() { }
        public ApiAttribute(string name) => Name = name;
        public string Name { get; }
        public ScriptType SupportScript { get; set; } = ScriptType.All;
    }
}
