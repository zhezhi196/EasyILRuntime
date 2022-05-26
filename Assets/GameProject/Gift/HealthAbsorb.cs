using Project.Data;
using Module;

namespace GameGift
{
    /// <summary>
    /// 生命吸取
    /// 成功暗杀时，回复少量生命
    /// </summary>
    public class HealthAbsorb : Gift
    {
        float arg1 = 10;
        public HealthAbsorb(GiftData data, GiftSaveData saveData) : base(data, saveData)
        {
            arg1 = data.giftArg1;
        }

        public override void ActivateGife()
        {
            //注册事件
            EventCenter.Register<AttackMonster, TimeLineType>(EventKey.MonsterEndTimeLine, OnPlayerTimelineEnd);
        }
        private void OnPlayerTimelineEnd(AttackMonster m, TimeLineType type)
        {
            //如果是处决吸收能量恢复能量
            if (type == TimeLineType.Ass)
            {
                Player.player.ChangeHp(arg1);
            }
        }
        public override void ExitBattle()
        {
            EventCenter.UnRegister<AttackMonster, TimeLineType>(EventKey.MonsterEndTimeLine, OnPlayerTimelineEnd);
        }
    }
}