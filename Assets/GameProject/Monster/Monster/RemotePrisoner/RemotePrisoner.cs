using Module;
using UnityEngine;

public class RemotePrisoner : Prisoner
{
    public GameObject axi;

    public override string GetLayerDefaultAnimation(int layer)
    {
        if (layer == 0)
        {
            return "Normal";
        }
        else
        {
            return "Wprisoner@fightIdle";
        }
    }

    public void GetAxi()
    {
        axi.gameObject.OnActive(true);
    }

    public void ThrowAxi(RemoteAttack skill)
    {
        axi.gameObject.OnActive(false);
        AssetLoad.LoadGameObject<PrisionAxi>("Monster/RemotePrisoner/Axi.prefab", null, (go, arg) =>
        {
            go.transform.position = damagePoint.transform.position;
            go.transform.rotation = damagePoint.rotation;
            go.Fire(Player.player.CenterPostion - go.transform.position, skill);
        });
    }
}