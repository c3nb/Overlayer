using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;

namespace Overlayer.Core.Utils
{
    public static class MiscUtils
    {
        static MiscUtils()
        {
            loadedAsss = AppDomain.CurrentDomain.GetAssemblies();
        }
        public static Assembly[] loadedAsss { get; private set; }
        public static Type[] loadedTypes { get; private set; }
        public static Assembly AssByName(string assName)
        {
            return loadedAsss.FirstOrDefault(t => t.FullName == assName) ??
                loadedAsss.FirstOrDefault(t => t.GetName().Name == assName);
        }
        public static Delegate CreateDelegateAuto(this MethodInfo method)
        {
            var prms = method.GetParameters().Select(p => p.ParameterType);
            if (method.ReturnType != typeof(void))
                return method.CreateDelegate(Expression.GetFuncType(prms.Append(method.ReturnType).ToArray()));
            return method.CreateDelegate(Expression.GetActionType(prms.ToArray()));
        }
        public static void ExecuteSafe(Action exec, out Exception ex)
        {
            ex = null;
            try { exec.Invoke(); }
            catch (Exception e) { ex = e; }
        }
        public static T ExecuteSafe<T>(Func<T> exec, out Exception ex)
        {
            ex = null;
            try { return exec.Invoke(); }
            catch (Exception e) { ex = e; return default; }
        }
        public static T ExecuteSafe<T>(Func<T> exec, T defaultValue, out Exception ex)
        {
            ex = null;
            try { return exec.Invoke(); }
            catch (Exception e) { ex = e; return defaultValue; }
        }
        public static TimeSpan MeasureTime(Action a)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            a.Invoke();
            watch.Stop();
            return watch.Elapsed;
        }
        public static Color ShiftHue(Color color, float amount)
        {
            Color.RGBToHSV(color, out float hue, out _, out _);
            hue += amount;
            return Color.HSVToRGB(hue, 1, 1);
        }
        public static Color ShiftSaturation(Color color, float amount)
        {
            Color.RGBToHSV(color, out _, out float sat, out _);
            sat += amount;
            return Color.HSVToRGB(1, sat, 1);
        }
        public static Color ShiftValue(Color color, float amount)
        {
            Color.RGBToHSV(color, out _, out _, out float val);
            val += amount;
            return Color.HSVToRGB(1, 1, val);
        }
        public static Type TypeByName(string typeName)
        {
            if (loadedTypes == null)
                loadedTypes = loadedAsss.Select(ass => ExecuteSafe(ass.GetTypes, out _)).Where(t => t != null).SelectMany(ts => ts).ToArray();
            return Type.GetType(typeName, false) ??
                loadedTypes.FirstOrDefault(t => t.FullName == typeName) ??
                loadedTypes.FirstOrDefault(t => t.Name == typeName);
        }
    }
}