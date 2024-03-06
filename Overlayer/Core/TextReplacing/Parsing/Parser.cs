using Overlayer.Core.TextReplacing.Lexing;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Overlayer.Core.TextReplacing.Parsing
{
    public static class Parser
    {
        public static IEnumerable<IParsed> Parse(IEnumerable<Token> tokens, List<Tag> tags, LexConfig config = null)
        {
            config ??= new LexConfig();
            Queue<Token> queue = new Queue<Token>(tokens);
            while (queue.Count > 0)
            {
                Token t = queue.Dequeue();
                if (t.type == TokenType.TagStart)
                {
                    if (queue.Peek().type == TokenType.TagEnd)
                    {
                        queue.Dequeue();
                        yield return new ParsedString(config.TagStart.ToString() + config.TagEnd.ToString());
                        continue;
                    }
                    Tag found = null;
                    StringBuilder sb = new StringBuilder();
                    sb.Append(config.TagStart);
                    bool tagNotFound = false;
                    List<string> arguments = new List<string>();
                    while (queue.Count > 0 && t.type != TokenType.TagEnd)
                    {
                        t = queue.Dequeue();
                        if (tagNotFound)
                        {
                            sb.Append(t.value);
                            continue;
                        }
                        if (t.type == TokenType.Identifier)
                        {
                            found = tags.FirstOrDefault(tag => tag.Name == t.value);
                            if (tagNotFound = found == null)
                            {
                                sb.Append(t.value);
                                continue;
                            }
                        }
                        if (t.type == TokenType.ArgStart || t.type == TokenType.Colon)
                        {
                            while (queue.Count > 0 && t.type != TokenType.ArgEnd && t.type != TokenType.TagEnd)
                            {
                                t = queue.Dequeue();
                                if (t.type == TokenType.Identifier)
                                    arguments.Add(t.value);
                            }
                        }
                    }
                    if (tagNotFound)
                        yield return new ParsedString(sb.ToString());
                    else yield return new ParsedTag(found, arguments);
                }
                else yield return new ParsedString(t.value);
            }
        }
    }
}
