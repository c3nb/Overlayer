using Newtonsoft.Json;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Overlayer.Core.Api.Adofaigg.Types;
using AgLevel = Overlayer.Core.Api.Adofaigg.Types.Level;
using System;

namespace Overlayer.Core.Api.Adofaigg
{
    public class Adofaigg : Api
    {
        public static bool EscapeParameter { get; set; } = false;
        public const string API = "https://adofai.gg:9200/api/v1";
        public override string Name => "ADOFAI.GG";
        public override string Url => API;
        Adofaigg(string header) => this.header = header;
        public readonly string header;
        public static readonly Adofaigg Level = new Adofaigg("/levels");
        public static readonly Adofaigg PlayLogs = new Adofaigg("/playLogs");
        public static readonly Adofaigg Ranking = new Adofaigg("/ranking");
        public async Task<Response<T>> Request<T>(params Parameter[] parameters) where T : Json
            => await Request<T>(new Parameters(parameters));
        public async Task<Response<T>> Request<T>(Parameters parameters) where T : Json
        {
            string reqUrl = $"{API}{header}{parameters}";
            Main.Logger.Log($"Request Url: {reqUrl}");
            string json = await client.DownloadStringTaskAsync(reqUrl);
            Response<T> r = JsonConvert.DeserializeObject<Response<T>>(json);
            r.json = json;
            return r;
        }
        public static AgLevel GetLevel(int id)
            => JsonConvert.DeserializeObject<AgLevel>(client.DownloadString($"{API}/levels/{id}"));
    }
}
