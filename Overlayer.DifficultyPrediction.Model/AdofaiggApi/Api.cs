using Newtonsoft.Json;
using System.Net;
using System.Text;
using Overlayer.AdofaiggApi.Types;

namespace Overlayer.AdofaiggApi
{
    public class Api
    {
        public const string API = "https://adofai.gg:9200/api/v1";
        static readonly WebClient api;
        static Api()
        {
            api = new WebClient();
            api.Encoding = Encoding.UTF8;
        }
        Api(string header)
            => this.header = header;
        public readonly string header;
        public static readonly Api Level = new Api("/levels");
        public static readonly Api PlayLogs = new Api("/playLogs");
        public static readonly Api Ranking = new Api("/ranking");
        public Response<T> Request<T>(params Parameter[] parameters) where T : Json
            => Request<T>(new Parameters(parameters));
        public Response<T> Request<T>(Parameters parameters) where T : Json
        {
            string reqUrl = $"{API}{header}{parameters}";
            string json = api.DownloadString(reqUrl);
            Response<T> r = JsonConvert.DeserializeObject<Response<T>>(json);
            r.json = json;
            return r;
        }
        public static Level GetLevel(int id)
            => JsonConvert.DeserializeObject<Level>(api.DownloadString($"{API}/levels/{id}"));
    }
}
