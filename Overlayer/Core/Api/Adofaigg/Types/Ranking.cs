using System.Threading.Tasks;

namespace Overlayer.Core.Api.Adofaigg.Types
{
    public class Ranking : Json
    {
        public static async Task<Response<Ranking>> Request(params Parameter[] parameters) => await AdofaiggApi.Ranking.Request<Ranking>(parameters);
        public int id;
        public string name;
        public double totalPp;
        public Play bestPlay;
        public static Parameter<int> Offset(int offset = 0) => new Parameter<int>("offset", offset);
        public static Parameter<int> Amount(int amount = 50) => new Parameter<int>("amount", amount);
    }
}
