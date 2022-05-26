using System;
using System.Collections.Generic;
using Module;
using Project.Data;
using GameGift;

public class GiftCtrl: BattleSystem
{
    public List<Gift> gift = new List<Gift>();
    public List<Gift> attGift = new List<Gift>();
    public List<Gift> buffGift = new List<Gift>();
    public Action<Gift> OnStudyGift;

    public RunTimeAction initGift;
    
    public override void OnRestart(EnterNodeType enterType)
    {
        //return;
        List<GiftData> datas = DataMgr.Instance.GetSqlService<GiftData>().tableList;
        for (int i = 0; i < datas.Count; i++)
        {
            Gift _gift = null;
            var saveService = DataMgr.Instance.GetSqlService<GiftSaveData>();
            GiftSaveData saveData = saveService.Where(ds => ds.targetId == datas[i].ID);
            if (datas[i].giftType == 0)//属性天赋
            {
                _gift = new Gift(datas[i], saveData);
            }
            else
            {
                _gift = CreateTalenSkill(datas[i].ID.ToEnum<GiftName>(), datas[i], saveData);
            }
            if (_gift != null)
            {
                gift.Add(_gift);
                if (_gift.station == GiftStation.Owned)
                {
                    OwnGift(_gift);
                }
            }
            else
            {
                GameDebug.LogErrorFormat("初始化Gift失败:{}", datas[i].ID);
            }
        }
        initGift = new RunTimeAction(() =>
        {
            for (int i = 0; i < gift.Count; i++)
            {
                if (gift[i].station == GiftStation.Owned)
                {
                    gift[i].ActivateGife();
                }
            }
            BattleController.Instance.NextFinishAction("InitGift");
        });
    }
    private Gift CreateTalenSkill(GiftName name, GiftData data, GiftSaveData save)
    {
        Gift g = Activator.CreateInstance(Type.GetType("GameGift."+ name.ToString() ?? throw new InvalidOperationException(), true), new object[] { data, save }) as Gift;
        return g;
    }

    public void StudyGift(Gift g)
    {
        g.station = GiftStation.Owned;
        OwnGift(g);
        g.ActivateGife();
        OnStudyGift?.Invoke(g);
        var data = DataMgr.Instance.GetSqlService<GiftSaveData>();
        GiftSaveData saveData = new GiftSaveData()
        {
            targetId = g.dbData.ID,
            station = 2
        };
        data.Insert(saveData);
    }

    public void OwnGift(Gift g)
    {
        switch (g.giftType)
        {
            case GiftType.Attribute:
                attGift.Add(g);
                break;
            case GiftType.Buff:
                buffGift.Add(g);
                break;
        }
    }

    public override void ExitBattle(OutGameStation station)
    {
        for (int i = 0; i < gift.Count; i++)
        {
            gift[i].ExitBattle();
        }
    }
}