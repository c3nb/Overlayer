using Overlayer.MapParser.Helpers;
using System;

namespace Overlayer.MapParser.Types
{
    public class TileAngle
    {
        public TileAngle(double angle)
            => this.angle = angle;
        public static readonly TileAngle Midspin = new TileAngle(0) { isMidspin = true };
        public static readonly TileAngle Zero = new TileAngle(0);
        public static TileAngle CreateNormal(double angle) => new TileAngle(angle.GeneralizeAngle());
        public bool isMidspin;
        private double angle;
        public double Angle 
        { 
            get
            {
                if (isMidspin)
                    throw new InvalidOperationException("This Angle Is Midspin!");
                return angle;
            }
            set => angle = value; 
        }

        public static explicit operator TileAngle(double angle) => new TileAngle(angle);
        public static explicit operator double(TileAngle angle) => angle.Angle;
    }
}
