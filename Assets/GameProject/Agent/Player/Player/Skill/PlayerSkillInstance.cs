using System.Collections.Generic;
using Module;
using Project.Data;
using UnityEngine;

public abstract class PlayerSkillInstance : PlayerAction
{
    public override AgentType agentType => (AgentType)skillModle.dbData.morphology;

    public PlayerSkill skillModle;


    public override void OnInit(ISkillObject owner)
    {
        skillModle = PlayerSkill.allSkill.Find(fd => fd.dbData.skillName == name);
    }

    protected override void OnDispose()
    {
    }

}