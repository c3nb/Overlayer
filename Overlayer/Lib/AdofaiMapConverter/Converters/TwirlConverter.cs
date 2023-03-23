using AdofaiMapConverter.Actions;
using AdofaiMapConverter.Helpers;
using AdofaiMapConverter.Types;

namespace AdofaiMapConverter.Converters
{
    public static class TwirlConverter
    {
        public static CustomLevel Convert(CustomLevel customLevel, double twirlRate = 1)
        {
            return MapConverterBase.ConvertBasedOnTravelAngle(customLevel, t => t.tileMeta.travelAngle, tile =>
            {
                tile.RemoveActions(LevelEventType.Twirl);
                if (twirlRate > RandomHelper.random.NextDouble())
                    tile.AddAction(new Twirl());
            });
        }
    }
}
