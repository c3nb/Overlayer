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
using System.Reflection;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using ADOFAI.Editor.Actions;
using Discord;

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
        public static bool ShortcutLoaded = false;
        static readonly string[] events = { "SetSpeed", "Twirl", "Pause", "ScaleMargin", "Hold", "FreeRoam", "MultiPlanet" };
        public static string GetHash(LevelData levelData)
        {
            string angle = string.Join(",", levelData.angleData);
            string @event = string.Join(",", levelData.levelEvents.Where(x => events.Contains(x["eventType"])).Select(x => x.Encode()));
            return MD5Hash.GetHash(angle + @event + levelData.songSettings["bpm"].ToString() + levelData.songSettings["pitch"].ToString());
        }

        public static User GetDiscordUser()
        {
            var discord = (Discord.Discord)AccessTools.Field(typeof(DiscordController), "discord").GetValue(DiscordController.instance);
            return discord.GetUserManager().GetCurrentUser();
        }

        public static string GetFormattedDiscordUsername()
        {
            var user = GetDiscordUser();
            return $"{user.Username}#{user.Discriminator}";
        }
        
        public static void SendResultToServer(scrController ctrl)
        {
            Result result = new Result
            {
                playerName = SteamIntegration.Instance?.GetPlayersName(),
                xAcc = ctrl.mistakesManager.percentXAcc,
                hitMargins = scrMistakesManager.hitMarginsCount,
                difficulty = (int)GCS.difficulty,
                isTimingScaleChanged = IsTimingScaleChanged,
                playSeqId = FirstSeqId,
                discordUsername = GetFormattedDiscordUsername(),
                pitch = scrConductor.instance.song.pitch,
                usedCheckpoints = scrController.checkpointsUsed,
                levelHash = GetHash(scnEditor.instance.levelData),
                isAutoplay = RDC.auto || RDC.debug
            };

            // if (result.isAutoplay) yield break;

            var resultJson = result.ToJson();

            // convert to bytes
            var bytes = System.Text.Encoding.UTF8.GetBytes(resultJson);
            
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create($"https://api.awc.enak.kr/player/{DiscordController.currentUserID}/rate");
            request.Method = "PUT";
            request.ContentType = "application/json";
            request.ContentLength = bytes.Length;
            
            // write the contents into stream
            using (var stream = request.GetRequestStream())
            {
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush();
                stream.Close();
            }
            
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                StreamReader reader = new StreamReader(response.GetResponseStream()!);

                string json = reader.ReadToEnd();
                Main.Logger.Log($"Tournament Feature> Packet successfully sent, received ({json}). Sent content: {resultJson}");
            }
            catch (WebException e)
            {
                var resp = new StreamReader(e.Response.GetResponseStream()!).ReadToEnd();
                Main.Logger.Log($"Tournament Feature> An error occurred while sending a web request ({resp}). Was sending the content: {resultJson}");
            }
        }
        public static void LoadLevel(string path)
        {
            ShortcutLoaded = true;
            // File.SetAttributes(path, FileAttributes.ReadOnly);
            scrController.instance.LoadCustomLevel(path);
            GCS.useNoFail = true;
        }
        public static readonly FastInvokeHandler openLevelCo = MethodInvoker.GetHandler(AccessTools.Method(typeof(scnEditor), "OpenLevelCo"));
    }
    
    [HarmonyPatch]
    public static class OpenLevelShortcut
    {                

        public static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(scnLevelSelect), "Update");
            yield return AccessTools.Method(typeof(scnLevelSelectTaro), "Update");
        }
        public static void Postfix()
        {
            if (SteamIntegration.Instance == null) return;

            // if (Input.GetKeyDown(KeyCode.A))
            // {
            //     // Tournament.ForceSend();
            //     // Main.Logger.Log(Tournament.GetDiscordUser().Discriminator);
            // }
            
            if (RDEditorUtils.CheckForKeyCombo(true, true, KeyCode.O))
                Tournament.LoadLevel(Path.Combine(Main.Mod.Path, "LevelContent/level.adofai"));
        }
    }
    
    [HarmonyPatch(typeof(scrUIController), "ShowDifficultyContainer")]
    public static class HideDifficultyContainer
    {
        public static void Prefix()
        {
            if (Tournament.ShortcutLoaded)
            {
                GCS.difficulty = Difficulty.Strict;
            }
        }
        public static void Postfix(scrUIController __instance)
        {
            if (Tournament.ShortcutLoaded)
            {
                __instance.MinimizeDifficultyContainer();
            }
        }
    }

    [HarmonyPatch(typeof(PauseMenu), "RefreshLayout")]
    public static class ReplacePauseButtons
    {
        public static void Postfix(PauseMenu __instance, ref GeneralPauseButton[] ___pauseButtons, List<PauseButton> ___allPauseButtons)
        {
            if (Tournament.ShortcutLoaded)
            {
                ___pauseButtons = new GeneralPauseButton[] 
                {
                    ___allPauseButtons[0], // continue button
                    __instance.restartButton,
                    __instance.settingsButton,
                    __instance.quitButton
                };
            }
        }
    }
    
    [HarmonyPatch(typeof(scrController), "QuitToMainMenu")]
    public static class RemoveCustomLevelPaths
    {
        public static void Prefix()
        {
            if (Tournament.ShortcutLoaded)
            {
                GCS.customLevelPaths = null;
                Tournament.ShortcutLoaded = false;
            }
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
            // only allow shortcut loads
            if (!CustomLevel.instance || !Tournament.ShortcutLoaded) return;

            Tournament.SendResultToServer(__instance);
        }
    }
    
    public struct Result
    {
        public string playerName;
        public float xAcc;
        public int[] hitMargins;
        public int difficulty;
        public bool isTimingScaleChanged;
        public int playSeqId;
        public string discordUsername;
        public float pitch;
        public int usedCheckpoints;
        public string levelHash;
        public bool isAutoplay;
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
