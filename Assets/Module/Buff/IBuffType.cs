
namespace Module
{
    public enum BuffType
    {
        /// <summary>
        /// 忽视第二个buff
        /// </summary>
        IgnoreSecond,
        
        /// <summary>
        /// 重置时间
        /// </summary>
        Restart,
        
        /// <summary>
        /// 叠加层数,但是以前的buff会继续持有
        /// </summary>
        Independent,
    }
}