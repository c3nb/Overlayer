using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdofaiMapConverter.Types;
using AdofaiMapConverter.Actions;

namespace AdofaiMapConverter.Helpers
{
    public static class LETHelper
    {
        private static readonly Dictionary<LevelEventType, bool> isSingleonly = new Dictionary<LevelEventType, bool>()
        {
            { LevelEventType.None, false },
            { LevelEventType.SetSpeed, true },
            { LevelEventType.Twirl, true },
            { LevelEventType.Checkpoint, true },
            { LevelEventType.MoveCamera, false },
            { LevelEventType.CustomBackground, false },
            { LevelEventType.ChangeTrack, false },
            { LevelEventType.ColorTrack, true },
            { LevelEventType.AnimateTrack, false },
            { LevelEventType.RecolorTrack, false },
            { LevelEventType.MoveTrack, false },
            { LevelEventType.AddDecoration, false },
            { LevelEventType.AddText, false },
            { LevelEventType.SetText, false },
            { LevelEventType.Flash, false },
            { LevelEventType.SetHitsound, true },
            { LevelEventType.SetFilter, false },
            { LevelEventType.SetPlanetRotation, true },
            { LevelEventType.HallOfMirrors, false },
            { LevelEventType.ShakeScreen, false },
            { LevelEventType.MoveDecorations, false },
            { LevelEventType.PositionTrack, true },
            { LevelEventType.RepeatEvents, true },
            { LevelEventType.Bloom, false },
            { LevelEventType.Hold, true },
            { LevelEventType.SetHoldSound, true },
            { LevelEventType.SetConditionalEvents, true },
            { LevelEventType.ScreenTile, false },
            { LevelEventType.ScreenScroll, false },
            { LevelEventType.EditorComment, false },
            { LevelEventType.Bookmark, true },
            { LevelEventType.CallMethod, false },
            { LevelEventType.AddComponent, false },
            { LevelEventType.PlaySound, false },
            { LevelEventType.MultiPlanet, true },
            { LevelEventType.FreeRoam, true },
            { LevelEventType.FreeRoamTwirl, false },
            { LevelEventType.FreeRoamRemove, false },
            { LevelEventType.FreeRoamWarning, false },
            { LevelEventType.Pause, true },
            { LevelEventType.AutoPlayTiles, true },
            { LevelEventType.Hide, true },
            { LevelEventType.ScaleMargin, true },
            { LevelEventType.ScaleRadius, true },
            { LevelEventType.Multitap, false },
            { LevelEventType.TileDimensions, false },
            { LevelEventType.KillPlayer, false },
        };
        public static bool IsSingleOnly(this LevelEventType let)
            => isSingleonly[let];
    }
}
