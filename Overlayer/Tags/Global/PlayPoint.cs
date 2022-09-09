using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagLib.Tags;
using HarmonyLib;
using Overlayer.AdofaiggApi;
using AgLevel = Overlayer.AdofaiggApi.Types.Level;
using System.Reflection;
using ADOFAI;
using TagLib.Utils;

namespace Overlayer.Tags.Global
{
    [ClassTag("PlayPoint", "PlayPoint(PP) In Adofai.gg")]
    public static class PlayPoint
    {
        public static double CurrentDifficulty = 0;
        [Tag]
        public static float PlayPointValue(float digits = -1)
        {
            float result;
            var edit = scnEditor.instance;
            if (edit)
                result = (float)CalculatePlayPoint(CurrentDifficulty, edit.levelData.pitch, Misc.XAccuracy(), scrLevelMaker.instance.listFloors.Count);
            else result = (float)CalculatePlayPoint(CurrentDifficulty, (int)Math.Round(Misc.Pitch() * 100), Misc.XAccuracy(), scrLevelMaker.instance.listFloors.Count);
            return result.Round(digits);
        }
        public static double CalculatePlayPoint(double difficulty, int speed, double accuracy, int tile)
        {
            if (difficulty < 1) return 0.0;
            double difficultyRating = 1600.0 / (1.0 + Math.Exp(-0.3 * difficulty + 5.5));
            double xAccuracy = accuracy / 100.0;
            double accuracyRating = 0.03 / (-PowEx(Math.Min(1, xAccuracy), PowEx(tile, 0.05)) + 1.05) + 0.4;
            double pitch = speed / 100.0;
            double pitchRating = pitch >= 1 ? PowEx((2 + pitch) / 3.0, PowEx(0.1 + PowEx(tile, 0.5) / PowEx(2000, 0.5), 1.1)) : PowEx(pitch, 1.8);
            double tilesRating = tile < 2000 ? 0.9 + tile / 10000.0 : PowEx(tile / 2000.0, 0.05);
            return PowEx(difficultyRating * accuracyRating * pitchRating * tilesRating, 1.01);
        }
        public static void Setup(scnEditor instance)
        {
            if (!instance) CurrentDifficulty = 0;
            else
            {
                var levelData = instance.levelData;
                string artist = levelData.artist.BreakRichTag(), author = levelData.author.BreakRichTag(), title = levelData.song.BreakRichTag();
                Main.Logger.Log($"<b>Requesting {artist} - {title}, {author}</b>");
                var result = Request(artist, title, author);
                if (result == null)
                {
                    CurrentDifficulty = 0;
                    Main.Logger.Log("Failed");
                    Main.Logger.Log($"Adjustmenting Difficulty To {CurrentDifficulty = levelData.difficulty.Map(1, 10, 1, 21)}..");
                }
                else
                {
                    CurrentDifficulty = result.difficulty;
                    Main.Logger.Log("Success");
                }
            }
        }
        public static AgLevel Request(string artist, string title, string author)
        {
            artist = artist.TrimEx();
            title = title.TrimEx();
            author = author.TrimEx();
            var result = AgLevel.Request(AgLevel.QueryArtist(artist), AgLevel.QueryCreator(author), AgLevel.QueryTitle(title));
            if (result.count <= 0) return null;
            return result.results[0];
        }
        public static string TrimEx(this string s)
        {
            var result = s.BreakRichTag().Trim();
            if (result.Contains("&") || result.Contains(",")) 
                return result.Replace(" ", "");
            return result;
        }
        public static double PowEx(double x, double y)
            => x.Pow(y);
    }
}
