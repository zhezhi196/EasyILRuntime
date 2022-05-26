using Module;

public class AdvDog : Dog
{
    public override float seePlayerFightTime
    {
        get { return 0; }
    }
    public override bool isSeePlayer => isAlive&& Player.player != null;
    
    public void ThrowBlood(RemoteAttack skill)
    {
        AssetLoad.LoadGameObject<AdvDogBlood>("Monster/AdvDog/Blood.prefab", null, (go, arg) =>
        {
            go.transform.position = damagePoint.transform.position;
            go.transform.rotation = damagePoint.rotation;
            go.Fire(Player.player.CenterPostion - go.transform.position, skill);
        });
    }
}