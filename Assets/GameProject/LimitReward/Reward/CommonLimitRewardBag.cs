using System;
using Module;
using Project.Data;
using UnityEngine;

public abstract class CommonLimitRewardBag : RewardBag, ILimitReward
{
    
    public CommonLimitRewardBag(IapSqlData sqlData) : base(sqlData)
    {
    }

    public CommonLimitRewardBag(Iap iap) : base(iap)
    {
    }
    
    protected float exitTime;
    
    protected bool hasShow;
    protected bool isTrigger;
    
    public override int stationCode
    {
        get
        {
            if (isActive && base.stationCode == 0) return 0;
            return base.stationCode;
        }
    }

    public bool isActive
    {
        get { return !hasShow && isTrigger && remainTime > 0; }
    }

    public float remainTime
    {
        get
        {
            if (iap.dbData is IRwardData data)
            {
                return Mathf.Clamp(data.deadTune - exitTime, 0, float.MaxValue);
            }
            return 0;
        }
    }

    public abstract void TryTrigger();


    public virtual void OnUpdate()
    {
        exitTime += TimeHelper.unscaledDeltaTimeIgnorePause;
        if (remainTime <= 0)
        {
            hasShow = true;
        }
    }

    public virtual void OnExitBattle()
    {
    }

    public override void GetReward(Action<IapResult> callback, IapRewardFlag flag = (IapRewardFlag) 0)
    {
        base.GetReward(res =>
        {
            callback?.Invoke(res);
            if (res.result == IapResultMessage.Success)
            {
                hasShow = true;
            }
        }, flag);
    }

    public void Trigger()
    {
        isTrigger = true;
        exitTime = 0;
    }
}