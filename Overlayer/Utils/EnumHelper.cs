using System;
using System.Collections.Generic;

namespace Overlayer.Utils
{
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
        public static int IndexOf(string name) => Array.IndexOf(Names, name);
        public static int IndexOf(T value) => Array.IndexOf(Values, value);
    }
}
