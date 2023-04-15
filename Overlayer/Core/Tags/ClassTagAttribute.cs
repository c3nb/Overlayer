using System;
using System.Threading;
using HarmonyEx;
using System.Collections.Generic;
using System.Reflection;

namespace Overlayer.Core.Tags
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ClassTagAttribute : TagAttribute
    {
        public ClassTagAttribute() : base() { }
        public ClassTagAttribute(string name) : base(name) { }
        public string[] Threads { get; set; }
        public IEnumerable<MethodInfo> GetThreads(Type thisType)
        {
            if (Threads == null)
                yield break;
            foreach (var thread in Threads)
                yield return thisType.GetMethod(thread, AccessTools.all);
        }
    }
}
