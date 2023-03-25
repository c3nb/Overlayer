using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overlayer.Scripting
{
    public abstract class Script : IDisposable
    {
        public string Source { get; set; }
        public abstract ScriptType ScriptType { get; }
        public abstract void Compile();
        public abstract object Evaluate();
        public abstract void Execute();
        public abstract void Dispose();
    }
}
