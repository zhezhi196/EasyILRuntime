namespace Module
{
    public interface IBuffData : ISqlData
    {
        int maxCount { get; set; }
        float totalTime { get; set; }
        float interval { get; set; }
    }
}