using Module;
using UnityEngine;

public class Prisoner : AttackMonster
{
    public Transform damagePoint;
    public override string GetLayerDefaultAnimation(int layer)
    {
        if (layer == 0)
        {
            return "Normal";
        }
        else
        {
            return "prisoner@Idle1";
        }
    }

    // public override float GetLayerFadeTime(int type, string name)
    // {
    //     if (name == "AssKilled" || name == "ExcKilled")
    //     {
    //         return 0;
    //     }
    //     else
    //     {
    //         return base.GetLayerFadeTime(type, name);
    //     }
    // }
    
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


    public override void Idle(int type = 0)
    {
        base.Idle(type);
        if (type == 1)
        {
            GetAgentCtrl<AnimatorCtrl>().Play("zhuangsi", 0, 0);
            GameDebug.LogError("装死吧兄弟");
        }
    }
}