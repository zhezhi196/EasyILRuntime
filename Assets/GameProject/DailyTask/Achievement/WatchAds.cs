using Module;

namespace Ach
{
    public class WatchAds: AchievementBase
    {
        public WatchAds(AchievementSaveData saveData) : base(saveData)
        {

        }

        private void OnGetReward(RewardBag obj)
        {
            if (obj.iap is AdsIap)
            {
                TryAddCount(1);
            }
        }

        public override void Register()
        {
            RewardBag.OnGetRewardBag += OnGetReward;
        }

        public override void UnRegister()
        {
            RewardBag.OnGetRewardBag -= OnGetReward;
        }

        public override bool IsAchieve(params object[] obj)
        {
            return true;
        }
    }
}