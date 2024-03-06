using Overlayer.Utils;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Overlayer.Core.TextReplacing
{
    public class Tag
    {
        public string Name { get; }
        public bool Referenced => ReferencedCount > 0;
        private int referencedCount;
        public int ReferencedCount
        {
            get => referencedCount;
            set
            {
                if (value < 0)
                    throw new InvalidOperationException("ReferencedCount Cannot Be Less Than 0!! Reference May Be Broken!");
                referencedCount = value;
            }
        }
        public bool HasArgument => ArgumentCount > 0;
        public int ArgumentCount { get; private set; }
        /// <summary>
        /// Original Getter's Return Type
        /// </summary>
        public Type ReturnType { get; private set; }
        /// <summary>
        /// String Getter
        /// </summary>
        public MethodInfo Getter { get; private set; }
        /// <summary>
        /// String Getter Delegate
        /// </summary>
        public Delegate GetterDelegate { get; private set; }
        /// <summary>
        /// Original Getter's Target (Instance)
        /// </summary>
        public object GetterOriginalTarget { get; private set; }
        /// <summary>
        /// Original Getter's Target (Instance) Holding Field
        /// </summary>
        public FieldInfo GetterOriginalTargetField { get; private set; }
        /// <summary>
        /// Original Getter
        /// </summary>
        public MethodInfo GetterOriginal { get; private set; }
        /// <summary>
        /// Original Getter's Direct Wrapper
        /// </summary>
        public MethodInfo GetterOriginalDirect { get; private set; }
        /// <summary>
        /// Original Getter Delegate
        /// </summary>
        public Delegate GetterOriginalDelegate { get; private set; }
        /// <summary>
        /// Original Getter => String Converter
        /// </summary>
        public MethodInfo ReturnConverter { get; private set; }
        /// <summary>
        /// Argument => Original Getter Converter
        /// </summary>
        public MethodInfo[] ArgumentConverter { get; private set; }
        public Tag(string name)
        {
            Name = name;
        }
        public Tag SetGetter(MethodInfo method, object target = null)
        {
            if (Getter != null) return null;
            if (method == null) throw new ArgumentNullException(nameof(method));
            if (method.ReturnType != typeof(string))
            {
                var rtc = StringConverter.GetFromConverter(method.ReturnType);
                if (rtc == null) throw new NotSupportedException($"{method.ReturnType} Is Not Supported Return Type!");
                ReturnConverter = rtc;
            }
            GetterOriginalTarget = target;
            var parameters = method.GetParameters();
            var argConverter = new MethodInfo[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                var param = parameters[i];
                if (param.ParameterType == typeof(string)) continue;
                if ((argConverter[i] = StringConverter.GetToConverter(param.ParameterType)) == null)
                    throw new NotSupportedException($"{param.ParameterType} Is Not Supported Parameter Type!");
            }
            ArgumentConverter = argConverter;
            GetterOriginal = method;
            GetterOriginalDelegate = method.CreateDelegate(Expression.GetFuncType(parameters.Select(p => p.ParameterType).Append(method.ReturnType).ToArray()));
            ReturnType = method.ReturnType;
            Getter = WrapMethodToGetter(out Delegate gDel);
            GetterDelegate = gDel;
            GetterOriginalDirect = method;
            ArgumentCount = parameters.Length;
            return this;
        }
        public Tag SetGetter(Delegate del)
        {
            if (Getter != null) return null;
            var method = del.Method;
            if (method == null) throw new ArgumentNullException(nameof(method));
            if (method.ReturnType != typeof(string))
            {
                var rtc = StringConverter.GetFromConverter(method.ReturnType);
                if (rtc == null) throw new NotSupportedException($"{method.ReturnType} Is Not Supported Return Type!");
                ReturnConverter = rtc;
            }
            GetterOriginalTarget = del;
            var parameters = method.GetParameters();
            var argConverter = new MethodInfo[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                var param = parameters[i];
                if (param.ParameterType == typeof(string)) continue;
                if ((argConverter[i] = StringConverter.GetToConverter(param.ParameterType)) == null)
                    throw new NotSupportedException($"{param.ParameterType} Is Not Supported Parameter Type!");
            }
            ArgumentConverter = argConverter;
            GetterOriginal = del.GetType().GetMethod("Invoke");
            GetterOriginalDelegate = del;
            ReturnType = GetterOriginal.ReturnType;
            Getter = WrapMethodToGetter(out Delegate gDel);
            GetterDelegate = gDel;
            GetterOriginalDirect = WrapDelegateDirect(Name, del);
            ArgumentCount = parameters.Length;
            return this;
        }
        static AssemblyBuilder TagWrapperAssembly;
        static ModuleBuilder TagWrapperModule;
        static int uniqueId = 0;
        static bool wrapperInitialized = false;
        MethodInfo WrapMethodToGetter(out Delegate del)
        {
            del = GetterDelegate;
            if (Getter != null) return Getter;
            string methodName, targetFieldName;
            TypeBuilder t = TagWrapperModule.DefineType($"{Name}_WrapperType${uniqueId++}", TypeAttributes.Public);
            ParameterInfo[] parameters = GetterOriginal.GetParameters();
            Type[] parameterTypes = Enumerable.Repeat(typeof(string), parameters.Length).ToArray();
            MethodBuilder m = t.DefineMethod(methodName = $"{Name}_WrapperMethod", MethodAttributes.Public | MethodAttributes.Static, typeof(string), parameterTypes);
            FieldBuilder targetField = t.DefineField(targetFieldName = "Target", GetterOriginal.DeclaringType, FieldAttributes.Public | FieldAttributes.Static);
            ILGenerator il = m.GetILGenerator();
            if (!GetterOriginal.IsStatic)
                il.Emit(OpCodes.Ldsfld, targetField);
            for (int i = 0; i < ArgumentConverter.Length; i++)
            {
                var converter = ArgumentConverter[i];
                m.DefineParameter(i + 1, ParameterAttributes.None, parameters[i].Name);
                il.Emit(OpCodes.Ldarg, i);
                if (converter == null) continue;
                il.Emit(OpCodes.Call, converter);
            }
            il.Emit(OpCodes.Call, GetterOriginal);
            if (ReturnConverter != null)
                il.Emit(OpCodes.Call, ReturnConverter);
            il.Emit(OpCodes.Ret);
            var resultT = t.CreateType();
            (GetterOriginalTargetField = resultT.GetField(targetFieldName)).SetValue(null, GetterOriginalTarget);
            var result = resultT.GetMethod(methodName);
            del = result.CreateDelegate(Expression.GetFuncType(parameterTypes.Append(typeof(string)).ToArray()));
            return result;
        }
        static MethodInfo WrapDelegateDirect(string name, Delegate del)
        {
            Type delType = del.GetType();
            MethodInfo invoke = delType.GetMethod("Invoke");
            MethodInfo method = del.Method;
            TypeBuilder type = TagWrapperModule.DefineType($"{name}_Delegate_WrapperType${uniqueId++}", TypeAttributes.Public);
            ParameterInfo[] parameters = method.GetParameters();
            Type[] paramTypes = parameters.Select(p => p.ParameterType).ToArray();
            MethodBuilder methodB = type.DefineMethod($"{name}_Delegate_WrapperMethod", MethodAttributes.Public | MethodAttributes.Static, invoke.ReturnType, paramTypes);
            FieldBuilder delField = type.DefineField("del", delType, FieldAttributes.Public | FieldAttributes.Static);
            ILGenerator il = methodB.GetILGenerator();
            il.Emit(OpCodes.Ldsfld, delField);
            int paramIndex = 1;
            foreach (ParameterInfo param in parameters)
            {
                methodB.DefineParameter(paramIndex++, ParameterAttributes.None, param.Name);
                il.Emit(OpCodes.Ldarg, paramIndex - 2);
            }
            il.Emit(OpCodes.Call, invoke);
            il.Emit(OpCodes.Ret);
            Type t = type.CreateType();
            t.GetField("del").SetValue(null, del);
            return t.GetMethod($"{name}_Delegate_WrapperMethod");
        }
        public static void InitializeWrapperAssembly()
        {
            if (wrapperInitialized) return;
            uniqueId = 0;
            wrapperInitialized = true;
            TagWrapperAssembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Overlayer.TagWrapper"), AssemblyBuilderAccess.RunAndCollect);
            TagWrapperModule = TagWrapperAssembly.DefineDynamicModule("Overlayer.TagWrapper");
            Main.Logger.Log("Initialized Tag Wrapper Assembly.");
        }
        public static void ReleaseWrapperAssembly()
        {
            TagWrapperAssembly = null;
            TagWrapperModule = null;
            wrapperInitialized = false;
        }
    }
}
