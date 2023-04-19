using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using Overlayer.Core;
using Jint;
using Jint.Native.Function;
using Jint.Native;
using System.IO;

namespace Overlayer.Scripting.JS
{
    public static class JSUtils
    {
        static Dictionary<string, Delegate> apis;
        public static Engine Prepare()
        {
            var engine = new Engine(op => 
                op.AllowClr(Utility.loadedAsss)
                    .Strict(false)
            );
            foreach (var tag in TagManager.All)
                engine.SetValue(tag.Name, tag.GetterDelegate);
            if (apis == null)
            {
                apis = new Dictionary<string, Delegate>();
                foreach (var api in Api.GetApiMethods(ScriptType.JavaScript))
                    apis.Add(api.Name, api.CreateDelegateAuto());
            }
            foreach (var api in apis)
                engine.SetValue(api.Key, api.Value);
            return engine;
        }
        public static Result Compile(string path) => new Result(Prepare(), File.ReadAllText(path));
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
        public static MethodInfo Wrap(this FunctionInstance func, MethodBase target, bool rtIsBool)
        {
            if (func == null) return null;
            FIWrapper holder = new FIWrapper(func);

            TypeBuilder type = Core.Utility.mod.DefineType(Core.Utility.TypeCount++.ToString(), TypeAttributes.Public);
            ParameterInfo[] parameters = SelectActualParams(target, target.GetParameters(), func.FunctionDeclaration.Params.Select(n => n.AssociatedData.ToString()).ToArray());
            if (parameters == null) return null;
            Type[] paramTypes = parameters.Select(p => p.ParameterType).ToArray();
            MethodBuilder methodB = type.DefineMethod("Wrapper", MethodAttributes.Public | MethodAttributes.Static, rtIsBool ? typeof(bool) : typeof(void), paramTypes);
            FieldBuilder holderfld = type.DefineField("holder", typeof(FIWrapper), FieldAttributes.Public | FieldAttributes.Static);

            var il = methodB.GetILGenerator();
            LocalBuilder arr = il.MakeArray<object>(parameters.Length);
            int paramIndex = 1;
            foreach (ParameterInfo param in parameters)
            {
                Type pType = param.ParameterType;
                Core.Utility.IgnoreAccessCheck(pType);
                methodB.DefineParameter(paramIndex++, ParameterAttributes.None, param.Name);
                int pIndex = paramIndex - 2;
                il.Emit(OpCodes.Ldloc, arr);
                il.Emit(OpCodes.Ldc_I4, pIndex);
                il.Emit(OpCodes.Ldarg, pIndex);
                il.Emit(OpCodes.Stelem_Ref);
            }
            il.Emit(OpCodes.Ldsfld, holderfld);
            il.Emit(OpCodes.Ldloc, arr);
            il.Emit(OpCodes.Call, FIWrapper.CallMethod);
            if (rtIsBool)
                il.Emit(OpCodes.Call, istrue);
            else il.Emit(OpCodes.Pop);
            il.Emit(OpCodes.Ret);

            Type t = type.CreateType();
            t.GetField("holder").SetValue(null, holder);
            return t.GetMethod("Wrapper");
        }
        public static bool IsTrue(object obj)
            => obj == null || obj.Equals(true);
        static readonly MethodInfo istrue = typeof(JSUtils).GetMethod("IsTrue", AccessTools.all);
        class CustomParameter : ParameterInfo
        {
            public CustomParameter(Type type, string name)
            {
                ClassImpl = type;
                NameImpl = name;
            }
        }
    }
    public class FIWrapper
    {
        public readonly Engine engine;
        public readonly FunctionInstance fi;
        public FIWrapper(FunctionInstance fi)
        {
            this.fi = fi;
            engine = fi.Engine;
        }
        public object Call(params object[] args) => fi.Call(null, Array.ConvertAll(args, o => JsValue.FromObject(engine, o))).ToObject();
        public static readonly MethodInfo CallMethod = typeof(FIWrapper).GetMethod("Call");
    }
}
