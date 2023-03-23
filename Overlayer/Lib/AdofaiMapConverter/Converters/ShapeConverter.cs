using AdofaiMapConverter.Actions;
using AdofaiMapConverter.Types;
using System.Collections.Generic;
using System.Linq;

namespace AdofaiMapConverter.Converters
{
    public static class ShapeConverter
    {
        public static CustomLevel Convert(CustomLevel customLevel, CustomLevel shapeLevel)
            => Convert(customLevel, shapeLevel, null);
        public static CustomLevel Convert(CustomLevel customLevel, IEnumerable<double> shapeAngles)
            => Convert(customLevel, null, shapeAngles);
        public static CustomLevel Convert(CustomLevel customLevel, params double[] shapeAngles)
        {
            if (shapeAngles.Length <= 0)
                throw new System.InvalidOperationException("shapeAngles Cannot Be 0!");
            return Convert(customLevel, null, shapeAngles);
        }
        private static CustomLevel Convert(CustomLevel customLevel, CustomLevel shapeLevel, IEnumerable<double> shapeAngles)
        {
            List<Tile> shapeTiles;
            if (shapeLevel == null)
            {
                shapeLevel = new CustomLevel();
                var ts = shapeAngles.Select(d => d == 999 ? new Tile(TileAngle.Midspin) : new Tile(TileAngle.CreateNormal(d))).ToList();
                ts.Add(new Tile(TileAngle.Zero));
                shapeLevel.Tiles = ts;
                shapeLevel.MakeTiles();
                shapeLevel.Tiles.RemoveAt(0);
                shapeTiles = shapeLevel.Tiles;
            }
            else shapeTiles = shapeLevel.Tiles;
            int index = 0;
            return MapConverterBase.Convert(customLevel, ae =>
            {
                List<Tile> nowTimingTiles = ae.oneTimingTiles;
                List<Tile> nowShapeTiles = MapConverterBase.GetSameTimingTiles(shapeTiles, index);
                List<Tile> newTiles = nowShapeTiles.Select(t =>
                {
                    Tile newTile = new Tile(t.angle);
                    foreach (var twirl in t.GetActions(LevelEventType.Twirl))
                        _ = newTile.AddAction(twirl);
                    return newTile;
                }).ToList();
                int newTileIdx = 0;
                foreach (Tile tile in nowTimingTiles)
                {
                    Tile newTile = newTiles[newTileIdx];
                    Tile mutableTimingTile = tile.Copy();
                    mutableTimingTile.RemoveActions(LevelEventType.Twirl);
                    newTile.Combine(mutableTimingTile);
                    if (++newTileIdx >= newTiles.Count)
                        newTileIdx--;
                }
                index += nowShapeTiles.Count;
                if (index >= shapeTiles.Count)
                {
                    index = 0;
                    if (shapeTiles[shapeTiles.Count - 1].tileMeta.reversed)
                    {
                        Tile newTile = newTiles[newTiles.Count - 1];
                        List<Action> twirls = newTile.GetActions(LevelEventType.Twirl);
                        if (twirls.Count <= 0)
                            newTile.AddAction(new Twirl());
                        else newTile.RemoveActions(LevelEventType.Twirl);
                    }
                }
                return newTiles;
            });
        }
    }
}
