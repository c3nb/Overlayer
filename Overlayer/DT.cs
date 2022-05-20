using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Overlayer
{
    public static unsafe class DT
    {
        public delegate TimeSpan GDTNUOFU(DateTime utcNow, out bool ald);
        public static readonly MethodInfo MonoCreateDM;
        public static readonly FieldInfo MonoHandleDM;
        public static readonly MethodInfo NetCreateDM;
        public static readonly bool IsMonoRuntime;
        public static long ticks;
        public static bool ald;
        public static readonly ConstructorInfo dtConstructor = typeof(DateTime).GetConstructor((BindingFlags)15420, null, new[] { typeof(long), typeof(DateTimeKind), typeof(bool) }, null);
        public static readonly MethodInfo utc = typeof(DateTime).GetProperty("UtcNow").GetGetMethod();
        public static readonly MethodInfo dtTicks = typeof(DateTime).GetProperty("Ticks").GetGetMethod();
        public static readonly FieldInfo ticksFld = typeof(DT).GetField("ticks", (BindingFlags)15420);
        public static readonly FieldInfo aldFld = typeof(DT).GetField("ald", (BindingFlags)15420);
        public static readonly GDTNUOFU getOffset;
        public static readonly delegate*<DateTime> GetNowPtr;
        static DT()
        {
            IsMonoRuntime = Type.GetType("Mono.Runtime") != null;
            MonoCreateDM = typeof(DynamicMethod).GetMethod("CreateDynMethod", (BindingFlags)15420);
            MonoHandleDM = typeof(DynamicMethod).GetField("mhandle", (BindingFlags)15420);
            NetCreateDM = typeof(DynamicMethod).GetMethod("GetMethodDescriptor", (BindingFlags)15420);
            getOffset = (GDTNUOFU)typeof(TimeZoneInfo).GetMethod("GetDateTimeNowUtcOffsetFromUtc", (BindingFlags)15420).CreateDelegate(typeof(GDTNUOFU));
            ticks = getOffset(DateTime.UtcNow, out ald).Ticks;
            DynamicMethod nowGetter = new DynamicMethod(string.Empty, typeof(DateTime), Type.EmptyTypes, true);
            ILGenerator il = nowGetter.GetILGenerator();
            LocalBuilder dtLoc = il.DeclareLocal(typeof(DateTime));
            LocalBuilder tLoc = il.DeclareLocal(typeof(long));
            il.Emit(OpCodes.Call, utc);
            il.Emit(OpCodes.Stloc, dtLoc);
            il.Emit(OpCodes.Ldloca, dtLoc);
            il.Emit(OpCodes.Call, dtTicks);
            il.Emit(OpCodes.Ldsfld, ticksFld);
            il.Emit(OpCodes.Add);
            il.Emit(OpCodes.Stloc, tLoc);
            il.Emit(OpCodes.Ldloc, tLoc);
            il.Emit(OpCodes.Ldc_I4_2);
            il.Emit(OpCodes.Ldsfld, aldFld);
            il.Emit(OpCodes.Newobj, dtConstructor);
            il.Emit(OpCodes.Ret);
                //.DeclareLocal(typeof(DateTime), out LocalBuilder dtLoc)
                //.DeclareLocal(typeof(long), out var tLoc)
                //.Call(utc).Stloc(dtLoc)
                //.Ldloca(dtLoc).Call(dtTicks)
                //.Ldsfld(ticksFld).Add().Stloc(tLoc)
                //.Ldloc(tLoc).Ldc_I4_2().Ldsfld(aldFld)
                //.Newobj(dtConstructor).Ret();
            GetNowPtr = (delegate*<DateTime>)GetHandleFromDynMethod(nowGetter).GetFunctionPointer();
        }
        public static DateTime Now => GetNowPtr();
        public static DateTime DTNow => DateTime.Now;
        public static RuntimeMethodHandle GetHandleFromDynMethod(DynamicMethod dynMethod)
        {
            if (IsMonoRuntime)
            {
                MonoCreateDM.Invoke(dynMethod, null);
                return (RuntimeMethodHandle)MonoHandleDM.GetValue(dynMethod);
            }
            return (RuntimeMethodHandle)NetCreateDM.Invoke(dynMethod, null);
        }
    }
}
