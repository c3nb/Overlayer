using DG.Tweening.Plugins;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using UnityEngine.UI;

namespace Overlayer.Core.Api.Overlayer
{
    public class Overlayer : Api
    {
        Overlayer() { }
        public const string API = "http://overlayer.info:6974";
        public override string Name => "Overlayer";
        public override string Url => API;
        public static Overlayer Instance => instance ??= new Overlayer();
        static readonly Uri upload = new Uri(API + "/upload");
        private static Overlayer instance;
        public async void Upload(string path, string name, string diff)
        {
            if (string.IsNullOrWhiteSpace(path) ||
                string.IsNullOrWhiteSpace(name))
                return;
            name = string.Concat(name.Split(Path.GetInvalidFileNameChars()));
            path = Path.GetDirectoryName(Path.GetFullPath(path));
            var zipPath = Path.Combine(path, $"{name}_{diff}.zip");
            await Task.Run(() =>
            {
                try { ZipUtil.Zip(zipPath, Directory.GetFiles(path, "*", SearchOption.AllDirectories)); }
                catch { }
            });
            client.UploadFileAsync(upload, zipPath);
        }
        public async Task<double> Predict(LevelMeta meta)
        {
            var result = await client.DownloadStringTaskAsync(Url + $"/predict/?tileCount={meta.tileCount}&twirlRatio={meta.twirlRatio}&setSpeedRatio={meta.setSpeedRatio}&minTA={meta.minTA}&maxTA={meta.maxTA}&taAverage={meta.taAverage}&taVariance={meta.taVariance}&taStdDeviation={meta.taStdDeviation}&minSA={meta.minSA}&maxSA={meta.maxSA}&saAverage={meta.saAverage}&saVariance={meta.saVariance}&saStdDeviation={meta.saStdDeviation}&minMs={meta.minMs}&maxMs={meta.maxMs}&msAverage={meta.msAverage}&msVariance={meta.msVariance}&msStdDeviation={meta.msStdDeviation}&minBpm={meta.minBpm}&maxBpm={meta.maxBpm}&bpmAverage={meta.bpmAverage}&bpmVariance={meta.bpmVariance}&bpmStdDeviation={meta.bpmStdDeviation}".Replace("+", "").Replace("∞", "-1"));
            return AdjustDiff(StringConverter.ToDouble(result));
        }
        private static double AdjustDiff(double diff)
        {
            double result;
            if (diff > 30)
                return 21;
            if (diff <= 20)
                if (diff > 18)
                    result = Math.Round(diff / .5) * .5;
                else result = Math.Round(diff);
            else result = Math.Round(20f + (diff % 20f / 10f), 1);
            return result;
        }
    }
}
