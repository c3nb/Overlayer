using System;
using System.Collections.Generic;
using System.Linq;
using Overlayer.Core;
using Overlayer.Core.Api.Adofaigg;
using AgLevel = Overlayer.Core.Api.Adofaigg.Types.Level;
using SA.GoogleDoc;
using System.Threading.Tasks;
using Overlayer.Core.Tags;
using Overlayer.Patches;
using ACL = AdofaiMapConverter.CustomLevel;
using System.IO;
using JSON;
using System.Management.Instrumentation;
using Discord;
using AdofaiggApi = Overlayer.Core.Api.Adofaigg.AdofaiggApi;
using OverlayerApi = Overlayer.Core.Api.Overlayer.OverlayerApi;
using Overlayer.Core.Api.Overlayer;
using System.Runtime.ConstrainedExecution;
using Overlayer.Core.Utils;
using ADOFAI;

namespace Overlayer.Tags
{
    public static class Adofaigg
    {
        [FieldTag(Round = true, Category = Category.Adofaigg)]
        public static double IntegratedDifficulty = 0;
        [FieldTag(Round = true, Category = Category.Adofaigg)]
        public static double ForumDifficulty = 0;
        [FieldTag(Round = true, Category = Category.Adofaigg)]
        public static double PredictedDifficulty = 0;
        public static AgLevel Result = null;
        [Tag("PlayPoint", Category = Category.Adofaigg)]
        public static double PlayPoint(int digits = -1)
        {
            double result;
            var edit = scnEditor.instance;
            if (edit)
                result = (double)CalculatePlayPoint(IntegratedDifficulty, edit.levelData.pitch, Misc.XAccuracy(), scrLevelMaker.instance.listFloors.Count);
            else result = (double)CalculatePlayPoint(IntegratedDifficulty, (int)Math.Round(Misc.Pitch() * 100), Misc.XAccuracy(), scrLevelMaker.instance.listFloors.Count);
            return result.Round(digits);
        }
        [Tag("LevelId", Category = Category.Adofaigg)]
        public static double Id() => Result?.id ?? -1;
        public static double CalculatePlayPoint(double difficulty, int speed, double accuracy, int tile)
        {
            if (difficulty < 1) return 0.0;
            double difficultyRating = 1600.0 / (1.0 + Math.Exp(-0.3 * difficulty + 5.5));
            double xAccuracy = accuracy / 100.0;
            double accuracyRating = 0.03 / (-Math.Pow(Math.Min(1, xAccuracy), Math.Pow(tile, 0.05)) + 1.05) + 0.4;
            double pitch = speed / 100.0;
            double pitchRating = pitch >= 1 ? Math.Pow((2 + pitch) / 3.0, Math.Pow(0.1 + Math.Pow(tile, 0.5) / Math.Pow(2000, 0.5), 1.1)) : Math.Pow(pitch, 1.8);
            double tilesRating = tile < 2000 ? 0.9 + tile / 10000.0 : Math.Pow(tile / 2000.0, 0.05);
            return Math.Pow(difficultyRating * accuracyRating * pitchRating * tilesRating, 1.01);
        }
        public static async void Setup(ADOBase aBase)
        {
            LevelData levelData = null;
            string levelPath = null;
            scnGame game = aBase as scnGame;
            if (game != null)
            {
                levelData = game.levelData;
                levelPath = game.levelPath;
            }
            else
            {
                scnEditor editor = aBase as scnEditor;
                if (editor != null)
                {
                    levelData = editor.levelData;
                    levelPath = editor.customLevel.levelPath;
                }
            }
            IntegratedDifficulty = -999;
            PredictedDifficulty = -999;
            ForumDifficulty = -999;
            if (aBase)
            {
                try
                {
                    string artist = levelData.artist.BreakRichTag(), author = levelData.author.BreakRichTag(), title = levelData.song.BreakRichTag();
                    Result = await Request(artist, title, author, string.IsNullOrWhiteSpace(levelData.pathData) ? levelData.angleData.Count : levelData.pathData.Length, (int)Math.Round(levelData.bpm));
                    if (Result == null)
                    {
                        OverlayerDebug.Log($"Requesting Failed. Re-Requesting With Escaping Parameters..");
                        AdofaiggApi.EscapeParameter = true;
                        Result = await Request(artist, title, author, string.IsNullOrWhiteSpace(levelData.pathData) ? levelData.angleData.Count : levelData.pathData.Length, (int)Math.Round(levelData.bpm));
                        AdofaiggApi.EscapeParameter = false;
                    }
                    if (Result != null)
                        ForumDifficulty = Result.difficulty;
                    IntegratedDifficulty = ForumDifficulty < -1 ? PredictedDifficulty : ForumDifficulty;
#if DIFFICULTY_PREDICTOR
                    PredictDiff(levelPath, levelData);
#else
                    PredictedDifficulty = ((double)instance.levelData.difficulty).Map(1, 10, 1, 21);
                    if ((Result?.difficulty).HasValue)
                        IntegratedDifficulty = ForumDifficulty = Result.difficulty;
                    else IntegratedDifficulty = PredictedDifficulty;
#endif
                }
                catch (Exception e)
                {
                    IntegratedDifficulty = PredictedDifficulty = ((double)levelData.difficulty).Map(1, 10, 1, 21);
                    OverlayerDebug.Log($"Error On Requesting Difficulty At Adofai.gg. Check Adofai.gg's Api State.\n{e}");
                }
            }
        }
        public static async Task<AgLevel> Request(string artist, string title, string author, int tiles, int bpm, params Parameter[] ifFailedWith)
        {
            OverlayerDebug.Log($"Requesting {artist} - {title}, {author}</b>");
            var result = await AgLevel.Request(ActualParams(-1));
            if (result.count <= 0)
            {
                OverlayerDebug.Log($"Result Count Is {result.count}. Re-Requesting With Fixup Artist Name..");
                result = await AgLevel.Request(ActualParams(0));
                if (result.count <= 0)
                {
                    OverlayerDebug.Log($"Result Count Is {result.count}. Re-Requesting With Fixup Author Name..");
                    result = await AgLevel.Request(ActualParams(1));
                    if (result.count <= 0)
                    {
                        OverlayerDebug.Log($"Result Count Is {result.count}. Re-Requesting With Fixup Artist And Author Name..");
                        result = await AgLevel.Request(ActualParams(2));
                        if (result.count <= 0)
                            return await Fail();
                        return result.results[0];
                    }
                    return result.results[0];
                }
                return result.results[0];
            }
            if (result.count > 1)
            {
                OverlayerDebug.Log($"Result Count Is {result.count}. Re-Requesting With Bpm..");
                result = await AgLevel.Request(ActualParams(-1, AgLevel.MinBpm(bpm - 1), AgLevel.MaxBpm(bpm + 1)));
            }
            if (result.count <= 0) return await Fail();
            if (result.count > 1)
            {
                OverlayerDebug.Log($"Result Count Is {result.count}. Re-Requesting With Tile Count..");
                result = await AgLevel.Request(ActualParams(-1, AgLevel.MinTiles(tiles - 1), AgLevel.MaxTiles(tiles + 1)));
            }
            if (result.count <= 0) return await Fail();
            if (result.count > 1)
            {
                OverlayerDebug.Log($"Result Count Is {result.count}. Re-Requesting With Bpm And Tile Count..");
                result = await AgLevel.Request(ActualParams(-1, AgLevel.MinBpm(bpm - 1), AgLevel.MaxBpm(bpm + 1), AgLevel.MinTiles(tiles - 1), AgLevel.MaxTiles(tiles + 1)));
            }
            if (result.count <= 0) return await Fail();
            if (result.count > 1)
                OverlayerDebug.Log($"Result Count Is {result.count}. Use First Level.");
            OverlayerDebug.Log("Success");
            return result.results[0];
            Parameters ActualParams(int level = -1, params Parameter[] with)
            {
                List<Parameter> parameters = new List<Parameter>(with.Concat(ifFailedWith));
                string nartist = artist.TrimEx();
                string ntitle = title.TrimEx();
                string nauthor = author.TrimEx();
                if (level != -1)
                {
                    if (level == 0)
                        nartist = nartist.Replace(" ", "");
                    if (level == 1)
                        nauthor = nauthor.Replace(" ", "");
                    if (level == 2)
                    {
                        nartist = nartist.Replace(" ", "");
                        nauthor = nauthor.Replace(" ", "");
                    }
                }
                if (!string.IsNullOrWhiteSpace(nartist) && !IsSameWithDefault("editor.artist", nartist))
                    parameters.Add(AgLevel.QueryArtist(nartist));
                if (!string.IsNullOrWhiteSpace(ntitle) && !IsSameWithDefault("editor.title", ntitle))
                    parameters.Add(AgLevel.QueryTitle(ntitle));
                if (!string.IsNullOrWhiteSpace(nauthor) && !IsSameWithDefault("editor.author", nauthor))
                    parameters.Add(AgLevel.QueryCreator(nauthor));
                return new Parameters(parameters);
            }
            async Task<AgLevel> Fail()
            {
                if (ifFailedWith.Length < 1)
                {
                    OverlayerDebug.Log($"Requesting Failed. Re-Requesting With Censored Level..");
                    return await Request(artist, title, author, tiles, bpm, AgLevel.ShowCensored(true));
                }
                OverlayerDebug.Log("Failed");
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
#if DIFFICULTY_PREDICTOR
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static async void PredictDiff(string levelPath, LevelData levelData)
        {
            try
            {
                PredictedDifficulty = await PredictDifficulty(levelPath, levelData).TryWaitAsync(TimeSpan.FromSeconds(10));
                IntegratedDifficulty = ForumDifficulty < -1 ? PredictedDifficulty : ForumDifficulty;
                var name = string.IsNullOrWhiteSpace(levelData.song) ? Path.GetDirectoryName(levelPath) : levelData.song;
                name = name.RemoveRichTags();
                if (Main.Settings.AllowCollectingLevels)
                    OverlayerApi.Instance.Upload(levelPath, name + (ForumDifficulty < -1 ? "" : "_Adofaigg"), IntegratedDifficulty.ToString());
            }
            catch (Exception e)
            {
                OverlayerDebug.Log($"Error On Predicting Difficulty:\n{e}");
                OverlayerDebug.Log($"Level Path: {levelPath}");
                OverlayerDebug.Log($"Adjusting PredictedDifficulty To Editor Difficulty {PredictedDifficulty = ((double)levelData.difficulty).Map(1, 10, 1, 21)}");
            }
        }
        [ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
        public static async Task<double> PredictDifficulty(string levelPath, LevelData levelData)
        {
            var hash = DataInit.MakeHash(levelData.author, levelData.artist, levelData.song);
            if (PredictedDiffCache.TryGetValue(hash, out var diff)) return diff;
            var meta = await Task.Run(() => GetMeta(levelPath));
            var predicted = await OverlayerApi.Instance.Predict(meta);
            return PredictedDiffCache[hash] = AdjustDiff(Clamp(Math.Round(predicted, 1), 1, 21));
        }
        public static LevelMeta GetMeta(string levelPath)
            => LevelMeta.GetMeta(ACL.Read(JsonNode.Parse(File.ReadAllText(levelPath))));
        public static double AdjustDiff(double diff)
        {
            double result;
            if (diff > 30)
                return 21;
            if (diff <= 20)
                if (diff > 18)
                    result = Math.Round(diff / .5) * .5;
                else result = Math.Round(diff);
            else result = Math.Round(20f + (diff % 20f / 10f), 1);
            return result;
        }
#endif
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
        public static double Clamp(double value, double min, double max)
            => value > max ? max : value < min ? min : value;
    }
}
