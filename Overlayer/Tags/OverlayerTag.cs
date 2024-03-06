using Overlayer.Core.TextReplacing;
using Overlayer.Tags.Attributes;
using Overlayer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Overlayer.Tags
{
    public class OverlayerTag
    {
        public static bool Initialized { get; private set; }
        public string Name { get; }
        public bool NotPlaying { get; }
        public bool Referenced => Tag.Referenced;
        public Tag Tag { get; }
        public TagAttribute Attributes { get; }
        public Type DeclaringType { get; }
        public OverlayerTag(MethodInfo method, TagAttribute attr, object target = null)
        {
            Tag = new Tag(Name = attr.Name ?? method.Name);
            Tag.SetGetter(WrapProcessor(method, target, attr.ProcessingFlags, attr.ProcessingFlagsArg), target);
            Attributes = attr;
            NotPlaying = attr.NotPlaying;
            DeclaringType = method.DeclaringType;
        }
        public OverlayerTag(FieldInfo field, TagAttribute attr, object target = null)
        {
            Tag = new Tag(Name = attr.Name ?? field.Name);
            Tag.SetGetter(WrapProcessor(field, target, attr.ProcessingFlags, attr.ProcessingFlagsArg));
            Attributes = attr;
            NotPlaying = attr.NotPlaying;
            DeclaringType = field.DeclaringType;
        }
        public OverlayerTag(PropertyInfo prop, TagAttribute attr, object target = null)
        {
            Tag = new Tag(Name = attr.Name ?? prop.Name);
            Tag.SetGetter(WrapProcessor(prop, target, attr.ProcessingFlags, attr.ProcessingFlagsArg));
            Attributes = attr;
            NotPlaying = attr.NotPlaying;
            DeclaringType = prop.DeclaringType;
        }
        public OverlayerTag(string name, Delegate del, bool notPlaying, ValueProcessing flags = ValueProcessing.None)
        {
            var attr = new TagAttribute(Name = name);
            attr.ProcessingFlags = flags;
            Tag = new Tag(name);
            Tag.SetGetter(del);
            Attributes = attr;
            NotPlaying = notPlaying;
            DeclaringType = del.Method.DeclaringType;
        }
        private static MethodInfo WrapProcessor(MemberInfo fieldPropMethod, object target, ValueProcessing flags, object flagsArg)
        {
            if (fieldPropMethod == null) throw new NullReferenceException(nameof(fieldPropMethod));
            if (fieldPropMethod is MethodInfo meth && flags == ValueProcessing.None) return meth;
            if (!IsValid(flags)) throw new InvalidOperationException($"Invalid FieldFlags! ({flags})");
            TypeBuilder t = mod.DefineType($"ValueProcessor_{fieldPropMethod?.Name}${uniqueNum++}", TypeAttributes.Public);
            MethodBuilder m = t.DefineMethod("Getter", MethodAttributes.Public | MethodAttributes.Static);
            FieldBuilder targetField = t.DefineField("target", target?.GetType() ?? typeof(object), FieldAttributes.Public | FieldAttributes.Static);
            ILGenerator il = m.GetILGenerator();
            Type rt = null;
            List<(Type, string, object)> parameters = new List<(Type, string, object)>();
            if (fieldPropMethod is FieldInfo field)
            {
                if (!field.IsStatic && target == null)
                    throw new InvalidOperationException($"Field '{field.Name}' Cannot Get Instance Member Without Target!!");
                if (!field.IsStatic && target != null)
                    il.Emit(OpCodes.Ldsfld, targetField);
                il.Emit(OpCodes.Ldsfld, field);
                rt = field.FieldType;
            }
            if (fieldPropMethod is PropertyInfo property)
            {
                MethodInfo getter = property.GetGetMethod();
                if (getter == null)
                    throw new InvalidOperationException($"Property '{property.Name}' Getter Is Not Exist Or Not Public!");
                fieldPropMethod = getter;
            }
            if (fieldPropMethod is MethodInfo method)
            {
                if (!method.IsStatic && target == null)
                    throw new InvalidOperationException($"Method '{method.Name}' Cannot Call Instance Member Without Target!!");
                if (!method.IsStatic && target != null)
                    il.Emit(OpCodes.Ldsfld, targetField);
                il.Emit(OpCodes.Call, method);
                rt = method.ReturnType;
            }
            if ((flags & ValueProcessing.AccessMember) != 0)
            {
                string accessor = flagsArg as string;
                if (string.IsNullOrWhiteSpace(accessor))
                {
                    il.Emit(OpCodes.Ldarg, parameters.Count);
                    il.Emit(OpCodes.Call, runtimeAccessor);
                    parameters.Add((typeof(string), "accessor", null));
                    goto Process;
                }
                string[] accessors = accessor.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < accessors.Length; i++)
                {
                    MemberInfo member = rt?.GetMember(accessors[i], MemberTypes.Field | MemberTypes.Property, BindingFlags.Public).FirstOrDefault();
                    rt = member is FieldInfo ff ? ff.FieldType : member is PropertyInfo pp ? pp.PropertyType : null;
                    if (rt == null) throw new InvalidOperationException($"Accessor '{accessors[i]}' Is Not Exist Or Not Public!");
                    if (member is FieldInfo f)
                    {
                        if (f.IsStatic)
                            throw new InvalidOperationException($"Accessor '{accessors[i]}' Must Not Be Static!");
                        il.Emit(OpCodes.Ldfld, f);
                    }
                    if (member is PropertyInfo p)
                    {
                        MethodInfo getter = p.GetGetMethod();
                        if (getter == null)
                            throw new InvalidOperationException($"Accessor '{accessors[i]}''s Getter Is Not Exist Or Not Public!");
                        if (getter.IsStatic)
                            throw new InvalidOperationException($"Accessor '{accessors[i]}''s Getter Must Not Be Static!");
                        il.Emit(OpCodes.Call, getter);
                    }
                }
            }
        Process:
            if ((flags & ValueProcessing.RoundNumber) != 0)
            {
                if (rt != typeof(double))
                    il.Emit(OpCodes.Conv_R8);
                il.Emit(OpCodes.Ldarg, parameters.Count);
                il.Emit(OpCodes.Call, round);
                if (rt != typeof(double))
                    il.Convert(rt);
                parameters.Add((typeof(int), "digits", -1));
            }
            else if ((flags & ValueProcessing.TrimString) != 0)
            {
                il.Emit(OpCodes.Ldarg, parameters.Count);
                il.Emit(OpCodes.Ldarg, parameters.Count + 1);
                il.Emit(OpCodes.Call, trim);
                parameters.Add((typeof(int), "maxLength", -1));
                parameters.Add((typeof(string), "afterTrimStr", Extensions.DefaultTrimStr));
            }
            il.Emit(OpCodes.Ret);
            m.SetParameters(parameters.Select(t => t.Item1).ToArray());
            int offset = 0;
            foreach (var (_, name, constant) in parameters)
            {
                var paramBuilder = m.DefineParameter(1 + offset++, ParameterAttributes.None, name);
                if (constant != null) paramBuilder.SetConstant(constant);
            }
            m.SetReturnType(rt);
            var createdType = t.CreateType();
            if (target != null) createdType.GetField("target").SetValue(null, target);
            return createdType.GetMethod("Getter", (BindingFlags)15420);
        }
        public static void Initialize()
        {
            if (Initialized) return;
            uniqueNum = 0;
            ass = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Overlayer.Tags.FieldTagWrappers"), AssemblyBuilderAccess.RunAndCollect);
            mod = ass.DefineDynamicModule("Overlayer.Tags.FieldTagWrappers");
            Initialized = true;
        }
        public static void Release()
        {
            if (!Initialized) return;
            ass = null;
            mod = null;
            Initialized = false;
        }
        private static bool IsValid(ValueProcessing flags)
        {
            if (flags.HasFlag(ValueProcessing.RoundNumber) &&
                flags.HasFlag(ValueProcessing.TrimString))
                return false;
            return true;
        }
        [Obsolete("Internal Only!!", true)]
        public static object RuntimeAccessor(object obj, string accessor)
        {
            object result = obj;
            string[] accessors = accessor.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (accessors.Length < 1) return obj;
            for (int i = 0; i < accessors.Length; i++)
            {
                var member = result?.GetType().GetMember(accessors[i], MemberTypes.Field | MemberTypes.Property, (BindingFlags)15420).FirstOrDefault();
                if (member is FieldInfo f && !f.IsStatic) result = f.GetValue(result);
                else if (member is PropertyInfo p && !p.GetGetMethod(true).IsStatic) result = p.GetValue(result);
                else result = null;
                if (result == null) return null;
            }
            return result;
        }
        private static int uniqueNum = 0;
        private static readonly MethodInfo round = typeof(Extensions).GetMethod("Round", new[] { typeof(double), typeof(int) });
        private static readonly MethodInfo trim = typeof(Extensions).GetMethod("Trim", new[] { typeof(string), typeof(int), typeof(string) });
        private static readonly MethodInfo runtimeAccessor = typeof(OverlayerTag).GetMethod("RuntimeAccessor", new[] { typeof(object), typeof(string) });
        private static AssemblyBuilder ass;
        private static ModuleBuilder mod;
    }
}
