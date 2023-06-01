using HarmonyLib;
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Overlayer.Core.Utils
{
    public static class PatchUtils
    {
        public static int TypeCount { get; internal set; }
        public static MethodInfo Postfix<T>(this Harmony harmony, MethodBase target, T del) where T : Delegate
            => harmony.Patch(target, postfix: new HarmonyMethod(del.Wrap()));
        public static MethodInfo Prefix<T>(this Harmony harmony, MethodBase target, T del) where T : Delegate
            => harmony.Patch(target, new HarmonyMethod(del.Wrap()));
        public static MethodInfo Wrap<T>(this T del) where T : Delegate
        {
            Type delType = del.GetType();
            EmitUtils.IgnoreAccessCheck(delType);
            MethodInfo invoke = delType.GetMethod("Invoke");
            MethodInfo method = del.Method;
            TypeBuilder type = EmitUtils.Mod.DefineType(TypeCount++.ToString(), TypeAttributes.Public);
            ParameterInfo[] parameters = method.GetParameters();
            Type[] paramTypes = parameters.Select(p => p.ParameterType).ToArray();
            MethodBuilder methodB = type.DefineMethod("Wrapper", MethodAttributes.Public | MethodAttributes.Static, invoke.ReturnType, paramTypes);
            FieldBuilder delField = type.DefineField("function", delType, FieldAttributes.Public | FieldAttributes.Static);
            EmitUtils.IgnoreAccessCheck(invoke.ReturnType);
            ILGenerator il = methodB.GetILGenerator();
            il.Emit(OpCodes.Ldsfld, delField);
            int paramIndex = 1;
            foreach (ParameterInfo param in parameters)
            {
                EmitUtils.IgnoreAccessCheck(param.ParameterType);
                methodB.DefineParameter(paramIndex++, ParameterAttributes.None, param.Name);
                il.Emit(OpCodes.Ldarg, paramIndex - 2);
            }
            il.Emit(OpCodes.Call, invoke);
            il.Emit(OpCodes.Ret);
            Type t = type.CreateType();
            t.GetField("function").SetValue(null, del);
            return t.GetMethod("Wrapper");
        }
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