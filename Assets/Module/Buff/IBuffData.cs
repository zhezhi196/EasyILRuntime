namespace Module
{
    public interface IBuffData : ISqlData
    {
        /// <summary>
        /// 最大层数
        /// </summary>
        int maxCount { get; set; }
        /// <summary>
        /// 持续总时间
        /// </summary>
        float totalTime { get; set; }
        /// <summary>
        /// 在持续时间内每隔几秒会调一下Interval函数
        /// </summary>
        float interval { get; set; }
    }
}