using IronPython.Runtime;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Overlayer.Core;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using System;
using Py = IronPython.Hosting.Python;
using System.IO;
using IronPython.Runtime.Types;
using Overlayer.Scripting.JS;
using System.Reflection;

namespace Overlayer.Scripting.Python
{
    public static class PythonUtils
    {
        public static readonly Dictionary<string, object> options = new Dictionary<string, object>();
        static bool apiInitialized = false;
        public static void Prepare()
        {
            if (!apiInitialized)
            {
                var delegates = Api.GetApiMethods(ScriptType.Python).Select(m => (m.Name, m.CreateDelegate(m.ReturnType != typeof(void) ? Expression.GetFuncType(m.GetParameters().Select(p => p.ParameterType).Append(m.ReturnType).ToArray()) : Expression.GetActionType(m.GetParameters().Select(p => p.ParameterType).ToArray())))).Concat(TagManager.All.Select(t => (t.Name, t.GetterDelegate)));
                foreach (var (name, del) in delegates)
                    options.Add(name, del);
                var types = Api.GetApiTypesWithAttr(ScriptType.Python);
                foreach (var (attr, t) in types)
                    options.Add(attr.Name ?? t.Name, DynamicHelpers.GetPythonTypeFromType(t));
                apiInitialized = true;
            }
        }
        public static Result Compile(string path)
        {
            Prepare();
            var engine = CreateEngine(path, out var source);
            var scope = engine.CreateScope();
            var scr = source.Compile();
            return new Result(scr, scope);
        }
        public static Result CompileSource(string source)
        {
            Prepare();
            var engine = CreateEngineFromSource(source, out var scrSource);
            var scope = engine.CreateScope();
            var scr = scrSource.Compile();
            return new Result(scr, scope);
        }
        public static string[] modulePaths = new string[] { Main.ScriptPath };
        public static ScriptEngine CreateEngine(string path, out ScriptSource source)
        {
            var engine = Py.CreateEngine();
            if (!File.Exists(path))
                path = Path.Combine(Main.ScriptPath, path);
            if (!File.Exists(path))
            {
                source = null;
                return null;
            }
            source = engine.CreateScriptSourceFromFile(path, Encoding.UTF8, SourceCodeKind.AutoDetect);
            ScriptScope scope = Py.GetBuiltinModule(engine);
            scope.SetVariable("__import__", new ImportDelegate(ResolveImport));
            engine.SetSearchPaths(modulePaths);
            return engine;
        }
        public static ScriptEngine CreateEngineFromSource(string src, out ScriptSource source)
        {
            var engine = Py.CreateEngine();
            source = engine.CreateScriptSourceFromString(src, SourceCodeKind.AutoDetect);
            ScriptScope scope = Py.GetBuiltinModule(engine);
            scope.SetVariable("__import__", new ImportDelegate(ResolveImport));
            engine.SetSearchPaths(modulePaths);
            return engine;
        }
        private static object ResolveImport(CodeContext context, string moduleName, PythonDictionary globals, PythonDictionary locals, PythonTuple fromlist, int level)
        {
            var builtin = IronPython.Modules.Builtin.__import__(context, moduleName, globals, locals, fromlist, level);
            var module = builtin as PythonModule;
            foreach (var kvp in options)
                module?.__setattr__(context, kvp.Key, kvp.Value);
            return builtin;
        }
        public static void WriteType(Type type, StringBuilder sb, string alias = null)
        {
            sb.Append("class ");
            var tName = (alias ?? type.Name).RemoveAfter("`");
            sb.AppendLine($"{tName}():");
            #region Fields And Properties
            bool any = false;
            sb.AppendLine("  def __init__(self):");
            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                if (field.Name.StartsWith("<"))
                    continue;
                if (field.IsStatic) continue;
                sb.AppendLine($"    self.{field.Name}:{PythonImpl.GetTypeStr(field.FieldType, true, type)} = None");
                any = true;
            }
            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (prop.Name.StartsWith("<"))
                    continue;
                var name = prop.Name.Split('.').Last();
                sb.AppendLine($"    self.{prop.Name}:{PythonImpl.GetTypeStr(prop.PropertyType, true, type)} = None");
                any = true;
            }

            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (field.Name.StartsWith("<"))
                    continue;
                sb.AppendLine($"    {field.Name}:{PythonImpl.GetTypeStr(field.FieldType, true, type)} = None");
                any = true;
            }
            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Static))
            {
                if (prop.Name.StartsWith("<"))
                    continue;
                var name = prop.Name.Split('.').Last();
                sb.AppendLine($"    {prop.Name}:{PythonImpl.GetTypeStr(prop.PropertyType, true, type)} = None");
                any = true;
            }
            if (!any) sb.AppendLine("    pass");
            #endregion
            #region Methods
            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance).OrderBy(x => x.Name))
            {
                if (method.IsObjectDeclared()) continue;
                if (method.Name.StartsWith("<"))
                    continue;
                if (method.IsSpecialName && !method.Name.StartsWith("add_") && !method.Name.StartsWith("remove_"))
                    continue;
                var prms = method.GetParameters();
                var isStatic = method.IsStatic;
                if (isStatic)
                    sb.AppendLine("  @staticmethod");
                var accessor = isStatic ? tName : "self";
                if (prms.Length > 0)
                    sb.AppendLine($"  def {method.Name}({PythonImpl.GetArgStr(prms, isStatic, type)}) -> {PythonImpl.GetTypeStr(method.ReturnType, self: type)}: {(method.ReturnType != typeof(void) ? "return " : "")}{accessor}.{method.Name}({PythonImpl.GetCallArgStr(prms)})");
                else
                    sb.AppendLine($"  def {method.Name}({PythonImpl.GetArgStr(prms, isStatic, type)}) -> {PythonImpl.GetTypeStr(method.ReturnType, self: type)}: {(method.ReturnType != typeof(void) ? "return " : "")}{accessor}.{method.Name}()");
            }
            #endregion
        }
    }
    public delegate object ImportDelegate(CodeContext context, string moduleName, PythonDictionary globals, PythonDictionary locals, PythonTuple fromlist, int level);
}