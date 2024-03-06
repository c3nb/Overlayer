using JSON;

namespace Overlayer.Core.Interfaces
{
    public interface IModel
    {
        JsonNode Serialize();
        void Deserialize(JsonNode node);
    }
}
