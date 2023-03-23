using System.Collections.Generic;

namespace AdofaiMapConverter.Converters
{
    public static class LinearConverter
    {
        public static CustomLevel Convert(CustomLevel customLevel)
            => ShapeConverter.Convert(customLevel, new List<double>() { 0 });
    }
}
