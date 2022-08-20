using UnityModManagerNet;
using System.Reflection;
using System.Xml.Serialization;
#pragma warning disable

namespace Overlayer
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        public void OnChange() 
        {
            SeperateAllKorean &= SeperateKorean;
            if (seperatePrev != SeperateKorean)
            {
                if (SeperateKorean && (!Main.IsSimpleSeperatePatched || !Main.IsAllSeperatePatched))
                    Main.SeperatePatcher.Start();
                else
                {
                    Main.IsSimpleSeperatePatched = false;
                    Main.IsAllSeperatePatched = false;
                    Main.Harmony.UnpatchAll(Main.Harmony.Id);
                    Main.Harmony.PatchAll(Assembly.GetExecutingAssembly());
                }
            }
            seperatePrev = SeperateKorean;
        }
        [XmlIgnore]
        public bool seperatePrev;
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
        [Draw("FrameTime UpdateRate")]
        public int FrameTimeUpdateRate = 500;
        [Draw("Unlock ErrorMeter On Auto")]
        public bool UnlockErrorMeterAtAuto = true;
        [Draw("Add All Judgements At ErrorMeter")]
        public bool AddAllJudgementsAtErrorMeter = true;
        [Draw("Tick Images Of ErrorMeter")]
        public int ErrorMeterHitImages = 60;
        [Draw("Tick Life Of ErrorMeter (Seconds)", Type = DrawType.Slider, Min = 0, Max = 10, Precision = 1)]
        public float ErrorMeterTickLife = 3f;
        [Draw("Apply Pitch At Bpm Tags")]
        public bool ApplyPitchAtBpmTags = true;
        [Draw("Seperate Korean Words")]
        public bool SeperateKorean = true;
        [Draw("Seperate <b>All</b> Korean Words <color=red>(WARNING: THIS OPTION CAN MAKE YOUR ADOFAI PERFORMANCE DOWN!)</color>")]
        public bool SeperateAllKorean = false;
        public string DeathMessage = "";
        public bool EditingCustomTags = false;
    }
}
