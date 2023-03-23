using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using AdofaiMapConverter.Types;
using AdofaiMapConverter.Actions.Default;

namespace AdofaiMapConverter.Generators
{
    public static class ImageGenerator
    {
        public static CustomLevel Generate(Bitmap image, int width = -1, int height = -1)
            => Generate(image, null, width, height);
        public static CustomLevel Generate(Bitmap image, IProgress<int> progress, int width = -1, int height = -1)
        {
            CustomLevel customLevel = new CustomLevel();
            customLevel.Setting.song = "";
            customLevel.Setting.artist = "";
            if (width < 0)
                width = image.Width;
            if (height < 0)
                height = image.Height;
            image = image.Resize(width, height);
            int iwidth = image.Width;
            int iheight = image.Height;
            Color lastColor = Color.Empty;
            int ycache = 0;
            int count = 0;
            progress?.Report(count++);
            for (int y = 0; y < iheight; y++)
            {
                for (int x = 0; x < iwidth; x++)
                {
                    Color color = image.GetPixel(x, y);
                    Tile tile = new Tile();
                    if (y != ycache)
                    {
                        var pts = tile.GetActions(LevelEventType.PositionTrack);
                        pts.Add(new PositionTrack() { positionOffset = new Vector2(-width, -0.75), editorOnly = Toggle.Disabled });
                        ycache = y;
                    }
                    var cts = tile.GetActions(LevelEventType.ColorTrack);
                    if (color != lastColor)
                        tile.GetActions(LevelEventType.ColorTrack).Add(new ColorTrack() { trackColor = color.ToHexA() });
                    if (cts.Count > 0)
                        ((ColorTrack)cts[0]).trackStyle = TrackStyle.Gems;
                    customLevel.Tiles.Add(tile);
                    lastColor = color;
                    count++;
                    progress?.Report(count);
                }
            }
            customLevel.Setting.trackStyle = TrackStyle.Gems;
            return customLevel;
        }
        public static Bitmap Crop(this Bitmap image, int width, int height)
        {
            int iheight = image.Height;
            int iwidth = image.Width;
            if (iheight * width < iwidth * height)
            {
                // cut left / right
                double newWidth = iheight * width / height;
                return image.Clone(new Rectangle((int)(iwidth - newWidth) / 2, 0, (int)newWidth, iheight), image.PixelFormat);
            }
            else
            {
                // cut top / bottom
                double newHeight = iwidth * height / width;
                return image.Clone(new Rectangle(0, (int)(iheight - newHeight) / 2, iwidth, (int)newHeight), image.PixelFormat);
            }
        }
        public static Bitmap Resize(this Bitmap image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);
            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);
            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }
            return destImage;
        }
        public static string ToHex(this Color c) => $"{c.R:X2}{c.G:X2}{c.B:X2}";
        public static string ToHexA(this Color c) => $"{c.R:X2}{c.G:X2}{c.B:X2}{c.A:X2}";
        public static string ToRGB(this Color c) => $"RGB({c.R},{c.G},{c.B})";
        public static string ToRGBA(this Color c) => $"RGB({c.R},{c.G},{c.B},{c.A})";
    }
}
