using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace Overlayer.Core.Utils
{
    public static class PatchUtils
    {
        public static readonly AssemblyBuilder ass;
        public static readonly ModuleBuilder mod;
        public static int TypeCount { get; internal set; }
        static PatchUtils()
        {
            var assName = new AssemblyName("Overlayer.Core.PatchUtils_Assembly");
            ass = AssemblyBuilder.DefineDynamicAssembly(assName, AssemblyBuilderAccess.Run);
            mod = ass.DefineDynamicModule(assName.Name);
        }
        public static MethodInfo Wrap<T>(this T del) where T : Delegate
        {
            Type delType = del.GetType();
            IgnoreAccessCheck(delType);
            MethodInfo invoke = delType.GetMethod("Invoke");
            MethodInfo method = del.Method;
            TypeBuilder type = mod.DefineType(TypeCount++.ToString(), TypeAttributes.Public);
            ParameterInfo[] parameters = method.GetParameters();
            Type[] paramTypes = parameters.Select(p => p.ParameterType).ToArray();
            MethodBuilder methodB = type.DefineMethod("Wrapper", MethodAttributes.Public | MethodAttributes.Static, invoke.ReturnType, paramTypes);
            FieldBuilder delField = type.DefineField("function", delType, FieldAttributes.Public | FieldAttributes.Static);
            IgnoreAccessCheck(invoke.ReturnType);
            ILGenerator il = methodB.GetILGenerator();
            il.Emit(OpCodes.Ldsfld, delField);
            int paramIndex = 1;
            foreach (ParameterInfo param in parameters)
            {
                IgnoreAccessCheck(param.ParameterType);
                methodB.DefineParameter(paramIndex++, ParameterAttributes.None, param.Name);
                il.Emit(OpCodes.Ldarg, paramIndex - 2);
            }
            il.Emit(OpCodes.Call, invoke);
            il.Emit(OpCodes.Ret);
            Type t = type.CreateType();
            t.GetField("function").SetValue(null, del);
            return t.GetMethod("Wrapper");
        }
        public static MethodInfo Prefix<T>(this Harmony harmony, MethodBase target, T del) where T : Delegate
            => harmony.Patch(target, new HarmonyMethod(Wrap(del)));
        public static MethodInfo Postfix<T>(this Harmony harmony, MethodBase target, T del) where T : Delegate
            => harmony.Patch(target, postfix: new HarmonyMethod(Wrap(del)));
        static readonly HashSet<string> accessIgnored = new HashSet<string>();
        internal static void IgnoreAccessCheck(Type type)
        {
            var name = type.Assembly.GetName();
            if (name.Name.StartsWith("System"))
                return;
            if (accessIgnored.Add(name.Name))
                ass.SetCustomAttribute(GetIACT(name.Name));
        }
        static CustomAttributeBuilder GetIACT(string name) => new CustomAttributeBuilder(iact, new[] { name });
        static readonly ConstructorInfo iact = typeof(IgnoresAccessChecksToAttribute).GetConstructor(new[] { typeof(string) });
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
