using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdofaiMapConverter.Types;
using AdofaiMapConverter.Actions;

namespace AdofaiMapConverter.Converters.Effects
{
    public static class TransparentConverter
    {
        public static CustomLevel Convert(CustomLevel customLevel, int opacity)
        {
            return MapConverterBase.Convert(customLevel, 
                ae => ae.oneTimingTiles.Select(t => t.Copy()).ForEach(t => EditOpacityEvents(t, opacity)).ToList(),
                level =>
                {
                    Tile firstTile = level.Tiles[0];
                    EditOpacityEvents(firstTile, opacity);
                    firstTile.AddAction(GetTransparentMoveTrack(opacity));
                });
        }
        public static MoveTrack GetTransparentMoveTrack(int opacity)
        {
            MoveTrack mt = new MoveTrack();
            mt.startTile = (0, TileRelativeTo.Start);
            mt.endTile = (0, TileRelativeTo.End);
            mt.duration = 0;
            mt.opacity = opacity;
            mt.eventTag = "";
            return mt;
        }
        public static void EditOpacityEvents(Tile tile, int opacity)
        {
            tile.RemoveActions(LevelEventType.AnimateTrack);
            tile.EditActions(LevelEventType.MoveTrack, (MoveTrack mt) => mt.opacity = mt.opacity == 0 ? 0 : opacity);
        }
    }
}
