using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Overlayer.Core.Api.Adofaigg
{
    public abstract class Parameter
    {
        public abstract string name { get; }
        public abstract object value { get; }
        public override string ToString()
        {
            if (!(value is string) && value is IEnumerable)
                return $"{name}={CombineEnumerable()}";
            else return $"{name}={value}";
            //else return $"{name}={Uri.EscapeDataString(value.ToString())}";
        }
        public string ToString(bool isFirst)
        {
            if (isFirst)
                return $"?{ToString()}";
            else return $"&{ToString()}";
        }
        private string CombineEnumerable()
        {
            IEnumerable<object> e = value as IEnumerable<object>;
            object last = e.Last();
            StringBuilder sb = new StringBuilder();
            foreach (object item in e)
            {
                if (last == item)
                    sb.Append(item);
                else sb.Append($"{item}, ");
            }
            return sb.ToString();
        }
    }

}
