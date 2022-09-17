using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Overlayer.MapParser.Types;
using JSON;
using static Overlayer.MapParser.Actions.ActionUtils;

namespace Overlayer.MapParser.Actions.Default
{
    public class SetSpeed : Action
    {
        public SpeedType speedType = SpeedType.Bpm;
        public double beatsPerMinute = 100;
        public double bpmMultiplier = 1;
        public SetSpeed() : base(LevelEventType.SetSpeed) { }
        public SetSpeed(SpeedType speedType, double beatsPerMinute, double bpmMultiplier, bool active) : base(LevelEventType.SetSpeed, active)
        {
            this.speedType = speedType;
            this.beatsPerMinute = beatsPerMinute;
            this.bpmMultiplier = bpmMultiplier;
        }
        public override JsonNode ToNode()
        {
            JsonNode node = InitNode(eventType, active);
            node["speedType"] = speedType.ToString();
            node["beatsPerMinute"] = beatsPerMinute;
            node["bpmMultiplier"] = bpmMultiplier;
            return node;
        }
    }
}
