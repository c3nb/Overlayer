using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Overlayer.Core.Api.Adofaigg
{
    public class Parameters : IEnumerable<Parameter>
    {
        public readonly IEnumerable<Parameter> Params;
        public Parameters(IEnumerable<Parameter> @params)
            => Params = @params;
        public Parameters(params Parameter[] @params)
            => Params = @params;
        public IEnumerator<Parameter> GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Params.GetEnumerator();
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            bool isFirst = true;
            foreach (Parameter param in Params)
            {
                sb.Append(param.ToString(isFirst));
                isFirst = false;
            }
            return sb.ToString();
        }
    }
}
