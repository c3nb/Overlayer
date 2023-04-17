using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Scripting;

namespace Overlayer.Core
{
    [Flags]
    public enum CleanOption
    {
        Default = 0,
        Incremental = 1,
        UnloadAssets = 2,
        All = Incremental | UnloadAssets
    }
    public static class MemoryHelper
    {
        public static IntPtr Alloc(int bytes)
        {
            return Marshal.AllocHGlobal(bytes);
        }
        public static void Free(IntPtr ptr)
        {
            Marshal.FreeHGlobal(ptr);
        }
        public static unsafe IntPtr GetAddress<T>(ref T obj)
        {
            TypedReference tr = __makeref(obj);
            return *(IntPtr*)&tr;
        }
        public static void Clean(CleanOption option = CleanOption.Default)
        {
            OverlayerDebug.Begin($"Cleaning Memory With Option {option}");
            var prevMode = GarbageCollector.GCMode;
            GarbageCollector.GCMode = GarbageCollector.Mode.Enabled;
            if ((option & CleanOption.Incremental) != 0)
                GarbageCollector.CollectIncremental(100000);
            else GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, false);
            if ((option & CleanOption.UnloadAssets) != 0)
                Resources.UnloadUnusedAssets();
            GarbageCollector.GCMode = prevMode;
            OverlayerDebug.End();
        }
        public static async void CleanAsync(CleanOption option = CleanOption.Default)
        {
            await Task.Run(() => Clean(option));
        }
    }
}
