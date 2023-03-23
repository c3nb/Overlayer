using AdofaiMapConverter.Types;
using JSON;
using static AdofaiMapConverter.Actions.ActionUtils;

namespace AdofaiMapConverter.Actions.Default
{
    public class CustomBackground : Action
    {
        public string color = "000000";
        public string bgImage = "";
        public string imageColor = "ffffff";
        public Vector2 parallax = Vector2.Hrd;
        public BgDisplayMode bgDisplayMode = BgDisplayMode.FitToScreen;
        public Toggle lockRot = Toggle.Disabled;
        public Toggle loopBG = Toggle.Disabled;
        public double unscaledSize = 100;
        public double angleOffset = 0;
        public string eventTag = "";
        public CustomBackground() : base(LevelEventType.CustomBackground) { }
        public CustomBackground(string color, string bgImage, string imageColor, Vector2 parallax, BgDisplayMode bgDisplayMode, Toggle lockRot, Toggle loopBG, double unscaledSize, double angleOffset, string eventTag, bool active) : base(LevelEventType.CustomBackground, active)
        {
            this.color = color;
            this.bgImage = bgImage;
            this.imageColor = imageColor;
            this.parallax = parallax;
            this.bgDisplayMode = bgDisplayMode;
            this.lockRot = lockRot;
            this.loopBG = loopBG;
            this.unscaledSize = unscaledSize;
            this.angleOffset = angleOffset;
            this.eventTag = eventTag;
        }
        public override JsonNode ToNode()
        {
            JsonNode node = InitNode(eventType, active);
            node["color"] = color;
            node["bgImage"] = bgImage;
            node["imageColor"] = imageColor;
            node["parallax"] = parallax.ToNode();
            node["bgDisplayMode"] = bgDisplayMode.ToString();
            node["lockRot"] = lockRot.ToString();
            node["loopBG"] = loopBG.ToString();
            node["unscaledSize"] = unscaledSize;
            node["angleOffset"] = angleOffset;
            node["eventTag"] = eventTag;
            return node;
        }
    }
}
