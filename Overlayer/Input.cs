using UnityEngine;
using Overlayer.Core.JavaScript;
using Overlayer.Core.JavaScript.Library;

namespace Overlayer
{
    public class Ipt : ObjectInstance
    {
        public Ipt(ScriptEngine engine) : base(engine) => PopulateFunctions();
        [JSFunction(Name = "getKeyDown")]
        public static bool GetKeyDown(int key) => Input.GetKeyDown((KeyCode)key);
        [JSFunction(Name = "getKeyUp")]
        public static bool GetKeyUp(int key) => Input.GetKeyUp((KeyCode)key);
        [JSFunction(Name = "getKey")]
        public static bool GetKey(int key) => Input.GetKey((KeyCode)key);
    }
}
