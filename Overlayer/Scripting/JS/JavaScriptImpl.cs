using Overlayer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Overlayer.Scripting.JS
{
    public class JavaScriptImpl : Impl
    {
        public override ScriptType ScriptType => ScriptType.JavaScript;
        public override string Generate()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var tag in TagManager.All)
            {
                Type rt = tag.Getter.ReturnType;
                ParameterInfo[] tagOptions = tag.Getter.GetParameters();
                if (tag.HasOption)
                    sb.AppendLine(GetPRTypeHintComment(rt, "", (tagOptions[0].ParameterType, tagOptions[0].Name.IfNullOrEmpty("digits"))))
                        .AppendLine($"function {tag.Name}({tagOptions[0].Name.IfNullOrEmpty("digits")});");
                else sb.AppendLine(GetPRTypeHintComment(rt, ""))
                        .AppendLine($"function {tag.Name}();");
            }    
            foreach (var api in Api.GetApi(ScriptType))
            {
                Type rt = api.ReturnType;
                ParameterInfo[] options = api.GetParameters();
                if (options.Length > 0)
                {
                    var opt = options.Select(p => (p.ParameterType, p.Name)).ToArray();
                    sb.AppendLine(GetPRTypeHintComment(rt, "", opt));
                    var optStr = opt.Aggregate("", (c, n) => $"{c}{n.Name}, ");
                    sb.AppendLine($"function {api.Name}({optStr.Remove(optStr.Length - 2)});");
                }
                else
                {
                    sb.AppendLine(GetPRTypeHintComment(rt, ""));
                    sb.AppendLine($"function {api.Name}();");
                }
            }
            return sb.ToString();
        }
        private static string GetPRTypeHintComment(Type returnType, string indent, params (Type, string)[] parameters)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(indent + "/**");
            for (int i = 0; i < parameters.Length; i++)
            {
                var param = parameters[i];
                sb.AppendLine(indent + $" * @param {{{GetTypeHintCode(param.Item1)}}} {param.Item2}");
            }
            sb.AppendLine(indent + $" * @returns {{{GetTypeHintCode(returnType)}}}");
            sb.Append(indent + " */");
            return sb.ToString();
        }
        private static string GetTypeHintCode(Type type)
        {
            if (type == typeof(void))
                return "void";
            else if (type == typeof(Array))
                return "any[]";
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Empty:
                case TypeCode.DBNull:
                    return "null";
                case TypeCode.Boolean:
                    return "boolean";
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return "number";
                case TypeCode.DateTime:
                    return "Date";
                case TypeCode.Char:
                case TypeCode.String:
                    return "string";
                case TypeCode.Object:
                    if (type.Namespace != null)
                        return RemoveAfter((type.FullName?.Replace(type.Namespace + ".", "").Replace('+', '.') ?? type.Name), "`");
                    else return RemoveAfter(type.Name, "`");
                default:
                    return "undefined";
            }
        }
        private static string GetPTypeHintComment(Type type, string name) => $"/**@param {{{GetTypeHintCode(type)}}} {name}*/";
        private static string GetTypeHintComment(Type type) => $"/**@type {{{GetTypeHintCode(type)}}}*/";
        static string RemoveAfter(string str, string after)
        {
            int index = str.IndexOf(after);
            if (index < 0) return str;
            return str.Remove(index, str.Length - index);
        }
    }
}
