using Newtonsoft.Json;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Overlayer.Core.Api.Adofaigg.Types;
using AgLevel = Overlayer.Core.Api.Adofaigg.Types.Level;
using System;

namespace Overlayer.Core.Api.Adofaigg
{
    public class AdofaiggApi : Api
    {
        public static bool EscapeParameter { get; set; } = false;
        public const string API = "https://adofai.gg:9200/api/v1";
        public override string Name => "ADOFAI.GG";
        public override string Url => API;
        AdofaiggApi(string header) => this.header = header;
        public readonly string header;
        public static readonly AdofaiggApi Level = new AdofaiggApi("/levels");
        public static readonly AdofaiggApi PlayLogs = new AdofaiggApi("/playLogs");
        public static readonly AdofaiggApi Ranking = new AdofaiggApi("/ranking");
        public async Task<Response<T>> Request<T>(params Parameter[] parameters) where T : Json
            => await Request<T>(new Parameters(parameters));
        public async Task<Response<T>> Request<T>(Parameters parameters) where T : Json
        {
            string reqUrl = $"{API}{header}{parameters}";
            string json = await client.DownloadStringTaskAsync(reqUrl);
            Response<T> r = JsonConvert.DeserializeObject<Response<T>>(json);
            r.json = json;
            return r;
        }
        public static AgLevel GetLevel(int id)
            => JsonConvert.DeserializeObject<AgLevel>(client.DownloadString($"{API}/levels/{id}"));
    }
}
