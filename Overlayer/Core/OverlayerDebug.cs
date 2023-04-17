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
        static readonly StringBuilder Buffer = new StringBuilder();
        static readonly Stack<Stopwatch> TimerStack = new Stack<Stopwatch>();
        static readonly Stack<string> ExecutingStack = new Stack<string>();
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
            if (Main.Settings.DebugMode)
                File.WriteAllText(Path.Combine(Main.Mod.Path, "Debug.log"), Buffer.ToString());
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
            Log($"Exception Has Occured.\n{ex}");
            return ex;
        }
        public static void Begin(string toExecute)
        {
            var timer = new Stopwatch();
            TimerStack.Push(timer);
            ExecutingStack.Push(toExecute);
            timer.Start();
        }
        public static void End()
        {
            if (ExecutingStack.Count < 1) return;
            var timer = TimerStack.Pop();
            timer.Stop();
            var executing = ExecutingStack.Pop();
            Log($"{executing} For {timer.Elapsed}");
        }
        public static void OpenDebugLog()
        {
            SaveLog();
            Application.OpenURL(Path.Combine(Main.Mod.Path, "Debug.log"));
        }
    }
}
