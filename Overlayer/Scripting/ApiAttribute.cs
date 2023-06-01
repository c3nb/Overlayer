using System;

namespace Overlayer.Scripting
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ApiAttribute : Attribute
    {
        public ApiAttribute() { }
        public ApiAttribute(string name) => Name = name;
        public string Name { get; }
        public ScriptType SupportScript { get; set; } = ScriptType.All;
        public string[] Comment { get; set; }
        public Type[] RequireTypes { get; set; }
        public string[] RequireTypesAliases { get; set; }

        public string[] JSParamComment { get; set; }
        public string JSReturnComment { get; set; }

        public string GetRequireTypeAlias(int index) => (RequireTypesAliases != null ? index < RequireTypesAliases.Length ? RequireTypesAliases[index] : null : null) ?? RequireTypes[index].Name;
    }
}
