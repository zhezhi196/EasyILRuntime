using Module;
using GameGift;

namespace Ach
{
    public class SneakFull: AchievementBase
    {
        public SneakFull(AchievementSaveData saveData) : base(saveData)
        {
        }

        private void OnStudyGift(Gift obj)
        {
            GiftBranchType branch = GiftBranchType.Survival;
            if (obj.branchType == branch && BattleController.Instance.ctrlProcedure is MainProcedure)
            {
                var gift = BattleController.GetCtrl<GiftCtrl>().gift;
                for (int i = 0; i < gift.Count; i++)
                {
                    if (gift[i].branchType == branch)
                    {
                        if (gift[i].station != GiftStation.Owned) return;
                    }
                }

                TryAddCount(1);
            }
        }

        public override void Register()
        {
            EventCenter.Register<Gift>(EventKey.OnStudyGift, OnStudyGift);
        }

        public override void UnRegister()
        {
            EventCenter.UnRegister<Gift>(EventKey.OnStudyGift, OnStudyGift);
        }

        public override bool IsAchieve(params object[] obj)
        {
            return true;
        }
    }
}