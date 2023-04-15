using System;
using System.IO;
using System.Text;
using UnityEngine;

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
        static void SaveLog() => File.WriteAllText(Path.Combine(Main.Mod.Path, "Debug.log"), Buffer.ToString());
        static void UnhandledException(Exception e)
        {
            Log($"Unhandled Exception Has Occured.\n{e}");
        }
        static void CatchedException(Exception e)
        {
            Log($"Exception Has Occured. (Catched)\n{e}");
        }
        public static T Log<T>(T obj)
        {
            if (!Main.Settings.DebugMode) return obj;
            Buffer.AppendLine(obj.ToString());
            return obj;
        }
    }
}
