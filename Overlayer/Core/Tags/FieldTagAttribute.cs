using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overlayer.Core.Tags
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class FieldTagAttribute : TagAttribute
    {
        public FieldTagAttribute() : base() { }
        public FieldTagAttribute(string name) : base(name) { }
        public bool Round { get; set; }
    }
}
