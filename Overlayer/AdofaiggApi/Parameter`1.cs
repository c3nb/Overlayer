namespace Overlayer.AdofaiggApi
{
    public class Parameter<T> : Parameter
    {
        public override string name => name_;
        public string name_;
        public override object value => value_;
        public T value_;
        public Parameter(string name, T value)
        {
            name_ = name;
            value_ = value;
        }

        public Parameter<T> SetName(string name)
        {
            name_ = name;
            return this;
        }
        public Parameter<T> SetValue(T value)
        {
            value_ = value;
            return this;
        }
    }
}
