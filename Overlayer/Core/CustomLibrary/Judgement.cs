using JSEngine.Library;
using JSEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSEngine.CustomLibrary
{
    public class Judgement : ObjectInstance
    {
        public Judgement(ScriptEngine engine) : base(engine) => PopulateFields();
        [JSField] public static int TooEarly = (int)HitMargin.TooEarly;
        [JSField] public static int VeryEarly = (int)HitMargin.VeryEarly;
        [JSField] public static int EarlyPerfect = (int)HitMargin.EarlyPerfect;
        [JSField] public static int Perfect = (int)HitMargin.Perfect;
        [JSField] public static int LatePerfect = (int)HitMargin.LatePerfect;
        [JSField] public static int VeryLate = (int)HitMargin.VeryLate;
        [JSField] public static int TooLate = (int)HitMargin.TooLate;
        [JSField] public static int Multipress = (int)HitMargin.Multipress;
        [JSField] public static int FailMiss = (int)HitMargin.FailMiss;
        [JSField] public static int FailOverload = (int)HitMargin.FailOverload;
        [JSField] public static int Auto = (int)HitMargin.Auto;
    }
}
