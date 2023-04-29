using System;

namespace Overlayer.Core.Api.Adofaigg
{
    public class Parameter<T> : Parameter
    {
        public override string name => name_;
        private string name_;
        public override object value => value_;
        private T value_;
        private static bool str = typeof(T) == typeof(string);
        public Parameter(string name, T value)
        {
            SetName(name);
            SetValue(value);
        }

        public Parameter<T> SetName(string name)
        {
            name_ = name;
            return this;
        }
        public Parameter<T> SetValue(T value)
        {
            if (AdofaiggApi.EscapeParameter && str)
                value_ = (T)(object)Uri.EscapeDataString(value.ToString());
            else value_ = value;
            return this;
        }
    }
}
