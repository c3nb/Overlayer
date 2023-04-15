using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityModManagerNet;

namespace Overlayer.Core
{
    public static class OverlayerDebug
    {
        static readonly StringBuilder Buffer = new StringBuilder();
        public static void Init()
        {
            Application.quitting += SaveLog;
            ExceptionCatcher.Unhandled += UnhandledException;
            ExceptionCatcher.Catched += CatchedException;
        }
        public static void Term()
        {
            Application.quitting -= SaveLog;
            ExceptionCatcher.Unhandled -= UnhandledException;
            ExceptionCatcher.Catched -= CatchedException;
        }
        static void SaveLog()
        {
            if (Main.Settings.DebugMode)
                File.WriteAllText(Path.Combine(Main.Mod.Path, "Debug.log"), Buffer.ToString());
        }
        static void UnhandledException(Exception e)
        {
            Log($"Unhandled Exception Has Occured.\n{e}");
        }
        static void CatchedException(Exception e)
        {
            Log($"Exception Has Occured. (Catched)\n{e}");
        }
        public static T Log<T>(T obj, Func<T, string> toString = null)
        {
            if (!Main.Settings.DebugMode) return obj;
            Buffer.AppendLine(toString != null ? toString(obj) : obj?.ToString());
            return obj;
        }
        public static T Exception<T>(T ex, string message = null) where T : Exception
        {
            if (message != null)
                Log(message);
            CatchedException(ex);
            return ex;
        }
        public static void OpenDebugLog()
        {
            SaveLog();
            Application.OpenURL(Path.Combine(Main.Mod.Path, "Debug.log"));
        }
    }
}
