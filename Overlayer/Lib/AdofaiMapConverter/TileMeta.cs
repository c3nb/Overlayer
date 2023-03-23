using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdofaiMapConverter.Actions;
using AdofaiMapConverter.Types;
using AdofaiMapConverter.Helpers;
using MT = System.Math;

namespace AdofaiMapConverter
{
    public class TileMeta : System.ICloneable
    {
        public int floor;
        public double bpm;
        public double travelAngle;
        public double staticAngle;
        public bool reversed;
        public double additionalSleepAngle;
        public bool hold;
        public int planets;
        public double realX;
        public double realY;
        public double editorX;
        public double editorY;
        public TileMeta Copy()
        {
            TileMeta tileMeta = new TileMeta();
            tileMeta.floor = floor;
            tileMeta.bpm = bpm;
            tileMeta.travelAngle = travelAngle;
            tileMeta.staticAngle = staticAngle;
            tileMeta.reversed = reversed;
            tileMeta.additionalSleepAngle = additionalSleepAngle;
            tileMeta.hold = hold;
            tileMeta.planets = planets;
            tileMeta.realX = realX;
            tileMeta.realY = realY;
            tileMeta.editorX = editorX;
            tileMeta.editorY = editorY;
            return tileMeta;
        }
        public object Clone() => Copy();
        TileMeta() { }
        public TileMeta(Dictionary<LevelEventType, List<Action>> actions, LevelSetting levelSetting, TileAngle nextAngle)
        {
            floor = 0;
            bpm = levelSetting.bpm;
            travelAngle = 360;
            staticAngle = 0;
            reversed = false;
            planets = 2;
            realX = 0;
            realY = 0;
            editorX = 0;
            editorY = 0;
            Update(actions, 0, TileAngle.Zero, nextAngle);
        }
        public TileMeta(Dictionary<LevelEventType, List<Action>> actions, TileMeta prevTileMeta, TileAngle currAngle, TileAngle nextAngle)
        {
            floor = prevTileMeta.floor + 1;
            bpm = prevTileMeta.bpm;
            staticAngle = prevTileMeta.staticAngle;
            reversed = prevTileMeta.reversed;
            planets = prevTileMeta.planets;
            realX = prevTileMeta.realX;
            realY = prevTileMeta.realY;
            editorX = prevTileMeta.editorX;
            editorY = prevTileMeta.editorY;
            Update(actions, prevTileMeta.staticAngle, currAngle, nextAngle);
            double rad = prevTileMeta.staticAngle.ToRad();
            double x = MT.Cos(rad);
            double y = MT.Sin(rad);
            realX += x;
            realY += y;
            editorX += x;
            editorY += y;
        }
        public void Update(Dictionary<LevelEventType, List<Action>> actionDict, double staticAngle, TileAngle curAngle, TileAngle nextAngle)
        {
            List<Action> actions = actionDict.GetValueSafe(LevelEventType.SetSpeed);
            if (actions != null && actions.Count > 0)
            {
                SetSpeed ss = (SetSpeed)actions[0];
                if (ss.speedType == SpeedType.Bpm)
                    bpm = ss.beatsPerMinute;
                else bpm *= ss.bpmMultiplier;
            }
            actions = actionDict.GetValueSafe(LevelEventType.Twirl);
            if (actions != null && actions.Count > 0)
                reversed = !reversed;
            actions = actionDict.GetValueSafe(LevelEventType.MultiPlanet);
            if (actions != null && actions.Count > 0)
            {
                MultiPlanet mp = (MultiPlanet)actions[0];
                planets = mp.planets;
            }
            AngleHelper.Result result = AngleHelper.CalculateAngleData(staticAngle, curAngle, nextAngle, CalculatePlanetAngle(planets), reversed);
            this.staticAngle = result.curStaticAngle;
            travelAngle = result.curTravelAngle;
            actions = actionDict.GetValueSafe(LevelEventType.Hold);
            if (actions != null && actions.Count > 0)
            {
                Hold hold = (Hold)actions[0];
                this.hold = true;
                additionalSleepAngle = 360 * hold.duration;
            }
            actions = actionDict.GetValueSafe(LevelEventType.FreeRoam);
            if (actions != null && actions.Count > 0)
            {
                FreeRoam fr = (FreeRoam)actions[0];
                additionalSleepAngle = 180 * fr.duration;
            }
            actions = actionDict.GetValueSafe(LevelEventType.Pause);
            if (actions != null && actions.Count > 0)
            {
                Pause p = (Pause)actions[0];
                if (travelAngle == 360)
                    additionalSleepAngle = 180 * MT.Max(p.duration - 1, 0);
                else additionalSleepAngle = 180 * p.duration;
            }
            actions = actionDict.GetValueSafe(LevelEventType.PositionTrack);
            if (actions != null && actions.Count > 0)
            {
                PositionTrack pt = (PositionTrack)actions[0];
                if(pt.editorOnly == Toggle.Disabled) 
                {
                    realX += pt.positionOffset.x;
                    realY += pt.positionOffset.y;
                }
                editorX += pt.positionOffset.x;
                editorY += pt.positionOffset.y;
            }
        }
        public static double CalculateTotalPerceivedBpm(List<Tile> tiles) => CalculatePerceivedBpmFromDurationMs(CalculateTotalDurationMs(tiles));
        public static double CalculatePerceivedBpmFromDurationMs(double durationMs) => 60000.0 / durationMs;
        public static double CalculateTotalDurationMs(List<Tile> tiles)
            => tiles.Select(t => t.tileMeta).Select(tm => tm.TravelMs).Sum();
        public static double CalculateTotalTravelAndPlanetAngle(List<Tile> tiles)
            => tiles.Select(t => t.tileMeta).Select(tm => tm.travelAngle + tm.additionalSleepAngle).Sum();
        public static double CalculateTotalTravelAngle(List<Tile> tiles)
            => tiles.Select(t => t.tileMeta).Select(tm => tm.travelAngle).Sum();
        public static double CalculatePerceivedBpm(double bpm, double travelAngle) => 180.0 * bpm / travelAngle;
        public static double CalculateTravelMs(double bpm, double travelAngle) => 60000.0 / CalculatePerceivedBpm(bpm, travelAngle);
        public static double CalculatePlanetAngle(int planets) => 360.0 / planets;
        public double PerceivedBpm => CalculatePerceivedBpm(bpm, travelAngle);
        public double TravelMs => CalculateTravelMs(bpm, travelAngle);
        public double PossibleMaxBpm => bpm * 360 / travelAngle;
        public double PlanetAngle => CalculatePlanetAngle(planets);
    }
}
