using AdofaiMapConverter.Types;
using JSON;
using static AdofaiMapConverter.Actions.ActionUtils;

namespace AdofaiMapConverter.Actions
{
    public class SetHitsound : Action
    {
        public GameSound gameSound
        {
            get => _gameSound;
            set
            {
                gameSoundflag = true;
                _gameSound = value;
            }
        }
        private GameSound _gameSound = GameSound.Hitsound;
        private bool gameSoundflag;
        public HitSound hitsound
        {
            get => _hitsound;
            set
            {
                hitsoundflag = true;
                _hitsound = value;
            }
        }
        private HitSound _hitsound = HitSound.Kick;
        private bool hitsoundflag;
        public int hitsoundVolume
        {
            get => _hitsoundVolume;
            set
            {
                hitsoundVolumeflag = true;
                _hitsoundVolume = value;
            }
        }
        private int _hitsoundVolume = 100;
        private bool hitsoundVolumeflag;
        public SetHitsound() : base(LevelEventType.SetHitsound) { }
        public SetHitsound(GameSound gameSound, HitSound hitsound, int hitsoundVolume, bool active) : base(LevelEventType.SetHitsound, active)
        {
            this.gameSound = gameSound;
            this.hitsound = hitsound;
            this.hitsoundVolume = hitsoundVolume;
        }
        public override JsonNode ToNode()
        {
            JsonNode node = InitNode(eventType, active);
            if (gameSoundflag)
                node["gameSound"] = _gameSound.ToString();
            if (hitsoundflag)
                node["hitsound"] = _hitsound.ToString();
            if (hitsoundVolumeflag)
                node["hitsoundVolume"] = _hitsoundVolume;
            return node;
        }
    }
}
