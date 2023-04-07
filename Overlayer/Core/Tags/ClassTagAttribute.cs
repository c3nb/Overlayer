using System;
using System.Threading;
using HarmonyLib;
using System.Collections.Generic;

namespace Overlayer.Core.Tags
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ClassTagAttribute : TagAttribute
    {
        public ClassTagAttribute() : base() { }
        public ClassTagAttribute(string name) : base(name) { }
        public string[] Threads { get; set; }
        internal Harmony harmony;
        internal List<Thread> threads;
        internal List<Tag> tags;
        internal Type target;
        public void Combine(TagAttribute tagAttr)
        {
            tagAttr.NotPlaying |= NotPlaying;
            tagAttr.Name ??= Name;
        }
    }
}
