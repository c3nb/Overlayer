using System;
using System.Text;
using System.IO;
using System.Linq;
using System.Reflection;

namespace JSEngine
{
    public static class JSUtils
    {
        /*
        public static void BuildProxies(string @namespace, string path = "")
        {
            var subNameSpaces = new HashSet<string>();
            var types = new List<string>();
            var filePath = @namespace.Replace('.', '/').Replace('+', '/');
            filePath = Path.Combine(path, filePath);
            var dir = Directory.CreateDirectory(filePath);

            foreach (var type in NamespaceProvider.GetTypesByPrefix(@namespace))
            {
                if (type.Namespace == @namespace)
                {
                    BuildProxy(type);
                    if (!type.IsNested)
                        types.Add(type.Name);
                }
                else
                {
                    if (type.Namespace.Length > @namespace.Length
                        && (type.Namespace[@namespace.Length] == '.' || type.Namespace[@namespace.Length] == '+'))
                    {
                        var separatorIndex = type.Namespace.IndexOf('.', @namespace.Length + 1);
                        if (separatorIndex == -1)
                            separatorIndex = type.Namespace.IndexOf('+', @namespace.Length + 1);
                        if (separatorIndex == -1)
                            separatorIndex = type.Namespace.Length;

                        subNameSpaces.Add(type.Namespace.Substring(0, separatorIndex));
                    }
                }
            }

            using (var nsFileStream = new FileStream(dir.FullName + ".js", FileMode.Create))
            using (var nsFile = new StreamWriter(nsFileStream))
            {
                for (var i = 0; i < types.Count; i++)
                {
                    nsFile.Write("import { ");
                    nsFile.Write(types[i].RemoveAfter("`"));
                    nsFile.Write(" } from \"");
                    nsFile.Write(@namespace.Substring(@namespace.LastIndexOf('.') + 1));
                    nsFile.Write("/");
                    nsFile.Write(types[i].RemoveAfter("`"));
                    nsFile.WriteLine("\";");
                }

                nsFile.WriteLine();

                nsFile.WriteLine("export {");
                for (var i = 0; i < types.Count; i++)
                {
                    if (i > 0)
                        nsFile.WriteLine(",");
                    nsFile.Write("  ");
                    nsFile.Write(types[i].RemoveAfter("`"));
                }
                nsFile.WriteLine();
                nsFile.WriteLine("}");
            }
        }
        */
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
                        return (type.FullName?.Replace(type.Namespace + ".", "").Replace('+', '.') ?? type.Name).RemoveAfter("`");
                    else return type.Name.RemoveAfter("`");
                default:
                    return "undefined";
            }
        }
        private static string GetPTypeHintComment(Type type, string name) => $"/**@param {{{GetTypeHintCode(type)}}} {name}*/";
        private static string GetTypeHintComment(Type type) => $"/**@type {{{GetTypeHintCode(type)}}}*/";
        static string RemoveAfter(this string str, string after)
        {
            int index = str.IndexOf(after);
            if (index < 0) return str;
            return str.Remove(index, str.Length - index);
        }
        static string TrimBetween(this string str, string start, string end)
        {
            int sIdx = str.IndexOf(start);
            int eIdx = str.LastIndexOf(end);
            if (sIdx < 0 || eIdx < 0) return str;
            return str.Remove(sIdx, eIdx - sIdx + 1);
        }
        public static string NormalizeTypeName(Type type, Type parent = null) => (type.FullName ?? (parent != null ? $"{parent.Namespace}.{type.Name}" : type.Name)).Replace('.', '/').Replace('+', '/').TrimBetween("[", "]");
        public static string BuildProxy(Type type, string path = "", bool curDir = false, bool buildNestedTypes = false)
            => BuildProxy_(type, path, curDir, buildNestedTypes);
        private static string BuildProxy_(Type type, string path = "", bool curDir = false, bool buildNestedTypes = false, Type parent = null)
        {
            var fileName = NormalizeTypeName(type, parent) + "_Proxy.js";
            if (curDir)
            {
                if (parent == null)
                    fileName = type.Name + "_Proxy.js";
                else fileName = $"{parent.Name}/{type.Name}_Proxy.js";
            }
            fileName = Path.Combine(path, fileName);
            Directory.GetParent(fileName).Create();
            var bindingFlags = BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic;
            using (var file = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            using (var writer = new StreamWriter(file, Encoding.UTF8))
            {
                var needNl = false;
                var lastImportedIndex = 0;
                var sb = new StringBuilder();
                sb.AppendLine($"// {type.FullName} Proxy");
                foreach (var nestedType in type.GetNestedTypes(bindingFlags))
                {
                    if (nestedType.Name.StartsWith("<"))
                        continue;
                    needNl = true;
                    if (buildNestedTypes)
                        BuildProxy_(nestedType, path, curDir, buildNestedTypes, type);
                    sb.Append("import { ");
                    sb.Append(nestedType.Name.RemoveAfter("`"));
                    sb.Append(" } from \"");
                    sb.Append($"./{type.Name}/{nestedType.Name}_Proxy.js");
                    sb.AppendLine("\";");
                }
                lastImportedIndex = sb.Length - 1;

                if (needNl)
                    sb.AppendLine();

                sb.Append("export class ");
                sb.Append(type.Name.RemoveAfter("`"));
                sb.AppendLine(" {");
                #region NestedTypes
                foreach (var nestedType in type.GetNestedTypes(bindingFlags))
                {
                    if (nestedType.Name.StartsWith("<"))
                        continue;
                    sb.Append("  static get ");
                    sb.Append(nestedType.Name.RemoveAfter("`"));
                    sb.Append("() {");
                    sb.Append(" return ");
                    sb.Append(nestedType.Name.RemoveAfter("`"));
                    sb.Append(";");
                    sb.AppendLine(" }");
                }
                #endregion
                #region Fields
                sb.AppendLine("  constructor() {");
                foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (field.Name.StartsWith("<"))
                        continue;
                    if (field.IsStatic) continue;
                    sb.AppendLine("    " + GetTypeHintComment(field.FieldType));
                    sb.AppendLine($"    this.{field.Name} = null;");
                }
                foreach (var field in type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    if (field.Name.StartsWith("<"))
                        continue;
                    if (field.IsStatic) continue;
                    sb.AppendLine("    " + GetTypeHintComment(field.FieldType));
                    sb.AppendLine($"    this.#{field.Name} = null;");
                }
                sb.AppendLine("  }");


                foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Static))
                {
                    if (field.Name.StartsWith("<"))
                        continue;
                    sb.AppendLine("  " + GetTypeHintComment(field.FieldType));
                    sb.AppendLine($"  static {field.Name};");
                }
                foreach (var field in type.GetFields(BindingFlags.NonPublic | BindingFlags.Static))
                {
                    if (field.Name.StartsWith("<"))
                        continue;
                    sb.AppendLine("  " + GetTypeHintComment(field.FieldType));
                    sb.AppendLine($"  static #{field.Name};");
                }
                #endregion
                #region Properties
                foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance).OrderBy(x => x.Name))
                {
                    if (prop.Name.StartsWith("<"))
                        continue;
                    var name = prop.Name.Split('.').Last();
                    var getter = prop.GetGetMethod(true);
                    var setter = prop.GetSetMethod(true);
                    if (getter != null)
                    {
                        sb.AppendLine("  " + GetTypeHintComment(prop.PropertyType));
                        if (getter.IsStatic)
                            sb.AppendLine($"  static get {name}() {{}}");
                        else sb.AppendLine($"  get {name}() {{}}");
                    }
                    if (setter != null)
                    {
                        sb.AppendLine("  " + GetPTypeHintComment(prop.PropertyType, "value"));
                        if (getter.IsStatic)
                            sb.AppendLine($"  static set {name}(value) {{}}");
                        else sb.AppendLine($"  set {name}(value) {{}}");
                    }
                }
                foreach (var prop in type.GetProperties(BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance).OrderBy(x => x.Name))
                {
                    if (prop.Name.StartsWith("<"))
                        continue;
                    var name = prop.Name.Split('.').Last();
                    var getter = prop.GetGetMethod(true);
                    var setter = prop.GetSetMethod(true);
                    if (getter != null)
                    {
                        sb.AppendLine("  " + GetTypeHintComment(prop.PropertyType));
                        if (getter.IsStatic)
                            sb.AppendLine($"  static get #{name}() {{}}");
                        else sb.AppendLine($"  get #{name}() {{}}");
                    }
                    if (setter != null)
                    {
                        sb.AppendLine("  " + GetPTypeHintComment(prop.PropertyType, "value"));
                        if (getter.IsStatic)
                            sb.AppendLine($"  static set #{name}(value) {{}}");
                        else sb.AppendLine($"  set #{name}(value) {{}}");
                    }
                }
                #endregion
                #region Methods
                foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance).OrderBy(x => x.Name))
                {
                    if (method.Name.StartsWith("<"))
                        continue;
                    if (method.IsSpecialName && !method.Name.StartsWith("add_") && !method.Name.StartsWith("remove_"))
                        continue;
                    var prms = method.GetParameters();
                    var tuples = prms.Select(p => (p.ParameterType, p.Name));
                    sb.AppendLine(GetPRTypeHintComment(method.ReturnType, "  ", tuples.ToArray()));
                    var prmString = prms.Aggregate("", (c, n) => $"{c}{n.Name}, ");
                    if (prmString.Length > 2)
                        prmString = prmString.Remove(prmString.Length - 2);
                    var name = method.Name.Split('.').Last();
                    if (method.IsStatic)
                        sb.AppendLine($"  static {name}({prmString});");
                    else sb.AppendLine($"  {name}({prmString});");
                }
                foreach (var method in type.GetMethods(BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance).OrderBy(x => x.Name))
                {
                    if (method.Name.StartsWith("<"))
                        continue;
                    if (method.IsSpecialName && !method.Name.StartsWith("add_") && !method.Name.StartsWith("remove_"))
                        continue;
                    var prms = method.GetParameters();
                    var tuples = prms.Select(p => (p.ParameterType, p.Name));
                    sb.AppendLine(GetPRTypeHintComment(method.ReturnType, "  ", tuples.ToArray()));
                    var prmString = prms.Aggregate("", (c, n) => $"{c}{n.Name}, ");
                    if (prmString.Length > 2)
                        prmString = prmString.Remove(prmString.Length - 2);
                    var name = method.Name.Split('.').Last();
                    if (method.IsStatic)
                        sb.AppendLine($"  static #{name}({prmString});");
                    else sb.AppendLine($"  #{name}({prmString});");
                }
                #endregion
                sb.Append("}");
                writer.Write(sb.ToString());
                writer.BaseStream.SetLength(writer.BaseStream.Position);
            }
            return fileName;
        }
    }
}
