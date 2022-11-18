using System;
using System.Net;
using System.Text;
using System.Linq;
using Overlayer.MapParser.Types;
using System.Collections.Generic;
using ACL = Overlayer.MapParser.CustomLevel;


namespace Overlayer
{
    public record LevelMeta(int tileCount, float twirlRatio, float setSpeedRatio,
        float minTA, float maxTA, float taAverage, float taVariance, float taStdDeviation,
        float minSA, float maxSA, float saAverage, float saVariance, float saStdDeviation,
        float minMs, float maxMs, float msAverage, float msVariance, float msStdDeviation,
        float minBpm, float maxBpm, float bpmAverage, float bpmVariance, float bpmStdDeviation)
    {
        static readonly WebClient client = new WebClient() { Encoding = Encoding.UTF8 };
        public override string ToString() => $"{tileCount},{twirlRatio},{setSpeedRatio},"
                    + $"{minTA},{maxTA},{taAverage},{taVariance},{taStdDeviation},"
                    + $"{minSA},{maxSA},{saAverage},{saVariance},{saStdDeviation},"
                    + $"{minMs},{maxMs},{msAverage},{msVariance},{msStdDeviation},"
                    + $"{minBpm},{maxBpm},{bpmAverage},{bpmVariance},{bpmStdDeviation}";
        public string RequestUrl { get; } = $"http://152.69.230.61:9200/?tileCount={tileCount}&twirlRatio={twirlRatio}&setSpeedRatio={setSpeedRatio}&minTA={minTA}&maxTA={maxTA}&taAverage={taAverage}&taVariance={taVariance}&taStdDeviation={taStdDeviation}&minSA={minSA}&maxSA={maxSA}&saAverage={saAverage}&saVariance={saVariance}&saStdDeviation={saStdDeviation}&minMs={minMs}&maxMs={maxMs}&msAverage={msAverage}&msVariance={msVariance}&msStdDeviation={msStdDeviation}&minBpm={minBpm}&maxBpm={maxBpm}&bpmAverage={bpmAverage}&bpmVariance={bpmVariance}&bpmStdDeviation={bpmStdDeviation}".Replace("+", "").Replace("∞", "-1");
        public string Difficulty => client.DownloadString(RequestUrl);
        public static LevelMeta GetMeta(ACL level)
        {
            var tiles = level.Tiles;

            var twirlRatio = GetRatio(tiles, t => t.GetActions(LevelEventType.Twirl).Any());
            var setSpeedRatio = GetRatio(tiles, t => t.GetActions(LevelEventType.SetSpeed).Any());

            var allTA = tiles.Select(t => (float)t.tileMeta.travelAngle);
            var minTA = allTA.Min();
            var maxTA = allTA.Max();
            var taAvg = allTA.Average();
            var taVariance = allTA.Select(m => (float)Math.Pow(m - taAvg, 2)).Average();
            var taStdDev = (float)Math.Sqrt(taVariance);

            var allSA = tiles.Select(t => (float)t.tileMeta.staticAngle);
            var minSA = allSA.Min();
            var maxSA = allSA.Max();
            var saAvg = allSA.Average();
            var saVariance = allSA.Select(m => (float)Math.Pow(m - saAvg, 2)).Average();
            var saStdDev = (float)Math.Sqrt(saVariance);

            var allMs = tiles.Select(t => (float)t.tileMeta.TravelMs);
            var minMs = allMs.Min();
            var maxMs = allMs.Max();
            var msAvg = allMs.Average();
            var msVariance = allMs.Select(m => (float)Math.Pow(m - msAvg, 2)).Average();
            var msStdDev = (float)Math.Sqrt(msVariance);

            var allBpm = tiles.Select(t => (float)t.tileMeta.bpm);
            var minBpm = allBpm.Min();
            var maxBpm = allBpm.Max();
            var bpmAvg = allBpm.Average();
            var bpmVariance = allBpm.Select(b => (float)Math.Pow(b - bpmAvg, 2)).Average();
            var bpmStdDev = (float)Math.Sqrt(bpmVariance);

            return new LevelMeta(
                tiles.Count,
                twirlRatio, setSpeedRatio,
                minTA, maxTA,
                taAvg, taVariance, taStdDev,
                minSA, maxSA,
                saAvg, saVariance, saStdDev,
                minMs, maxMs,
                msAvg, msVariance, msStdDev,
                minBpm, maxBpm,
                bpmAvg, bpmVariance, bpmStdDev);
        }
        public static float GetRatio<T>(IEnumerable<T> enumerable, Predicate<T> predicate)
        {
            int count, selectedCount = 0;
            if (enumerable is T[] arr)
            {
                count = arr.Length;
                for (int i = 0; i < count; i++)
                    if (predicate(arr[i]))
                        selectedCount++;
            }
            else if (enumerable is ICollection<T> collection)
            {
                count = collection.Count;
                foreach (T t in collection)
                    if (predicate(t))
                        selectedCount++;
            }
            else
            {
                count = enumerable.Count();
                foreach (T t in enumerable)
                    if (predicate(t))
                        selectedCount++;
            }
            return selectedCount / (float)count * 100f;
        }
    }
}