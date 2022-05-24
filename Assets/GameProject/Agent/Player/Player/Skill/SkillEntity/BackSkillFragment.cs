using System;
using Module;
using UnityEngine;

public class BackSkillFragment: SkillFragment
{
    public override int targetFragment => skill.dbData.backPices;


    public BackSkillFragment(PlayerSkill skill, int savePices) : base(skill, savePices)
    {
    }
    public override void GetIcon(string type, Action<Sprite> callback)
    {
    }

    public override string GetText(string type)
    {
        return String.Empty;
    }

}