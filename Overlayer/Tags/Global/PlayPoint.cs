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
using SA.GoogleDoc;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using UnityEngine;
using System.Runtime.CompilerServices;
using Overlayer.Patches;

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
                var result = Request(artist, title, author, string.IsNullOrWhiteSpace(levelData.pathData) ? levelData.angleData.Count : levelData.pathData.Length, (int)Math.Round(levelData.bpm));
                if (result == null)
                {
                    if (DifficultyPredictor.SupportPredictDifficulty)
                        Main.Logger.Log($"Adjusting Difficulty To Predicted Difficulty {CurrentDifficulty = PredictDifficulty(instance)}..");
                    else
                        Main.Logger.Log($"Adjusting Difficulty To {CurrentDifficulty = levelData.difficulty.Map(1, 10, 1, 21)}..");
                }
                else CurrentDifficulty = result.difficulty;
            }
        }
        public static AgLevel Request(string artist, string title, string author, int tiles, int bpm)
        {
            Main.Logger.Log($"<b>Requesting {artist} - {title}, {author}</b>");
            var result = AgLevel.Request(ActualParams());
            if (result.count <= 0) return Fail();
            if (result.count > 1)
            {
                Main.Logger.Log($"<b>Result Count Is {result.count}. Re-Requesting With Bpm..</b>");
                result = AgLevel.Request(ActualParams(AgLevel.MinBpm(bpm - 1), AgLevel.MaxBpm(bpm + 1)));
            }
            if (result.count <= 0) return Fail();
            if (result.count > 1)
            {
                Main.Logger.Log($"<b>Result Count Is {result.count}. Re-Requesting With Tile Count..</b>");
                result = AgLevel.Request(ActualParams(AgLevel.MinTiles(tiles - 1), AgLevel.MaxTiles(tiles + 1)));
            }
            if (result.count <= 0) return Fail();
            if (result.count > 1)
            {
                Main.Logger.Log($"<b>Result Count Is {result.count}. Re-Requesting With Bpm And Tile Count..</b>");
                result = AgLevel.Request(ActualParams(AgLevel.MinBpm(bpm - 1), AgLevel.MaxBpm(bpm + 1), AgLevel.MinTiles(tiles - 1), AgLevel.MaxTiles(tiles + 1)));
            }
            if (result.count <= 0) return Fail();
            if (result.count > 1)
            {
                Main.Logger.Log($"<b>Result Count Is {result.count}. Use First Level.</b>");
                return result.results[0];
            }
            Main.Logger.Log("Success");
            return result.results[0];
            Parameters ActualParams(params Parameter[] with)
            {
                List<Parameter> parameters = new List<Parameter>(with);
                artist = artist.TrimEx();
                title = title.TrimEx();
                author = author.TrimEx();
                if (!string.IsNullOrWhiteSpace(artist) && !IsSameWithDefault("editor.artist", artist))
                    parameters.Add(AgLevel.QueryArtist(artist));
                if (!string.IsNullOrWhiteSpace(title) && !IsSameWithDefault("editor.title", title))
                    parameters.Add(AgLevel.QueryTitle(title));
                if (!string.IsNullOrWhiteSpace(author) && !IsSameWithDefault("editor.author", author))
                    parameters.Add(AgLevel.QueryCreator(author));
                return new Parameters(parameters);
            }
            AgLevel Fail()
            {
                Main.Logger.Log("Failed");
                return null;
            }
        }
        public static bool IsSameWithDefault(string key, string value)
        {
            if (!CachedState.Contains(key))
            {
                CachedStrings[key] = new string[]
                {
                    Localization.GetLocalizedString(key, LangSection.Translations, LangCode.Korean),
                    Localization.GetLocalizedString(key, LangSection.Translations, LangCode.English),
                    Localization.GetLocalizedString(key, LangSection.Translations, LangCode.Japanese),
                    Localization.GetLocalizedString(key, LangSection.Translations, LangCode.Spanish),
                    Localization.GetLocalizedString(key, LangSection.Translations, LangCode.Portuguese),
                    Localization.GetLocalizedString(key, LangSection.Translations, LangCode.French),
                    Localization.GetLocalizedString(key, LangSection.Translations, LangCode.Polish),
                    Localization.GetLocalizedString(key, LangSection.Translations, LangCode.Romanian),
                    Localization.GetLocalizedString(key, LangSection.Translations, LangCode.Russian),
                    Localization.GetLocalizedString(key, LangSection.Translations, LangCode.Vietnamese),
                    Localization.GetLocalizedString(key, LangSection.Translations, LangCode.Czech),
                    Localization.GetLocalizedString(key, LangSection.Translations, LangCode.German),
                    Localization.GetLocalizedString(key, LangSection.Translations, LangCode.ChineseSimplified),
                    Localization.GetLocalizedString(key, LangSection.Translations, LangCode.ChineseTraditional),
                };
                CachedState.Add(key);
            }
            return CachedStrings[key].Contains(value);
        }
        public static double PredictDifficulty(scnEditor editor)
        {
            var levelData = editor.levelData;
            if (PredictedDiffCache.TryGetValue(DataInit.MakeHash(levelData.author, levelData.artist, levelData.song), out double diff))
                return diff;
            var floors = scrLevelMaker.instance.listFloors;
            int tiles = string.IsNullOrWhiteSpace(levelData.pathData) ? levelData.angleData.Count : levelData.pathData.Length;
            float bpmCache = levelData.bpm;
            var allBpm = floors.Select(f => f.speed * bpmCache);
            var bpmAvg = allBpm.Average();
            var bpmDev = allBpm.Select(b => b - bpmAvg);
            var bpmVariance = bpmDev.Select(d => d * d).Average();
            var bpmStdDev = (float)Math.Sqrt(bpmVariance);
            var predicted =  DifficultyPredictor.Predict(tiles, bpmAvg, bpmVariance, bpmStdDev);
            return PredictedDiffCache[DataInit.MakeHash(levelData.author, levelData.artist, levelData.song)] = Clamp(Math.Round(predicted, 1), 1, 21);
        }
        public static List<string> CachedState = new List<string>();
        public static Dictionary<string, double> PredictedDiffCache = new Dictionary<string, double>();
        public static Dictionary<string, string[]> CachedStrings = new Dictionary<string, string[]>();
        public static string TrimEx(this string s)
        {
            var result = s.BreakRichTag().Trim();
            if (result.Contains("&") || result.Contains(",")) 
                return result.Replace(" ", "");
            return result;
        }
        public static double PowEx(double x, double y)
            => x.Pow(y);
        public static double Clamp(double value, double min, double max)
            => value > max ? max : value < min ? min : value;
    }
    public static class DifficultyPredictor
    {
        public static readonly bool SupportPredictDifficulty = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        public static double Predict(int tiles, double bpmAvg, double bpmVariance, double bpmStdDev)
        {
            if (!SupportPredictDifficulty) return -1;
            Process proc = new Process();
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.UseShellExecute = false;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.CreateNoWindow = true;
            psi.RedirectStandardOutput = true;
            psi.FileName = "Mods/Overlayer/Overlayer.DifficultyPrediction.exe";
            psi.Arguments = $"{tiles} {bpmAvg} {bpmVariance} {bpmStdDev}";
            proc.StartInfo = psi;
            proc.Start();
            return proc.StandardOutput.ReadLine().ToDouble();
        }
    }
}
