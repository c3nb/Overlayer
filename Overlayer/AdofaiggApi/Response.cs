using Newtonsoft.Json;

namespace Overlayer.AdofaiggApi
{
    public class Response<T>
    {
        public T[] results;
        public int count;
        [JsonIgnore]
        public string json;
        public override string ToString()
            => JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}
