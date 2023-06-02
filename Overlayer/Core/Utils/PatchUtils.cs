using HarmonyLib;
using System;
using System.Reflection;

namespace Overlayer.Core.Utils
{
    public static class PatchUtils
    {
        public static MethodInfo Postfix<T>(this Harmony harmony, MethodBase target, T del) where T : Delegate
            => harmony.Patch(target, postfix: new HarmonyMethod(del.Wrap()));
        public static MethodInfo Prefix<T>(this Harmony harmony, MethodBase target, T del) where T : Delegate
            => harmony.Patch(target, new HarmonyMethod(del.Wrap()));
    }
}
namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed class IgnoresAccessChecksToAttribute : Attribute
    {
        public IgnoresAccessChecksToAttribute(string assemblyName)
        {
            AssemblyName = assemblyName;
        }
        public string AssemblyName { get; }
    }
}