using System.Data;
using System;
using System.Collections.Generic;
using Module;
using Project.Data;

public class LimitRewardCtrl : BattleSystem,IRedPoint
{
    public bool enterUI;
    private ILimitReward _reward;

    public ILimitReward reward
    {
        get { return _reward; }
        set
        {
            _reward = value;
            if (value == null)
            {
                enterUI = false;
            }
            EventCenter.Dispatch<bool>(EventKey.LimitReward, _reward != null);
            onSwitchStation?.Invoke();
        }
    }
    public List<ILimitReward> limitReward = new List<ILimitReward>();
    public RunTimeAction loadLimit;

    public override void StartBattle(EnterNodeType enterType)
    {
        loadLimit = new RunTimeAction(() =>
        {
            var service = DataMgr.Instance.GetSqlService<AdsData>().WhereList(fd => fd.showPosition == 3);
            for (int i = 0; i < service.Count; i++)
            {
                string id = service[i].ID.ToString();
                //注射器
                if (id == DataMgr.CommonData(33020))
                {
                    limitReward.Add(new LifeRewardBag(service[i]));
                }
                //手枪子弹
                else if (id == DataMgr.CommonData(33021))
                {
                    limitReward.Add(new BulletRewardBag(service[i]));
                }
                //合金
                else if (id == DataMgr.CommonData(33023))
                {
                    limitReward.Add(new WeaponUpdateRewardBag(service[i]));
                }
                //记忆碎片
                else if (id == DataMgr.CommonData(33024))
                {
                    limitReward.Add(new GiftRewardBag(service[i]));
                }
            }
            BattleController.Instance.NextFinishAction("loadLimit");
            loadLimit = null;
        });
    }

    public override void ExitBattle(OutGameStation station)
    {
        for (int i = 0; i < limitReward.Count; i++)
        {
            limitReward[i].OnExitBattle();
        }
    }

    public override void OnUpdate()
    {
        if (BattleController.Instance.ctrlProcedure.isStartFight)
        {
            if (reward == null)
            {
                for (int i = 0; i < limitReward.Count; i++)
                {
                    var temp=limitReward[i];
                    temp.TryTrigger();
                    if (temp.isActive)
                    {
                        reward = temp;
                        break;
                    }
                }
            }
            else
            {
                if (enterUI)
                {
                    if (!reward.isActive)
                    {
                        reward = null;
                    }
                    else
                    {
                        reward.OnUpdate();
                    }
                }
            }
        }
    }

    public bool redPointIsOn => reward != null;
    public event Action onSwitchStation;
}