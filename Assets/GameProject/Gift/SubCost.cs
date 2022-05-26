using Project.Data;

namespace GameGift
{
    /// <summary>
    /// 熟练强化
    /// 降低武器升级消耗
    /// </summary>
    public class SubCost : Gift
    {
        float arg;
        public SubCost(GiftData data, GiftSaveData saveData) : base(data, saveData)
        {
            arg = data.giftArg1;
        }
        public override void ActivateGife()
        {
            //改变武器升级折扣
        }
    }
}
