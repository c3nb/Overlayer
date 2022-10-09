using UnityModManagerNet;
using System.Reflection;
using System.Xml.Serialization;
using UnityEngine;
#pragma warning disable

namespace Overlayer
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        public void OnChange() { }
        public static void Load(UnityModManager.ModEntry modEntry)
            => Instance = Load<Settings>(modEntry);
        public static void Save(UnityModManager.ModEntry modEntry)
            => Save(Instance, modEntry);
        public static Settings Instance;
        [Draw("Reset Stats On Start")]
        public bool Reset = true;
        [Draw("KPS UpdateRate")]
        public int KPSUpdateRate = 20;
        [Draw("FPS UpdateRate")]
        public int FPSUpdateRate = 500;
        [Draw("Performance Status UpdateRate")]
        public int PerfStatUpdateRate = 500;
        [Draw("FrameTime UpdateRate")]
        public int FrameTimeUpdateRate = 500;
        [Draw("Add All Judgements At ErrorMeter")]
        public bool AddAllJudgementsAtErrorMeter = true;
        [Draw("Apply Pitch At Bpm Tags")]
        public bool ApplyPitchAtBpmTags = true;
        public SystemLanguage lang = SystemLanguage.English;
        public string DeathMessage = "";
        public bool EditingCustomTags = false;
    }
}
