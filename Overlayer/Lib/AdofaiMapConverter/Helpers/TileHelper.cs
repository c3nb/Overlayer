using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdofaiMapConverter.Types;
using AdofaiMapConverter.Actions;

namespace AdofaiMapConverter.Helpers
{
    public static class TileHelper
    {
        public static void CombineTile(Tile destTile, Tile srcTile)
        {
            foreach (var kvp in srcTile.actions)
            {
                List<Action> destList = destTile.actions.GetValueSafeOrAdd(kvp.Key, new List<Action>()).ToList();
                destList.AddRange(kvp.Value);
                if (kvp.Key.IsSingleOnly())
                {
                    if (kvp.Key == LevelEventType.Twirl)
                    {
                        bool hasTwirl = destList.Count % 2 == 1;
                        destList.Clear();
                        if (hasTwirl)
                            destList.Add(new Twirl());
                    }
                    else
                        while (destList.Count > 1)
                            destList.RemoveAt(0);
                }

            }
        }
        public static List<Tile> GetSameTimingTiles(List<Tile> tiles, int fromIndex)
        {
            List<Tile> sameTimingTiles = new List<Tile>();
            Tile tile = tiles[fromIndex];
            sameTimingTiles.Add(tile);
            for (int i = fromIndex + 1; i < tiles.Count && tile.tileMeta.travelAngle == 0; i++)
            {
                tile = tiles[i];
                sameTimingTiles.Add(tile);
            }
            return sameTimingTiles;
        }
        public static double CalculateZeroTileTravelMs(CustomLevel customLevel)
        {
            double zeroTileTravelAngle = TileMeta.CalculateTotalTravelAngle(
                GetSameTimingTiles(customLevel.Tiles, 0));
            int straightAngleOffset = customLevel.Setting.offset;
            double additionalOffset = 60000.0 / (customLevel.Setting.bpm * (180 / (zeroTileTravelAngle - 180)));
            return straightAngleOffset + additionalOffset;
        }
    }
}
