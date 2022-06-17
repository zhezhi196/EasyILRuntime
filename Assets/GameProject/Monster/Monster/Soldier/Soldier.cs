using UnityEngine;

public class Soldier : AttackMonster
{
    public Transform damagePoint;
    public override string GetLayerDefaultAnimation(int layer)
    {
        return "Normal";
    }
    
    public override float GetLayerFadeTime(int type, string name)
    {
        if (name == "Exc"|| name=="Ass")
        {
            return 0;
        }
        else
        {
            return base.GetLayerFadeTime(type, name);
        }
    }

    public override float GetTranslateTime(string name)
    {
        if (name == "Exc"|| name=="Ass")
        {
            return 0;
        }
        else
        {
            return base.GetTranslateTime(name);
        }
    }


    public GetDangObject geDang;
}