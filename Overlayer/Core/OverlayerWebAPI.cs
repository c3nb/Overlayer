using JSON;
using Overlayer.Models;
using System;
using System.Threading.Tasks;

namespace Overlayer.Core
{
    public static class OverlayerWebAPI
    {
        public const string PROD_API = "https://overlayer.c3nb.net";
        public const string DEV_API = "http://localhost:7777";
        public static async Task<string> Handshake() => await Main.HttpClient.GetStringAsync(PROD_API + "/handshake");
        public static async Task<Version> GetVersion() => Version.Parse(JsonNode.Parse(await Main.HttpClient.GetStringAsync(PROD_API + "/version")).Value);
        public static async Task<string> GetDiscordLink() => await Main.HttpClient.GetStringAsync(PROD_API + "/discord");
        public static async Task<string> GetDownloadLink() => await Main.HttpClient.GetStringAsync(PROD_API + "/download");
        public static async Task<string> GetLanguageJson(OverlayerLanguage lang) => await Main.HttpClient.GetStringAsync(PROD_API + "/language/" + lang);
    }
}
