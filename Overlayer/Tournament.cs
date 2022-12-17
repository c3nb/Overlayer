#if TOURNAMENT
using ADOFAI;
using HarmonyLib;
using Overlayer.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using TinyJson;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using System.IO;

namespace Overlayer
{
    public static class Tournament
    {
        [Tag("HitMarginScale")]
        public static double HitMarginScale(double digits = -1) => HMScale.Round(digits);
        public static double HMScale = 0;
        public static int FirstSeqId = 0;
        public static bool IsFirst = false;
        public static bool IsTimingScaleChanged = false;
        static readonly string[] events = { "SetSpeed", "Twirl", "Pause", "ScaleMargin", "Hold", "FreeRoam", "MultiPlanet" };
        public static string GetHash(LevelData levelData)
        {
            string angle = string.Join(",", levelData.angleData);
            string @event = string.Join(",", levelData.levelEvents.Where(x => events.Contains(x["eventType"])).Select(x => x.Encode()));
            return MD5Hash.GetHash(angle + @event + levelData.songSettings["bpm"].ToString() + levelData.songSettings["pitch"].ToString());
        }
        public static IEnumerator SendResultToServer(scrController ctrl)
        {
            Result result = new Result
            {
                xAcc = ctrl.mistakesManager.percentXAcc,
                hitMargins = scrMistakesManager.hitMarginsCount,
                difficulty = (int)GCS.difficulty,
                isTimingScaleChanged = IsTimingScaleChanged,
                playSeqId = FirstSeqId,
                discordUserId = DiscordController.currentUserID.ToString(),
                pitch = scrConductor.instance.song.pitch,
                usedCheckpoints = scrController.checkpointsUsed,
                levelHash = GetHash(scnEditor.instance.levelData)
            };
            //$"https://api.awc.enak.kr/player/{result.discordUserId}/rate"
            using (UnityWebRequest r = UnityWebRequest.Put($"http://direct.ppapman.kro.kr:3000/api/player/{result.discordUserId}/rate", result.ToJson()))
            {
                yield return r.SendWebRequest();
                if (r.result != UnityWebRequest.Result.Success)
                    Main.Logger.Log($"WOW FUCKING ERROR? ({r.error})");
                else Main.Logger.Log("SUCCESS");
            }
        }
        public static void LoadLevelStrictly(string level)
        {
            File.WriteAllText("tempLevel.adofai", level);
            File.SetAttributes("tempLevel.adofai", FileAttributes.ReadOnly);
            OpenLevel("tempLevel.adofai");
        }
        public static readonly FastInvokeHandler checkUnsavedChanges = MethodInvoker.GetHandler(AccessTools.Method(typeof(scnEditor), "CheckUnsavedChanges"));
        public static readonly FastInvokeHandler pauseIfUnpaused = MethodInvoker.GetHandler(AccessTools.Method(typeof(scnEditor), "PauseIfUnpaused"));
        public static readonly FastInvokeHandler openLevelCo = MethodInvoker.GetHandler(AccessTools.Method(typeof(scnEditor), "OpenLevelCo"));
        public static void OpenLevel(string path)
        {
            var editor = scnEditor.instance;
            checkUnsavedChanges(editor, () =>
            {
                pauseIfUnpaused(editor);
                editor.StartCoroutine((IEnumerator)openLevelCo(editor, path));
            });
            if (GCS.standaloneLevelMode)
                editor.DeselectFloors(true);
        }
    }
    [HarmonyPatch(typeof(scrPlanet), "SwitchChosen")]
    public static class DetectHMRange
    {
        public static double prevScale;
        public static bool first = true;
        public static void Postfix(scrPlanet __instance)
        {
            Tournament.HMScale = __instance.currfloor.nextfloor == null ? 1.0 : __instance.currfloor.nextfloor.marginScale;
            if (first)
            {
                prevScale = Tournament.HMScale;
                first = false;
                return;
            }
            Tournament.IsTimingScaleChanged = prevScale != Tournament.HMScale;
            prevScale = Tournament.HMScale;
        }
    }
    [HarmonyPatch(typeof(scnEditor), "Play")]
    public static class CheckIsFirst
    {
        public static void Postfix(scnEditor __instance)
        {
            Tournament.FirstSeqId = scrController.instance.currFloor.seqID;
            Tournament.IsFirst = Tournament.FirstSeqId == 0;
            DetectHMRange.first = true;
        }
    }
    [HarmonyPatch(typeof(scrController), "OnLandOnPortal")]
    public static class SendResult
    {
        public static void Postfix(scrController __instance)
        {
            if (!CustomLevel.instance) return;
            Main.Logger.Log("WOW ONLANDONPORTAL");
            StaticCoroutine.Instance.Run(Tournament.SendResultToServer(__instance));
        }
    }
    public struct Result
    {
        public float xAcc;
        public int[] hitMargins;
        public int difficulty;
        public bool isTimingScaleChanged;
        public int playSeqId;
        public string discordUserId;
        public float pitch;
        public int usedCheckpoints;
        public string levelHash;
    }
    public class StaticCoroutine : MonoBehaviour
    {
        private static StaticCoroutine instance;
        public static StaticCoroutine Instance
        {
            get
            {
                if (!instance)
                {
                    instance = new GameObject().AddComponent<StaticCoroutine>();
                    DontDestroyOnLoad(instance);
                }
                return instance;
            }
        }
        public void Run(IEnumerator coroutine) => StartCoroutine(coroutine);
    }
}
#endif
