using Overlayer.Core;
using Overlayer.Core.Tags;
using Overlayer.Core.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Overlayer.Scripting.JS;

namespace Overlayer.Scripting.Python
{
    public class PythonImpl : Impl
    {
        public override ScriptType ScriptType => ScriptType.Python;
        public override string Generate()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var tag in TagManager.All)
            {
                ParameterInfo[] tagOptions = tag.Getter.GetParameters();
                if (tagOptions.Length > 0)
                    sb.AppendLine($"def {tag.Name}({GetArgStr(tagOptions)}) -> {GetTypeStr(tag.Getter.ReturnType)}: return {tag.Name}({GetCallArgStr(tagOptions)})");
                else
                    sb.AppendLine($"def {tag.Name}() -> {GetTypeStr(tag.Getter.ReturnType)}: return {tag.Name}()");
            }
            foreach (var (attr, t) in Api.GetApiTypesWithAttr(ScriptType))
                PythonUtils.WriteType(t, sb, attr.Name);
            foreach (var (attr, api) in Api.GetApiMethodsWithAttr(ScriptType))
                AppendFunction4(sb, attr, api);
            return sb.ToString();
        }
        public static void AppendFunction4(StringBuilder sb, ApiAttribute attr, MethodInfo api)
        {
            ParameterInfo[] options = api.GetParameters();
            if (options.Length > 0)
                sb.AppendLine($"def {api.Name}({GetArgStr(options)}) -> {GetTypeStr(api.ReturnType)}:")
                    .Append(attr.Comment != null ? $"    \"\"\"\n    {string.Join("\\n\n    ", attr.Comment)}\n    \"\"\"\n" : "")
                    .AppendLine($"    {(api.ReturnType != typeof(void) ? "return " : "")}{api.Name}({GetCallArgStr(options)})");
            else
                sb.AppendLine($"def {api.Name}() -> {GetTypeStr(api.ReturnType)}:")
                    .Append(attr.Comment != null ? $"    \"\"\"\n    {string.Join("\\n\n    ", attr.Comment)}\n    \"\"\"\n" : "")
                    .AppendLine($"    {(api.ReturnType != typeof(void) ? "return " : "")}{api.Name}()");
        }
        public static string GetArgStr(ParameterInfo[] args, bool staticFunc = true, Type self = null)
        {
            if (!args.Any()) return staticFunc ? "" : "self";
            StringBuilder sb = new StringBuilder();
            if (!staticFunc)
                sb.Append("self, ");
            foreach (var arg in args)
                sb.Append($"{ProcessKwName(arg.Name).IfNullOrEmpty("digits")}:{GetTypeStr(arg.ParameterType, self: self)}, ");
            var result = sb.ToString();
            return result.Remove(result.Length - 2);
        }
        public static string GetCallArgStr(ParameterInfo[] args)
        {
            if (!args.Any()) return "";
            StringBuilder sb = new StringBuilder();
            foreach (var arg in args)
                sb.Append($"{ProcessKwName(arg.Name).IfNullOrEmpty("digits")}, ");
            var result = sb.ToString();
            return result.Remove(result.Length - 2);
        }
        public static string GetTypeStr(Type type, bool defaultIsOriginal = false, Type self = null)
        {
            if (type == null || type == typeof(void)) return "None";
            else if (type.IsArray) return "list";
            else if (typeof(IDictionary).IsAssignableFrom(type)) return "dict";
            string result;
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Single:
                case TypeCode.Double:
                    result = "float";
                    break;
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    result = "int";
                    break;
                case TypeCode.String:
                    result = "str";
                    break;
                case TypeCode.Boolean:
                    result = "bool";
                    break;
                default:
                    var toSearch = type.IsArray ? type.GetElementType() : type;
                    result = Api.Get(toSearch)?.Name ?? "object";
                    if (defaultIsOriginal) result = result.RemoveAfter("`");
                    break;
            }
            if (self == type)
                return $"\"{result}\"";
            return result;
        }
        static string ProcessKwName(string str)
        {
            switch (str)
            {
                case "from":
                    return "_" + str;
                default: return str;
            }
        }
    }
}
