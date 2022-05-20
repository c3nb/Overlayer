using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityModManagerNet;
using Overlayer.KeyViewer;
using System.Xml.Serialization;
using UnityEngine;
#pragma warning disable

namespace Overlayer
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        public void OnChange() { }
        public static void Load(UnityModManager.ModEntry modEntry)
        {
            Instance = Load<Settings>(modEntry);
        }
        public static void Save(UnityModManager.ModEntry modEntry)
            => Save(Instance, modEntry);
        public static Settings Instance { get; private set; }
        [Draw("Decimals On Displaying Accuracy")]
        public int AccuracyDecimals = 2;
        [Draw("Decimals On Displaying XAccuracy")]
        public int XAccuracyDecimals = 2;
        [Draw("Decimals On Displaying HitTiming")]
        public int TimingDecimals = 2;
        [Draw("Decimals On Displaying Progress")]
        public int ProgressDecimals = 2;
        [Draw("Decimals On Displaying Best Progress")]
        public int BestProgDecimals = 2;
        [Draw("Decimals On Displaying Start Progress")]
        public int StartProgDecimals = 2;
        [Draw("Decimals On Displaying Perceived Bpm")]
        public int PerceivedBpmDecimals = 2;
        [Draw("Decimals On Displaying Tile Bpm")]
        public int TileBpmDecimals = 2;
        [Draw("Decimals On Displaying Perceived KPS")]
        public int PerceivedKpsDecimals = 2;
        [Draw("Decimals On Displaying Current KPS")]
        public int KPSDecimals = 2;
        [Draw("Decimals On Displaying Current FPS")]
        public int FPSDecimals = 2;
        [Draw("Shadow On Displaying Texts (Restart Required)")]
        public bool Shadow  = true;
        [Draw("Reset Stats On Start")]
        public bool Reset = true;
        [Draw("KPS UpdateRate")]
        public int KPSUpdateRate = 20;
        [Draw("FPS UpdateRate")]
        public int FPSUpdateRate = 500;
        [Draw("Unlock ErrorMeter At Auto")]
        public bool UnlockErrorMeterAtAuto = true;
        [Draw("Tile Length")]
        public float TileLength = -1;

        [Draw("KeyViewer Color On Perfect")]
        public Color p = new Color(0.376f, 1f, 0.307f, 1f);
        [Draw("KeyViewer Color On EarlyPerfect")]
        public Color ep = new Color(0.627f, 1f, 0.306f, 1f);
        [Draw("KeyViewer Color On LatePerfect")]
        public Color lp = new Color(0.627f, 1f, 0.306f, 1f);
        [Draw("KeyViewer Color On VeryEarly")]
        public Color ve = new Color(1f, 0.436f, 0.306f, 1f);
        [Draw("KeyViewer Color On VeryLate")]
        public Color vl = new Color(1f, 0.436f, 0.306f, 1f);
        [Draw("KeyViewer Color On TooEarly")]
        public Color te = new Color(1f, 0f, 0f, 1f);
        [Draw("KeyViewer Color On TooLate")]
        public Color tl = new Color(1f, 0f, 0f, 1f);
        [Draw("KeyViewer Color On MultiPress")]
        public Color mp = new Color(0f, 1f, 0.93f, 1f);


        public List<KeyViewerProfile> Profiles { get; set; }
        public int ProfileIndex { get; set; }
        [XmlIgnore]
        public KeyViewerProfile CurrentProfile { get => Profiles[ProfileIndex]; }
        [XmlIgnore]
        public bool IsListening { get; set; }
        public bool IsKeyViewerEnabled = false;
        public Settings()
        {
            Profiles = new List<KeyViewerProfile>();
            ProfileIndex = 0;
        }
    }
}
