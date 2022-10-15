using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overlayer.Core.OScript
{
    [AttributeUsage(AttributeTargets.Class |
                    AttributeTargets.Enum |
                    AttributeTargets.Property |
                    AttributeTargets.Field |
                    AttributeTargets.Method |
                    AttributeTargets.Struct | 
                    AttributeTargets.Interface, 
                    AllowMultiple = false,
                    Inherited = false)]
    public class BindAttribute : Attribute
    {
        public string Name;
    }
}
