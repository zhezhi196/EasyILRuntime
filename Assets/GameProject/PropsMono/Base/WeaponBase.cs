using Module;

public class WeaponBase : InteractiveToBag
{
    public override InterActiveStyle interactiveStyle => InterActiveStyle.Handle;

    public override void OnButtonPutToBag()
    {
        //BattleController.GetCtrl<ProgressCtrl>().TryNextProgress(creator.id, CompleteProgressPredicate.PutToBag);
        entity.GetReward(count, creator.id, creator.matchInfo, 0);
        RunLogicalOnSelf(RunLogicalName.Destroy);
        UIController.Instance.Open("GameUI", UITweenType.None, OpenFlag.Insertion);
        //WeaponManager.AddWeapon(((WeaponEntity)entity).weaponData.ID);
    }
}