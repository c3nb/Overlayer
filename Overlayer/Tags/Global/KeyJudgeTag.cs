using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Overlayer.Utils;

namespace Overlayer.Tags.Global
{
    [ClassTag("KeyJudge", "KeyCode:Judgement")]
    public static class KeyJudgeTag
    {
        [Tag]
        public static string KJ(float lines = 1)
        {
            if (lines <= 0) return $"Invalid Line Count! ({lines})";
            if (lineCount != lines)
            {
                lineCount = (int)lines;
                keyJudges = new KeyJudge[lineCount];
            }
            sb.Clear();
            for (int i = 0; i < lineCount; i++)
            {
                var kj = keyJudges[i];
                if (kj == null) continue;
                sb.AppendLine($"{kj.code}:{RDString.Get($"HitMargin.{kj.margin}")}");
            }
            return sb.ToString();
        }
        public static int lineCount = 1;
        public static StringBuilder sb = new StringBuilder();
        public static KeyJudge[] keyJudges = new KeyJudge[0];
        public static void Add(KeyJudge kj)
        {
            if (keyJudges.Length < 1) return;
            ArrayUtils.MoveFirst(ref keyJudges, kj);
        }
    }
    public class KeyJudge
    {
        public KeyCode code;
        public HitMargin margin;
        public KeyJudge(KeyCode code, HitMargin margin)
        {
            this.code = code;
            this.margin = margin;
        }
    }
}