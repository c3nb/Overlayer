using System;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using Overlayer.Core.Utils;
using Overlayer.Core.Translation;
using System.Xml.XPath;

namespace Overlayer.Core.Tags.Nodes
{
    public class Parser
    {
        public NToken[] tokens;
        public Tag[] tags;
        public Dictionary<string, (int, Tag)> tagDict;
        public string[] Errors;
        public int position;
        public int length;
        public bool CanUsedByNotPlaying = true;
        public bool IsString = false;
        public List<ArgumentNode> args;
        public NToken Current => Peek(0);
        public NToken NextToken()
        {
            var cur = Current;
            position++;
            return cur;
        }
        public Dictionary<string, List<MethodInfo>> Functions;
        public Dictionary<string, float> Variables;
        public NToken Peek(int offset)
        {
            int index = offset + position;
            if (index >= length)
                return null;
            return tokens[index];
        }
        public Parser(string text, TagCollection tagsReference, List<ArgumentNode> args, Dictionary<string, float> variables = null, Dictionary<string, List<MethodInfo>> functions = null)
        {
            Errors = new string[0];
            tokens = NToken.Tokenize(text)
                .Where(t => !string.IsNullOrWhiteSpace(t.Text))
                .Select(t =>
                {
                    if (t.TokenKind != NToken.Kind.String)
                        t.text = new StringBuilder(t.Text.Trim());
                    return t;
                })
                .ToArray();
            this.args = args;
            tagDict = new Dictionary<string, (int, Tag)>();
            int index = 0;
            foreach (NToken t in tokens)
            {
                if (t.TokenKind != NToken.Kind.Tag) continue;
                var key = t.Text;
                if (!tagDict.TryGetValue(key, out _))
                {
                    Tag result;
                    if ((result = tagsReference[key]) == null)
                    {
                        Errors = Errors.Add($"{Main.Language[TranslationKeys.CannotFind]} {{{key}}} {Main.Language[TranslationKeys.Tag]}!");
                        continue;
                    }
                    else
                    {
                        tagDict.Add(key, (index++, result));
                        CanUsedByNotPlaying &= TagManager.NotPlayingTags.Contains(result);
                    }
                    continue;
                }
                continue;
            }
            tags = tagDict.Values.Select(t => t.Item2).ToArray();
            position = 0;
            length = tokens.Length;
            Functions = functions;
            Variables = variables ?? new Dictionary<string, float>();
        }
        public Node ParseExpression()
        {
            Node node = ParseAS();
            IsString = node.ResultType == typeof(string);
            return node;
        }
        Node ParseAS()
        {
            Node left = ParseMDRP();
            while (true)
            {
                OperatorNode op;
                if (Current == null)
                    return left;
                switch (Current.TokenKind)
                {
                    case NToken.Kind.Add:
                        op = new OperatorNode('+', IsString);
                        break;
                    case NToken.Kind.Sub:
                        op = new OperatorNode('-', IsString);
                        break;
                    default:
                        op = new OperatorNode('\0', IsString);
                        break;
                }
                if (op.op == "\0")
                    return left;
                NextToken();
                var right = ParseMDRP();
                left = new BinaryNode(left, op, right);
            }
        }
        Node ParseMDRP()
        {
            var left = ParseUnaryExpression();
            while (true)
            {
                OperatorNode op;
                if (Current == null)
                    return left;
                IsString |= left.ResultType == typeof(string);
                switch (Current.TokenKind)
                {
                    case NToken.Kind.Mul:
                        op = new OperatorNode('*', IsString);
                        break;
                    case NToken.Kind.Div:
                        op = new OperatorNode('/', IsString);
                        break;
                    case NToken.Kind.Rem:
                        op = new OperatorNode('%', IsString);
                        break;
                    case NToken.Kind.Pow:
                        op = new OperatorNode('^', IsString);
                        break;

                    case NToken.Kind.And:
                        op = new OperatorNode("&&", IsString);
                        break;
                    case NToken.Kind.Or:
                        op = new OperatorNode("||", IsString);
                        break;
                    case NToken.Kind.Eq:
                        op = new OperatorNode("==", IsString);
                        break;
                    case NToken.Kind.Ine:
                        op = new OperatorNode("!=", IsString);
                        break;
                    case NToken.Kind.Gt:
                        op = new OperatorNode(">", IsString);
                        break;
                    case NToken.Kind.Ge:
                        op = new OperatorNode(">=", IsString);
                        break;
                    case NToken.Kind.Lt:
                        op = new OperatorNode("<", IsString);
                        break;
                    case NToken.Kind.Le:
                        op = new OperatorNode("<=", IsString);
                        break;
                    default:
                        op = new OperatorNode('\0', IsString);
                        break;
                }
                if (op.op == "\0")
                    return left;
                NextToken();
                var right = ParseUnaryExpression();
                left = new BinaryNode(left, op, right);
            }
        }
        Node ParseUnaryExpression()
        {
            while (true)
            {
                switch (Current.TokenKind)
                {
                    case NToken.Kind.Add:
                        NextToken();
                        continue;
                    case NToken.Kind.Sub:
                        NextToken();
                        var right = ParseUnaryExpression();
                        return new UnaryNode(right);
                    default:
                        return ParsePrimaryExpression();
                }
            }
        }
        Node ParsePrimaryExpression()
        {
            switch (Current.TokenKind)
            {
                case NToken.Kind.String:
                    Node node = new StringNode(Current.Text);
                    IsString = true;
                    NextToken();
                    return node;
                case NToken.Kind.Number:
                    node = new NumberNode((float)Current.Value);
                    NextToken();
                    return node;
                case NToken.Kind.Tag:
                    if (tagDict.TryGetValue(Current.Text, out var tuple))
                    {
                        node = new TagNode(tuple.Item2, Current.Option, tuple.Item1, il => il.Emit(OpCodes.Ldarg_0));
                        NextToken();
                        return node;
                    }
                    else Errors = Errors.Add($"{Main.Language[TranslationKeys.CannotFind]} '{Current.Text}' {Main.Language[TranslationKeys.Tag]}.");
                    return IsString ? new StringNode("") : new NumberNode(0);
                case NToken.Kind.LParen:
                    NextToken();
                    node = ParseAS();
                    if (Current?.TokenKind != NToken.Kind.RParen)
                        Errors = Errors.Add($"{Main.Language[TranslationKeys.MissingCloseParenthesis]}!");
                    NextToken();
                    return node;
                case NToken.Kind.Identifier:
                    var name = Current.Text;
                    NextToken();
                    if (Current?.TokenKind != NToken.Kind.LParen)
                    {
                        if (args?.Any() ?? false && name == "arg")
                        {
                            if (Current?.TokenKind != NToken.Kind.Number)
                            {
                                Errors = Errors.Add($"{Main.Language[TranslationKeys.ArgMustHaveIndex]}!");
                                return IsString ? new StringNode("") : new NumberNode(0);
                            }
                            var index = (float)Current.Value;
                            if (index >= args.Count)
                            {
                                Errors = Errors.Add($"{Main.Language[TranslationKeys.ArgIndexOutOfRange]}!");
                                return IsString ? new StringNode("") : new NumberNode(0);
                            }
                            NextToken();
                            return args[(int)index];
                        }
                        if (Variables.TryGetValue(name, out float num))
                            return IsString ? new StringNode(num.ToString()) : new NumberNode(num);
                        return IsString ? new StringNode("") : new NumberNode(0);
                    }
                    else
                    {
                        NextToken();
                        var args = new Node[0];
                        if (Current.TokenKind == NToken.Kind.RParen)
                        {
                            if (Functions.TryGetValue(name, out List<MethodInfo> meths))
                            {
                                NextToken();
                                var meth = meths.Find(m => m.GetParameters().All(p => p?.HasDefaultValue ?? false));
                                var parameters = meth.GetParameters();
                                Node[] arguments = new Node[0];
                                if (meth is DynamicMethod)
                                {
                                    if (parameters.Length == 1)
                                        return new FunctionNode(meth, new Node[0]);
                                    else if (parameters.All(p =>
                                    {
                                        if (p?.HasDefaultValue ?? false)
                                        {
                                            var defvalue = p.DefaultValue;
                                            if (defvalue is string s)
                                                arguments = arguments.Add(new StringNode(s));
                                            else arguments = arguments.Add(new NumberNode((float)p?.DefaultValue));
                                            return true;
                                        }
                                        return false;
                                    }))
                                        return new FunctionNode(meth, arguments);
                                }
                                else
                                {
                                    if (parameters.Length == 0)
                                        return new FunctionNode(meth, new Node[0]);
                                    else if (parameters.All(p =>
                                    {
                                        if (p.HasDefaultValue)
                                        {
                                            var defvalue = p.DefaultValue;
                                            if (defvalue is string s)
                                                arguments = arguments.Add(new StringNode(s));
                                            else arguments = arguments.Add(new NumberNode((float)p.DefaultValue));
                                            return true;
                                        }
                                        return false;
                                    }))
                                        return new FunctionNode(meth, arguments);
                                }
                            }
                            else
                            {
                                Errors = Errors.Add($"{Main.Language[TranslationKeys.CannotFind]} {Main.Language[TranslationKeys.Function]} {name}()");
                                return IsString ? new StringNode("") : new NumberNode(0);
                            }
                        }
                        while (true)
                        {
                            args = args.Add(ParseAS());
                            if (Current.TokenKind == NToken.Kind.Comma)
                            {
                                NextToken();
                                continue;
                            }
                            break;
                        }
                        if (Current.TokenKind != NToken.Kind.RParen)
                            Errors = Errors.Add($"{Main.Language[TranslationKeys.MissingCloseParenthesis]}!");
                        NextToken();
                        var argsLength = args.Length;
                        if (Functions.TryGetValue(name, out List<MethodInfo> ms))
                        {
                            var m = ms.Find(m =>
                            {
                                var @params = m.GetParameters();
                                if (m is DynamicMethod)
                                    return @params.Length == argsLength + 1 && 
                                        (m.ReturnType == typeof(float) || m.ReturnType == typeof(string)) &&
                                        @params.Select(p => p.ParameterType).Where(t => t != typeof(Tag[])).SequenceEqual(args.Select(n => n.ResultType));
                                else
                                    return @params.Length == argsLength &&
                                        (m.ReturnType == typeof(float) || m.ReturnType == typeof(string)) &&
                                        @params.Select(p => p.ParameterType).SequenceEqual(args.Select(n => n.ResultType));
                            });
                            if (m == null)
                                goto notfound;
                            return new FunctionNode(m, args);
                        }
                    notfound:
                        Errors = Errors.Add($"{Main.Language[TranslationKeys.CannotFind]} {Main.Language[TranslationKeys.Function]} '{name}({FormatArgument(args)})'");
                        return IsString ? new StringNode("") : new NumberNode(0);
                    }
                default:
                    Errors = Errors.Add($"{Main.Language[TranslationKeys.InvalidToken]}! (Kind:{Current.TokenKind}, Value:{Current.Text})");
                    return IsString ? new StringNode("") : new NumberNode(0);
            }
        }
        static string FormatArgument(Node[] arg)
        {
            if (arg.Length == 0)
                return "";
            if (arg.Length == 1)
                return arg[0].ResultType.Name;
            string result = arg.Aggregate("", (s, n) => $"{s}{n.ResultType.Name}, ");
            return result.Remove(result.Length - 2);
        }
    }
}
