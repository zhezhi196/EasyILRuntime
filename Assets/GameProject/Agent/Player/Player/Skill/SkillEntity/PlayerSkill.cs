using System;
using System.Collections.Generic;
using Module;
using Project.Data;
using UnityEngine;

public enum PlayerSkillStatus
{
    UnActive,
    Empty,
    UnComplete,
    Complete,
    UpdateMax
}

public class PlayerSkill: IRewardObject
{
    public static List<PlayerSkill> allSkill = new List<PlayerSkill>();
    public static AsyncLoadProcess Init(AsyncLoadProcess process)
    {
        process.IsDone = false;
        var dbData = DataInit.Instance.GetSqlService<PlayerSkillData>().tableList;

        for (int i = 0; i < dbData.Count; i++)
        {
            var skill = GetSkill(dbData[i].skillName);
            if (skill.allData.IsNullOrEmpty())
            {
                skill.Init(dbData.FindAll(fd => fd.skillName == dbData[i].skillName));
            }
        }
        
        SkillSlot.Init();
        PlayerAideSkill.Init();
        
        process.SetComplete();
        return process;
    }

    public static SkillFragment RandomSkill(int qulity)
    {
        var skillData = allSkill.FindAll(fd => fd.dbData.quility == qulity && fd.skillStation != PlayerSkillStatus.UpdateMax);
        
        List<float> weight = new List<float>();
        for (int i = 0; i < skillData.Count; i++)
        {
            weight.Add(skillData[i].dbData.randomWeight);
        }

        var resut = RandomHelper.RandomWeight(weight.ToArray());
        var tar = skillData[resut];
        return tar.GetFragment();
    }

    public static void Update()
    {
        if (BattleController.Instance.procedure == null)
        {
            for (int i = 0; i < allSkill.Count; i++)
            {
                allSkill[i].OnUpdate();
            }
        }
    }

    public int SelectIndex
    {
        get { return LocalFileMgr.GetInt($"skill{name}"); }
        set { LocalFileMgr.SetInt($"skill{name}", value); }
    }

    public int SelectSlot
    {
        get { return LocalFileMgr.GetInt($"skillSlot{name}"); }
        set { LocalFileMgr.SetInt($"skillSlot{name}", value); }
    }

    public bool canUse
    {
        get
        {
            var tempStation = skillStation;
            return tempStation == PlayerSkillStatus.Complete || tempStation == PlayerSkillStatus.UpdateMax;
        }
    }

    public PlayerSkillData dbData
    {
        get { return allData[level - 1]; }
    }

    public string name => dbData.skillName;
    public bool updateSql;
    public int level = 1;
    public int exp;

    public List<PlayerSkillData> allData;
    /// <summary>
    /// 卡片前面碎片
    /// </summary>
    public FrontSkillFragment frontFragment;
    /// <summary>
    /// 卡片背面碎片
    /// </summary>
    public BackSkillFragment backFragment;

    /// <summary>
    /// 0:激活未获得 1:未激活
    /// </summary>
    public int stationCode
    {
        get
        {
            if (skillStation == PlayerSkillStatus.UpdateMax) return 2;
            return dbData.switchStation;
        }
    }

    public PlayerSkillStatus skillStation
    {
        get
        {
            if (dbData.switchStation != 0) return PlayerSkillStatus.UnActive;
            if (frontFragment.skillStation == PlayerSkillStatus.Complete && backFragment.skillStation == PlayerSkillStatus.Complete)
            {
                if (dbData.updateExp != -1)
                {
                    return PlayerSkillStatus.Complete;
                }
                else
                {
                    return PlayerSkillStatus.UpdateMax;
                }
            }
            
            if (frontFragment.skillStation == PlayerSkillStatus.Empty && backFragment.skillStation == PlayerSkillStatus.Empty) return PlayerSkillStatus.Empty;
            return PlayerSkillStatus.UnComplete;
        }
    }

    public PlayerSkill(string name)
    {
        var saveData = DataInit.Instance.GetSqlService<SkillSaveData>().Where(fd => fd.skillName == name);
        if (saveData == null)
        {
            frontFragment = new FrontSkillFragment(this, 0);
            backFragment = new BackSkillFragment(this, 0);
        }
        else
        {
            frontFragment = new FrontSkillFragment(this, saveData.frontFragment);
            backFragment = new BackSkillFragment(this, saveData.backFragment);
        }
    }


    public void Init(List<PlayerSkillData> skillDatas)
    {
        this.allData = skillDatas;
    }

    public static PlayerSkill GetSkill(string name)
    {
        for (int i = 0; i < allSkill.Count; i++)
        {
            if (allSkill[i].name == name) return allSkill[i];
        }

        PlayerSkill skill = new PlayerSkill(name);
        allSkill.Add(skill);
        return skill;
    }

    public void GetIcon(string type, Action<Sprite> callback)
    {
        switch (type)
        {
            case DataType.Normal:
                SpriteLoader.LoadIcon(dbData.icon, callback);
                break;
        }
    }

    public string GetText(string type)
    {
        switch (type)
        {
            case DataType.Title:
                return Language.GetContent(dbData.title);
            case DataType.Des:
                return Language.GetContent(dbData.des);
        }

        return string.Empty;
    }

    public SkillFragment GetFragment()
    {
        if (frontFragment.skillStation == PlayerSkillStatus.Complete && backFragment.skillStation != PlayerSkillStatus.Complete)
        {
            return backFragment;
        }
        else if (frontFragment.skillStation != PlayerSkillStatus.Complete && backFragment.skillStation == PlayerSkillStatus.Complete)
        {
            return frontFragment;
        }
        else
        {
            if (RandomHelper.RandomValue(dbData.randomBack))
            {
                return backFragment;
            }
            else
            {
                return frontFragment;
            }
        }
    }
    public float GetReward(float rewardCount, RewardFlag flag)
    {
        frontFragment.GetReward(dbData.frontPices, flag);
        backFragment.GetReward(dbData.backPices, flag);
        return 1;
    }

    public void GetExp(int frontPis, int backPis)
    {
        var front = frontPis * dbData.frontExp + backPis * dbData.backExp;
        exp += front;
        if (front >= dbData.updateExp)
        {
            level++;
            exp -= dbData.updateExp;
        }

        updateSql = true;
    }

    public void LoadInstance(Action<PlayerSkillInstance> callback)
    {
        //加载Instance
        AssetLoad.PreloadAsset<PlayerSkillInstance>($"Agent/Player/Skill/{name}.asset", res =>
        {
            var instance = GameObject.Instantiate(res.Result);
            callback?.Invoke(instance);
        });
    }

    public void OnUpdate()
    {
        if (updateSql)
        {
            updateSql = false;
            var service = DataInit.Instance.GetSqlService<SkillSaveData>();
            var oldDat = service.Where(df => df.skillName == name);
            if (oldDat == null)
            {
                oldDat = new SkillSaveData()
                {
                    backFragment = backFragment.getFragmentCount, exp = exp,
                    frontFragment = frontFragment.getFragmentCount, level = level, skillName = name
                };
                service.Insert(oldDat);
            }
            else
            {
                oldDat.backFragment = backFragment.getFragmentCount;
                oldDat.exp = exp;
                oldDat.frontFragment = frontFragment.getFragmentCount;
                oldDat.level = level;
                oldDat.skillName = name;
                service.Update(oldDat);
            }
        }
    }

}