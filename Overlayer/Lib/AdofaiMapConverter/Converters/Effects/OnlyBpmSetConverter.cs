using System.Linq;

namespace AdofaiMapConverter.Converters.Effects
{
    public static class OnlyBpmSetConverter
    {
        public static CustomLevel Convert(CustomLevel customLevel)
            => MapConverterBase.Convert(customLevel, ae => ae.oneTimingTiles.Select(t => t.Copy()).ToList());
    }
}
