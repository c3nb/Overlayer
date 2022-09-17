using System;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using TagLib.Utils;

namespace TagLib.Tags.Nodes
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
        public Parser(string text, TagCollection tagsReference, Dictionary<string, float> variables = null, Dictionary<string, List<MethodInfo>> functions = null)
        {
            Errors = new string[0];
            tokens = NToken.Tokenize(text)
                .Where(t => !string.IsNullOrWhiteSpace(t.Text))
                .Select(t =>
                {
                    t.text = new StringBuilder(t.Text.Trim());
                    return t;
                })
                .ToArray();
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
                        Errors = Errors.Add($"Cannot Find {{{key}}} Tag!");
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
            return ParseAS();
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
                        op = new OperatorNode('+');
                        break;
                    case NToken.Kind.Sub:
                        op = new OperatorNode('-');
                        break;
                    default:
                        op = new OperatorNode('\0');
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
                switch (Current.TokenKind)
                {
                    case NToken.Kind.Mul:
                        op = new OperatorNode('*');
                        break;
                    case NToken.Kind.Div:
                        op = new OperatorNode('/');
                        break;
                    case NToken.Kind.Rem:
                        op = new OperatorNode('%');
                        break;
                    case NToken.Kind.Pow:
                        op = new OperatorNode('^');
                        break;

                    case NToken.Kind.And:
                        op = new OperatorNode("&&");
                        break;
                    case NToken.Kind.Or:
                        op = new OperatorNode("||");
                        break;
                    case NToken.Kind.Eq:
                        op = new OperatorNode("==");
                        break;
                    case NToken.Kind.Ine:
                        op = new OperatorNode("!=");
                        break;
                    case NToken.Kind.Gt:
                        op = new OperatorNode(">");
                        break;
                    case NToken.Kind.Ge:
                        op = new OperatorNode(">=");
                        break;
                    case NToken.Kind.Lt:
                        op = new OperatorNode("<");
                        break;
                    case NToken.Kind.Le:
                        op = new OperatorNode("<=");
                        break;
                    default:
                        op = new OperatorNode('\0');
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
                case NToken.Kind.Number:
                    Node node = new NumberNode((float)Current.Value);
                    NextToken();
                    return node;
                case NToken.Kind.Tag:
                    if (tagDict.TryGetValue(Current.Text, out var tuple))
                    {
                        node = new TagNode(tuple.Item2, Current.Option, tuple.Item1, il => il.Emit(OpCodes.Ldarg_0));
                        NextToken();
                        return node;
                    }
                    else Errors = Errors.Add($"Cannot Find '{Current.Text}' Tag.");
                    return new NumberNode(0);
                case NToken.Kind.LParen:
                    NextToken();
                    node = ParseAS();
                    if (Current?.TokenKind != NToken.Kind.RParen)
                        Errors = Errors.Add("Missing Close Parenthesis!");
                    NextToken();
                    return node;
                case NToken.Kind.Identifier:
                    var name = Current.Text;
                    NextToken();
                    if (Current?.TokenKind != NToken.Kind.LParen)
                    {
                        if (Variables.TryGetValue(name, out float num))
                            return new NumberNode(num);
                        return new NumberNode(0);
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
                                var meth = meths.Find(m => m.GetParameters().All(p => p.HasDefaultValue));
                                var parameters = meth.GetParameters();
                                Node[] arguments = new Node[0];
                                if (parameters.Length == 0)
                                    return new FunctionNode(meth, new Node[0]);
                                else if (parameters.All(p =>
                                {
                                    if (p.HasDefaultValue)
                                    {
                                        arguments = arguments.Add(new NumberNode((float)p.DefaultValue));
                                        return true;
                                    }
                                    return false;
                                }))
                                    return new FunctionNode(meth, arguments);
                            }
                            else
                            {
                                Errors = Errors.Add($"Cannot Find Function {name}()");
                                return new NumberNode(0);
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
                            Errors = Errors.Add("Missing Close Parenthesis!");
                        NextToken();
                        var argsLength = args.Length;
                        if (Functions.TryGetValue(name, out List<MethodInfo> ms))
                        {
                            var m = ms.Find(mt => mt.GetParameters().Length == argsLength);
                            if (m == null)
                                goto notfound;
                            var parameters = m.GetParameters();
                            var paramTypes = parameters.Select(p => p.ParameterType);
                            if (m.ReturnType == typeof(float))
                                return new FunctionNode(m, args);
                        }
                    notfound:
                        Errors = Errors.Add($"Cannot Find Function {name}({(argsLength == 1 ? "float" : Enumerable.Repeat("float", argsLength).Aggregate((c, n) => $"{c}, {n}"))})");
                        return new NumberNode(0);
                    }
                default:
                    Errors = Errors.Add($"Invalid Token! (Kind:{Current.TokenKind}, Value:{Current.Text})");
                    return new NumberNode(0);
            }
        }
    }
}
