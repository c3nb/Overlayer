using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Overlayer.Core.ExceptionHandling
{
    public static class StackTraceParser
    {
        #region Regex
        const string Space = @"[\x20\t]";
        const string NotSpace = @"[^\x20\t]";
        static readonly Regex Regex = new Regex(@"
            ^
            " + Space + @"*
            \w+ " + Space + @"+
            (?<frame>
                (?<type> " + NotSpace + @"+ ) \.
                (?<method> " + NotSpace + @"+? ) " + Space + @"*
                (?<params>  \( ( " + Space + @"* \)
                               |                    (?<pt> .+?) " + Space + @"+ (?<pn> .+?)
                                 (, " + Space + @"* (?<pt> .+?) " + Space + @"+ (?<pn> .+?) )* \) ) )
                ( " + Space + @"+
                    ( # Microsoft .NET stack traces
                    \w+ " + Space + @"+
                    (?<file> ( [a-z] \: # Windows rooted path starting with a drive letter
                             | / )      # *nix rooted path starting with a forward-slash
                             .+? )
                    \: \w+ " + Space + @"+
                    (?<line> [0-9]+ ) \p{P}?
                    | # Mono stack traces
                    (?<iloffset>\[0x[0-9a-f]+\] )" + Space + @"+ \w+ " + Space + @"+
                    <(?<file> [^>]+ )>
                    :(?<line> [0-9]+ )
                    )
                )?
            )
            \s*
            $",
            RegexOptions.IgnoreCase
            | RegexOptions.Multiline
            | RegexOptions.ExplicitCapture
            | RegexOptions.CultureInvariant
            | RegexOptions.IgnorePatternWhitespace
            | RegexOptions.Compiled,
            // Cap the evaluation time to make it obvious should the expression
            // fall into the "catastrophic backtracking" trap due to over
            // generalization.
            // https://github.com/atifaziz/StackTraceParser/issues/4
            TimeSpan.FromSeconds(5));
        #endregion
        public static IEnumerable<Frame> Parse(string stackTrace)
        {
            return Parse(
                stackTrace,
                (idx, len, txt) => new Token
                {
                    Index = idx,
                    Text = txt,
                },
                (type, method) => new Method
                {
                    Type = type,
                    Name = method,
                },
                (type, name) => new Parameter
                {
                    Type = type,
                    Name = name,
                },
                (pl, ps) => new Parameters
                {
                    List = pl,
                    Params = ps,
                },
                (file, line) => new Source
                {
                    File = file,
                    Line = line,
                },
                (f, tm, p, fl) => new Frame
                {
                    Raw = f,
                    Method = tm,
                    Parameters = p,
                    Source = fl
                });
        }
        public static IEnumerable<Frame> ParseMono(string stackTrace)
        {
            return ParseMono(
                stackTrace,
                (idx, len, txt) => new Token
                {
                    Index = idx,
                    Text = txt,
                },
                (type, method, il) => new Method
                {
                    Type = type,
                    Name = method,
                    ILOffset = il
                },
                (type, name) => new Parameter
                {
                    Type = type,
                    Name = name,
                },
                (pl, ps) => new Parameters
                {
                    List = pl,
                    Params = ps,
                },
                (file, line) => new Source
                {
                    File = file,
                    Line = line,
                },
                (f, tm, p, fl) => new Frame
                {
                    Raw = f,
                    Method = tm,
                    Parameters = p,
                    Source = fl
                });
        }
        public static IEnumerable<T> Parse<T>(
            string text,
            Func<string, string, string, string, IEnumerable<KeyValuePair<string, string>>, string, string, T> selector)
        {
            if (selector == null) throw new ArgumentNullException("selector");

            return Parse(text, (idx, len, txt) => txt,
                               (t, m) => new { Type = t, Method = m },
                               (pt, pn) => new KeyValuePair<string, string>(pt, pn),
                               // ReSharper disable once PossibleMultipleEnumeration
                               (pl, ps) => new { List = pl, Items = ps },
                               (fn, ln) => new { File = fn, Line = ln },
                               (f, tm, p, fl) => selector(f, tm.Type, tm.Method, p.List, p.Items, fl.File, fl.Line));
        }
        public static IEnumerable<TFrame> Parse<TToken, TMethod, TParameters, TParameter, TSourceLocation, TFrame>(
            string text,
            Func<int, int, string, TToken> tokenSelector,
            Func<TToken, TToken, TMethod> methodSelector,
            Func<TToken, TToken, TParameter> parameterSelector,
            Func<TToken, IEnumerable<TParameter>, TParameters> parametersSelector,
            Func<TToken, TToken, TSourceLocation> sourceLocationSelector,
            Func<TToken, TMethod, TParameters, TSourceLocation, TFrame> selector)
        {
            if (tokenSelector == null) throw new ArgumentNullException("tokenSelector");
            if (methodSelector == null) throw new ArgumentNullException("methodSelector");
            if (parameterSelector == null) throw new ArgumentNullException("parameterSelector");
            if (parametersSelector == null) throw new ArgumentNullException("parametersSelector");
            if (sourceLocationSelector == null) throw new ArgumentNullException("sourceLocationSelector");
            if (selector == null) throw new ArgumentNullException("selector");

            return from Match m in Regex.Matches(text)
                   select m.Groups into groups
                   let pt = groups["pt"].Captures
                   let pn = groups["pn"].Captures
                   select selector(Token(groups["frame"], tokenSelector),
                                   methodSelector(
                                       Token(groups["type"], tokenSelector),
                                       Token(groups["method"], tokenSelector)),
                                   parametersSelector(
                                       Token(groups["params"], tokenSelector),
                                       from i in Enumerable.Range(0, pt.Count)
                                       select parameterSelector(Token(pt[i], tokenSelector),
                                                                Token(pn[i], tokenSelector))),
                                   sourceLocationSelector(Token(groups["file"], tokenSelector),
                                                          Token(groups["line"], tokenSelector)));
        }
        public static IEnumerable<TFrame> ParseMono<TToken, TMethod, TParameters, TParameter, TSourceLocation, TFrame>(
            string text,
            Func<int, int, string, TToken> tokenSelector,
            Func<TToken, TToken, TToken, TMethod> methodSelector,
            Func<TToken, TToken, TParameter> parameterSelector,
            Func<TToken, IEnumerable<TParameter>, TParameters> parametersSelector,
            Func<TToken, TToken, TSourceLocation> sourceLocationSelector,
            Func<TToken, TMethod, TParameters, TSourceLocation, TFrame> selector)
        {
            if (tokenSelector == null) throw new ArgumentNullException("tokenSelector");
            if (methodSelector == null) throw new ArgumentNullException("methodSelector");
            if (parameterSelector == null) throw new ArgumentNullException("parameterSelector");
            if (parametersSelector == null) throw new ArgumentNullException("parametersSelector");
            if (sourceLocationSelector == null) throw new ArgumentNullException("sourceLocationSelector");
            if (selector == null) throw new ArgumentNullException("selector");

            return from Match m in Regex.Matches(text)
                   select m.Groups into groups
                   let pt = groups["pt"].Captures
                   let pn = groups["pn"].Captures
                   select selector(Token(groups["frame"], tokenSelector),
                                   methodSelector(
                                       Token(groups["type"], tokenSelector),
                                       Token(groups["method"], tokenSelector),
                                       TokenTrim(groups["iloffset"], tokenSelector)),
                                   parametersSelector(
                                       Token(groups["params"], tokenSelector),
                                       from i in Enumerable.Range(0, pt.Count)
                                       select parameterSelector(Token(pt[i], tokenSelector),
                                                                Token(pn[i], tokenSelector))),
                                   sourceLocationSelector(Token(groups["file"], tokenSelector),
                                                          Token(groups["line"], tokenSelector)));
        }
        static T Token<T>(Capture capture, Func<int, int, string, T> tokenSelector) => tokenSelector(capture.Index, capture.Length, capture.Value);
        static T TokenTrim<T>(Capture capture, Func<int, int, string, T> tokenSelector) => tokenSelector(capture.Index, capture.Length, capture.Value.Substring(1, capture.Value.Length - 2));
    }
    public struct Token
    {
        public int Index;
        public int Length;
        public string Text;
    }
    public struct Method
    {
        public Token Type;
        public Token Name;
        public Token? ILOffset;
    }
    public struct Parameter
    {
        public Token Type;
        public Token Name;
    }
    public struct Parameters
    {
        public Token List;
        public IEnumerable<Parameter> Params;
    }
    public struct Source
    {
        public Token File;
        public Token Line;
    }
    public struct Frame
    {
        public Token Raw;
        public Method Method;
        public Parameters Parameters;
        public IEnumerable<Parameter> Params;
        public Source Source;
    }
}
