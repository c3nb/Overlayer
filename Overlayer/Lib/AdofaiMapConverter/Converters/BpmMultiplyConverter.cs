namespace AdofaiMapConverter.Converters
{
    public static class BpmMultiplyConverter
    {
        public static CustomLevel Convert(CustomLevel customLevel, double multiplier)
            => MapConverterBase.ConvertBasedOnTravelAngle(customLevel, t => t.tileMeta.travelAngle * multiplier, t => { });
    }
}
