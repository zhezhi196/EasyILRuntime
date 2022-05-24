using System;
using System.Collections.Generic;
using Module;
using PLAYERSKILL;
using Project.Data;
using UnityEngine;

public class PlayerAideSkill : IRewardObject
{
    public static List<PlayerAideSkill> skillList = new List<PlayerAideSkill>();
    
    public static void Init()
    {
        var sd = DataInit.Instance.GetSqlService<AideSkillData>().tableList;
        for (int i = 0; i < sd.Count; i++)
        {
            skillList.Add(new PlayerAideSkill(sd[i]));
        }

        var count = skillList[0].GetReward(1, 0);
        if (count > 0)
        {
            SelectSkill(PlayerType.Aide, skillList[0]);
        }

        var count1 = skillList[1].GetReward(1, 0);
        if (count1 > 0)
        {
            SelectSkill(PlayerType.HumanBase, skillList[1]);
        }
    }

    public static PlayerAideSkill CurrSkill(PlayerType owner)
    {
        for (int i = 0; i < skillList.Count; i++)
        {
            if (skillList[i].dbData.owner == (int)owner && skillList[i].isSelect) return skillList[i];
        }

        return null;
    }

    public static void SelectSkill(PlayerType type, PlayerAideSkill skill)
    {
        var curr = skillList.Find(fd => fd.dbData.owner == (int) type && fd.isSelect);
        if (curr != null && curr == skill) return;
        if (curr != null)
        {
            curr.isSelect = false;
        }

        skill.isSelect = true;
    }
    
    public AideSkillData dbData { get; }
    public bool isGet { get; set; }

    public bool isSelect
    {
        get { return LocalFileMgr.GetBool($"AideSkill{dbData.ID}"); }
        set { LocalFileMgr.SetBool($"AideSkill{dbData.ID}", value); }
    }

    public PlayerAideSkill(AideSkillData dbData)
    {
        this.dbData = dbData;
        var saveData = DataInit.Instance.GetSqlService<AideSkillSaveData>().Where(fd => fd.skillName == dbData.skillName);
        isGet = saveData != null;
    }

    public void GetIcon(string type, Action<Sprite> callback)
    {
        SpriteLoader.LoadIcon(dbData.icon, callback);
    }

    public string GetText(string type)
    {
        return string.Empty;
    }

    public void LoadSkillInstance(Action<AidesSkillInstance> callback)
    {
        AssetLoad.PreloadAsset<AidesSkillInstance>($"Base/Skill/{dbData.skillName}.asset", resut =>
            {
                var ins = GameObject.Instantiate(resut.Result);
                callback?.Invoke(ins);
            });
    }

    public int stationCode
    {
        get { return dbData.switchStation; }
    }

    public float GetReward(float rewardCount, RewardFlag flag)
    {
        if (!isGet)
        {
            DataInit.Instance.GetSqlService<AideSkillSaveData>().Insert(new AideSkillSaveData() {skillName = dbData.skillName});
            isGet = true;
            return rewardCount;
        }

        return 0;
    }
}