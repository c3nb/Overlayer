using Overlayer.Core.JavaScript.Library;
using System.IO;
using UnityEngine;
using USprite = UnityEngine.Sprite;

namespace Overlayer.Core.JavaScript.CustomLibrary
{
    public class Sprite : ObjectInstance
    {
        public Sprite(ScriptEngine engine) : base(engine)
        {
            PopulateFunctions();
        }
        [JSFunction(Name = "load", Flags = JSFunctionFlags.HasEngineParameter)]
        public static Sprite Load(ScriptEngine engine, string path)
        {
            Sprite spr = new Sprite(engine);
            Texture2D texture = new Texture2D(1, 1);
            if (texture.LoadImage(File.ReadAllBytes(path)))
                return null;
            spr.orig = texture.ToSprite();
            return spr;
        }
        public USprite orig;
    }
}
