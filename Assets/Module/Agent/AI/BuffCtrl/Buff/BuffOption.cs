
namespace Module
{
    public struct BuffOption
    {
        public int layCount;
        public float totalTime;
        public float[] interval;
        
        public BuffOption(float totalTime, float[] interval,int layCount)
        {
            this.totalTime = totalTime;
            this.interval = interval;
            this.layCount = layCount;
        }

    }
}