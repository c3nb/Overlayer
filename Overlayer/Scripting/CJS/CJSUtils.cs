using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using Overlayer.Core;
using System.IO;
using Overlayer.Core.Utils;
using JSEngine;
using JSEngine.Library;
using ILGenerator = System.Reflection.Emit.ILGenerator;

namespace Overlayer.Scripting.CJS
{
    public static class CJSUtils
    {
        static Dictionary<string, object> apis;
        public static ScriptEngine Prepare()
        {
            var engine = new ScriptEngine();
            engine.EnableExposedClrTypes = true;
            foreach (var tag in TagManager.All)
                engine.SetGlobalFunction(tag.Name, tag.GetterDelegate);
            if (apis == null)
            {
                apis = new Dictionary<string, object>();
                foreach (var api in Api.GetApiMethods(ScriptType.CompilableJS))
                    apis.Add(api.Name, api);
                foreach (var (attr, t) in Api.GetApiTypesWithAttr(ScriptType.CompilableJS))
                    apis.Add(attr.Name ?? t.Name, t);
            }
            foreach (var api in apis)
                engine.SetGlobalValue(api.Key, api.Value);
            return engine;
        }
        public static Result Compile(string path)
        {
            string source;
            if (File.Exists(path))
                source = File.ReadAllText(path);
            else if (File.Exists(path = Path.Combine(Main.ScriptPath, path)))
                source = File.ReadAllText(path);
            else return null;
            return new Result(Prepare(), source);
        }
        public static Result CompileSource(string source) => new Result(Prepare(), source);
        static ParameterInfo[] SelectActualParams(MethodBase m, ParameterInfo[] p, string[] n)
        {
            Type dType = m.DeclaringType;
            List<ParameterInfo> pList = new List<ParameterInfo>();
            for (int i = 0; i < n.Length; i++)
            {
                int index = Array.FindIndex(p, pa => pa.Name == n[i]);
                if (index > 0)
                    pList.Add(p[index]);
                else
                {
                    string s = n[i];
                    switch (s)
                    {
                        case "__instance":
                            pList.Add(new CustomParameter(dType, s));
                            break;
                        case "__originalMethod":
                            pList.Add(new CustomParameter(typeof(MethodBase), s));
                            break;
                        case "__args":
                            pList.Add(new CustomParameter(typeof(MethodBase), s));
                            break;
                        case "__result":
                            pList.Add(new CustomParameter(m is MethodInfo mi ? mi.ReturnType : typeof(object), s));
                            break;
                        case "__exception":
                            pList.Add(new CustomParameter(typeof(Exception), s));
                            break;
                        case "__runOriginal":
                            pList.Add(new CustomParameter(typeof(bool), s));
                            break;
                        case "il":
                            pList.Add(new CustomParameter(typeof(ILGenerator), s));
                            break;
                        case "instructions":
                            pList.Add(new CustomParameter(typeof(IEnumerable<CodeInstruction>), s));
                            break;
                        default:
                            if (s.StartsWith("__"))
                            {
                                if (int.TryParse(s.Substring(0, 2), out int num))
                                {
                                    if (num < 0 || num >= p.Length)
                                        return null;
                                    pList.Add(new CustomParameter(p[num].ParameterType, s));
                                }
                                else return null;
                            }
                            else if (s.StartsWith("___"))
                            {
                                string name = s.Substring(0, 3);
                                FieldInfo field = dType.GetField(name, AccessTools.all);
                                if (field == null)
                                    return null;
                            }
                            break;
                    }
                }
            }
            return pList.ToArray();
        }
        public static MethodInfo Wrap(this UserDefinedFunction func, MethodBase target, bool rtIsBool)
        {
            if (func == null) return null;
            UDFWrapper holder = new UDFWrapper(func);

            TypeBuilder type = EmitUtils.Mod.DefineType(PatchUtils.TypeCount++.ToString(), TypeAttributes.Public);
            var prmStrs = holder.args;
            ParameterInfo[] parameters = SelectActualParams(target, target.GetParameters(), prmStrs.ToArray());
            if (parameters == null) return null;
            Type[] paramTypes = parameters.Select(p => p.ParameterType).ToArray();
            MethodBuilder methodB = type.DefineMethod("Wrapper", MethodAttributes.Public | MethodAttributes.Static, rtIsBool ? typeof(bool) : typeof(void), paramTypes);
            FieldBuilder holderfld = type.DefineField("holder", typeof(UDFWrapper), FieldAttributes.Public | FieldAttributes.Static);

            var il = methodB.GetILGenerator();
            LocalBuilder arr = il.MakeArray<object>(parameters.Length);
            int paramIndex = 1;
            foreach (ParameterInfo param in parameters)
            {
                Type pType = param.ParameterType;
                EmitUtils.IgnoreAccessCheck(pType);
                methodB.DefineParameter(paramIndex++, ParameterAttributes.None, param.Name);
                int pIndex = paramIndex - 2;
                il.Emit(OpCodes.Ldloc, arr);
                il.Emit(OpCodes.Ldc_I4, pIndex);
                il.Emit(OpCodes.Ldarg, pIndex);
                il.Emit(OpCodes.Stelem_Ref);
            }
            il.Emit(OpCodes.Ldsfld, holderfld);
            il.Emit(OpCodes.Ldloc, arr);
            il.Emit(OpCodes.Call, UDFWrapper.CallMethod);
            if (rtIsBool)
                il.Emit(OpCodes.Call, istrue);
            else il.Emit(OpCodes.Pop);
            il.Emit(OpCodes.Ret);

            Type t = type.CreateType();
            t.GetField("holder").SetValue(null, holder);
            return t.GetMethod("Wrapper");
        }
        public static MethodInfo WrapTranspiler(this UserDefinedFunction func)
        {
            if (func == null) return null;
            UDFWrapper holder = new UDFWrapper(func);

            TypeBuilder type = EmitUtils.Mod.DefineType(PatchUtils.TypeCount++.ToString(), TypeAttributes.Public);
            MethodBuilder methodB = type.DefineMethod("Wrapper_Transpiler", MethodAttributes.Public | MethodAttributes.Static, typeof(IEnumerable<CodeInstruction>), 
                new[] { typeof(IEnumerable<CodeInstruction>), typeof(MethodBase), typeof(ILGenerator) });
            FieldBuilder holderfld = type.DefineField("holder", typeof(UDFWrapper), FieldAttributes.Public | FieldAttributes.Static);

            var il = methodB.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Ldsfld, holderfld);
            il.Emit(OpCodes.Call, transpilerAdapter);
            il.Emit(OpCodes.Ret);

