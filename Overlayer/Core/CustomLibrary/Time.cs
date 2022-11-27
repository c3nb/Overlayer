using JSEngine.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSEngine.CustomLibrary
{
    public class Time : ObjectInstance
    {
        public Time(ScriptEngine engine) : base(engine)
        {
            PopulateFunctions();
        }
        [JSFunction(Name = "getDeltaTime")]
        public static double GetDeltaTime()
        {
            return UnityEngine.Time.deltaTime;
        }
        [JSFunction(Name = "getFixedTime")]
        public static double GetFixedTime()
        {
            return UnityEngine.Time.fixedTimeAsDouble;
        }
        [JSFunction(Name = "getFixedUnscaledDeltaTime")]
        public static double GetFixedUnscaledDeltaTime()
        {
            return UnityEngine.Time.fixedUnscaledDeltaTime;
        }
        [JSFunction(Name = "getFixedUnscaledTime")]
        public static double GetFixedUnscaledTime()
        {
            return UnityEngine.Time.fixedUnscaledTimeAsDouble;
        }
        [JSFunction(Name = "getFrameCount")]
        public static double GetFrameCount()
        {
            return UnityEngine.Time.frameCount;
        }
    }
}
