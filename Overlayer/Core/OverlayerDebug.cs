using HarmonyLib;
using Overlayer.Core.ExceptionHandling;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityModManagerNet;

namespace Overlayer.Core
{
    public static class OverlayerDebug
    {
        struct ExecuteInfo
        {
            public Stopwatch timer;
            public string executer;
        }
        static readonly StringBuilder Buffer = new StringBuilder();
        static readonly Stack<ExecuteInfo> ExecutingStack = new Stack<ExecuteInfo>();
        static bool prevActiveStatus = true;
        public static void Init()
        {
            Application.quitting += SaveLog;
            Application.logMessageReceived += UnityLogCallback;
        }
        public static void Term()
        {
            SaveLog();
            Application.quitting -= SaveLog;
            Application.logMessageReceived -= UnityLogCallback;
        }
        static void UnityLogCallback(string condition, string stackTrace, LogType type)
        {
            if (type == LogType.Log || type == LogType.Warning) return;
            if (string.IsNullOrEmpty(stackTrace))
                Log($"{condition} {type}");
            else Log($"{condition} {type}\n{stackTrace}");
        }
        static void SaveLog()
        {
            if (Main.Settings.DebugMode || !Main.Initialized)
                File.WriteAllText(Path.Combine(Main.Mod.Path, "Debug.log"), Buffer.ToString());
        }
        public static T Log<T>(T obj, Func<T, string> toString = null)
        {
            if (Main.Initialized && !Main.Settings.DebugMode) return obj;
            Buffer.AppendLine(toString != null ? toString(obj) : obj?.ToString());
            return obj;
        }
        public static T Exception<T>(T ex, string message = null) where T : Exception
        {
            if (message != null)
                Log(message);
            Log($"Exception Has Occured.\n{ex}");
            return ex;
        }
        public static void Begin(string toExecute)
        {
            if (Main.Initialized && !Main.Settings.DebugMode) return;
            var timer = new Stopwatch();
            ExecuteInfo info;
            info.timer = timer;
            info.executer = toExecute;
            ExecutingStack.Push(info);
            timer.Start();
        }
        public static string End(bool success = true)
        {
            if (Main.Initialized && !Main.Settings.DebugMode) return null;
            if (ExecutingStack.Count < 1) return null;
            var info = ExecutingStack.Pop();
            info.timer.Stop();
            return Log($"{(success ? "" : "Failed ")}{info.executer} For {info.timer.Elapsed}");
        }
        public static void OpenDebugLog()
        {
            SaveLog();
            Application.OpenURL(Path.Combine(Main.Mod.Path, "Debug.log"));
        }
        public static void Enable()
        {
            Main.Settings.DebugMode = prevActiveStatus;
        }
        public static void Disable()
        {
            prevActiveStatus = Main.Settings.DebugMode;
            Main.Settings.DebugMode = false;
        }
    }
}
