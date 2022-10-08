using System.Globalization;

namespace Vostok.Metrics.Helpers
{
    internal static class DoubleSerializer
    {
        public static string Serialize(double value) =>
            value.ToString(CultureInfo.InvariantCulture);

        public static double Deserialize(string input) =>
            double.Parse(input, CultureInfo.InvariantCulture);
    }
}