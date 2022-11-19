using System;

namespace Overlayer.Core
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class TagAttribute : Attribute
    {
        public TagAttribute() { }
        public TagAttribute(string name) => Name = name;
        public string Name { get; }
        public bool Hidden = false;
        public bool IsDefault => Name == null;
    }
}
