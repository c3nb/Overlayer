using System;
using HarmonyLib;
using Overlayer.Core;

namespace Overlayer.Core.Tags
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ClassTagAttribute : TagAttribute
    {
        public ClassTagAttribute(string name) : base(name) { }
        public string[] Threads;
        internal Harmony Harmony;
        public Type PatchesType;
        public void Combine(TagAttribute tagAttr)
        {
            tagAttr.NotPlaying |= NotPlaying;
            tagAttr.Name ??= Name;
        }
    }
}
