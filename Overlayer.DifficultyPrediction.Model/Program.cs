using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AdofaiMapConverter;
using JSON;
using AdofaiMapConverter.Types;
using AdofaiMapConverter.Actions;

namespace Overlayer.DifficultyPrediction.Model
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            StringBuilder csv = new StringBuilder();
            csv.AppendLine("타일 수,평균 BPM,BPM 분산,BPM 표준편차,난이도");
            try
            {
                Logger.Log("Reading Files..");
                List<string> list = new List<string>();
                var dirs = Directory.GetDirectories(@"E:\aufb");
                var nestedDirs = dirs.SelectMany(d => Directory.GetDirectories(d));
                var files = nestedDirs.SelectMany(d => Directory.GetFiles(d));
                var adofais = files.Select(f => new FileInfo(f)).Where(f => f.Extension == ".adofai" && !f.Name.Contains("backup"));
                foreach (var adofai in adofais)
                {
                    string fullLevel = File.ReadAllText(adofai.FullName);
                    Logger.Log($"Try To Read {adofai.Name}..");
                    CustomLevel level = CustomLevel.Read(JsonNode.Parse(fullLevel));
                    var lSetting = level.Setting;
                    var diff = 22.0;
                    var id = $"{lSetting.song} - {lSetting.artist}, {lSetting.author}";
                    if (list.Contains(id))
                    {
                        Logger.Log($"Duplicate ID! ({id})");
                        continue;
                    }
                    list.Add(id);
                    Logger.Log($"Try To Get Difficulty {id}");
                    if (TryGetLevel(lSetting.song, lSetting.author, lSetting.artist, level.Tiles.Count, (int)Math.Round(lSetting.bpm), out Level lev))
                    {
                        Logger.Log($"Request Successfully. ({id})");
                        diff = lev.difficulty;
                    }
                    else
                    {
                        Logger.Log($"Request Failed. ({id})");
                        continue;
                    }
                    var meta = level.GetMeta();
                    Console.WriteLine(meta);
                    csv.AppendLine($"{meta},{diff}");
                }
            }
            catch (Exception e)
            {
                Logger.Log(e);
            }
            finally
            {
                Logger.Final();
                File.WriteAllText("data.csv", csv.ToString());
            }
        }
        public static Level Request(string artist, string title, string author, int tiles, int bpm)
        {
            Logger.Log($"<b>Requesting {artist} - {title}, {author}</b>");
            var result = Level.Request(ActualParams());
            if (result.count <= 0) return Fail();
            if (result.count > 1)
            {
                Logger.Log($"<b>Result Count Is {result.count}. Re-Requesting With Bpm..</b>");
                result = Level.Request(ActualParams(Level.MinBpm(bpm - 1), Level.MaxBpm(bpm + 1)));
            }
            if (result.count <= 0) return Fail();
            if (result.count > 1)
            {
                Logger.Log($"<b>Result Count Is {result.count}. Re-Requesting With Tile Count..</b>");
                result = Level.Request(ActualParams(Level.MinTiles(tiles - 1), Level.MaxTiles(tiles + 1)));
            }
            if (result.count <= 0) return Fail();
            if (result.count > 1)
            {
                Logger.Log($"<b>Result Count Is {result.count}. Re-Requesting With Bpm And Tile Count..</b>");
                result = Level.Request(ActualParams(Level.MinBpm(bpm - 1), Level.MaxBpm(bpm + 1), Level.MinTiles(tiles - 1), Level.MaxTiles(tiles + 1)));
            }
            if (result.count <= 0) return Fail();
            if (result.count > 1)
            {
                Logger.Log($"<b>Result Count Is {result.count}. Use First Level.</b>");
                return result.results[0];
            }
            Logger.Log("Success");
            return result.results[0];
            Parameters ActualParams(params Parameter[] with)
            {
                List<Parameter> parameters = new List<Parameter>(with);
                artist = artist.TrimEx();
                title = title.TrimEx();
                author = author.TrimEx();
                if (!string.IsNullOrWhiteSpace(artist) && artist == "작곡가")
                    parameters.Add(Level.QueryArtist(artist));
                if (!string.IsNullOrWhiteSpace(title) && title == "곡이름")
                    parameters.Add(Level.QueryTitle(title));
                if (!string.IsNullOrWhiteSpace(author) && author == "만든이")
                    parameters.Add(Level.QueryCreator(author));
                return new Parameters(parameters);
            }
            Level Fail()
            {
                Logger.Log("Failed");
                return null;
            }
        }
        public static string TrimEx(this string s)
        {
            var result = s.BreakRichTag().Trim();
            if (result.Contains("&") || result.Contains(","))
                return result.Replace(" ", "");
            return result;
        }
        public static string BreakRichTag(this string s)
            => RichTagBreaker.Replace(s, string.Empty);
        public static readonly Regex RichTagBreaker = new Regex(@"<(color|material|quad|size)=(.|\n)*?>|<\/(color|material|quad|size)>|<(b|i)>|<\/(b|i)>", RegexOptions.Compiled | RegexOptions.Multiline);
        public static bool TryGetLevel(string name, string author, string artist, int tiles, int bpm, out Level level)
        {
            //name = richTextBreaker.Replace(name, "");
            try
            {
                level = Request(artist, name, author, tiles, bpm);
                //var resp = Level.Request(Level.QueryTitle(name), Level.QueryCreator(author), Level.QueryArtist(artist));
                //if (resp.count <= 0)
                //{
                //    level = null;
                //    return false;
                //}
                //level = resp.results[0];
                return level != null;
            }
            catch (Exception e)
            {
                Logger.Log($"Error At Requesting {name} - {artist}, {author}. ({e})");
                level = null;
                return false;
            }
        }
        public static LevelMeta GetMeta(this CustomLevel level)
        {
            var tiles = level.Tiles;
            var allBpm = tiles.Select(t => t.tileMeta.bpm);
            var bpmAvg = allBpm.Average();
            var bpmVariance = allBpm.Select(b => Math.Pow(b - bpmAvg, 2)).Average();
            return new LevelMeta(tiles.Count, bpmAvg, bpmVariance, Math.Sqrt(bpmVariance));
        }
    }
    public record LevelMeta(int tileCount, double bpmAverage, double bpmVariance, double bpmStdDeviation)
    {
        public override string ToString() => $"{tileCount},{bpmAverage},{bpmVariance},{bpmStdDeviation}";
}
public static class Logger
{
    public static StringBuilder sb = new StringBuilder();
    public static void Log(this object obj)
    {
        //Console.WriteLine(obj);
        sb.AppendLine(obj.ToString());
    }
    public static void Final()
    {
        File.WriteAllText("log.txt", sb.ToString());
    }
}
    }
}
