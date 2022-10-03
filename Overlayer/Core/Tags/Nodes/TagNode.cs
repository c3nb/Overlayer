using Overlayer.Core.Utils;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Overlayer.Core.Nodes
{
    public class TagNode : Node
    {
        public override Type ResultType => isString() ? typeof(string) : typeof(float);
        public Tag tag;
        public int index;
        public string option;
        public Action<ILGenerator> getTags;
        public Func<bool> isString;
        public TagNode(Tag tag, string option, int index, Action<ILGenerator> getTagsArray, Func<bool> isString)
        {
            this.tag = tag;
            this.index = index;
            this.option = string.IsNullOrWhiteSpace(option) ? null : option;
            getTags = getTagsArray;
            this.isString = isString;
        }
        public override void Emit(ILGenerator il)
        {
            getTags(il);
            il.Emit(OpCodes.Ldc_I4, index);
            il.Emit(OpCodes.Ldelem_Ref);
            LocalBuilder loc = Push(tag);
            if (loc.LocalType == typeof(float))
            {
                if (isString())
                {
                    il.Emit(OpCodes.Ldloca, loc);
                    il.Emit(OpCodes.Call, float_ToString);
                }
                else il.Emit(OpCodes.Ldloc, loc);
            }
            else
            {
                il.Emit(OpCodes.Ldloc, loc);
                if (!isString())
                    il.Emit(OpCodes.Call, string_ToFloat);
            }
            LocalBuilder Push(Tag tag)
            {
                if (tag.IsOpt)
                {
                    if (tag.IsString)
                    {
                        LocalBuilder str = il.DeclareLocal(typeof(string));
                        if (tag.IsStringOpt)
                        {
                            var result = !string.IsNullOrEmpty(option) ? option : tag.DefOptStr == null ? "" : tag.DefOptStr;
                            il.Emit(OpCodes.Ldstr, result);
                            il.Emit(OpCodes.Callvirt, tag_optValue_String);
                            il.Emit(OpCodes.Stloc, str);
                        }
                        else
                        {
                            var result = !string.IsNullOrEmpty(option) ? option.ToFloat() : tag.DefOptNum == null ? 0 : tag.DefOptNum.Value;
                            il.Emit(OpCodes.Ldc_R4, result);
                            il.Emit(OpCodes.Callvirt, tag_optValue_Float);
                            il.Emit(OpCodes.Stloc, str);
                        }
                        return str;
                    }
                    else
                    {
                        LocalBuilder num = il.DeclareLocal(typeof(float));
                        if (tag.IsStringOpt)
                        {
                            var result = !string.IsNullOrEmpty(option) ? option : tag.DefOptStr == null ? "" : tag.DefOptStr;
                            il.Emit(OpCodes.Ldstr, result);
                            il.Emit(OpCodes.Callvirt, tag_optValueFloat_String);
                            il.Emit(OpCodes.Stloc, num);
                        }
                        else
                        {
                            var result = !string.IsNullOrEmpty(option) ? option.ToFloat() : tag.DefOptNum == null ? 0 : tag.DefOptNum.Value;
                            il.Emit(OpCodes.Ldc_R4, result);
                            il.Emit(OpCodes.Callvirt, tag_optValueFloat_Float);
                            il.Emit(OpCodes.Stloc, num);
                        }
                        return num;
                    }
                }
                else
                {
                    if (tag.IsString)
                    {
                        LocalBuilder str = il.DeclareLocal(typeof(string));
                        il.Emit(OpCodes.Callvirt, tag_string_Value);
                        il.Emit(OpCodes.Stloc, str);
                        return str;
                    }
                    else
                    {
                        LocalBuilder num = il.DeclareLocal(typeof(float));
                        il.Emit(OpCodes.Callvirt, tag_float_Value);
                        il.Emit(OpCodes.Stloc, num);
                        return num;
                    }
                }
            }
        }
        public static readonly MethodInfo string_ToFloat = typeof(StringUtils).GetMethod("ToFloat");
        public static readonly MethodInfo float_ToString = typeof(float).GetMethod("ToString", Type.EmptyTypes);

        public static MethodInfo tag_string_Value => TextCompiler.tag_string_Value;
        public static MethodInfo tag_float_Value => TextCompiler.tag_float_Value;
        public static MethodInfo tag_optValue_Float => TextCompiler.tag_optValue_Float;
        public static MethodInfo tag_optValueFloat_Float => TextCompiler.tag_optValueFloat_Float;
        public static MethodInfo tag_optValue_String => TextCompiler.tag_optValue_String;
        public static MethodInfo tag_optValueFloat_String => TextCompiler.tag_optValueFloat_String;
        public override string ToString() => tag.ToString();
    }
}
