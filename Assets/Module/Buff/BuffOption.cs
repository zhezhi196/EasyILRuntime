
namespace Module
{
    public struct BuffOption
    {
        public int layCount;
        public float totalTime;
        public float[] interval;

        public BuffOption(IBuffData dbData,int layCount)
        {
            float interval = dbData.interval;
            this.totalTime = dbData.totalTime;;
            if (interval != 0)
            {
                int totalCount = (int) (totalTime / interval);
                this.interval = new float[totalCount];
                for (int i = 0; i < this.interval.Length; i++)
                {
                    this.interval[i] = interval * (i + 1);
                }
            }
            else
            {
                this.interval = null;
            }
            
            this.layCount = layCount;
        }
        
        public BuffOption(float totalTime, float interval,int layCount)
        {
            this.totalTime = totalTime;
            int totalCount = (int)(totalTime / interval);
            this.interval = new float[totalCount];
            for (int i = 0; i < this.interval.Length; i++)
            {
                this.interval[i] = interval;
            }

            this.layCount = layCount;
        }

        public BuffOption(float totalTime, float[] interval,int layCount)
        {
            this.totalTime = totalTime;
            this.interval = interval;
            this.layCount = layCount;
        }

        public BuffOption(float totalTime,int layCount)
        {
            this.totalTime = totalTime;
            this.interval = null;
            this.layCount = layCount;
        }
    }
}