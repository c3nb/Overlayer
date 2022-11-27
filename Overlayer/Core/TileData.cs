namespace Overlayer.Core
{
    public class TileData
    {
        public int seqID;
        public double timing;
        public double xAccuracy;
        public double accuracy;
        public int judgement;
        public TileData(int seqID, double timing, double xAccuracy, double accuracy, int judgement)
        {
            this.seqID = seqID;
            this.timing = timing;
            this.xAccuracy = xAccuracy;
            this.accuracy = accuracy;
            this.judgement = judgement;
        }
    }
}
