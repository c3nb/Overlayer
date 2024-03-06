using JSON;
using Overlayer.Core.Interfaces;
using Overlayer.Models;
using Overlayer.Utils;
using UnityModManagerNet;

namespace Overlayer
{
    public class Settings : UnityModManager.ModSettings, IModel, ICopyable<Settings>
    {
        public bool ChangeFont = false;
        public FontMeta AdofaiFont = new FontMeta();
        public OverlayerLanguage Lang = OverlayerLanguage.English;
        public float FPSUpdateRate = 100;
        public float FrameTimeUpdateRate = 100;
        public JsonNode Serialize()
        {
            var node = JsonNode.Empty;
            node[nameof(ChangeFont)] = ChangeFont;
            node[nameof(AdofaiFont)] = AdofaiFont.Serialize();
            node[nameof(Lang)] = Lang.ToString();
            node[nameof(FPSUpdateRate)] = FPSUpdateRate;
            node[nameof(FrameTimeUpdateRate)] = FrameTimeUpdateRate;
            return node;
        }
        public void Deserialize(JsonNode node)
        {
            ChangeFont = node[nameof(ChangeFont)];
            AdofaiFont = ModelUtils.Unbox<FontMeta>(node[nameof(AdofaiFont)]);
            Lang = EnumHelper<OverlayerLanguage>.Parse(node[nameof(Lang)]);
            FPSUpdateRate = node[nameof(FPSUpdateRate)];
            FrameTimeUpdateRate = node[nameof(FrameTimeUpdateRate)];
        }
        public Settings Copy()
        {
            var newSettings = new Settings();
            newSettings.ChangeFont = ChangeFont;
            newSettings.AdofaiFont = AdofaiFont.Copy();
            newSettings.Lang = Lang;
            newSettings.FPSUpdateRate = FPSUpdateRate;
            newSettings.FrameTimeUpdateRate = FrameTimeUpdateRate;
            return newSettings;
        }
    }
}
