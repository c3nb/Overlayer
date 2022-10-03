using System;

namespace Overlayer.Core
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class TagAttribute : Attribute
    {
        public TagAttribute() { }
        public TagAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }
        public string Name { get; }
        public string Description { get; }
        public bool Hidden = false;
        public bool IsDefault => Name == null && Description == null;
    }
}
