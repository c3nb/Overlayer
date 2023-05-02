using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Overlayer.Scripting;

namespace Overlayer.Core.Api.Overlayer
{
    public class OverlayerApi : Api
    {
        OverlayerApi() { }
        public const string API = "http://overlayer.info:6974";
        public override string Name => "Overlayer";
        public override string Url => API;
        public static OverlayerApi Instance => instance ??= new OverlayerApi();
        static readonly Uri upload = new Uri(API + "/upload");
        static readonly Uri online = new Uri(API + "/online");
        private static OverlayerApi instance;
        static Socket heartbeatClient;
        static Thread heartbeatThread;
        public int OnlineUsers;
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
        public void StartSendHeartbeat()
        {
            try
            {
                heartbeatClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                heartbeatClient.Connect(new IPEndPoint(IPAddress.Parse("220.81.234.183"), 6969));
                heartbeatThread = new Thread(() =>
                {
                    while (true)
                    {
                        heartbeatClient.Send(new byte[1]);
                        Thread.Sleep(3000);
                    }
                });
                heartbeatThread.IsBackground = true;
                heartbeatThread.Start();
            }
            catch (Exception e) { Main.Logger.Log(e.ToString()); }
        }
        public void StopSendHeartbeat()
        {
            try
            {
                heartbeatThread.Abort();
                heartbeatClient.Disconnect(false);
                heartbeatClient.Shutdown(SocketShutdown.Both);
                heartbeatClient.Close();
                heartbeatClient = null;
                heartbeatThread = null;
            }
            catch { }
        }
        public async void UpdateOnlineUsers()
        {
            OnlineUsers = StringConverter.ToInt32(await client.DownloadStringTaskAsync(online));
        }
    }
}
