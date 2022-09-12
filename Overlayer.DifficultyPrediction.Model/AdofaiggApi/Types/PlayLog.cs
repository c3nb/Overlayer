using System;

namespace Overlayer.AdofaiggApi.Types
{
    public class PlayLog : Json
    {
        public static Response<PlayLog> Request(params Parameter[] parameters) => Api.Level.Request<PlayLog>(parameters);
        public int id;
        public DateTime timestamp;
        public TinyLevel level;
        public double speed;
        public string url;
        public bool accept;
        public double playPoint;
        public string description;
        public double xAccuracy;
        public static Parameter<int> Offset(int offset = 0) => new Parameter<int>("offset", offset);
        public static Parameter<int> Amount(int amount = 50) => new Parameter<int>("amount", amount);
        public static Parameter<int> PlayerId(int playerId = 0) => new Parameter<int>("playerId", playerId);
        public static Parameter<int> LevelId(int levelId = 0) => new Parameter<int>("levelId", levelId);
        public static Parameter<SortType> Sort(SortType sort = SortType.LIKE_DESC) => new Parameter<SortType>("sort", sort);
        public static Parameter<bool> ShowNotVerified(bool showNotVerified = false) => new Parameter<bool>("showNotVerified", showNotVerified);
        public static Parameter<bool> ShowDuplicatedPerson(bool showDuplicatedPerson = false) => new Parameter<bool>("showDuplicatedPerson", showDuplicatedPerson);
    }
}
