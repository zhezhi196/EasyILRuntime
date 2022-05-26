using Module;

namespace Ach
{
    public class FindXRK: AchievementBase
    {
        public FindXRK(AchievementSaveData saveData) : base(saveData)
        {

        }
        private void OnGetReward(PropEntity entity, int count)
        {
            if (entity.dbData.ID == 20202)
            {
                TryAddCount(count);
            }
        }

         public override void Register()
         {
             EventCenter.Register<PropEntity,int>(EventKey.OnGetProp , OnGetReward);
         }

         public override void UnRegister()
         {
             EventCenter.UnRegister<PropEntity,int>(EventKey.OnGetProp , OnGetReward);
         }
        public override bool IsAchieve(params object[] obj)
        {
            return true;
        }
    }
}