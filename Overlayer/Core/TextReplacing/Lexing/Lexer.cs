using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Overlayer.Core.TextReplacing.Lexing
{
    public static class Lexer
    {
        public static List<Token> Lex(string source, LexConfig config = null)
        {
            var lex = Lex_(source, config).ToList();
            var invalid = lex.FindIndex(t => t.afterInvalid);
            if (invalid >= 0)
            {
                StringBuilder values = new StringBuilder();
                for (int i = invalid; i < lex.Count; i++)
                    values.Append(lex[i].value);
                lex.RemoveRange(invalid, lex.Count - invalid);
                lex.Add(new Token(TokenType.Identifier, values.ToString()));
            }
            return lex;
        }
        static IEnumerable<Token> Lex_(string source, LexConfig config = null)
        {
            config ??= new LexConfig();
            StringBuilder sb = new StringBuilder();
            Stack<Token> lastTagToken = new Stack<Token>();
            bool tagStarted = false, escaping = false, colonActivated = false;
            int argDepth = 0;
            for (int i = 0; i < source.Length; i++)
            {
                char c = source[i];
                if (escaping)
                {
                    escaping = false;
                    sb.Append(c);
                    continue;
                }
                if (c == config.TagStart)
                {
                    if (tagStarted || colonActivated)
                    {
                        sb.Append(c);
                        continue;
                    }
                    if (sb.Length > 0)
                        yield return new Token(TokenType.Identifier, sb.ToString());
                    sb.Clear();
                    if (i + 1 < source.Length && source[i + 1] == config.TagStart)
                    {
                        sb.Append(c);
                        continue;
                    }
                    var tok = new Token(TokenType.TagStart, c.ToString());
                    tok.afterInvalid = true;
                    lastTagToken.Push(tok);
                    yield return tok;
                    tagStarted = true;
                }
                else if (c == config.TagEnd)
                {
                    if (!tagStarted)
                    {
                        sb.Append(c);
                        continue;
                    }
                    if (sb.Length > 0)
                        yield return new Token(TokenType.Identifier, sb.ToString());
                    sb.Clear();
                    yield return new Token(TokenType.TagEnd, c.ToString());
                    lastTagToken.Pop().afterInvalid = false;
                    tagStarted = colonActivated = false;
                    argDepth = 0;
                }
                else if (c == config.TagArgStart)
                {
                    if (!tagStarted || argDepth++ > 0 || colonActivated)
                    {
                        sb.Append(c);
                        continue;
                    }
                    if (sb.Length > 0)
                        yield return new Token(TokenType.Identifier, sb.ToString());
                    sb.Clear();
                    yield return new Token(TokenType.ArgStart, c.ToString());
                }
                else if (c == config.TagArgEnd)
                {
                    if (!tagStarted || --argDepth > 0 || colonActivated)
                    {
                        sb.Append(c);
                        continue;
                    }
                    if (sb.Length > 0)
                        yield return new Token(TokenType.Identifier, sb.ToString());
                    sb.Clear();
                    yield return new Token(TokenType.ArgEnd, c.ToString());
                }
                else if (c == config.TagOptSeparator)
                {
                    if (!tagStarted || argDepth > 0 || colonActivated)
                    {
                        sb.Append(c);
                        continue;
                    }
                    if (sb.Length > 0)
                        yield return new Token(TokenType.Identifier, sb.ToString());
                    sb.Clear();
                    yield return new Token(TokenType.Colon, c.ToString());
                    colonActivated = true;
                }
                else if (c == config.TagArgSeparator)
                {
                    if (!tagStarted || colonActivated)
                    {
                        sb.Append(c);
                        continue;
                    }
                    if (sb.Length > 0)
                        yield return new Token(TokenType.Identifier, sb.ToString());
                    sb.Clear();
                    if (argDepth > 0)
                        yield return new Token(TokenType.Comma, c.ToString());
                    else
                    {
                        sb.Append(c);
                        continue;
                    }
                }
                else if (c == '\\') escaping = true;
                else sb.Append(c);
            }
            if (sb.Length > 0)
                yield return new Token(TokenType.Identifier, sb.ToString());
        }
    }
}
