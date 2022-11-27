using JSEngine.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSEngine.CustomLibrary
{
    public class PT : ObjectInstance
    {
        public PT(ScriptEngine engine) : base(engine)
        {
            PopulateFields();
        }
        [JSField]
        public static int Red = 0;
        [JSField]
        public static int Blue = 1;
        [JSField]
        public static int Green = 2;
        [JSField]
        public static int Yellow = 3;
        [JSField]
        public static int Purple = 4;
        [JSField]
        public static int Pink = 5;
        [JSField]
        public static int Orange = 6;
        [JSField]
        public static int Cyan = 7;
        [JSField]
        public static int Current = 8;
        [JSField]
        public static int Other = 9;
    }
    public enum PlanetType
    {
        Red,
        Blue,
        Green,
        Yellow,
        Purple,
        Pink,
        Orange,
        Cyan,
        Current,
        Other
    }
}
