using AdofaiMapConverter.Helpers;
using AdofaiMapConverter.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdofaiMapConverter.Converters
{
    public static class PseudoConverter
    {
        public static CustomLevel Convert(CustomLevel customLevel, int pseudos, double pseudoMaxAngle, bool removeColorTrack)
        {
            return MapConverterBase.Convert(customLevel, ae =>
            {
                List<Tile> oneTimingTiles = ae.oneTimingTiles;
                List<Tile> newTiles = oneTimingTiles.Select(t => t.Copy()).ToList();
                double travelAngle = TileMeta.CalculateTotalTravelAngle(oneTimingTiles);
                double eachHitTravelAngle = Math.Min(pseudoMaxAngle, travelAngle / 2 / pseudos);
                TileMeta lastTileMeta = oneTimingTiles[oneTimingTiles.Count - 1].tileMeta;
                double curStaticAngle = lastTileMeta.staticAngle;
                if (oneTimingTiles.Count % 2 == 0)
                    curStaticAngle = AngleHelper.GetNextStaticAngle(curStaticAngle, 0, lastTileMeta.PlanetAngle, lastTileMeta.reversed);
                curStaticAngle = AngleHelper.GetNextStaticAngle(curStaticAngle, eachHitTravelAngle, lastTileMeta.PlanetAngle, lastTileMeta.reversed);
                if (removeColorTrack)
                    newTiles.ForEach(t =>
                    {
                        t.RemoveActions(LevelEventType.ColorTrack);
                        t.RemoveActions(LevelEventType.RecolorTrack);
                    });
                for (int i = 1; i < pseudos; i++)
                {
                    newTiles.Add(new Tile(TileAngle.CreateNormal(curStaticAngle)));
                    newTiles.Add(new Tile(TileAngle.Midspin));
                    curStaticAngle = AngleHelper.GetNextStaticAngle(curStaticAngle, eachHitTravelAngle, lastTileMeta.PlanetAngle, lastTileMeta.reversed);
                    curStaticAngle = AngleHelper.GetNextStaticAngle(curStaticAngle, 0, lastTileMeta.PlanetAngle, lastTileMeta.reversed);
                }
                return newTiles;
            });
        }
    }
}
