using Project.Data;

namespace GameGift
{
    /// <summary>
    /// 蹑足潜踪
    /// 降低慢走，奔跑下的噪声
    /// </summary>
    public class Sneak : Gift
    {
        float arg1 = 10;
        float arg2 = 10;
        public Sneak(GiftData data, GiftSaveData saveData) : base(data, saveData)
        {
            arg1 = data.giftArg1;
            arg2 = data.giftArg2;
        }

        public override void ActivateGife()
        {
            Player.player.walkStepTange.value += arg1;
            Player.player.runStepRang.value += arg2;
        }
    }
}