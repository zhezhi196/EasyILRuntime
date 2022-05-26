using UnityEngine;

public class Soldier : AttackMonster
{
    public Transform damagePoint;
    public override string GetLayerDefaultAnimation(int layer)
    {
        return "Normal";
    }

}