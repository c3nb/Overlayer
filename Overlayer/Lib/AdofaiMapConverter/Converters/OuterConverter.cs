using AdofaiMapConverter.Actions;
using AdofaiMapConverter.Types;
using System.Collections.Generic;
using System.Linq;

namespace AdofaiMapConverter.Converters
{
    public static class OuterConverter
    {
        public static CustomLevel Convert(CustomLevel customLevel)
        {
            return MapConverterBase.Convert(customLevel, ae =>
            {
                List<Tile> oneTimingTiles = ae.oneTimingTiles;
                List<Tile> newTiles = oneTimingTiles.Select(t => t.Copy()).ToList();
                if (ae.floor == 1)
                {
                    Tile firstTile = newTiles[0];
                    if (!firstTile.GetActions(LevelEventType.Twirl).Any())
                        _ = firstTile.AddAction(new Twirl());
                    else firstTile.RemoveActions(LevelEventType.Twirl);
                }
                return newTiles;
            });
        }
    }
}
