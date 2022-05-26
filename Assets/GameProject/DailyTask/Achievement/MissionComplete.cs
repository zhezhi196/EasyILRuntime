using Module;

namespace Ach
{
    public class MissionComplete: AchievementBase
    {
        public MissionComplete(AchievementSaveData saveData) : base(saveData)
        {
        }

        private void OnMissionComplet(Mission obj)
        {
            if (BattleController.Instance.ctrlProcedure is MainProcedure)
            {
                if (obj.dbData.ID == 17001)
                {
                    TrySetCount(1);
                }
                else if (obj.dbData.ID == 17002)
                {
                    TrySetCount(2);
                }
                else if (obj.dbData.ID == 17003)
                {
                    TrySetCount(3);
                }
            }
        }

        public override void Register()
        {
            EventCenter.Register<Mission>(EventKey.CompleteMission, OnMissionComplet);

        }

        public override void UnRegister()
        {
            EventCenter.UnRegister<Mission>(EventKey.CompleteMission, OnMissionComplet);
        }

        public override bool IsAchieve(params object[] obj)
        {
            return true;
        }
    }
}