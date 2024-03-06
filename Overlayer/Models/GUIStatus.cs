using JSON;
using Overlayer.Core.Interfaces;

namespace Overlayer.Models
{
    public class GUIStatus : IModel, ICopyable<GUIStatus>
    {
        public bool Expanded = false;
        public bool Enabled = true;
        public GUIStatus Copy()
        {
            var status = new GUIStatus();
            status.Expanded = Expanded;
            status.Enabled = Enabled;
            return status;
        }
        public JsonNode Serialize()
        {
            var node = JsonNode.Empty;
            node[nameof(Expanded)] = Expanded;
            node[nameof(Enabled)] = Enabled;
            return node;
        }
        public void Deserialize(JsonNode node)
        {
            Expanded = node[nameof(Expanded)];
            Enabled = node[nameof(Enabled)];
        }
    }
}
