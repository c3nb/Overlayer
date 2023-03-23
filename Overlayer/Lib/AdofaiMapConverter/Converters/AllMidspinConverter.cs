using AdofaiMapConverter.Helpers;
using AdofaiMapConverter.Types;
using System;
using System.Collections.Generic;

namespace AdofaiMapConverter.Converters
{
    public static class AllMidspinConverter
    {
        public static CustomLevel Convert(CustomLevel customLevel, int midspinAmount)
        {
            if (midspinAmount < 0)
                throw new InvalidOperationException("midspinAmount Cannot Be Less Than Zero!");
            double staticAngle = AngleHelper.GetNextStaticAngle(0,
                TileMeta.CalculateTotalTravelAngle(MapConverterBase.GetSameTimingTiles(customLevel.Tiles, 0)),
                customLevel.Tiles[0].tileMeta.PlanetAngle,
                false);
            return MapConverterBase.Convert(customLevel, ae =>
            {
                List<Tile> oneTimingTiles = ae.oneTimingTiles;
                TileMeta lastTileMeta = oneTimingTiles[oneTimingTiles.Count - 1].tileMeta;
                double travelAngle = TileMeta.CalculateTotalTravelAngle(oneTimingTiles);
                List<Tile> newTiles = new List<Tile>
                {
                    new Tile(TileAngle.CreateNormal(staticAngle))
                };
                for (int i = 0; i < midspinAmount; i++)
                {
                    staticAngle = AngleHelper.GetNextStaticAngle(staticAngle, 0, lastTileMeta.PlanetAngle, lastTileMeta.reversed);
                    newTiles.Add(new Tile(TileAngle.Midspin));
                }
                staticAngle = AngleHelper.GetNextStaticAngle(staticAngle, travelAngle, lastTileMeta.PlanetAngle, lastTileMeta.reversed);
                for (int i = 0; i < oneTimingTiles.Count; i++)
                {
                    Tile timingTile = oneTimingTiles[i];
                    newTiles[Math.Min(i, newTiles.Count - 1)].Combine(timingTile);
                }
                return newTiles;
            });
        }
    }
}
