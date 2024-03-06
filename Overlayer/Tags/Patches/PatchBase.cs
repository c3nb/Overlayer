using Overlayer.Core.Patches;

namespace Overlayer.Tags.Patches
{
    public class PatchBase<T> where T : PatchBase<T>
    {
        public static void Patch() => LazyPatchManager.PatchNested(typeof(T));
        public static void Unpatch() => LazyPatchManager.UnpatchNested(typeof(T));
    }
}
