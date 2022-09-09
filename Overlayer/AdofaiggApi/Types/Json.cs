using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Overlayer.AdofaiggApi.Types
{
    public class Json
    {
        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}
