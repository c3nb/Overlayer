using JSON;
using Overlayer.MapParser.Types;
using Overlayer.MapParser.Helpers;

namespace Overlayer.MapParser.Decorations
{
    public static class DecorationUtils
    {
        public static JsonNode InitNode(LevelEventType evtType, int floor, bool visible)
        {
            JsonNode node = JsonNode.Empty;
            node.Inline = true;
            node["floor"] = floor;
            node["eventType"] = evtType.ToString();
            if (!visible)
                node["visible"] = false;
            return node;
        }
        static bool CheckIsNull(JsonNode node)
            => node is JsonLazyCreator;
        public static Decoration ParseDecoration(JsonNode node)
        {
            switch (node["eventType"].ToString().TrimLR().Parse<LevelEventType>())
            {
                case LevelEventType.AddDecoration:
                    AddDecoration adddecoration = new AddDecoration();
                    if (!CheckIsNull(node["floor"]))
                        adddecoration.floor = node["floor"].AsInt;
                    if (!CheckIsNull(node["decorationImage"]))
                        adddecoration.decorationImage = node["decorationImage"].ToString().TrimLR();
                    if (!CheckIsNull(node["position"]))
                        adddecoration.position = Vector2.FromNode(node["position"]);
                    if (!CheckIsNull(node["relativeTo"]))
                        adddecoration.relativeTo = node["relativeTo"].ToString().TrimLR().Parse<DecPlacementType>();
                    if (!CheckIsNull(node["pivotOffset"]))
                        adddecoration.pivotOffset = Vector2.FromNode(node["pivotOffset"]);
                    if (!CheckIsNull(node["rotation"]))
                        adddecoration.rotation = node["rotation"].AsDouble;
                    if (!CheckIsNull(node["scale"]))
                        adddecoration.scale = Vector2.FromNode(node["scale"]);
                    if (!CheckIsNull(node["tile"]))
                        adddecoration.tile = Vector2.FromNode(node["tile"]);
                    if (!CheckIsNull(node["color"]))
                        adddecoration.color = node["color"].ToString().TrimLR();
                    if (!CheckIsNull(node["opacity"]))
                        adddecoration.opacity = node["opacity"].AsInt;
                    if (!CheckIsNull(node["depth"]))
                        adddecoration.depth = node["depth"].AsDouble;
                    if (!CheckIsNull(node["parallax"]))
                        adddecoration.parallax = Vector2.FromNode(node["parallax"]);
                    if (!CheckIsNull(node["tag"]))
                        adddecoration.tag = node["tag"].ToString().TrimLR();
                    if (!CheckIsNull(node["imageSmoothing"]))
                        adddecoration.imageSmoothing = node["imageSmoothing"].ToString().TrimLR().Parse<Toggle>();
                    if (!CheckIsNull(node["failHitbox"]))
                        adddecoration.failHitbox = node["failHitbox"].ToString().TrimLR().Parse<Toggle>();
                    if (!CheckIsNull(node["failHitboxType"]))
                        adddecoration.failHitboxType = node["failHitboxType"].ToString().TrimLR().Parse<Hitbox>();
                    if (!CheckIsNull(node["failHitboxScale"]))
                        adddecoration.failHitboxScale = Vector2.FromNode(node["failHitboxScale"]);
                    if (!CheckIsNull(node["failHitboxOffset"]))
                        adddecoration.failHitboxOffset = Vector2.FromNode(node["failHitboxOffset"]);
                    if (!CheckIsNull(node["failHitboxRotation"]))
                        adddecoration.failHitboxRotation = node["failHitboxRotation"].AsDouble;
                    if (!CheckIsNull(node["components"]))
                        adddecoration.components = node["components"].ToString().TrimLR();
                    if (!CheckIsNull(node["visible"]))
                        adddecoration.visible = node["visible"].AsBool;
                    return adddecoration;
                case LevelEventType.AddText:
                    AddText addtext = new AddText();
                    if (!CheckIsNull(node["floor"]))
                        addtext.floor = node["floor"].AsInt;
                    if (!CheckIsNull(node["decText"]))
                        addtext.decText = node["decText"].ToString().TrimLR();
                    if (!CheckIsNull(node["font"]))
                        addtext.font = node["font"].ToString().TrimLR().Parse<FontName>();
                    if (!CheckIsNull(node["position"]))
                        addtext.position = Vector2.FromNode(node["position"]);
                    if (!CheckIsNull(node["relativeTo"]))
                        addtext.relativeTo = node["relativeTo"].ToString().TrimLR().Parse<DecPlacementType>();
                    if (!CheckIsNull(node["pivotOffset"]))
                        addtext.pivotOffset = Vector2.FromNode(node["pivotOffset"]);
                    if (!CheckIsNull(node["rotation"]))
                        addtext.rotation = node["rotation"].AsDouble;
                    if (!CheckIsNull(node["scale"]))
                        addtext.scale = Vector2.FromNode(node["scale"]);
                    if (!CheckIsNull(node["color"]))
                        addtext.color = node["color"].ToString().TrimLR();
                    if (!CheckIsNull(node["opacity"]))
                        addtext.opacity = node["opacity"].AsInt;
                    if (!CheckIsNull(node["depth"]))
                        addtext.depth = node["depth"].AsDouble;
                    if (!CheckIsNull(node["parallax"]))
                        addtext.parallax = Vector2.FromNode(node["parallax"]);
                    if (!CheckIsNull(node["tag"]))
                        addtext.tag = node["tag"].ToString().TrimLR();
                    if (!CheckIsNull(node["visible"]))
                        addtext.visible = node["visible"].AsBool;
                    return addtext;
                default: return new UnknownDecoration(node);
            }
        }
    }
}
