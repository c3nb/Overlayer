using System;
using System.Reflection;

namespace Overlayer.Scripting
{
    public class CustomParameter : ParameterInfo
    {
        public CustomParameter(Type type, string name)
        {
            ClassImpl = type;
            NameImpl = name;
        }
    }
}
