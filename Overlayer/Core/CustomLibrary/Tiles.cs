using JSEngine.Library;
using JSEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Overlayer;
using System.Text;
using System.Threading.Tasks;
using System.Security.Policy;
using Newtonsoft.Json.Utilities.LinqBridge;
using Overlayer.Core;

namespace JSEngine.CustomLibrary
{
    public class TilesConstructor : ClrFunction
    {
        public TilesConstructor(ScriptEngine engine) : base(engine.Function.InstancePrototype, "Tiles", new Tiles(engine.Object.InstancePrototype)) { }
        public Tiles Construct() => new Tiles(InstancePrototype);
    }
    public class Tiles : ObjectInstance
    {
        public Tiles(ObjectInstance obj) : base(obj) => PopulateFunctions();
        [JSFunction(Name = "get")]
        public Tile Get(int index)
        {
            TileData tile = Ovlr.tiles[index];
            return new TileConstructor(Engine).Construct(tile.seqID, tile.timing, tile.xAccuracy, tile.accuracy, tile.judgement);
        }
        [JSProperty(Name = "count")]
        public int Count => Ovlr.tiles.Count;
    }
}
