using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Overlayer.Core.Utils
{
    public static class EmitUtils
    {
        static readonly AssemblyBuilder ass;
        static readonly ModuleBuilder mod;
        static EmitUtils()
        {
            var assName = new AssemblyName("Overlayer.Core.Utils.RuntimeAssembly");
            ass = AssemblyBuilder.DefineDynamicAssembly(assName, AssemblyBuilderAccess.Run);
            mod = ass.DefineDynamicModule(assName.Name);
        }
        public static AssemblyBuilder Asm => ass;
        public static ModuleBuilder Mod => mod;
        public static void Convert(this ILGenerator il, Type to)
        {
            switch (Type.GetTypeCode(to))
            {
                case TypeCode.Object:
                    il.Emit(OpCodes.Box);
                    break;
                case TypeCode.Char:
                case TypeCode.Int16:
                    il.Emit(OpCodes.Conv_I2);
                    break;
                case TypeCode.SByte:
                    il.Emit(OpCodes.Conv_I1);
                    break;
                case TypeCode.Byte:
                    il.Emit(OpCodes.Conv_U1);
                    break;
                case TypeCode.UInt16:
                    il.Emit(OpCodes.Conv_U2);
                    break;
                case TypeCode.Boolean:
                case TypeCode.Int32:
                    il.Emit(OpCodes.Conv_I4);
                    break;
                case TypeCode.UInt32:
                    il.Emit(OpCodes.Conv_U4);
                    break;
                case TypeCode.Int64:
                    il.Emit(OpCodes.Conv_I8);
                    break;
                case TypeCode.UInt64:
                    il.Emit(OpCodes.Conv_U8);
                    break;
                case TypeCode.Single:
                    il.Emit(OpCodes.Conv_R4);
                    break;
                case TypeCode.Double:
                    il.Emit(OpCodes.Conv_R8);
                    break;
                case TypeCode.String:
                    il.Emit(OpCodes.Call, typeof(Convert).GetMethod("ToString", new[] { to }));
                    break;
                default:
                    break;
            }
        }
        public static Type CopyMethods(string typeName, IEnumerable<MethodInfo> methods)
        {
            TypeBuilder t = mod.DefineType(typeName, TypeAttributes.Public | TypeAttributes.Class);
            foreach (MethodInfo method in methods)
            {
                var parameters = method.GetParameters();
                MethodBuilder m = t.DefineMethod(method.Name, method.Attributes, method.CallingConvention, method.ReturnType, parameters.Select(p => p.ParameterType).ToArray());
                ILGenerator il = m.GetILGenerator();
                int offset = 1;
                foreach (var param in parameters)
                {
                    m.DefineParameter(offset++, param.Attributes, param.Name);
                    il.Emit(OpCodes.Ldarg, param.Position);
                }
                il.Emit(OpCodes.Call);
                il.Emit(OpCodes.Ret);
            }
            return t.CreateType();
        }
        public static IntPtr EmitObject<T>(this ILGenerator il, ref T obj)
        {
            IntPtr ptr = Type<T>.GetAddress(ref obj);
            if (IntPtr.Size == 4)
                il.Emit(OpCodes.Ldc_I4, ptr.ToInt32());
            else
                il.Emit(OpCodes.Ldc_I8, ptr.ToInt64());
            il.Emit(OpCodes.Ldobj, obj.GetType());
            return ptr;
        }
        public static GCHandle EmitObjectGC(this ILGenerator il, object obj)
        {
            GCHandle handle = GCHandle.Alloc(il);
            IntPtr ptr = GCHandle.ToIntPtr(handle);
            if (IntPtr.Size == 4)
                il.Emit(OpCodes.Ldc_I4, ptr.ToInt32());
            else
                il.Emit(OpCodes.Ldc_I8, ptr.ToInt64());
            il.Emit(OpCodes.Ldobj, obj.GetType());
            return handle;
        }
        public static LocalBuilder MakeArray<T>(this ILGenerator il, int length)
        {
            LocalBuilder array = il.DeclareLocal(typeof(T[]));
            il.Emit(OpCodes.Ldc_I4, length);
            il.Emit(OpCodes.Newarr, typeof(T));
            il.Emit(OpCodes.Stloc, array);
            return array;
        }
        public static void IgnoreAccessCheck(Type type)
        {
            var name = type.Assembly.GetName();
            if (name.Name.StartsWith("System"))
                return;
            if (accessIgnored.Add(name.Name))
                ass.SetCustomAttribute(GetIACT(name.Name));
        }
        static readonly HashSet<string> accessIgnored = new HashSet<string>();
        static readonly ConstructorInfo iact = typeof(IgnoresAccessChecksToAttribute).GetConstructor(new[] { typeof(string) });
        static CustomAttributeBuilder GetIACT(string name) => new CustomAttributeBuilder(iact, new[] { name });
    }
    public static class Type<T>
    {
        delegate IntPtr AddrGetter(ref T obj);
        static readonly AddrGetter addrGetter;
        delegate int SizeGetter();
        static readonly SizeGetter sizeGetter;
        static Type()
        {
            addrGetter = CreateAddrGetter();
            sizeGetter = CreateSizeGetter();
        }
        static AddrGetter CreateAddrGetter()
        {
            DynamicMethod dm = new DynamicMethod($"{typeof(T).FullName}_Address", typeof(IntPtr), new[] { typeof(T).MakeByRefType() });
            ILGenerator il = dm.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Conv_U);
            il.Emit(OpCodes.Ret);
            return (AddrGetter)dm.CreateDelegate(typeof(AddrGetter));
        }
        static SizeGetter CreateSizeGetter()
        {
            DynamicMethod dm = new DynamicMethod($"{typeof(T).FullName}_Size", typeof(int), Type.EmptyTypes);
            ILGenerator il = dm.GetILGenerator();
            il.Emit(OpCodes.Sizeof, typeof(T));
            il.Emit(OpCodes.Ret);
            return (SizeGetter)dm.CreateDelegate(typeof(SizeGetter));
        }
        public static readonly Type Base = typeof(T);
        public static int Size => sizeGetter();
        public static IntPtr GetAddress(ref T obj) => addrGetter(ref obj);
    }
}