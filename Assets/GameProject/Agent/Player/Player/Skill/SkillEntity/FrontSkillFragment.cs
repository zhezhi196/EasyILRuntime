using System;
using Module;
using UnityEngine;

public class FrontSkillFragment : SkillFragment
{
    public override int targetFragment => skill.dbData.frontPices;

    public FrontSkillFragment(PlayerSkill skill, int savePices) : base(skill, savePices)
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