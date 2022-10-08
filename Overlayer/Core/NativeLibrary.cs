using Overlayer.Core.Utils;
using System;
using System.Collections.Generic;

namespace Overlayer.Core
{
    public class NativeLibrary : IDisposable
    {
        public IntPtr LibHandle;
        private bool disposed = false;
        public Dictionary<string, IntPtr> Functions = new Dictionary<string, IntPtr>();
        public NativeLibrary(string lib) => LibHandle = InteropUtils.LoadLibrary(lib);
        ~NativeLibrary() => Dispose();
        public void Dispose()
        {
            if (disposed) return;
            LibHandle = IntPtr.Zero;
            InteropUtils.FreeLIbrary(LibHandle);
            disposed = true;
        }
        public IntPtr GetFunction(string name)
        {
            if (disposed)
                return IntPtr.Zero;
            if (Functions.TryGetValue(name, out IntPtr ptr))
                return ptr;
            return Functions[name] = InteropUtils.GetFunction(LibHandle, name);
        }
    }
}
