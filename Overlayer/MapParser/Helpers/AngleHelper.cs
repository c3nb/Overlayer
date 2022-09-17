using System;
using System.Collections.Generic;
using System.Linq;
using JSON;
using System.Text;
using System.Threading.Tasks;
using Overlayer.MapParser.Types;

namespace Overlayer.MapParser.Helpers
{
    public static class AngleHelper
    {
        public class Result
        {
            public double curStaticAngle;
            public double curTravelAngle;
            public Result(double curStaticAngle, double curTravelAngle)
            {
                this.curStaticAngle = curStaticAngle;
                this.curTravelAngle = curTravelAngle;
            }
        }
        internal static readonly Dictionary<char, (double, bool)> pathMeta = new Dictionary<char, (double, bool)>()
        {
            { 'R', (0, false) },
            { 'p', (15, false) },
            { 'J', (30, false) },
            { 'E', (45, false) },
            { 'T', (60, false) },
            { 'o', (75, false) },
            { 'U', (90, false) },
            { 'q', (105, false) },
            { 'G', (120, false) },
            { 'Q', (135, false) },
            { 'H', (150, false) },
            { 'W', (165, false) },
            { 'L', (180, false) },
            { 'x', (195, false) },
            { 'N', (210, false) },
            { 'Z', (225, false) },
            { 'F', (240, false) },
            { 'V', (255, false) },
            { 'D', (270, false) },
            { 'Y', (285, false) },
            { 'B', (300, false) },
            { 'C', (315, false) },
            { 'M', (330, false) },
            { 'A', (345, false) },
            { '5', (108, true) },
            { '6', (252, true) },
            { '7', (900.0 / 7.0, true) },
            { '8', (360 - 900.0 / 7.0, true) },
            { 't', (60, true) },
            { 'h', (120, true) },
            { 'j', (240, true) },
            { 'y', (300, true) },
            { '!', (999, true) },
        };
        internal static readonly List<(char c, double a, bool r)> pdMeta = new List<(char c, double a, bool r)>()
        {
            ('R', 0, false),
            ('p', 15, false),
            ('J', 30, false),
            ('E', 45, false),
            ('T', 60, false),
            ('o', 75, false),
            ('U', 90, false),
            ('q', 105, false),
            ('G', 120, false),
            ('Q', 135, false),
            ('H', 150, false),
            ('W', 165, false),
            ('L', 180, false),
            ('x', 195, false),
            ('N', 210, false),
            ('Z', 225, false),
            ('F', 240, false),
            ('V', 255, false),
            ('D', 270, false),
            ('Y', 285, false),
            ('B', 300, false),
            ('C', 315, false),
            ('M', 330, false),
            ('A', 345, false),
            ('5', 108, true),
            ('6', 252, true),
            ('7', 128.571428571429, true),
            ('8', 231.428571428571, true),
            ('t', 60, true),
            ('h', 120, true),
            ('j', 240, true),
            ('y', 300, true),
            ('!', 999, true),
        };
        public static List<TileAngle> ReadPathData(string pathData)
        {
            double staticAngle = 0;
            List<TileAngle> result = new List<TileAngle>();
            foreach (char c in pathData)
            {
                var tuple = pathMeta[c];
                if (tuple.Item1 == 999)
                    result.Add(TileAngle.Midspin);
                else
                {
                    if (tuple.Item2)
                        staticAngle = GeneralizeAngle(staticAngle + 180 - tuple.Item1);
                    else staticAngle = tuple.Item1;
                    result.Add(TileAngle.CreateNormal(staticAngle));
                }
            }
            return result;
        }
        public static List<TileAngle> ReadAngleData(IEnumerable<double> angles)
            => angles.Select(d => d == 999 ? TileAngle.Midspin : TileAngle.CreateNormal(d)).ToList();
        public static char GetCharFromAngle(double curAngle, double nextAngle)
        {
            if (curAngle == 999) return '!';
            double relativeAngle = GeneralizeAngle(180 - nextAngle + curAngle);
            foreach (var (c, a, r) in pdMeta)
                if (a == (r ? relativeAngle : curAngle))
                    return c;
            return char.MinValue;
        }
        public static Result CalculateAngleData(double prevStaticAngle, TileAngle curAngle, TileAngle nextAngle, double planetAngle, bool reversed)
        {
            double curStaticAngle = curAngle.isMidspin ? prevStaticAngle : curAngle.Angle;
            double curTravelAngle;
            if (nextAngle.isMidspin)
            {
                curTravelAngle = 0;
                if (curAngle.isMidspin)
                {
                    curStaticAngle += planetAngle;
                    curStaticAngle = curStaticAngle.GeneralizeAngle();
                }
            }
            else
            {
                curTravelAngle = curStaticAngle - nextAngle.Angle;
                if (reversed)
                    curTravelAngle = -curTravelAngle;
                if (!curAngle.isMidspin)
                    curTravelAngle += planetAngle;
                curTravelAngle = curTravelAngle.GeneralizeAngle();
                curTravelAngle = curTravelAngle == 0 ? 360 : curTravelAngle;
            }
            return new Result(curStaticAngle, curTravelAngle);
        }
        public static double GetNextStaticAngle(double staticAngle, double travelAngle, double planetAngle, bool reversed)
        {
            if (travelAngle == 0)
            {
                if (reversed)
                    return GeneralizeAngle(staticAngle + planetAngle);
                else return GeneralizeAngle(staticAngle - planetAngle);
            }
            if (reversed)
                return GeneralizeAngle(staticAngle + (travelAngle - planetAngle));
            else return GeneralizeAngle(staticAngle - (travelAngle - planetAngle));
        }
        public static double GeneralizeAngle(this double angle)
            => ((angle % 360) + 360) % 360;
    }
}
