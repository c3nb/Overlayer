using System.Collections.Generic;
using System.Text;
using System.Linq;
using TagLib.Utils;
using System;

namespace TagLib.Tags
{
    public class Token
    {
        public enum Kind
        {
            Identifier,
            Add,
            Sub,
            Mul,
            Div,
            Rem,
            Pow,
            LParen,
            RParen,
            Comma,
            Tag,
            Number,
            End,
        }
        internal Token(Token prev)
        {
            Previous = prev;
            if (prev != null)
                prev.Next = this;
        }
        internal StringBuilder raw = new StringBuilder();
        internal StringBuilder text = new StringBuilder();
        internal StringBuilder option = new StringBuilder();
        internal StringBuilder format = new StringBuilder();
        internal void SetKind(string str)
        {
            switch (str)
            {
                case "+":
                    TokenKind = Kind.Add;
                    return;
                case "-":
                    TokenKind = Kind.Sub;
                    return;
                case "*":
                    TokenKind = Kind.Mul;
                    return;
                case "/":
                    TokenKind = Kind.Div;
                    return;
                case "%":
                    TokenKind = Kind.Rem;
                    return;
                case "(":
                    TokenKind = Kind.LParen;
                    return;
                case ")":
                    TokenKind = Kind.RParen;
                    return;
                case ",":
                    TokenKind = Kind.Comma;
                    return;
                default:
                    {
                        if (str.All(c => char.IsDigit(c) || c == '.'))
                        {
                            TokenKind = Kind.Number;
                            Value = str.ToFloat();
                        }
                        else TokenKind = Kind.Identifier;
                        return;
                    }
            }
        }
        public string Raw => raw.ToString();
        public string Text => text.ToString();
        public string Option => option.ToString();
        public string Format => format.ToString();
        public bool HasOption => option.Length > 0;
        public bool HasFormat => format.Length > 0;
        public Token Previous { get; private set; }
        public Token Next { get; private set; }
        public bool Closed { get; private set; }
        public string[] Split { get; private set; }
        public object Value { get; private set; }
        public Kind TokenKind { get; private set; }
        public bool IsOperator => TokenKind == Kind.Add || TokenKind == Kind.Sub || TokenKind == Kind.Mul || TokenKind == Kind.Div || TokenKind == Kind.Rem;
        public override string ToString() => Text.ToString();
        public Token NextToken(Action<Token> callback = null)
        {
            Token newToken = new Token(this);
            callback?.Invoke(newToken);
            return newToken;
        }
        public static IEnumerable<Token> Tokenize(string text, bool ignoreWhitespace = false)
        {
            text += ' ';
            List<Token> tokens = new List<Token>();
            bool tag = false;
            bool option = false;
            bool format = false;
            int formatBracesStack = 0;
            Token current = new Token(null);
            Stack<Token> unresolvedTok = new Stack<Token>();
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                First:
                if (tag)
                {
                    if (c == '{')
                    {
                        if (!format)
                        {
                            unresolvedTok.Push(current);
                            current = current.NextToken(tokens.Add);
                            current.raw.Append(c);
                            continue;
                        }
                        formatBracesStack++;
                    }
                    if (c == '|')
                    {
                        if (format)
                        {
                            current.format.Append(c);
                            continue;
                        }
                        option = false;
                        format = true;
                        continue;
                    }
                    if (c == ':')
                    {
                        if (option)
                        {
                            current.option.Append(c);
                            continue;
                        }
                        format = false;
                        option = true;
                        continue;
                    }
                    else if (format)
                    {
                        if (c == '}')
                        {
                            if (formatBracesStack > 0)
                            {
                                formatBracesStack--;
                                current.format.Append(c);
                                continue;
                            }
                            format = false;
                            tag = false;
                            tokens.Add(current);
                            current.Closed = true;
                            current.TokenKind = Kind.Tag;
                            unresolvedTok.Pop();
                            current.raw.Append(c);
                            current = new Token(current);
                            continue;
                        }
                        else current.format.Append(c);
                    }
                    else if (option)
                    {
                        if (c == '}')
                        {
                            option = false;
                            tag = false;
                            tokens.Add(current);
                            current.Closed = true;
                            current.TokenKind = Kind.Tag;
                            unresolvedTok.Pop();
                            current.raw.Append(c);
                            current = new Token(current);
                            continue;
                        }
                        else current.option.Append(c);
                    }
                    else if (c == '}')
                    {
                        option = false;
                        format = false;
                        tag = false;
                        tokens.Add(current);
                        current.Closed = true;
                        current.TokenKind = Kind.Tag;
                        unresolvedTok.Pop();
                        current.raw.Append(c);
                        current = new Token(current);
                        continue;
                    }
                    else current.text.Append(c);
                }
                else
                {
                    switch (c)
                    {
                        case '{':
                            {
                                tag = true;
                                unresolvedTok.Push(current);
                                tokens.Add(current);
                                current = new Token(current);
                                current.raw.Append(c);
                                continue;
                            }
                        case '+':
                        case '-':
                        case '*':
                        case '/':
                        case '%':
                        case '(':
                        case ')':
                        case ',':
                            tokens.Add(current);
                            current = new Token(current);
                            current.text.Append(c);
                            current.raw.Append(c);
                            current.SetKind(current.Text);

                            tokens.Add(current);
                            current = new Token(current);
                            continue;
                        default:
                            if (char.IsDigit(c))
                            {
                                tokens.Add(current);
                                current = new Token(current);
                                int start = i;
                                bool hasDecimalPoint = false;
                                while (char.IsDigit(c) || (!hasDecimalPoint && c == '.'))
                                {
                                    c = text[i++];
                                    if (c == '.')
                                        hasDecimalPoint = true;
                                }
                                c = text[--i];
                                var digits = text.Substring(start, i - start);
                                current.text.Append(digits);
                                current.raw.Append(digits);
                                current.SetKind(current.Text);
                                tokens.Add(current);
                                current = new Token(current);
                                goto First;
                            }
                            current.text.Append(c);
                            break;
                    }
                }
                current.raw.Append(c);
            }
            //if (braceStack != 0)
            //    Main.Logger.Log("Warning! Braces Is Not Fully Closed! Text Compiler May Not Compile This Normally!");
            //else Main.Logger.Log("Compiled Successful.");
            foreach (Token tok in unresolvedTok)
                tok.text = new StringBuilder(tok.Raw);
            if (ignoreWhitespace)
                tokens.RemoveAll(t => string.IsNullOrWhiteSpace(t.Text));
            else tokens.RemoveAll(t => string.IsNullOrEmpty(t.Text));
            if (!string.IsNullOrWhiteSpace(current.Raw))
                tokens.Add(current);
            Token end = new Token(current);
            end.TokenKind = Kind.End;
            tokens.Add(end);
            return tokens;
        }
    }
}
