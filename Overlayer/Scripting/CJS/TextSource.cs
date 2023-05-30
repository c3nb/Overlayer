using Overlayer.Core.Utils;
using System;
using System.IO;
using System.Linq;
using System.Text;
using JSEngine;

namespace Overlayer.Scripting.CJS
{
    public class TextSource : ScriptSource
    {
        public string str;
        public ScriptEngine engine;
        public bool IsProxy => ProxyType != null;
        public Type ProxyType { get; private set; }
        public TextSource(string str, ScriptEngine engine, string path = null)
        {
            this.str = str;
            Path = path;
            this.engine = engine;
            using (var reader = new StringReader(str))
            {
                var firstLine = reader.ReadLine();
                if (firstLine == null) return;
                if (firstLine.EndsWith(" Proxy"))
                    ProxyType = MiscUtils.TypeByName(firstLine.Split(' ')[1]);
            }
        }
        public override string Path { get; }
        public override TextReader GetReader()
        {
            using (StringReader sr = new StringReader(str))
            {
                StringBuilder sb = new StringBuilder();
                string line = null;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.StartsWith("import"))
                    {
                        sb.AppendLine();
                        continue;
                    }
                    sb.AppendLine(line);
                }
                return new StringReader(sb.ToString());
            }
        }
    }
}
