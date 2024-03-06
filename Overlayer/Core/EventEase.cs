using DG.Tweening;
using System;
using System.Collections.Generic;

namespace Overlayer.Core
{
    public class EventEase
    {
        static Dictionary<string, double> valueCache = new Dictionary<string, double>();
        static Dictionary<string, double> pvalueCache = new Dictionary<string, double>();
        static Dictionary<string, long> startTimeCache = new Dictionary<string, long>();
        public Func<double> Getter { get; }
        public Ease Ease { get; set; }
        public double Speed { get; set; }
        public bool Invert { get; set; }
        public double Value => Getter();
        public EventEase(Func<double> getter, Ease ease = Ease.Linear, double speed = 500, bool invert = false)
        {
            Getter = getter;
            Ease = ease;
            Speed = speed;
            Invert = invert;
        }
        /// <summary>
        /// Compute Ease Of Value
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public double Compute(string id)
        {
            var val = Value;
            valueCache.TryGetValue(id, out double vCache);
            startTimeCache.TryGetValue(id, out long stCache);
            long mills = FastDateTime.Now.Ticks / 10000;
            if (val != vCache)
            {
                startTimeCache[id] = stCache = mills;
                pvalueCache[id] = vCache;
                valueCache[id] = val;
            }
            long elapsed = mills - stCache;
            if (elapsed < Speed)
            {
                float eased = DOVirtual.EasedValue(0, 1, (float)(elapsed / Speed), Ease);
                return Invert ? 1 - eased : eased;
            }
            return Invert ? 0 : 1;
        }
        public double GetPrevValue(string id) => pvalueCache.TryGetValue(id, out var vCache) ? vCache : Value;
    }
}
