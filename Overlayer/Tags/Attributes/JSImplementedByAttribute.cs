using System;

namespace Overlayer.Tags.Attributes
{
    public class JSImplementedByAttribute : Attribute
    {
        public string Author { get; }
        public JSImplementedByAttribute(string author) => Author = author;
    }
}
