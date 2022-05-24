using System;
using Module;
using UnityEngine;

public abstract class SkillFragment : IRewardObject
{
    protected int _getFragmentCount;
    public int getFragmentCount => _getFragmentCount;
    public PlayerSkillStatus skillStation
    {
        get
        {
            if (skill.dbData.switchStation != 0) return PlayerSkillStatus.UnActive;
            if (getFragmentCount >= targetFragment) return PlayerSkillStatus.Complete;
            if (getFragmentCount == 0) return PlayerSkillStatus.Empty;
            if (getFragmentCount < targetFragment) return PlayerSkillStatus.UnComplete;
            return PlayerSkillStatus.Complete;
        }
    }
    public PlayerSkill skill { get; }
    public abstract int targetFragment { get; }
    public SkillFragment(PlayerSkill skill,int savePices)
    {
        this.skill = skill;
        this._getFragmentCount = savePices;
    }

    public abstract void GetIcon(string type, Action<Sprite> callback);
    public abstract string GetText(string type);

    public int stationCode
    {
        get
        {
            if (skill.skillStation == PlayerSkillStatus.UpdateMax) return 2;
            return skill.dbData.switchStation;
        }
    }
    public float GetReward(float rewardCount, RewardFlag flag)
    {
        _getFragmentCount += (int) rewardCount;
        if (_getFragmentCount > targetFragment)
        {
            skill.GetExp(_getFragmentCount - targetFragment, 0);
            _getFragmentCount = targetFragment;
        }

        skill.updateSql = true;
        return rewardCount;
    }
}