            Type t = type.CreateType();
            t.GetField("holder").SetValue(null, holder);
            return t.GetMethod("Wrapper_Transpiler");
        }
        public static IEnumerable<CodeInstruction> TranspilerAdapter(IEnumerable<CodeInstruction> instructions, MethodBase original, ILGenerator il, UDFWrapper func)
        {
            object[] args = new object[func.args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                var argName = func.args[i];
                if (argName.StartsWith("il"))
                    args[i] = il;
                else if (argName.StartsWith("o") ||
                    argName.StartsWith("m"))
                    args[i] = original;
                else if (argName.StartsWith("ins"))
                    args[i] = FromObject(instructions.ToArray(), func.udf.Engine);
                else args[i] = Undefined.Value;
            }
            var result = ToObject(func.Call(args) as ObjectInstance);
            if (IsNull(result)) return Enumerable.Empty<CodeInstruction>();
            else return ((object[])result).Cast<CodeInstruction>();
        }
        public static bool IsNull(object obj)
            => obj == null || obj == Null.Value || obj == Undefined.Value;
        public static bool IsTrue(object obj)
            => IsNull(obj) || obj.Equals(true);
        static readonly MethodInfo istrue = typeof(CJSUtils).GetMethod("IsTrue", AccessTools.all);
        static readonly MethodInfo transpilerAdapter = typeof(CJSUtils).GetMethod("TranspilerAdapter", AccessTools.all);
        public static ObjectInstance FromObject(object value, ScriptEngine engine)
        {
            if (value is IEnumerable arr) return engine.Array.New(arr.Cast<object>().ToArray());
            else return engine.Object.Construct(value);
        }
        public static object ToObject(object value)
        {
            if (value is not ObjectInstance objInst) return value;
            if (objInst is ArrayInstance arr) return arr.ElementValues.ToArray();
            if (objInst is BooleanInstance b)
                return b.Value;
            else if (objInst is NumberInstance num)
                return num.Value;
            else if (objInst is StringInstance s)
                return s.Value;
            else if (objInst is ClrInstanceTypeWrapper itw)
                return itw.WrappedType;
            else if (objInst is ClrInstanceWrapper iw)
                return iw.WrappedInstance;
            else if (objInst is ClrStaticTypeWrapper stw)
                return stw.WrappedType;
            else return null;
        }
    }
}
