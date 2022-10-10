using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Overlayer.Core.Utils
{
    public static class EmitUtils
    {
        public static IntPtr EmitObject<T>(this ILGenerator il, ref T obj)
        {
            IntPtr ptr = Address<T>.Get(ref obj);
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
        public static class Address<T>
        {
            delegate IntPtr AddrGetter(ref T obj);
            static readonly AddrGetter getter;
            static Address()
            {
                DynamicMethod dm = new DynamicMethod($"{typeof(T).FullName}_Address", typeof(IntPtr), new[] { typeof(T).MakeByRefType() });
                ILGenerator il = dm.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Conv_U);
                il.Emit(OpCodes.Ret);
                getter = (AddrGetter)dm.CreateDelegate(typeof(AddrGetter));
            }
            public static IntPtr Get(ref T obj) => getter(ref obj);
        }
    }
}
