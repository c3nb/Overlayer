using System;

namespace Overlayer.Tags.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class TagAttribute : Attribute
    {
        public string Name { get; }
        public bool NotPlaying { get; set; }
        public ValueProcessing ProcessingFlags { get; set; }
        public object ProcessingFlagsArg { get; set; }
        public TagAttribute() : this(null) { }
        public TagAttribute(string name) => Name = name;
    }
}
