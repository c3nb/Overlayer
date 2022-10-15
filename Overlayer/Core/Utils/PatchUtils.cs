using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Overlayer.Core.Utils
{
    public static class PatchUtils
    {
        public static readonly AssemblyBuilder ass;
        public static readonly ModuleBuilder mod;
        public static int TypeCount { get; private set; }
        static PatchUtils()
        {
            var assName = new AssemblyName("Overlayer.Core.PatchUtils_Assembly");
            ass = AssemblyBuilder.DefineDynamicAssembly(assName, AssemblyBuilderAccess.Run);
            mod = ass.DefineDynamicModule(assName.Name);
        }
        public static MethodInfo Wrap<T>(this T del) where T : Delegate
        {
            Type delType = del.GetType();
            MethodInfo invoke = delType.GetMethod("Invoke");
            MethodInfo method = del.Method;
            TypeBuilder type = mod.DefineType(TypeCount++.ToString(), TypeAttributes.Public);
            ParameterInfo[] parameters = method.GetParameters();
            Type[] paramTypes = parameters.Select(p => p.ParameterType).ToArray();
            MethodBuilder methodB = type.DefineMethod("Wrapper", MethodAttributes.Public | MethodAttributes.Static, invoke.ReturnType, paramTypes);
            ILGenerator il = methodB.GetILGenerator();
            int paramIndex = 1;
            foreach (ParameterInfo param in parameters)
            {
                methodB.DefineParameter(paramIndex++, ParameterAttributes.None, param.Name);
                il.Emit(OpCodes.Ldarg, paramIndex - 2);
            }
            il.Emit(OpCodes.Call, invoke);
            il.Emit(OpCodes.Ret);
            return type.CreateType().GetMethod("Wrapper");
        }
        public static MethodInfo Prefix<T>(this Harmony harmony, MethodBase target, T del) where T : Delegate
            => harmony.Patch(target, new HarmonyMethod(Wrap(del)));
        public static MethodInfo Postfix<T>(this Harmony harmony, MethodBase target, T del) where T : Delegate
            => harmony.Patch(target, postfix: new HarmonyMethod(Wrap(del)));
    }
}
