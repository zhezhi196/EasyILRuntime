using System;
using System.Collections.Generic;
using Module;
using Project.Data;
using UnityEngine;

public class SkillSlot : IRewardObject
{
    public static List<SkillSlot> slots = new List<SkillSlot>();

    public static void Init()
    {
        var dbSlot = DataInit.Instance.GetSqlService<SkillSlotData>().tableList;
        for (int i = 0; i < dbSlot.Count; i++)
        {
            slots.Add(new SkillSlot(dbSlot[i]));
        }

        if (!slots[0].isGet)
        {
            slots[0].GetReward(1, 0);
        }
    }
    
    
    public static void SetSlot(PlayerSkill skill)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            var tempSlot = slots[i];
            if (tempSlot.isGet && skill.dbData.slotType == tempSlot.dbData.type)
            {
                if (tempSlot.AddCount < tempSlot.dbData.Capacity)
                {
                    tempSlot.AddToLast(skill);
                    break;
                }
            }
        }
    }

    public static void RemoveSlot(PlayerSkill skill)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            var tempSlot = slots[i];
            if (tempSlot.isGet && skill.dbData.slotType == tempSlot.dbData.type)
            {
                tempSlot.Remove(skill);
            }
        }
    }
    
    public List<PlayerSkill> skills { get; }=new List<PlayerSkill>();
    public SkillSlotData dbData { get; }
    public int stationCode { get; }

    public bool isGet
    {
        get
        {
            return DataInit.Instance.GetSqlService<SkillSlotSaveData>().tableList.Contains(fd => fd.saveSlotId == dbData.ID);
        }
    }

    public int AddCount
    {
        get { return skills.Count; }
    }

    public Action onSelect; 

    public SkillSlot(SkillSlotData dbDta)
    {
        this.dbData = dbDta;
        for (int i = 0; i < PlayerSkill.allSkill.Count; i++)
        {
            var sk = PlayerSkill.allSkill[i];
            if (sk.dbData.slotType == dbDta.type && sk.canUse && sk.SelectIndex > 0 && sk.SelectSlot == dbDta.index)
            {
                skills.Add(sk);
            }
        }

        skills.Sort((a, b) => a.SelectIndex.CompareTo(b.SelectIndex));
    }

    public void AddToLast(PlayerSkill skill)
    {
        if (skill.canUse && !skills.Contains(skill))
        {
            skills.Add(skill);
            skill.SelectIndex = skills.Count;
            skill.SelectSlot = dbData.index;
            onSelect?.Invoke();
        }
    }

    public void Remove(PlayerSkill tar)
    {
        for (int i = 0; i < skills.Count; i++)
        {
            if (skills[i] == tar)
            {
                tar.SelectIndex = 0;
                tar.SelectSlot = 0;
                skills.Remove(tar);
                for (int j = 0; j < skills.Count; j++)
                {
                    skills[j].SelectIndex = j + 1;
                }
                onSelect?.Invoke();
                break;
            }
        }
    }
        
    public void GetIcon(string type, Action<Sprite> callback)
    {
    }

    public string GetText(string type)
    {
        return string.Empty;
    }


    public float GetReward(float rewardCount, RewardFlag flag)
    {
        if (!isGet)
        {
            DataInit.Instance.GetSqlService<SkillSlotSaveData>().Insert(new SkillSlotSaveData() {saveSlotId = dbData.ID});
        }

        return 1;
    }

    public void LoadSkillInstance(Action<List<PlayerSkillInstance>> callback)
    {
        List<PlayerSkillInstance> result = new List<PlayerSkillInstance>();
        Voter voter = new Voter(skills.Count, () => callback?.Invoke(result));
        for (int i = 0; i < skills.Count; i++)
        {
            if (skills[i].canUse)
            {
                skills[i].LoadInstance(ins =>
                {
                    result.Add(ins);
                    voter.Add();
                });
            }
        }
    }
}