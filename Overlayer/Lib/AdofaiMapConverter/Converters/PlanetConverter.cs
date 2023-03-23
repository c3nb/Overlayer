using AdofaiMapConverter.Actions;
using AdofaiMapConverter.Types;
using System.Linq;

namespace AdofaiMapConverter.Converters
{
    public static class PlanetConverter
    {
        public static CustomLevel Convert(CustomLevel customLevel, int planets = 2, bool keepShape = true)
        {
            bool haveToAddEvent = true;
            if (keepShape)
            {
                return MapConverterBase.Convert(customLevel, ae =>
                {
                    return ae.oneTimingTiles.Select(t => t.Copy()).Select(t =>
                    {
                        if (haveToAddEvent)
                        {
                            if (planets > 2)
                                t.AddAction(new MultiPlanet() { planets = planets });
                            haveToAddEvent = false;
                        }
                        return t;
                    }).ToList();
                });
            }
            return MapConverterBase.ConvertBasedOnTravelAngle(customLevel, tile => tile.tileMeta.travelAngle, tile =>
            {
                tile.RemoveActions(LevelEventType.MultiPlanet);
                if (haveToAddEvent)
                {
                    if (planets > 2)
                        tile.AddAction(new MultiPlanet() { planets = planets });
                    haveToAddEvent = false;
                }
            });
        }
    }
}
