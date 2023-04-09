using System;
using System.Collections.Generic;
using System.Reflection;

namespace Overlayer.Core
{
    public static class StringConverter
    {
        public static unsafe sbyte ToInt8(string s)
        {
            if (s.Length == 0) return 0;
            sbyte result = 0;
            bool unary = s[0] == 45;
            fixed (char* v = s)
            {
                char* c = v;
                if (unary) c++;
                while (*c != '\0')
                {
                    result = (sbyte)(10 * result + (*c - 48));
                    c++;
                }
            }
            if (unary)
                return (sbyte)-result;
            return result;
        }
        public static string FromInt8(sbyte s) => s.ToString();
        public static unsafe short ToInt16(string s)
        {
            if (s.Length == 0) return 0;
            short result = 0;
            bool unary = s[0] == 45;
            fixed (char* v = s)
            {
                char* c = v;
                if (unary) c++;
                while (*c != '\0')
                {
                    result = (short)(10 * result + (*c - 48));
                    c++;
                }
            }
            if (unary)
                return (short)-result;
            return result;
        }
        public static string FromInt16(short s) => s.ToString();
        public static unsafe int ToInt32(string s)
        {
            if (s.Length == 0) return 0;
            int result = 0;
            bool unary = s[0] == 45;
            fixed (char* v = s)
            {
                char* c = v;
                if (unary) c++;
                while (*c != '\0')
                {
                    result = 10 * result + (*c - 48);
                    c++;
                }
            }
            if (unary)
                return -result;
            return result;
        }
        public static string FromInt32(int s) => s.ToString();
        public static unsafe long ToInt64(string s)
        {
            if (s.Length == 0) return 0;
            long result = 0;
            bool unary = s[0] == 45;
            fixed (char* v = s)
            {
                char* c = v;
                if (unary) c++;
                while (*c != '\0')
                {
                    result = 10 * result + (*c - 48);
                    c++;
                }
            }
            if (unary)
                return -result;
            return result;
        }
        public static string FromInt64(long s) => s.ToString();
        public static unsafe byte ToUInt8(string s)
        {
            if (s.Length == 0) return 0;
            byte result = 0;
            fixed (char* v = s)
            {
                char* c = v;
                while (*c != '\0')
                {
                    result = (byte)(10 * result + (*c - 48));
                    c++;
                }
            }
            return result;
        }
        public static string FromUInt8(byte s) => s.ToString();
        public static unsafe ushort ToUInt16(string s)
        {
            if (s.Length == 0) return 0;
            ushort result = 0;
            fixed (char* v = s)
            {
                char* c = v;
                while (*c != '\0')
                {
                    result = (ushort)(10 * result + (*c - 48));
                    c++;
                }
            }
            return result;
        }
        public static string FromUInt16(ushort s) => s.ToString();
        public static unsafe uint ToUInt32(string s)
        {
            if (s.Length == 0) return 0;
            uint result = 0;
            fixed (char* v = s)
            {
                char* c = v;
                while (*c != '\0')
                {
                    result = (uint)(10 * result + (*c - 48));
                    c++;
                }
            }
            return result;
        }
        public static string FromUInt32(uint s) => s.ToString();
        public static unsafe ulong ToUInt64(string s)
        {
            if (s.Length == 0) return 0;
            ulong result = 0;
            fixed (char* v = s)
            {
                char* c = v;
                while (*c != '\0')
                {
                    result = 10 * result + (*c - 48ul);
                    c++;
                }
            }
            return result;
        }
        public static string FromUInt64(ulong s) => s.ToString();
        public static unsafe double ToDouble(string s)
        {
            if (s.Length == 0) return 0;
            double result = 0;
            bool isDot = false;
            int dCount = 1;
            bool unary = s[0] == 45;
            fixed (char* v = s)
            {
                char* c = v;
                if (unary) c++;
                while (*c != '\0')
                {
                    if (*c == '.')
                    {
                        isDot = true;
                        goto Continue;
                    }
                    if (!isDot)
                        result = 10 * result + (*c - 48);
                    else result += (*c - 48) / dPow[dCount++];
                    Continue:
                    c++;
                }
            }
            if (unary)
                return -result;
            return result;
        }
        public static string FromDouble(double s) => s.ToString();
        public static unsafe float ToFloat(string s)
        {
            if (s.Length == 0) return 0;
            float result = 0;
            bool isDot = false;
            int dCount = 1;
            bool unary = s[0] == 45;
            fixed (char* v = s)
            {
                char* c = v;
                if (unary) c++;
                while (*c != '\0')
                {
                    if (*c == '.')
                    {
                        isDot = true;
                        goto Continue;
                    }
                    if (!isDot)
                        result = 10 * result + (*c - 48);
                    else result += (*c - 48) / fPow[dCount++];
                    Continue:
                    c++;
                }
            }
            if (unary)
                return -result;
            return result;
        }
        public static string FromFloat(float s) => s.ToString();
        private static readonly double[] dPow = GetDoublePow();
        private static double[] GetDoublePow()
        {
            var max = 309;
            var exps = new double[max];
            for (var i = 0; i < max; i++)
                exps[i] = Math.Pow(10, i);
            return exps;
        }
        private static readonly float[] fPow = GetFloatPow();
        private static float[] GetFloatPow()
        {
            var max = 39;
            var exps = new float[max];
            for (var i = 0; i < max; i++)
                exps[i] = (float)Math.Pow(10, i);
            return exps;
        }
        public static T ToEnum<T>(string s) where T : Enum => EnumHelper<T>.Parse(s);
        public static string FromEnum<T>(T e) where T : Enum => e.ToString();
        public static MethodInfo GetToConverter(Type numType)
        {
            if (numType == typeof(sbyte)) return TInt8;
            else if (numType == typeof(short)) return TInt16;
            else if (numType == typeof(int)) return TInt32;
            else if (numType == typeof(long)) return TInt64;
            else if (numType == typeof(byte)) return TUInt8;
            else if (numType == typeof(ushort)) return TUInt16;
            else if (numType == typeof(uint)) return TUInt32;
            else if (numType == typeof(ulong)) return TUInt64;
            else if (numType == typeof(float)) return TFloat;
            else if (numType == typeof(double)) return TDouble;
            else if (typeof(Enum).IsAssignableFrom(numType)) return TEnum.MakeGenericMethod(numType);
            else return null;
        }
        public static MethodInfo GetFromConverter(Type numType)
        {
            if (numType == typeof(sbyte)) return FInt8;
            else if (numType == typeof(short)) return FInt16;
            else if (numType == typeof(int)) return FInt32;
            else if (numType == typeof(long)) return FInt64;
            else if (numType == typeof(byte)) return FUInt8;
            else if (numType == typeof(ushort)) return FUInt16;
            else if (numType == typeof(uint)) return FUInt32;
            else if (numType == typeof(ulong)) return FUInt64;
            else if (numType == typeof(float)) return FFloat;
            else if (numType == typeof(double)) return FDouble;
            else if (typeof(Enum).IsAssignableFrom(numType)) return FEnum.MakeGenericMethod(numType);
            else return FObject;
        }
        public static string FromObject(object s) => s?.ToString();
        public static readonly MethodInfo TInt8 = typeof(StringConverter).GetMethod("ToInt8");
        public static readonly MethodInfo TInt16 = typeof(StringConverter).GetMethod("ToInt16");
        public static readonly MethodInfo TInt32 = typeof(StringConverter).GetMethod("ToInt32");
        public static readonly MethodInfo TInt64 = typeof(StringConverter).GetMethod("ToInt64");
        public static readonly MethodInfo TUInt8 = typeof(StringConverter).GetMethod("ToUInt8");
        public static readonly MethodInfo TUInt16 = typeof(StringConverter).GetMethod("ToUInt16");
        public static readonly MethodInfo TUInt32 = typeof(StringConverter).GetMethod("ToUInt32");
        public static readonly MethodInfo TUInt64 = typeof(StringConverter).GetMethod("ToUInt64");
        public static readonly MethodInfo TFloat = typeof(StringConverter).GetMethod("ToFloat");
        public static readonly MethodInfo TDouble = typeof(StringConverter).GetMethod("ToDouble");
        public static readonly MethodInfo TEnum = typeof(StringConverter).GetMethod("ToEnum");
        public static readonly MethodInfo FInt8 = typeof(StringConverter).GetMethod("FromInt8");
        public static readonly MethodInfo FInt16 = typeof(StringConverter).GetMethod("FromInt16");
        public static readonly MethodInfo FInt32 = typeof(StringConverter).GetMethod("FromInt32");
        public static readonly MethodInfo FInt64 = typeof(StringConverter).GetMethod("FromInt64");
        public static readonly MethodInfo FUInt8 = typeof(StringConverter).GetMethod("FromUInt8");
        public static readonly MethodInfo FUInt16 = typeof(StringConverter).GetMethod("FromUInt16");
        public static readonly MethodInfo FUInt32 = typeof(StringConverter).GetMethod("FromUInt32");
        public static readonly MethodInfo FUInt64 = typeof(StringConverter).GetMethod("FromUInt64");
        public static readonly MethodInfo FFloat = typeof(StringConverter).GetMethod("FromFloat");
        public static readonly MethodInfo FDouble = typeof(StringConverter).GetMethod("FromDouble");
        public static readonly MethodInfo FEnum = typeof(StringConverter).GetMethod("FromEnum");
        public static readonly MethodInfo FObject = typeof(StringConverter).GetMethod("FromObject");
    }
    public static class EnumHelper<T> where T : Enum
    {
        static readonly string[] Names;
        static readonly T[] Values;
        static readonly Dictionary<string, T> NameValues;
        static EnumHelper()
        {
            NameValues = new Dictionary<string, T>();
            Names = Enum.GetNames(typeof(T));
            Values = (T[])Enum.GetValues(typeof(T));
            for (int i = 0; i < Names.Length; i++)
                NameValues[Names[i]] = Values[i];
        }
        public static T Parse(string name) => NameValues.TryGetValue(name, out var value) ? value : default;
        public static bool TryParse(string name, out T value) => NameValues.TryGetValue(name, out value);
        public static string[] GetNames() => Names;
        public static T[] GetValues() => Values;
    }
}
