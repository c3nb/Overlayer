using JSEngine;
using JSEngine.Library;

namespace JSEngine.CustomLibrary
{

    public class TileConstructor : ClrFunction
    {
        public TileConstructor(ScriptEngine engine) : base(engine.Function.InstancePrototype, "Tile", new Tile(engine.Object.InstancePrototype)) { }
        [JSConstructorFunction]
        public Tile Construct(int seqID, double timing, double xAccuracy, double accuracy, int judgement)
            => new Tile(InstancePrototype, seqID, timing, xAccuracy, accuracy, judgement);
    }
    public class Tile : ObjectInstance
    {
        public Tile(ObjectInstance obj) : base(obj)
        {
            PopulateFunctions();
        }
        public Tile(ObjectInstance obj, int seqID, double timing, double xAccuracy, double accuracy, int judgement) : base(obj)
        {
            this.seqID = seqID;
            this.timing = timing;
            this.xAccuracy = xAccuracy;
            this.accuracy = accuracy;
            this.judgement = judgement;
        }
        public int seqID;
        public double timing;
        public double xAccuracy;
        public double accuracy;
        public int judgement;
        [JSFunction(Name = "getSeqID")]
        public int GetSeqID() => seqID;
        [JSFunction(Name = "getTiming")]
        public double GetTiming() => timing;
        [JSFunction(Name = "getXAccuracy")]
        public double GetXAccuracy() => xAccuracy;
        [JSFunction(Name = "getAccuracy")]
        public double GetAccuracy() => accuracy;
        [JSFunction(Name = "getJudgement")]
        public int GetJudgement() => judgement;
    }
}
