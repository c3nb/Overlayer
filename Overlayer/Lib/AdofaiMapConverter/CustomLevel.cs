using System.Collections.Generic;
using System.Linq;
using System.Text;
using JSON;
using AdofaiMapConverter.Actions;
using AdofaiMapConverter.Decorations;
using AdofaiMapConverter.Helpers;
using AdofaiMapConverter.Types;

namespace AdofaiMapConverter
{
    public class CustomLevel : System.ICloneable
    {
        public List<Tile> Tiles;
        public LevelSetting Setting;
        public CustomLevel()
        {
            Tiles = new List<Tile>();
            Setting = new LevelSetting();
        }
        public object Clone() => Copy();
        public CustomLevel Copy()
        {
            CustomLevel customLevel = new CustomLevel();
            customLevel.Tiles = Tiles.Select(t => t.Copy()).ToList();
            customLevel.Setting = Setting.Copy();
            return customLevel;
        }
        public CustomLevel ShallowCopy()
        {
            CustomLevel customLevel = new CustomLevel();
            customLevel.Tiles = Tiles;
            customLevel.Setting = Setting;
            return customLevel;
        }
        public static CustomLevel Read(JsonNode node)
        {
            JsonNode pd = node["pathData"];
            JsonNode ad = node["angleData"];
            JsonNode s = node["settings"];
            JsonNode ac = node["actions"];
            JsonNode dc = node["decorations"];
            bool hasNotpd = pd is JsonLazyCreator;
            bool hasNotad = ad is JsonLazyCreator;
            if (hasNotad && hasNotpd)
                throw new System.InvalidOperationException("There's No pathData, angleData.");
            if (s is JsonLazyCreator)
                throw new System.InvalidOperationException("settings Not Found.");
            if (ac is JsonLazyCreator)
                throw new System.InvalidOperationException("actions Not Found.");
            LevelSetting setting = LevelSetting.FromNode(s);
            if (setting.version > 10)
                System.Console.WriteLine("This MapConverter May Not Work Normally On Greater Then Version 10.");
            List<TileAngle> tileAngles = hasNotpd ? AngleHelper.ReadAngleData(ad.Values.Select(n => n.AsDouble)) : AngleHelper.ReadPathData(pd.ToString().TrimLR());
            tileAngles.Insert(0, TileAngle.Zero);
            Dictionary<int, List<Decoration>> decorations = new Dictionary<int, List<Decoration>>();
            if (!(dc is JsonLazyCreator))
                foreach (var deco in dc.Values.Select(n => DecorationUtils.ParseDecoration(n)))
                {
                    if (decorations.TryGetValue(deco.floor, out var list))
                        list.Add(deco);
                    else decorations.Add(deco.floor, new List<Decoration>() { deco });
                }
            CustomLevel level = new CustomLevel();
            level.Setting = setting;
            level.Tiles = tileAngles.Select(d => new Tile(d)).ToList();
            foreach (JsonNode actionNode in ac.Values)
            {
                int floor = actionNode["floor"].AsInt;
                Tile tile = level.Tiles[floor];
                Action action = ActionUtils.ParseAction(actionNode);
                tile.AddAction(action);
                if (decorations.TryGetValue(floor, out var decoList))
                    tile.decorations = decoList;
            }
            return level.MakeTiles();
        }
        public CustomLevel MakeTiles()
        {
            Tile firstTile = Tiles[0];
            var nextAngle = Tiles.Count == 1 ? TileAngle.Zero : Tiles[1].angle;
            firstTile.MakeMetaFirst(Setting, nextAngle);
            TileMeta prevTileMeta = firstTile.tileMeta;
            if (Tiles.Count <= 1) return this;
            Tile curTile = Tiles[1];
            for (int i = 2; i < Tiles.Count; i++)
            {
                Tile nextTile = Tiles[i];
                curTile.MakeMeta(prevTileMeta, nextTile.angle);
                prevTileMeta = curTile.tileMeta;
                curTile = nextTile;
            }
            curTile.MakeMetaLast(prevTileMeta);
            return this;
        }
        public JsonNode ToNode()
        {
            JsonNode root = JsonNode.Empty;
            if (Setting.version >= 5)
            {
                JsonArray ad = root["angleData"].AsArray;
                ad.Inline = true;
                for (int i = 1; i < Tiles.Count; i++)
                {
                    Tile tile = Tiles[i];
                    ad.Add(tile.angle.isMidspin ? 999 : tile.angle.Angle);
                }
            }
            else
            {
                try
                {
                    TileAngle curAngle = Tiles[1].angle;
                    StringBuilder pdBuilder = new StringBuilder();
                    for (int i = 2; i < Tiles.Count; i++)
                    {
                        Tile tile = Tiles[i];
                        TileAngle nextAngle = tile.angle;
                        char c = AngleHelper.GetCharFromAngle(curAngle.isMidspin ? 999 : curAngle.Angle, nextAngle.isMidspin ? 999 : nextAngle.Angle);
                        if (c == char.MinValue) throw new System.Exception();
                        pdBuilder.Append(c);
                        curAngle = nextAngle;
                    }
                    char last = AngleHelper.GetCharFromAngle(curAngle.isMidspin ? 999 : curAngle.Angle, 999);
                    pdBuilder.Append(last);
                    root["pathData"] = pdBuilder.ToString();
                }
                catch
                {
                    Setting.version = 5;
                    JsonArray ad = root["angleData"].AsArray;
                    ad.Inline = true;
                    for (int i = 1; i < Tiles.Count; i++)
                    {
                        Tile tile = Tiles[i];
                        ad.Add(tile.angle.isMidspin ? 999 : tile.angle.Angle);
                    }
                }
            }
            root["settings"] = Setting.ToNode();
            JsonArray acts = root["actions"].AsArray;
            List<LevelEventType> letOrder = ((LevelEventType[])System.Enum.GetValues(typeof(LevelEventType))).ToList();
            letOrder.RemoveAll(let => let == LevelEventType.SetSpeed || let == LevelEventType.Twirl || let == LevelEventType.Checkpoint);
            letOrder.InsertRange(0, new LevelEventType[] { LevelEventType.SetSpeed, LevelEventType.Twirl, LevelEventType.Checkpoint });
            for (int i = 0; i < Tiles.Count; i++)
            {
                Tile tile = Tiles[i];
                foreach (LevelEventType let in letOrder)
                {
                    foreach (var action in tile.actions.GetValueSafe(let, new List<Action>()))
                    {
                        JsonObject actionNode = (JsonObject)action.ToNode();
                        JsonNode newActionNode = JsonNode.Empty;
                        newActionNode["floor"] = i;
                        foreach (var kvp in actionNode.m_Dict)
                            newActionNode[kvp.Key] = kvp.Value;
                        newActionNode["floor"] = i;
                        newActionNode.Inline = true;
                        acts.Add(newActionNode);
                    }
                }
            }
            if (Setting.version >= 9)
            {
                JsonArray decs = root["decorations"].AsArray;
                foreach (var dec in Tiles.SelectMany(t => t.decorations))
                    decs.Add(dec.ToNode());
            }
            else
            {
                foreach (var deco in Tiles.SelectMany(t => t.decorations))
                {
                    JsonObject decoNode = (JsonObject)deco.ToNode();
                    JsonNode newdecoNode = JsonNode.Empty;
                    foreach (var kvp in decoNode.m_Dict)
                    {
                        if (kvp.Key == "decorationImage")
                        {
                            if (Setting.version <= 3)
                                newdecoNode["decText"] = kvp.Value;
                            else newdecoNode[kvp.Key] = kvp.Value;
                        }
                        else newdecoNode[kvp.Key] = kvp.Value;
                    }
                    newdecoNode.Inline = true;
                    acts.Add(newdecoNode);
                }
            }
            return root;
        }
    }
}
