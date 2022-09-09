using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Overlayer.AdofaiggApi.Types
{
    public class Level : Json
    {
        public static Response<Level> Request(params Parameter[] parameters) => Api.Level.Request<Level>(parameters);
        public int id;
        public string title;
        public double difficulty;
        public User[] creators;
        public Music music;
        public int tiles;
        public int comments;
        public int likes;
        public bool epilepsyWarning;
        public bool censored;
        public string description;
        public string video;
        public string download;
        public string workshop;
        public Tag[] tags;
        public override string ToString()
            => JsonConvert.SerializeObject(this, Formatting.Indented);
        public static Parameter<int> Offset(int offset = 0) => new Parameter<int>("offset", offset);
        public static Parameter<int> Amount(int amount = 0) => new Parameter<int>("amount", amount);
        public static Parameter<string> QueryTitle(string title = "") => new Parameter<string>("queryTitle", title);
        public static Parameter<string> QueryArtist(string artist = "") => new Parameter<string>("queryArtist", artist);
        public static Parameter<string> QueryCreator(string creator = "") => new Parameter<string>("queryCreator", creator);
        public static Parameter<string> Query(string query = "") => new Parameter<string>("query", query);
        public static Parameter<SortType> Sort(SortType sort = SortType.LIKE_DESC) => new Parameter<SortType>("sort", sort);
        public static Parameter<int> MinDifficulty(int minDifficulty = 0) => new Parameter<int>("minDifficulty", minDifficulty);
        public static Parameter<int> MaxDifficulty(int maxDifficulty = 21) => new Parameter<int>("maxDifficulty", maxDifficulty);
        public static Parameter<int> MinBpm(int minBpm = 0) => new Parameter<int>("minBpm", minBpm);
        public static Parameter<int> MaxBpm(int maxBpm = int.MaxValue) => new Parameter<int>("maxBpm", maxBpm);
        public static Parameter<int> MinTiles(int minTiles = 0) => new Parameter<int>("minTiles", minTiles);
        public static Parameter<int> MaxTiles(int maxTiles = int.MaxValue) => new Parameter<int>("maxTiles", maxTiles);
        public static Parameter<bool> ShowNotVerified(bool showNotVerified = false) => new Parameter<bool>("showNotVerified", showNotVerified);
        public static Parameter<bool> ShowCensored(bool showNotCensored = false) => new Parameter<bool>("showNotCensored", showNotCensored);
        public static Parameter<IEnumerable<string>> IncludeTags(IEnumerable<string> includeTags = null) => new Parameter<IEnumerable<string>>("includeTags", includeTags ?? Enumerable.Empty<string>());
        public static Parameter<IEnumerable<string>> ExcludeTags(IEnumerable<string> excludeTags = null) => new Parameter<IEnumerable<string>>("excludeTags", excludeTags ?? Enumerable.Empty<string>());
        public static Parameter<int> RandomSeed(int seed = 0) => new Parameter<int>("randomSeed", seed);
    }
}
