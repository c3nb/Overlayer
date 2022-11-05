using Overlayer.Core.JavaScript;
using Overlayer.Core.JavaScript.Library;

namespace Overlayer
{
    public class Ovlr : ObjectInstance
    {
        public Ovlr(ScriptEngine engine) : base(engine) => PopulateFunctions();
        [JSFunction(Name = "log")]
        public static int Log(object obj)
        {
            Main.Logger.Log(obj.ToString());
            return 0;
        }
    }
}
