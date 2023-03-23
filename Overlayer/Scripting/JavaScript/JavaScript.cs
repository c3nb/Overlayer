using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JSEngine;

namespace Overlayer.Scripting.JavaScript
{
    public class JavaScript : Script
    {
        public JavaScript()
        {

        }
        public override ScriptType ScriptType => ScriptType.JavaScript;
        public override void Compile()
        {
            throw new NotImplementedException();
        }
        public override void Dispose()
        {
            throw new NotImplementedException();
        }

        public override object Evaluate()
        {
            throw new NotImplementedException();
        }

        public override object Evaluate(object[] args)
        {
            throw new NotImplementedException();
        }

        public override void Execute()
        {
            throw new NotImplementedException();
        }

        public override void Execute(object[] args)
        {
            throw new NotImplementedException();
        }
    }
}
