using AdofaiMapConverter.Actions;
using AdofaiMapConverter.Types;
using System;

namespace AdofaiMapConverter.Converters
{
    public static class NoSpeedChangeConverter
    {
        public static CustomLevel Convert(CustomLevel customLevel, double destBpm)
        {
            double restTravelAngle = 0;
            return MapConverterBase.ConvertBasedOnTravelAngle(customLevel, tile =>
            {
                double travelAngle = tile.tileMeta.travelAngle * destBpm / tile.tileMeta.bpm;
                restTravelAngle = Math.Max(travelAngle - 360, 0);
                return Math.Min(travelAngle, 360);
            },
            tile =>
            {
                tile.RemoveActions(LevelEventType.SetSpeed);
                if (restTravelAngle > 0)
                    _ = tile.AddAction(new Pause() { countdownTicks = 0, duration = restTravelAngle / 180 + 1, angleCorrectionDir = 0 });
            }, level => level.Setting.bpm = destBpm);
        }
    }
}
