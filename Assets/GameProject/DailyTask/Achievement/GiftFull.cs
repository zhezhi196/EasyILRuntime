using GameGift;
namespace Ach
{
    public class GiftFull: AchievementBase
    {
        public GiftFull(AchievementSaveData saveData) : base(saveData)
        {

        }

        private void OnStudyGift(Gift obj)
        {
            if (BattleController.Instance.ctrlProcedure is MainProcedure)
            {
                var gift = BattleController.GetCtrl<GiftCtrl>().gift;
                for (int i = 0; i < gift.Count; i++)
                {
                    if (gift[i].station != GiftStation.Owned) return;
                }

                TryAddCount(1);
            }
        }

        public override void Register()
        {
            Module.EventCenter.Register<Gift>(EventKey.OnStudyGift, OnStudyGift);
        }

        public override void UnRegister()
        {
            Module.EventCenter.UnRegister<Gift>(EventKey.OnStudyGift, OnStudyGift);
        }

        public override bool IsAchieve(params object[] obj)
        {
            return true;
        }
    }
}