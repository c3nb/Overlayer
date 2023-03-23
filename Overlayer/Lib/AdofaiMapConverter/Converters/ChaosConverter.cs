using AdofaiMapConverter.Helpers;

namespace AdofaiMapConverter.Converters
{
    public static class ChaosConverter
    {
        public static CustomLevel Convert(CustomLevel customLevel, double vibrationRate = 1)
        {
            double levelBaseBpm = customLevel.Tiles[0].tileMeta.PossibleMaxBpm * RandomHelper.MinMax(0.000001, 1);
            return MapConverterBase.ConvertBasedOnTravelAngle(customLevel, t =>
            {
                double travelAngle = t.tileMeta.travelAngle;
                return travelAngle == 0 ? 0 : travelAngle + RandomHelper.MinMax(-vibrationRate, vibrationRate) * travelAngle;
            }, t => { }, level => level.Setting.bpm = levelBaseBpm);
        }
    }
}
