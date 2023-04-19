using System;
using System.Collections.Generic;
using System.Linq;
using AdofaiMapConverter.Types;
using AdofaiMapConverter.Actions;
using AdofaiMapConverter.Decorations;
using AdofaiMapConverter.Helpers;
using Action = AdofaiMapConverter.Actions.Action;

namespace AdofaiMapConverter
{
    public class Tile : ICloneable
    {
        public Tile() : this(TileAngle.Zero) { }
        public Tile(TileAngle angle)
            => this.angle = angle;
        public TileAngle angle = TileAngle.Zero;
        public TileMeta tileMeta;
        public List<Decoration> decorations = new List<Decoration>();
        public Dictionary<LevelEventType, List<Action>> actions = new Dictionary<LevelEventType, List<Action>>();
        public bool AddAction(Action action)
        {
            List<Action> actions = GetActions(action.eventType);
            if (action.eventType.IsSingleOnly() && actions.Any())
                return false;
            actions.Add(action);
            return true;
        }
        public void AddActions(List<Action> actions)
        {
            if (actions.Count <= 0) return;
            LevelEventType let = actions[0].eventType;
            AssertCanAdd(actions);
            List<Action> addActions = this.actions.GetValueSafeOrAdd(let, new List<Action>());
            addActions.AddRange(actions);
        }
        public List<Action> GetActions(LevelEventType let)
            => actions.GetValueSafeOrAdd(let, new List<Action>());
        public bool RemoveActions(LevelEventType let) => actions.Remove(let);
        private void AssertCanAdd(List<Action> actions)
        {
            if (actions.Count <= 0) return;
            LevelEventType let = actions[0].eventType;
            if (actions.Any(act => act.eventType != let))
                throw new System.ArgumentException($"The Event Type Of Actions Is Not Constant. (Expected: = \"{let}\")");
            List<Action> addActions = this.actions.GetValueSafeOrAdd(let, new List<Action>());
            if (let.IsSingleOnly() && addActions.Count + actions.Count > 1)
                throw new System.ArgumentException("Size Of Actions Is Too Big!");
        }
        public void Combine(Tile tile)
        {
            foreach (var kvp in tile.actions)
            {
                LevelEventType let = kvp.Key;
                List<Action> list = kvp.Value;
                List<Action> destList = GetActions(let);
                destList.AddRange(list);
                if (let.IsSingleOnly())
                {
                    switch (let)
                    {
                        case LevelEventType.Twirl:
                            {
                                bool hasTwirl = (destList.Count % 2) == 1;
                                destList.Clear();
                                if (hasTwirl)
                                    destList.Add(new Twirl());
                                break;
                            }
                        default:
                            {
                                while (destList.Count > 1)
                                    destList.RemoveAt(0);
                                break;
                            }
                    }
                }
            }
        }
        public void MakeMetaFirst(LevelSetting setting, TileAngle nextAngle)
            => tileMeta = new TileMeta(actions, setting, nextAngle);
        public void MakeMeta(TileMeta prevMeta, TileAngle nextAngle)
            => tileMeta = new TileMeta(actions, prevMeta, angle, nextAngle);
        public void MakeMetaLast(TileMeta prevMeta)
            => tileMeta = new TileMeta(actions, prevMeta, angle, angle.isMidspin ? TileAngle.CreateNormal(prevMeta.staticAngle) : angle);
        public void EditActions<T>(LevelEventType let, System.Func<T, T> function) where T : Action
        {
            List<Action> actions = this.actions.GetValueSafeOrAdd(let, new List<Action>());
            List<T> newActions = actions.Select(a => function((T)a)).ToList();
            actions.Clear();
            actions.AddRange(newActions);
        }
        public void EditActions<T>(LevelEventType let, System.Action<T> function) where T : Action
            => actions.GetValueSafeOrAdd(let, new List<Action>()).ForEach(a => function((T)a));
        public Tile Copy()
        {
            Tile tile = new Tile(angle.isMidspin ? TileAngle.Midspin : new TileAngle(angle.Angle));
            tile.actions = actions;
            tile.decorations = decorations;
            return tile;
        }
        public object Clone() => Copy();
    }
}
