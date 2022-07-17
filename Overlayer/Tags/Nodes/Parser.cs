using System;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using Overlayer.Utils;

namespace Overlayer.Tags.Nodes
{
    public class Parser
    {
        public Token[] tokens;
        public Tag[] tags;
        public Dictionary<string, (int, Tag)> tagDict;
        public string[] Errors;
        public int position;
        public int length;
        public bool CanUsedByNotPlaying = true;
        public Token Current => Peek(0);
        public Token NextToken()
        {
            var cur = Current;
            position++;
            return cur;
        }
        public Dictionary<string, MethodInfo> Functions;
        public Dictionary<string, float> Variables;
        public Token Peek(int offset)
        {
            int index = offset + position;
            if (index >= length)
                return null;
            return tokens[index];
        }
        public Parser(string text, TagCollection tagsReference, Dictionary<string, float> variables = null, Dictionary<string, MethodInfo> functions = null)
        {
            Errors = new string[0];
            tokens = Token.Tokenize(text)
                .Where(t => !string.IsNullOrWhiteSpace(t.Text))
                .Select(t =>
                {
                    t.text = new StringBuilder(t.Text.Trim());
                    return t;
                })
                .ToArray();
            tagDict = new Dictionary<string, (int, Tag)>();
            int index = 0;
            foreach (Token t in tokens)
            {
                if (t.TokenKind != Token.Kind.Tag) continue;
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
                        CanUsedByNotPlaying = Main.NotPlayingTags.Contains(result);
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
            Node left = ParseMDR();
            while (true)
            {
                OperatorNode op;
                if (Current == null)
                    return left;
                switch (Current.TokenKind)
                {
                    case Token.Kind.Add:
                        op = new OperatorNode('+');
                        break;
                    case Token.Kind.Sub:
                        op = new OperatorNode('-');
                        break;
                    default:
                        op = new OperatorNode('\0');
                        break;
                }
                if (op.op == '\0')
                    return left;
                NextToken();
                var right = ParseMDR();
                left = new BinaryNode(left, op, right);
            }
        }
        Node ParseMDR()
        {
            var left = ParseUnaryExpression();
            while (true)
            {
                OperatorNode op;
                if (Current == null)
                    return left;
                switch (Current.TokenKind)
                {
                    case Token.Kind.Mul:
                        op = new OperatorNode('*');
                        break;
                    case Token.Kind.Div:
                        op = new OperatorNode('/');
                        break;
                    case Token.Kind.Rem:
                        op = new OperatorNode('%');
                        break;
                    default:
                        op = new OperatorNode('\0');
                        break;
                }
                if (op.op == '\0')
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
                    case Token.Kind.Add:
                        NextToken();
                        continue;
                    case Token.Kind.Sub:
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
                case Token.Kind.Number:
                    Node node = new NumberNode((float)Current.Value);
                    NextToken();
                    return node;
                case Token.Kind.Tag:
                    if (tagDict.TryGetValue(Current.Text, out var tuple))
                    {
                        node = new TagNode(tuple.Item2, Current.Option, tuple.Item1, il => il.Emit(OpCodes.Ldarg_0));
                        NextToken();
                        return node;
                    }
                    return new NumberNode(0);
                case Token.Kind.LParen:
                    NextToken();
                    node = ParseAS();
                    if (Current?.TokenKind != Token.Kind.RParen)
                        Errors = Errors.Add("Missing Close Parenthesis!");
                    NextToken();
                    return node;
                case Token.Kind.Identifier:
                    var name = Current.Text;
                    NextToken();
                    if (Current?.TokenKind != Token.Kind.LParen)
                    {
                        if (Variables.TryGetValue(name, out float num))
                            return new NumberNode(num);
                        return new NumberNode(0);
                    }
                    else
                    {
                        NextToken();
                        var args = new Node[0];
                        while (true)
                        {
                            args = args.Add(ParseAS());
                            if (Current.TokenKind == Token.Kind.Comma)
                            {
                                NextToken();
                                continue;
                            }
                            break;
                        }
                        if (Current.TokenKind != Token.Kind.RParen)
                            Errors = Errors.Add("Missing Close Parenthesis!");
                        NextToken();
                        var argsLength = args.Length;
                        if (Functions.TryGetValue(name, out MethodInfo m))
                        {
                            var parameters = m.GetParameters();
                            var paramTypes = parameters.Select(p => p.ParameterType);
                            if (parameters.Length == argsLength && paramTypes.All(t => t == typeof(float)) && m.ReturnType == typeof(float))
                                return new FunctionNode(m, args);
                        }
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
