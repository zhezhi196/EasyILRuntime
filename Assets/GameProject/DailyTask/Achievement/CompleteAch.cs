namespace Ach
{
    public class CompleteAch: AchievementBase
    {
        public CompleteAch(AchievementSaveData saveData) : base(saveData)
        {

        }

        private void OnRewardStationChanged(AchievementReward arg1, TaskStation arg2)
        {
            if (arg2 == TaskStation.Rewarded)
            {
                TryAddCount(1);
            }
        }

        public override void Register()
        {
            AchievementReward.onStationChanged += OnRewardStationChanged;
        }

        public override void UnRegister()
        {
            AchievementReward.onStationChanged -= OnRewardStationChanged;
        }

        public override bool IsAchieve(params object[] obj)
        {
            return true;
        }
    }
}