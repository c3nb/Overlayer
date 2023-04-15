using Overlayer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Unity.Services.Analytics;

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
                    sb.AppendLine($"def {tag.Name}({GetArgStr(tagOptions)}): return Overlayer_Internal.{tag.Name}({GetCallArgStr(tagOptions)})");
                else
                    sb.AppendLine($"def {tag.Name}(): return Overlayer_Internal.{tag.Name}()");
            }
            foreach (var api in Api.GetApi(ScriptType))
            {
                ParameterInfo[] options = api.GetParameters();
                if (options.Length > 0)
                    sb.AppendLine($"def {api.Name}({GetArgStr(options)}): {(api.ReturnType != typeof(void) ? "return " : "")}Overlayer_Internal.{api.Name}({GetCallArgStr(options)})");
                else
                    sb.AppendLine($"def {api.Name}(): {(api.ReturnType != typeof(void) ? "return " : "")}Overlayer_Internal.{api.Name}()");
            }
            return sb.ToString();
        }
        static string GetArgStr(ParameterInfo[] args)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var arg in args)
                sb.Append($"{arg.Name}:{GetTypeStr(arg.ParameterType)}, ");
            var result = sb.ToString();
            return result.Remove(result.Length - 2);
        }
        static string GetCallArgStr(ParameterInfo[] args)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var arg in args)
                sb.Append($"{arg.Name}, ");
            var result = sb.ToString();
            return result.Remove(result.Length - 2);
        }
        static string GetTypeStr(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Single:
                case TypeCode.Double:
                    return "float";
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.Int16: 
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return "int";
                case TypeCode.String:
                    return "str";
                case TypeCode.Boolean:
                    return "bool";
                default:
                    return "object";
            }
        }
    }
}
