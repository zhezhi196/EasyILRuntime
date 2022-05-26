using GameGift;
namespace Ach
{
    public class FirstTalent: AchievementBase
    {
        public FirstTalent(AchievementSaveData saveData) : base(saveData)
        {
        }

        private void OnStudyGift(Gift obj)
        {
            if (BattleController.Instance.ctrlProcedure is MainProcedure)
            {
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