using System;
using System.Collections.Generic;
using Module;
using Project.Data;
using Sirenix.OdinInspector;
using StationMachine;
using UnityEngine;
using Transform = StationMachine.Transform;

[Serializable]
public class Morphology
{
    public Animator animator;
    public int dbID;
    public AgentType agentType;
    [HideInPrefabs]
    public GameAttribute baseAttribute;
    [HideInPrefabs]
    public GameAttribute growupAttribute;

    public AttackCollider[] attCollider;

    public GameAttribute finalAttribute
    {
        get { return baseAttribute + this.weapon.attribute; }
    }
    [HideInPrefabs]
    public PlayerData dbData;
    public PlayerWeapon defaultWeapon;
    [HideInPrefabs]
    public PlayerWeapon weapon;

    public void Creat(IAgentObject owner)
    {
        dbData = DataInit.Instance.GetSqlService<PlayerData>().WhereID(dbID);
        weapon = defaultWeapon;
        var att = AttributeHelper.GetAttributeByType(dbData);
        baseAttribute = att[0] * (1 + Talent.talentAttribute);
        growupAttribute = att[1];
    }
    
    public void OnEnter(Morphology fromMorphology)
    {
        if (fromMorphology == null || fromMorphology.animator != animator)
        {
            if (fromMorphology != null)
            {
                fromMorphology.animator.gameObject.OnActive(false);
            }

            animator.gameObject.OnActive(true);
        }

        SwitchWeapon(defaultWeapon);
    }

    public void SwitchWeapon(int weapon)
    {
        PlayerWeapon.CreatWeapon(weapon, we =>
        {
            SwitchWeapon(we);
        });
    }

    public void SwitchWeapon(PlayerWeapon weapon)
    {
        if (weapon.agentType.HasFlag(agentType))
        {
            weapon.Init();
            this.weapon = weapon;
            Player.player.isChangeAtt = true;
        }
    }

}