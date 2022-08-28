using System;

namespace TagLib.Tags
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ClassTagAttribute : TagAttribute
    {
        public ClassTagAttribute(string name, string description) : base(name, description) { }
        public string[] Threads;
        public bool HasOtherTags;
    }
}
