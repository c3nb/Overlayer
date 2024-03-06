using Overlayer.Controllers;
using Overlayer.Core;
using Overlayer.Core.Patches;
using Overlayer.Core.TextReplacing;
using Overlayer.Core.Translation;
using Overlayer.Tags;
using Overlayer.Unity;
using Overlayer.Utils;
using Overlayer.Views;
using System;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityModManagerNet.UnityModManager;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace Overlayer
{
    public static class Main
    {
        public static Assembly Ass { get; private set; }
        public static ModEntry Mod { get; private set; }
        public static ModLogger Logger { get; private set; }
        public static Settings Settings { get; private set; }
        public static GUIController GUI { get; private set; }
        public static Scene ActiveScene { get; private set; }
        public static HttpClient HttpClient { get; private set; }
        public static Language Lang { get; internal set; }
        public static Version ModVersion { get; private set; }
        public static Version LastestVersion { get; private set; }
        public static string DiscordLink { get; private set; }
        public static string DownloadLink { get; private set; }
        public static void Load(ModEntry modEntry)
        {
            Ass = Assembly.GetExecutingAssembly();
            Mod = modEntry;
            Logger = modEntry.Logger;
            GUI = new GUIController();
            HttpClient = new HttpClient();
            ModVersion = modEntry.Version;
            modEntry.OnToggle = OnToggle;
            modEntry.OnShowGUI = OnShowGUI;
            modEntry.OnGUI = OnGUI;
            modEntry.OnHideGUI = OnHideGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            InitializeWebAPI();
            Language.OnInitialize += OnLanguageInitialize;
            SceneManager.activeSceneChanged += (f, t) => ActiveScene = t;
        }
        public static bool OnToggle(ModEntry modEntry, bool toggle)
        {
            if (toggle)
            {
                StaticCoroutine.Run(null);
                Settings = ModSettings.Load<Settings>(modEntry);
                Lang = Language.GetLangauge(Settings.Lang);
                LazyPatchManager.Load(Ass);
                LazyPatchManager.PatchInternal();
                Tag.InitializeWrapperAssembly();
                OverlayerTag.Initialize();
                TagManager.Initialize();
                TagManager.Load(Ass);
                FontManager.Initialize();
                TextManager.Initialize();
            }
            else
            {
                TextManager.Release();
                FontManager.Release();
                TagManager.Release();
                OverlayerTag.Release();
                Tag.ReleaseWrapperAssembly();
                LazyPatchManager.UnloadAll();
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
                ModSettings.Save(Settings, modEntry);
            }
            return true;
        }
        public static void OnShowGUI(ModEntry modEntry)
        {
            GUI.Flush();
        }
        public static void OnGUI(ModEntry modEntry)
        {
            if (!Lang.Initialized)
                Drawer.ButtonLabel("Preparing...", OpenDiscordLink);
            else GUI.Draw();
        }
        public static void OnHideGUI(ModEntry modEntry)
        {
            GUI.Flush();
        }
        public static void OnSaveGUI(ModEntry modEntry)
        {
            TextManager.Save();
            ModSettings.Save(Settings, modEntry);
        }
        public static bool IsPlaying
        {
            get
            {
                var ctrl = scrController.instance;
                var cdt = scrConductor.instance;
                if (ctrl != null && cdt != null)
                    return !ctrl.paused && cdt.isGameWorld;
                return false;
            }
        }
        public static async void InitializeWebAPI()
        {
            Logger.Log($"Handshake Response:{await OverlayerWebAPI.Handshake()}");
            LastestVersion = await OverlayerWebAPI.GetVersion();
            DiscordLink = await OverlayerWebAPI.GetDiscordLink();
            DownloadLink = await OverlayerWebAPI.GetDownloadLink();
            StaticCoroutine.Queue(StaticCoroutine.SyncRunner(EnsureOverlayerVersion));
        }
        public static void OpenDiscordLink()
        {
            Application.OpenURL(DiscordLink);
        }
        public static void OpenDownloadLink()
        {
            Application.OpenURL(DownloadLink);
        }
        public static void EnsureOverlayerVersion()
        {
            if (LastestVersion > ModVersion)
            {
                Lang.ActivateUpdateMode();
                ErrorCanvasContext ecc = new ErrorCanvasContext();
                ecc.titleText = "WOW YOUR OVERLAYER VERSION IS BEAUTIFUL!";
                ecc.errorMessage =
                    $"Current Overlayer Version v{ModVersion}.\n" +
                    $"But Latest Overlayer Is v{LastestVersion}.\n" +
                    $"PlEaSe UpDaTe YoUr OvErLaYeR!";
                ecc.ignoreBtnCallback = () =>
                {
                    ADOUtils.HideError(ecc);
                    OpenDownloadLink();
                };
                ADOUtils.ShowError(ecc);
            }
        }
        public static void OnLanguageInitialize()
        {
            GUI.Flush();
            GUI.Init(new SettingsDrawer(Settings));
        }
    }
}
