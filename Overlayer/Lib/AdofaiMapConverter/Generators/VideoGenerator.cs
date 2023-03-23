#if OPENCV
using System.Collections.Generic;
using System.Drawing;
using AdofaiMapConverter.Types;
using System;
using AdofaiMapConverter.Actions.Default;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace AdofaiMapConverter.Generators
{
    public static class VideoGenerator
    {
        public static CustomLevel Generate(string videoPath, int width = -1, int height = -1, int secondFrom = -1, int secondTo = -1)
            => Generate(new VideoCapture(videoPath), width, height, secondFrom, secondTo);
        public static CustomLevel Generate(VideoCapture video, int width = -1, int height = -1, int secondFrom = -1, int secondTo = -1)
            => Generate(video, null, null, width, height, secondFrom, secondTo);
        public static CustomLevel Generate(VideoCapture video, IProgress<int> imageProgress = null, IProgress<int> videoProgress = null, int width = -1, int height = -1, int secondFrom = -1, int secondTo = -1)
        {
            if (secondTo < secondFrom)
                return null;
            if (width < 0)
                width = video.FrameWidth;
            if (height < 0)
                height = video.FrameHeight;
            if (secondFrom < 0)
                secondFrom = 0;
            if (secondTo < 0)
                secondTo = video.FrameCount / (int)video.Fps;
            var info = new { frames = video.FrameCount, fps = video.Fps };
            List<Bitmap> images = video.GetFrames(secondFrom, secondTo);
            CustomLevel customLevel = ImageGenerator.Generate(images[0], width, height);
            int imageCount = 0;
            imageCount++;
            videoProgress?.Report(imageCount);
            bool notToResize = width < 0 && height < 0;
            double offsetperframe = 180 / (info.fps * (secondTo - secondFrom));
            double offset = 0;
            customLevel.Tiles[1].AddAction(new MoveCamera() { ease = Ease.Linear, position = new Vector2(width / 1.75, -height / 2.5), duration = .000001, zoom = width * height / 2, relativeTo = CamMovementType.Tile });
            Tile t = customLevel.Tiles[1];
            foreach (Bitmap image_ in images)
            {
                Bitmap image = notToResize ? image_ : image_.Resize(width, height);
                Color lastColor = Color.Empty;
                int count = 0;
                imageProgress?.Report(count);
                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        Color color = image.GetPixel(x, y);
                        if (lastColor != color)
                            t.GetActions(LevelEventType.RecolorTrack).Add(new RecolorTrack() { startTile = (count, TileRelativeTo.ThisTile), endTile = (count, TileRelativeTo.ThisTile), trackColor = color.ToHexA(), angleOffset = offset, trackStyle = TrackStyle.Gems });
                        count++;
                        imageProgress?.Report(count);
                        lastColor = color;
                    }
                }
                offset += offsetperframe;
                imageCount++;
                videoProgress?.Report(imageCount);
            }
            customLevel.Setting.legacySpriteTiles = true;
            customLevel.Setting.countdownTicks = 0;
            customLevel.Setting.relativeTo = CamMovementType.Tile;
            customLevel.Setting.bpm = 60 / (secondTo - secondFrom);
            video.Release();
            return customLevel;
        }
        public static List<Bitmap> GetFrames(this VideoCapture video, int secondFrom = -1, int secondTo = -1)
        {
            List<Bitmap> frames = new List<Bitmap>();
            var fps = (int)video.Fps;
            int minFrames, maxFrames;
            if (secondFrom < 0) minFrames = 0;
            else minFrames = secondFrom * fps;
            if (secondTo < 0) maxFrames = video.FrameCount;
            else maxFrames = Math.Min(secondTo * fps, video.FrameCount);
            video.PosFrames = minFrames;
            for (int i = minFrames; i < maxFrames; i++)
            {
                using (Mat mat = new Mat())
                {
                    if (video.Read(mat))
                        frames.Add(mat.ToBitmap());
                }
            }
            return frames;
        }
    }
}
#endif