using JSON;
using Overlayer.Core.Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Overlayer.Utils
{
    public static class ModelUtils
    {
        public static readonly Type model_t = typeof(IModel);
        public static readonly Type vec2_t = typeof(Vector2);
        public static readonly Type vec3_t = typeof(Vector3);
        public static readonly Type vec4_t = typeof(Vector4);
        public static readonly Type col_t = typeof(Color);
        public static readonly Type col32_t = typeof(Color32);
        public static readonly Type quat_t = typeof(Quaternion);
        public static readonly Type rect_t = typeof(Rect);
        public static readonly Type ro_t = typeof(RectOffset);
        public static readonly Type m4x4_t = typeof(Matrix4x4);
        public static JsonNode ToNode<T>(object obj)
        {
            if (obj == null) return JsonNode.Null;
            Type t = typeof(T);
            switch (Type.GetTypeCode(t))
            {
                case TypeCode.Object:
                    if (obj is IModel model)
                        return model.Serialize();
                    else if (obj is Vector2 vec2)
                        return vec2;
                    else if (obj is Vector3 vec3)
                        return vec3;
                    else if (obj is Vector4 vec4)
                        return vec4;
                    else if (obj is Color col)
                        return col;
                    else if (obj is Color32 col32)
                        return col32;
                    else if (obj is Quaternion quat)
                        return quat;
                    else if (obj is Rect r)
                        return r;
                    else if (obj is RectOffset ro)
                        return ro;
                    else if (obj is Matrix4x4 m4x4)
                        return m4x4;
                    goto default;
                case TypeCode.Boolean: return (bool)obj;
                case TypeCode.Char: return (char)obj;
                case TypeCode.SByte: return (sbyte)obj;
                case TypeCode.Byte: return (byte)obj;
                case TypeCode.Int16: return (short)obj;
                case TypeCode.UInt16: return (ushort)obj;
                case TypeCode.Int32: return (int)obj;
                case TypeCode.UInt32: return (uint)obj;
                case TypeCode.Int64: return (long)obj;
                case TypeCode.UInt64: return (ulong)obj;
                case TypeCode.Single: return (float)obj;
                case TypeCode.Double: return (double)obj;
                case TypeCode.Decimal: return (decimal)obj;
                case TypeCode.DateTime: return (DateTime)obj;
                case TypeCode.String: return obj.ToString();
                default: return JsonNode.Serialize(obj);
            }
        }
        public static object ToObject<T>(JsonNode node)
        {
            if (node == null) return null;
            Type t = typeof(T);
            switch (Type.GetTypeCode(t))
            {
                case TypeCode.Object:
                    if (model_t.IsAssignableFrom(t))
                    {
                        IModel model = (IModel)Activator.CreateInstance(t);
                        model.Deserialize(node);
                        return model;
                    }
                    else if (t == vec2_t)
                        return (Vector2)node;
                    else if (t == vec3_t)
                        return (Vector3)node;
                    else if (t == vec4_t)
                        return (Vector4)node;
                    else if (t == col_t)
                        return (Color)node;
                    else if (t == col32_t)
                        return (Color32)node;
                    else if (t == quat_t)
                        return (Quaternion)node;
                    else if (t == rect_t)
                        return (Rect)node;
                    else if (t == ro_t)
                        return (RectOffset)node;
                    else if (t == m4x4_t)
                        return (Matrix4x4)node;
                    goto default;
                case TypeCode.Boolean: return (bool)node;
                case TypeCode.Char: return (char)node;
                case TypeCode.SByte: return (sbyte)node;
                case TypeCode.Byte: return (byte)node;
                case TypeCode.Int16: return (short)node;
                case TypeCode.UInt16: return (ushort)node;
                case TypeCode.Int32: return (int)node;
                case TypeCode.UInt32: return (uint)node;
                case TypeCode.Int64: return (long)node;
                case TypeCode.UInt64: return (ulong)node;
                case TypeCode.Single: return (float)node;
                case TypeCode.Double: return (double)node;
                case TypeCode.Decimal: return (decimal)node;
                case TypeCode.DateTime: return (DateTime)node;
                case TypeCode.String: return node.Value;
                default: return JsonNode.Deserialize(node, t);
            }
        }
        public static T Unbox<T>(JsonNode node) where T : IModel, new()
        {
            if (node == null) return default;
            T t = new T();
            t.Deserialize(node);
            return t;
        }

        public static JsonArray WrapList<T>(List<T> list) where T : IModel, new()
        {
            var array = new JsonArray();
            list.ForEach(i => array.Add(i.Serialize()));
            return array;
        }
        public static List<T> UnwrapList<T>(JsonArray array) where T : IModel, new()
        {
            List<T> list = new List<T>();
            foreach (var v in array.Values)
            {
                var t = new T();
                t.Deserialize(v);
                list.Add(t);
            }
            return list;
        }
    }
}
