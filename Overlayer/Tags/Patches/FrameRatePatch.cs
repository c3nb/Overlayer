using Overlayer.Core.Patches;

namespace Overlayer.Tags.Patches
{
    public class FrameRatePatch : PatchBase<FrameRatePatch>
    {
        [LazyPatch("Tags.FrameRate.FrameRateGetter", "scrCamera", "Update", Triggers = new string[]
        {
            nameof(FrameRate.Fps), nameof(FrameRate.FrameTime)
        })]
        public static class FrameRateGetter
        {
            static float lastDeltaTime;
            static float fpsTimer;
            static float fpsTimeTimer;
            public static void Postfix()
            {
                var deltaTime = UnityEngine.Time.deltaTime;
                lastDeltaTime += (deltaTime - lastDeltaTime) * 0.1f;
                if (fpsTimer > Main.Settings.FPSUpdateRate / 1000.0f)
                {
                    FrameRate.Fps = 1.0f / lastDeltaTime;
                    fpsTimer = 0;
                }
                fpsTimer += deltaTime;
                if (fpsTimeTimer > Main.Settings.FrameTimeUpdateRate / 1000.0f)
                {
                    FrameRate.FrameTime = lastDeltaTime * 1000.0f;
                    fpsTimeTimer = 0;
                }
                fpsTimeTimer += deltaTime;
            }
        }
    }
}
