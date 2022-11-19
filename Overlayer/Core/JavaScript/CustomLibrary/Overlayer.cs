using Overlayer.Core.JavaScript;
using Overlayer.Core.JavaScript.Library;
using System.Collections.Generic;
using System;
using Overlayer.Tags.Global;

namespace Overlayer.Core.JavaScript.CustomLibrary
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
        public static List<Action> Hits = new List<Action>();
        public static List<Action> OpenLevels = new List<Action>();
        public static List<Action> SceneLoads = new List<Action>();
        public static List<Action> Updates = new List<Action>();
        public static Dictionary<string, object> Variables = new Dictionary<string, object>();
        [JSFunction(Name = "hit")]
        public static int OnHit(FunctionInstance func)
        {
            Hits.Add(() => func.Call(func.Prototype == null ? Undefined.Value : func.Prototype));
            return 0;
        }
        [JSFunction(Name = "openLevel")]
        public static int OpenLevel(FunctionInstance func)
        {
            OpenLevels.Add(() => func.Call(func.Prototype == null ? Undefined.Value : func.Prototype));
            return 0;
        }
        [JSFunction(Name = "sceneLoad")]
        public static int SceneLoad(FunctionInstance func)
        {
            SceneLoads.Add(() => func.Call(func.Prototype == null ? Undefined.Value : func.Prototype));
            return 0;
        }
        [JSFunction(Name = "update")]
        public static int Update(FunctionInstance func)
        {
            Updates.Add(() => func.Call(func.Prototype == null ? Undefined.Value : func.Prototype));
            return 0;
        }
        [JSFunction(Name = "getPlanet", Flags = JSFunctionFlags.HasEngineParameter)]
        public static Planet GetPlanet(ScriptEngine engine, int pt)
        {
            return new PlanetConstructor(engine).Construct(pt);
        }
        [JSFunction(Name = "calculatePP")]
        public static double CalculatePP(double difficulty, int speed, double accuracy, int totalTiles)
        {
            return PlayPoint.CalculatePlayPoint(difficulty, speed, accuracy, totalTiles);
        }
        [JSFunction(Name = "getGlobalVariable")]
        public static object GetGlobalVariable(string name)
        {
            return Variables[name];
        }
        [JSFunction(Name = "setGlobalVariable")]
        public static void SetGlobalVariable(string name, object obj)
        {
            Variables.Add(name, obj);
        }
    }
}
