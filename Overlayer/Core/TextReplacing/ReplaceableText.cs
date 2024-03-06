using Overlayer.Core.TextReplacing.Lexing;
using Overlayer.Core.TextReplacing.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Overlayer.Core.TextReplacing
{
    public class ReplaceableText : IDisposable
    {
        private bool disposed;
        public List<IParsed> Replaceables { get; private set; }
        public ReplaceableText(IEnumerable<IParsed> replaceables)
        {
            Replaceables = replaceables.ToList();
            Replaceables.ForEach(p =>
            {
                if (p is ParsedTag tag) tag.tag.ReferencedCount++;
            });
        }
        public string Replace()
        {
            if (disposed) throw new ObjectDisposedException(GetType().FullName);
            return Replaceables.Aggregate(new StringBuilder(), (sb, p) =>
            {
                if (p is ParsedString str)
                    sb.Append(str.str);
                else if (p is ParsedTag tag)
                    sb.Append(InvokeTag(tag.tag, tag.args.ToArray()));
                return sb;
            }).ToString();
        }
        public static object InvokeTag(Tag tag, params string[] args)
        {
            return tag.Getter.Invoke(tag.GetterOriginalTarget, args);
        }
        public static ReplaceableText Create(string source, IEnumerable<Tag> tags, LexConfig config = null)
        {
            return new ReplaceableText(Parser.Parse(Lexer.Lex(source, config), tags.ToList()));
        }
        public void Dispose()
        {
            if (disposed) return;
            Replaceables.ForEach(p =>
            {
                if (p is ParsedTag tag) tag.tag.ReferencedCount--;
            });
            Replaceables = null;
            GC.SuppressFinalize(this);
            disposed = true;
        }
        ~ReplaceableText() => Dispose();
    }
}
