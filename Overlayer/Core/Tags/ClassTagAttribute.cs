using System;
using Overlayer.Core;

namespace Overlayer.Core
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ClassTagAttribute : TagAttribute
    {
        public ClassTagAttribute(string name) : base(name) { }
        public string[] Threads;
        public bool HasOtherTags;
    }
}
