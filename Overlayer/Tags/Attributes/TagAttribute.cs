using System;

namespace Overlayer.Tags.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Field, AllowMultiple = false)]
    public class TagAttribute : Attribute
    {
        public string Name { get; }
        public bool NotPlaying { get; set; }
        public FieldValueProcessing FieldFlags { get; set; }
        public TagAttribute() : this(null) { }
        public TagAttribute(string name) => Name = name;
    }
}
