using HarmonyLib;
using Overlayer.Scripting;
using Overlayer.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overlayer.Patches
{
    [HarmonyPatch(typeof(scrController), "Hit")]
    public static class TileCapturer
    {
        public static void Postfix()
        {
            if (scrController.instance.gameworld)
                Api.CaptureTile(Misc.Accuracy(), Misc.XAccuracy(), Variables.CurrentTile, Variables.Timing, BpmUpdater.TimingAvg(), Variables.TileBpm, (int)CurHitTags.GetCurHitMargin(GCS.difficulty));
        }
    }
}
