using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Overlayer.Core.Utils
{
    public static class ExtUtils
    {
        public static void Deconstruct<T1, T2>(this KeyValuePair<T1, T2> tuple, out T1 key, out T2 value)
        {
            key = tuple.Key;
            value = tuple.Value;
        }
        public static string IfNullOrEmpty(this string s, string other) => string.IsNullOrEmpty(s) ? other : s;
        public static T MakeFlexible<T>(this T comp) where T : Component
        {
            comp.gameObject.MakeFlexible();
            return comp;
        }
        public static GameObject MakeFlexible(this GameObject go)
        {
            ContentSizeFitter csf = go.GetComponent<ContentSizeFitter>() ?? go.AddComponent<ContentSizeFitter>();
            csf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            return go;
        }
        public static int Map(this int value, int fromMin, int fromMax, int toMin, int toMax) => toMin + (value - fromMin) * (toMax - toMin) / (fromMax - fromMin);
        public static double Map(this double value, double fromMin, double fromMax, double toMin, double toMax) => toMin + (value - fromMin) * (toMax - toMin) / (fromMax - fromMin);
        public static float Map(this float value, float fromMin, float fromMax, float toMin, float toMax) => toMin + (value - fromMin) * (toMax - toMin) / (fromMax - fromMin);
        public static string RemoveLast(this string s, int count) => s.Remove(s.Length - count - 1);
        public static double Round(this double value, int digits) => digits < 0 ? value : Math.Round(value, digits);
        public static float Round(this float value, int digits) => digits < 0 ? value : (float)Math.Round((double)value, digits);
        public static async void RunAsynchronously(this Task task, TimeSpan? timeout = null)
        {
            if (timeout != null)
                await task.TryWaitAsync(timeout.Value);
            else await task;
        }
        public static async Task TryWaitAsync(this Task task, TimeSpan timeout)
        {
            if (task.IsCompleted) return;
            using (var cts = new CancellationTokenSource())
            {
                var delay = Task.Delay(timeout, cts.Token);
                var result = await Task.WhenAny(task, delay).ConfigureAwait(false);
                if (result == delay) return;
                cts.Cancel();
            }
        }
        public static async Task<T> TryWaitAsync<T>(this Task<T> task, TimeSpan timeout)
        {
            if (task.IsCompleted) return task.Result;
            using (var cts = new CancellationTokenSource())
            {
                var delay = Task.Delay(timeout, cts.Token);
                var result = await Task.WhenAny(task, delay).ConfigureAwait(false);
                if (result == delay) return default;
                cts.Cancel();
            }
            return task.Result;
        }
    }
}