using System;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Scripting;

namespace Overlayer.Core
{
    [Flags]
    public enum CleanOption
    {
        CollectGarbage = 1,
        UnloadAssets = 2,
        All = CollectGarbage | UnloadAssets
    }
    public static class MemoryHelper
    {
        public static IDisposable CurrentGC { get; private set; }
        public static bool Cleaning { get; private set; }
        public static IntPtr Alloc(int bytes)
        {
            return Marshal.AllocHGlobal(bytes);
        }
        public static unsafe IntPtr Calloc(int count, int size)
        {
            int total = count * size;
            IntPtr addr = Marshal.AllocHGlobal(total);
            byte* ptr = (byte*)addr;
            for (int i = 0; i < total; i++)
                ptr[i] = 0;
            return addr;
        }
        public static IntPtr Realloc(IntPtr addr, int size)
        {
            return Marshal.ReAllocHGlobal(addr, (IntPtr)size);
        }
        public static void Free(IntPtr ptr)
        {
            Marshal.FreeHGlobal(ptr);
        }
        public static void Free<T>(ref T obj)
        {
            if (ReferenceEquals(obj, default(T))) return;
            int generation = GC.GetGeneration(obj);
            obj = default;
            GC.Collect(generation, GCCollectionMode.Forced, false);
        }
        public static unsafe IntPtr GetAddress<T>(ref T obj)
        {
            TypedReference tr = __makeref(obj);
            return *(IntPtr*)&tr;
        }
        public static void Clean(CleanOption option = CleanOption.CollectGarbage)
        {
            OverlayerDebug.Begin($"Cleaning Memory With Option {option}");
            Cleaning = true;
            Overlayer.Tags.Expression.expressions.Clear();
            if ((option & CleanOption.CollectGarbage) != 0)
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            if ((option & CleanOption.UnloadAssets) != 0)
                Resources.UnloadUnusedAssets();
            Cleaning = false;
            OverlayerDebug.End();
        }
        public static IDisposable StartNoGC() => CurrentGC = new GCObj(GarbageCollector.Mode.Disabled);
        public static void EndNoGC()
        {
            CurrentGC?.Dispose();
            CurrentGC = null;
        }
        public static async void CleanAsync(CleanOption option = CleanOption.CollectGarbage)
        {
            if (Cleaning) return;
            await Task.Run(() => Clean(option));
        }
        struct GCObj : IDisposable
        {
            public GCObj(GarbageCollector.Mode mode) => GarbageCollector.GCMode = mode;
            void IDisposable.Dispose() => GarbageCollector.GCMode = GarbageCollector.Mode.Enabled;
        }
    }
}
