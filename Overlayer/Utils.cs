using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityModManagerNet;
using static UnityModManagerNet.UnityModManager.UI;

namespace Overlayer
{
    public static class Utils
    {
        public static HitMargin GetHitMargin(float angle)
        {
            double bpmTimesSpeed = scrConductor.instance.bpm * scrController.instance.speed;
            double conductorPitch = scrConductor.instance.song.pitch;
            double counted =
                scrMisc.GetAdjustedAngleBoundaryInDeg(
                    HitMarginGeneral.Counted, bpmTimesSpeed, conductorPitch);
            double perfect =
                scrMisc.GetAdjustedAngleBoundaryInDeg(
                    HitMarginGeneral.Perfect, bpmTimesSpeed, conductorPitch);
            double pure =
                scrMisc.GetAdjustedAngleBoundaryInDeg(
                    HitMarginGeneral.Pure, bpmTimesSpeed, conductorPitch);
            if (angle < -counted) return HitMargin.TooEarly;
            else if (angle < -perfect) return HitMargin.VeryEarly;
            else if (angle < -pure) return HitMargin.EarlyPerfect;
            else if (angle <= pure) return HitMargin.Perfect;
            else if (angle <= perfect) return HitMargin.LatePerfect;
            else if (angle <= counted) return HitMargin.VeryLate;
            else return HitMargin.TooLate;
        }
        public static HitMargin GetHitMarginForDifficulty(float angle, Difficulty difficulty)
        {
            Difficulty temp = GCS.difficulty;
            GCS.difficulty = difficulty;
            HitMargin margin = GetHitMargin(angle);
            GCS.difficulty = temp;
            return margin;
        }
        public static int GetCurDiffCount(HitMargin hit)
        {
            switch (GCS.difficulty)
            {
                case Difficulty.Lenient: return Variables.LenientCounts[hit];
                case Difficulty.Normal: return Variables.NormalCounts[hit];
                case Difficulty.Strict: return Variables.StrictCounts[hit];
                default: return 0;
            }
        }
        public static HitMargin GetCurHitMargin(Difficulty diff)
        {
            switch (diff)
            {
                case Difficulty.Lenient: return Variables.Lenient;
                case Difficulty.Normal: return Variables.Normal;
                case Difficulty.Strict: return Variables.Strict;
                default: return 0;
            }
        }
        public static void IndentGUI(Action GUI, float verticalSpace = 0f, float indentSize = 20f)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(indentSize);
            GUILayout.BeginVertical();
            GUILayout.Space(verticalSpace);
            GUI();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
        public static bool DrawColor(ref float[] color, GUIStyle style = null, params GUILayoutOption[] option) => DrawFloatMultiField(ref color, new string[]
            {
        "<color=#FF0000>R</color>",
        "<color=#00FF00>G</color>",
        "<color=#0000FF>B</color>",
        "A"
            }, style, option);
        public static bool DrawTextArea(ref string value, string label, GUIStyle style = null, params GUILayoutOption[] option)
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Label(label, new GUILayoutOption[]
            {
            GUILayout.ExpandWidth(false)
            });
            string text = GUILayout.TextArea(value, style ?? GUI.skin.textArea, option);
            GUILayout.EndHorizontal();
            if (text != value)
            {
                value = text;
                return true;
            }
            value = text;
            return false;
        }
    }
}
