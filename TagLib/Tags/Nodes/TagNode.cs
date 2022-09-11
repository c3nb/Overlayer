using System;
using System.Reflection;
using System.Reflection.Emit;

namespace TagLib.Tags.Nodes
{
    public class TagNode : Node
    {
        public Tag tag;
        public int index;
        public string option;
        public Action<ILGenerator> getTags;
        public TagNode(Tag tag, string option, int index, Action<ILGenerator> getTagsArray)
        {
            this.tag = tag;
            this.index = index;
            this.option = string.IsNullOrWhiteSpace(option) ? null : option;
            getTags = getTagsArray;
        }
        public override void Emit(ILGenerator il)
        {
            getTags(il);
            il.Emit(OpCodes.Ldc_I4, index);
            il.Emit(OpCodes.Ldelem_Ref);
            if (tag.IsOpt)
            {
                if (option != null)
                {
                    if (tag.IsStringOpt)
                    {
                        il.Emit(OpCodes.Ldstr, option);
                        il.Emit(OpCodes.Callvirt, tag_optValueFloat_String);
                    }
                    else
                    {
                        float.TryParse(option, out float result);
                        il.Emit(OpCodes.Ldc_R4, result);
                        il.Emit(OpCodes.Callvirt, tag_optValueFloat_Float);
                    }
                }
                else
                {
                    if (tag.IsStringOpt)
                    {
                        if (tag.DefOptStr != null)
                            il.Emit(OpCodes.Ldstr, tag.DefOptStr);
                        else il.Emit(OpCodes.Ldnull);
                        il.Emit(OpCodes.Callvirt, tag_optValueFloat_String);
                    }
                    else
                    {
                        if (tag.DefOptNum.HasValue)
                            il.Emit(OpCodes.Ldc_R4, tag.DefOptNum.Value);
                        else il.Emit(OpCodes.Ldc_R4, 0);
                        il.Emit(OpCodes.Callvirt, tag_optValueFloat_Float);
                    }
                }
            }
            else il.Emit(OpCodes.Callvirt, tag_get_ValueFloat);
        }
        public static readonly MethodInfo tag_get_ValueFloat = typeof(Tag).GetMethod("get_ValueFloat");
        public static readonly MethodInfo tag_optValueFloat_Float = typeof(Tag).GetMethod("OptValueFloat", new[] { typeof(float) });
        public static readonly MethodInfo tag_optValueFloat_String = typeof(Tag).GetMethod("OptValueFloat", new[] { typeof(string) });
        public override string ToString() => tag.ToString();
    }
}
