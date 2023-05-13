using System;
using System.Text.RegularExpressions;
using Random = System.Random;

namespace Overlayer.Core.Utils
{
    public static class StringUtils
    {
        public const int Upper = 65;
        public const int UpperLast = 90;
        public const int Lower = 97;
        public const int LowerLast = 122;
        public const char Undefined = char.MinValue;
        public static readonly Random Random = new Random(DateTime.Now.Millisecond);
        public static readonly Regex RichTagBreaker = new Regex(@"<(color|material|quad|size)=(.|\n)*?>|<\/(color|material|quad|size)>|<(b|i)>|<\/(b|i)>", RegexOptions.Compiled | RegexOptions.Multiline);
        public static readonly Regex RichTagBreakerWithoutSize = new Regex(@"<(color|material|quad)=(.|\n)*?>|<\/(color|material|quad)>|<(b|i)>|<\/(b|i)>", RegexOptions.Compiled | RegexOptions.Multiline);
        public static string BreakRichTag(this string s)
            => RichTagBreaker.Replace(s, string.Empty);
        public static string BreakRichTagWithoutSize(this string s)
            => RichTagBreakerWithoutSize.Replace(s, string.Empty);
        public static double CalculateDifference(this string a, string b)
        {
            if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b)) return 0.0;
            if (a == b) return 1.0;
            return 1.0 - a.GetDifference(b) / Math.Max(a.Length, b.Length);
        }
        public static string Escape(this string str) => str.Replace(@"\", @"\\").Replace(":", @"\:");
        public static string GetAfter(this string str, string after)
        {
            int index = str.IndexOf(after);
            if (index < 0) return null;
            return str.Substring(index + 1, str.Length - index - 1);
        }
        public static string GetBefore(this string str, string before)
        {
            int index = str.IndexOf(before);
            if (index < 0) return null;
            return str.Substring(0, index);
        }
        public static string GetBetween(this string str, string start, string end)
        {
            int sIdx = str.IndexOf(start);
            int eIdx = str.LastIndexOf(end);
            if (sIdx < 0 || eIdx < 0) return null;
            return str.Substring(sIdx + 1, eIdx - sIdx - 1);
        }
        public static int GetDifference(this string a, string b)
        {
            int aCount = a.Length;
            int bCount = b.Length;
            int[,] d = new int[aCount + 1, bCount + 1];
            if (aCount == 0) return bCount;
            if (bCount == 0) return aCount;
            for (int i = 0; i <= aCount; d[i, 0] = i++) ;
            for (int i = 0; i <= bCount; d[0, i] = i++) ;
            for (int i = 1; i <= aCount; i++)
                for (int j = 1; j <= bCount; j++)
                {
                    int cost = b[j - 1] == a[i - 1] ? 0 : 1;
                    d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
                }
            return d[aCount, bCount];
        }
        public static char Invert(this char c) => c.IsUpper() ? c.ToLower() : c.ToUpper();
        public static unsafe string Invert(this string s)
        {
            if (s == null || s.Length <= 0) return s;
            fixed (char* ptr = s)
            {
                char* c = ptr;
                char v;
                while ((v = *c) != '\0')
                {
                    if (v.IsAlphabet())
                        *c = v.Invert();
                    c++;
                }
            }
            return s;
        }
        public static unsafe string InvertAlternately(this string s)
        {
            if (s == null || s.Length <= 0) return s;
            bool isLower = s[0].IsLower();
            fixed (char* ptr = s)
            {
                char* c = ptr;
                char v;
                while ((v = *c) != '\0')
                {
                    if (v.IsAlphabet())
                        *c = (isLower = !isLower) ? v.ToLower() : v.ToUpper();
                    c++;
                }
            }
            return s;
        }
        public static bool IsAlphabet(this char c) => c.IsUpper() || c.IsLower();
        public static bool IsLower(this char c) => c >= Lower && c <= LowerLast;
        public static bool IsUpper(this char c) => c >= Upper && c <= UpperLast;
        public static string[] Split2(this string str, char separator)
        {
            int index = str.IndexOf(separator);
            if (index < 0) return new string[] { str };
            return new string[] { str.Substring(0, index), str.Substring(index + 1, str.Length - (index + 1)) };
        }
        public static char ToLower(this char c) => c.IsLower() ? c : (char)(c + 32);
        public static unsafe string ToLowerFast(this string s)
        {
            fixed (char* ptr = s)
            {
                char* c = ptr;
                char v;
                while ((v = *c) != '\0')
                {
                    if (v.IsAlphabet())
                        *c = v.ToLower();
                    c++;
                }
            }
            return s;
        }
        public static string ToStringFast(this int i, int radix = 10)
        {
            const string chars = "0123456789ABCDEF";
            var str = new char[32];
            var idx = str.Length;
            bool isNegative = i < 0;
            if (i <= 0)
            {
                str[--idx] = chars[-(i % radix)];
                i = -(i / radix);
            }
            while (i != 0)
            {
                str[--idx] = chars[i % radix];
                i /= radix;
            }
            if (isNegative)
                str[--idx] = '-';
            return new string(str, idx, str.Length - idx);
        }
        public static char ToUpper(this char c) => c.IsUpper() ? c : (char)(c - 32);
        public static unsafe string ToUpperFast(this string s)
        {
            fixed (char* ptr = s)
            {
                char* c = ptr;
                char v;
                while ((v = *c) != '\0')
                {
                    if (v.IsAlphabet())
                        *c = v.ToUpper();
                    c++;
                }
            }
            return s;
        }
        public static string TrimBetween(this string str, string start, string end)
        {
            int sIdx = str.IndexOf(start);
            int eIdx = str.LastIndexOf(end);
            if (sIdx < 0 || eIdx < 0) return null;
            return str.Remove(sIdx + 1, eIdx - sIdx - 1);
        }
        public static string Unescape(this string str) => str.Replace(@"\:", ":").Replace(@"\\", @"\");
    }
}