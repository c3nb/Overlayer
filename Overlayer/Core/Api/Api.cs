using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Overlayer.Core.Api
{
    public abstract class Api
    {
        protected static readonly WebClient client;
        static Api()
        {
            client = new WebClient();
            client.Encoding = Encoding.UTF8;
        }
        public abstract string Name { get; }
        public abstract string Url { get; }
    }
}
