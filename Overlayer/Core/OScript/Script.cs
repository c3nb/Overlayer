using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Overlayer.Core.OScript
{
    public class Script
    {
        public readonly Dictionary<string, Type> TypeBindings;
        public readonly Dictionary<string, MethodInfo> MethodBindings;
        public readonly Dictionary<string, PropertyInfo> PropertyBindings;
        public readonly Dictionary<string, FieldInfo> FieldBindings;
        public void Bind(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
                Bind(type);
        }
        public void Bind(Type type)
        {
            foreach (MethodInfo method in type.GetMethods((BindingFlags)15420))
            {
                var mBinding = method.GetCustomAttribute<BindAttribute>();
                if (mBinding == null) continue;
                string mName = mBinding.Name ?? method.Name;
                MethodBindings.Add(mName, method);
            }
            foreach (PropertyInfo property in type.GetProperties((BindingFlags)15420))
            {
                MethodInfo getter = property.GetGetMethod(true);
                MethodInfo setter = property.GetSetMethod(true);
                if (getter != null)
                {
                    var mBinding = getter.GetCustomAttribute<BindAttribute>();
                    if (mBinding == null) continue;
                    string mName = mBinding.Name ?? getter.Name;
                    MethodBindings.Add(mName, getter);
                }
                if (setter != null)
                {
                    var mBinding = setter.GetCustomAttribute<BindAttribute>();
                    if (mBinding == null) continue;
                    string mName = mBinding.Name ?? setter.Name;
                    MethodBindings.Add(mName, setter);
                }
                var pBinding = property.GetCustomAttribute<BindAttribute>();
                if (pBinding == null) continue;
                string pName = pBinding.Name ?? property.Name;
                PropertyBindings.Add(pName, property);
            }
            foreach (FieldInfo field in type.GetFields((BindingFlags)15420))
            {
                var fBinding = field.GetCustomAttribute<BindAttribute>();
                if (fBinding == null) continue;
                string fName = fBinding.Name ?? field.Name;
                FieldBindings.Add(fName, field);
            }
            var binding = type.GetCustomAttribute<BindAttribute>();
            if (binding == null) return;
            string name = binding.Name ?? type.Name;
            TypeBindings.Add(name, type);
        }
        public Script()
        {
            TypeBindings = new Dictionary<string, Type>();
            MethodBindings = new Dictionary<string, MethodInfo>();
            PropertyBindings = new Dictionary<string, PropertyInfo>();
            FieldBindings = new Dictionary<string, FieldInfo>();
        }
        public Script(Script script)
        {
            TypeBindings = script.TypeBindings;
            MethodBindings = script.MethodBindings;
            PropertyBindings = script.PropertyBindings;
            FieldBindings = script.FieldBindings;
        }

    }
}
