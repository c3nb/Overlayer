using Overlayer.Utils;
using System;
using System.Linq;
using System.Reflection;

namespace Overlayer.Core.Patches
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class LazyPatchAttribute : Attribute
    {
        public static readonly int CurrentVersion = (int)typeof(GCNS).GetField("releaseNumber").GetValue(null);
        public string Id { get; }
        public string TargetType { get; }
        public string TargetMethod { get; }
        public string[] TargetMethodArgs { get; }
        public string[] Triggers { get; set; } = new string[] { LazyPatch.InternalTrigger };
        public int MinVersion { get; set; } = -1;
        public int MaxVersion { get; set; } = -1;
        public LazyPatchAttribute(string id, string targetType, string targetMethod)
        {
            Id = id!;
            TargetType = targetType!;
            TargetMethod = targetMethod!;
            TargetMethodArgs = null;
        }
        public LazyPatchAttribute(string id, string targetType, string targetMethod, string[] targetMethodArgs)
        {
            Id = id!;
            TargetType = targetType!;
            TargetMethod = targetMethod!;
            TargetMethodArgs = targetMethodArgs;
        }
        public bool IsCompatible => (CurrentVersion >= MinVersion || MinVersion < 0) && (MaxVersion >= CurrentVersion || MaxVersion < 0);
        public MethodBase Resolve()
        {
            if (!IsCompatible)
            {
                Main.Logger.Log($"{Id} Patch Is Not Compatible! (Min:{MinVersion}, Max:{MaxVersion}, Current:{CurrentVersion})");
                return null;
            }
            try
            {
                var tt = MiscUtils.TypeByName(TargetType);
                var tma = TargetMethodArgs?.Select(MiscUtils.TypeByName).ToArray();
                if (TargetMethod == ".ctor")
                    return tma != null ?
                        tt?.GetConstructor((BindingFlags)15420, null, tma, null) :
                        tt?.GetConstructors((BindingFlags)15420).FirstOrDefault();
                else if (TargetMethod == ".cctor") return tt.TypeInitializer;
                else return tma != null ?
                        tt?.GetMethod(TargetMethod, (BindingFlags)15420, null, tma, null) :
                        tt?.GetMethod(TargetMethod, (BindingFlags)15420);
            }
            catch (AmbiguousMatchException)
            {
                Main.Logger.Log($"{Id} Patch Is Ambiguous Match! (Min:{MinVersion}, Max:{MaxVersion}, Current:{CurrentVersion})");
                return null;
            }
        }
    }
}
