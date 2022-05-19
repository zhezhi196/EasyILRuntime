namespace Module
{
    public interface IRewardBag : IRewardObject
    {
        /// <summary>
        /// 乘积
        /// </summary>
        float product { get; set; }
        /// <summary>

        /// 商品
        /// </summary>
        RewardContent[] content { get; }
    }
}