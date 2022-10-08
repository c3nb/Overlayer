using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO;

namespace Overlayer.Core.Utils
{
    public static class InteropUtils
    {
        public static bool IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        static InteropUtils()
        {
            if (!IsWindows)
                UnixGetLastError();
        }

        public static IntPtr LoadLibrary(string libPath)
        {
            if (IsWindows)
            {
                int error = WindowsGetLastError();
                if (error != 0)
                    throw new Win32Exception(error);
                IntPtr lib = WindowsLoadLibrary(libPath);
                if (lib != IntPtr.Zero)
                    return lib;
                throw new Exception($"Cannot Load Library: {libPath}!");
            }
            else
            {
                IntPtr error = UnixGetLastError();
                if (error != IntPtr.Zero)
                    throw new Exception(Marshal.PtrToStringAnsi(error));
                IntPtr lib = UnixLoadLibrary(libPath);
                if (lib != IntPtr.Zero)
                    return lib;
                throw new Exception($"Cannot Load Library: {libPath}!");
            }
        }
        public static int FreeLIbrary(IntPtr lib)
        {
            if (IsWindows)
                return WindowsFreeLibrary(lib);
            return UnixFreeLibrary(lib);
        }
        public static IntPtr GetFunction(IntPtr lib, string name)
        {
            if (IsWindows)
                return WindowsGetProcAddress(lib, name);
            return UnixGetProcAddress(lib, name);
        }

        [DllImport("libdl", EntryPoint = "dlopen")]
        public static extern IntPtr UnixLoadLibrary(string fileName, int flags = 2);
        [DllImport("libdl", EntryPoint = "dlclose", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int UnixFreeLibrary(IntPtr handle);
        [DllImport("libdl", EntryPoint = "dlsym", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern IntPtr UnixGetProcAddress(IntPtr handle, string symbol);
        [DllImport("libdl", EntryPoint = "dlerror", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern IntPtr UnixGetLastError();

        [DllImport("kernel32", EntryPoint = "LoadLibrary")]
        private static extern IntPtr WindowsLoadLibrary(string dllPath);
        [DllImport("kernel32", EntryPoint = "FreeLibrary")]
        private static extern int WindowsFreeLibrary(IntPtr handle);
        [DllImport("kernel32", EntryPoint = "GetProcAddress")]
        private static extern IntPtr WindowsGetProcAddress(IntPtr handle, string procedureName);
        [DllImport("kernel32", EntryPoint = "GetLastError")]
        public static extern int WindowsGetLastError();
    }
}
namespace System.Runtime.InteropServices
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class NativeCallableAttribute : Attribute
    {
        public string EntryPoint;
        public CallingConvention CallingConvention;
        public NativeCallableAttribute() { }
    }
}
