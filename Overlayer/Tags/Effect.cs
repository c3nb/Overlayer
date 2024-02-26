using DG.Tweening;
using Overlayer.Core;
using Overlayer.Core.TextReplacing;
using Overlayer.Tags.Attributes;
using Overlayer.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Overlayer.Tags
{
    public static class Effect
    {
        static Dictionary<string, double> movingMan_tagValueCache = new Dictionary<string, double>();
        static Dictionary<string, long> movingMan_tagStartTimeCache = new Dictionary<string, long>();
        [JSImplementedBy("Discord@kkitut")]
        [Tag]
        public static string ColorRange(string rawFunc, double valueMin, double valueMax, string colorMinHex, string colorMaxHex, string easeRaw = "Linear", int maxLength = -1, string afterTrimStr = Extensions.DefaultTrimStr)
        {
            Tag tag = TagManager.GetTag(rawFunc)?.Tag;
            if (tag == null) return "Tag Not Found!";
            Delegate getter = tag.GetterDelegate;
            double val = 0;
            if (getter is Func<string> fs)
                val = StringConverter.ToDouble(fs());
            else if (getter is Func<string, string> fss)
                val = StringConverter.ToDouble(fss("6"));
            else return "Not Supported Tag!";
            if (colorMinHex.Length < 6 || colorMaxHex.Length < 6) return "Color's Length Must Be Greater Than 6!";
            if (colorMinHex[0] != '#') colorMinHex = '#' + colorMinHex;
            if (colorMaxHex[0] != '#') colorMaxHex = '#' + colorMaxHex;
            val = Clamp(val, valueMin, valueMax);
            if (rawFunc == "XAccuracy" && val == 100) return "FFDA00";
            float eased = DOVirtual.EasedValue(0, 1, (float)ZeroAndOne(val, valueMin, valueMax), EnumHelper<Ease>.Parse(easeRaw));
            ColorUtility.TryParseHtmlString(colorMinHex, out Color min);
            ColorUtility.TryParseHtmlString(colorMaxHex, out Color max);
            Color newColor = new Color(((1 - eased) * min.r) + (eased * max.r), ((1 - eased) * min.g) + (eased * max.g), ((1 - eased) * min.b) + (eased * max.b), ((1 - eased) * min.a) + (eased * max.a));
            return ColorUtility.ToHtmlStringRGBA(newColor).Trim(maxLength, afterTrimStr);
        }
        [JSImplementedBy("Discord@kkitut")]
        [Tag]
        public static double MovingMan(string rawFunc = "Combo", double startSize = 30, double endSize = 80, double defaultSize = 30, double speed = 800, bool invert = false, Ease ease = Ease.OutExpo)
        {
            Tag tag = TagManager.GetTag(rawFunc)?.Tag;
            if (tag == null) return -1;
            Delegate getter = tag.GetterDelegate;
            double val = 0;
            if (getter is Func<string> fs)
                val = StringConverter.ToDouble(fs());
            else if (getter is Func<string, string> fss)
                val = StringConverter.ToDouble(fss("6"));
            else return -1;
            movingMan_tagValueCache.TryGetValue(rawFunc, out double vCache);
            movingMan_tagStartTimeCache.TryGetValue(rawFunc, out long stCache);
            long mills = FastDateTime.Now.Ticks / 10000;
            if (val != vCache)
            {
                movingMan_tagStartTimeCache[rawFunc] = stCache = mills;
                movingMan_tagValueCache[rawFunc] = val;
            }
            float elapsed = mills - stCache;
            if (elapsed < speed)
            {
                float inEase = (float)(elapsed / speed);
                float eased = DOVirtual.EasedValue(0, 1, inEase, ease);
                float changeOut = (float)(endSize * eased);
                if (invert) changeOut = (float)(endSize * (1 - eased));
                double interpolatedValue = Clamp(changeOut, 0, endSize);
                return interpolatedValue + startSize;
            }
            return defaultSize;
        }
        [JSImplementedBy("Discord@wsbimango")]
        [Tag]
        public static double EasedValue(string rawFunc = "TileBpm", int digits = -1, double speed = 500, Ease ease = Ease.Linear)
        {
            Tag tag = TagManager.GetTag(rawFunc)?.Tag;
            if (tag == null) return -1;
            Delegate getter = tag.GetterDelegate;
            EventEase ee = new EventEase(
                getter is Func<string> fs ?
                () => StringConverter.ToDouble(fs()) :
                getter is Func<string, string> fss ?
                () => StringConverter.ToDouble(fss("6")) :
                null, ease, speed, false);
            if (ee.Getter == null) return -11;
            var easedValue = ee.Compute(rawFunc);
            var prev = ee.GetPrevValue(rawFunc);
            return (prev + (ee.Value - prev) * easedValue).Round(digits);
        }
        public static double Clamp(double value, double min, double max)
            => value < min ? min : value > max ? max : value;
        public static double ZeroAndOne(double nowV, double minV, double maxV)
        {
            return (Math.Min(Math.Max(nowV, minV), maxV) - minV) / (maxV - minV);
        }
    }
}
