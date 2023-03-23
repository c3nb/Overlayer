using AdofaiMapConverter.Types;
using System.Collections.Generic;
using System.Linq;

namespace AdofaiMapConverter.Converters.Effects
{
    public static class NonEffectConverter
    {
        public static CustomLevel Convert(CustomLevel customLevel, params LevelEventType[] toRemoveTypes)
            => Convert(customLevel, (IEnumerable<LevelEventType>)toRemoveTypes);
        public static CustomLevel Convert(CustomLevel customLevel, IEnumerable<LevelEventType> toRemoveTypes)
        {
            return MapConverterBase.Convert(customLevel,
                ae => ae.oneTimingTiles.Select(t => t.Copy())
                    .ForEach(t => toRemoveTypes
                    .ForEach(let => t.RemoveActions(let)))
                    .ToList(),
                level =>
                {
                    Tile firstTile = level.Tiles[0];
                    toRemoveTypes.ForEach(let => firstTile.RemoveActions(let));
                });
        }
    }
}
