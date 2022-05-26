using System;
using System.Collections.Generic;
using Module;
using Project.Data;
using UnityEngine;

public enum DropType
{
    None = 0,
    FirstShow = 1,
    AlwaysShow = 2,
    AlwaysShowAndPopup = 3,
    JustPopup = 4,
}

public class PropEntity : IRewardObject, ICollection
{
    /// <summary>
    /// 配置表的所有的道具
    /// </summary>
    public static List<PropEntity> entityList = new List<PropEntity>();
    private GameObject _model;
    private bool isLoadModel;
    private Queue<Action<GameObject>> modelCallbackCache;

    private const string WatchModelPath = "Props/Watch/{0}.prefab";

    public virtual bool canAddMap
    {
        get { return true; }
    }

    public virtual bool showProgress
    {
        get { return true; }
    }

    public DropType dropType;
    public static AsyncLoadProcess Init(AsyncLoadProcess procerss)
    {
        var allData = DataMgr.Instance.GetSqlService<PropData>().tableList;
        for (int i = 0; i < allData.Count; i++)
        {
            if (allData[i].field.IsNullOrEmpty())
            {
                entityList.Add(new PropEntity(allData[i]));
            }
            else
            {
                var refObj = (PropEntity) Activator.CreateInstance(Type.GetType(allData[i].field), allData[i]);
                entityList.Add(refObj);
            }
        }

        return procerss;
    }
    public static PropEntity GetEntity(int id)
    {
        return entityList.Find(fd => fd.dbData.ID == id);
    }
    
    public PropData dbData { get; }

    public PropEntity(PropData dbData)
    {
        this.dbData = dbData;
        dropType = (DropType)dbData.DropType;
    }

    public virtual void GetIcon(string type, Action<Sprite> callback)
    {
        if (type == TypeList.Normal || type == TypeList.Achievement || type == TypeList.Collection)
        {
            SpriteLoader.LoadIcon(dbData.icon, callback);
        }
        else if (type == TypeList.High)
        {
            SpriteLoader.LoadIcon(dbData.highIcon, callback);
        }
        // else if (type == TypeList.Lock)
        // {
        //     SpriteLoader.LoadIcon(dbData.lockIcon, callback);
        // }
        else if (type == TypeList.MiniIcon)
        {
            if (!dbData.miniIcon.IsNullOrEmpty())
            {
                SpriteLoader.LoadIcon(dbData.miniIcon, callback);
            }
            else
            {
                GetIcon(TypeList.Normal, callback);
            }
        }
    }

    public virtual string GetText(string type)
    {
        switch (type)
        {
            case TypeList.Title:
                return Language.GetContent(dbData.title);
            case TypeList.Des:
                return Language.GetContent(dbData.des);
            case TypeList.BagDes:
                return Language.GetContent(dbData.bagDes);
            case TypeList.GetDes:
                return Language.GetContent(dbData.getDes);
        }

        return string.Empty;
    }


    public virtual int stationCode { get; }


    /// <summary>
    /// 获取物品
    /// </summary>
    /// <param name="rewardCount"></param>
    /// <param name="flag"></param>
    /// <returns></returns>
    public float GetReward(float rewardCount, RewardFlag flag)
    {
        return GetReward(rewardCount, 0, null, flag);
    }

    /// <summary>
    /// 获取物品
    /// </summary>
    /// <param name="rewardCount"></param>
    /// <param name="flag"></param>
    /// <returns></returns>
    public virtual float GetReward(float rewardCount, int editorId, string[] match, RewardFlag flag)
    {
        if (rewardCount < 0.9f) return 0;
        PutBagFlag fl = 0;
        if ((flag & RewardFlag.NoAudio) != 0)
        {
            fl = PutBagFlag.NotAudio;
        }

        BagPackCtrl bagCtrl = BattleController.GetCtrl<BagPackCtrl>();
        if (bagCtrl != null)
        {
            bagCtrl.PutToBag(this, (int) rewardCount, editorId,match, fl);
        }

        onGetCollection?.Invoke(this);
        //获得某个道具发送事件
        EventCenter.Dispatch<PropEntity,int>(EventKey.OnGetProp , this, (int)rewardCount);
        return rewardCount;
    }
    
    /// <summary>
    /// 当交互时的逻辑
    /// </summary>
    public virtual bool OnInteractive()
    {
        onGetCollection?.Invoke(this);
        return true;
    }

    /// <summary>
    /// 在背包界面是否显示某个按钮
    /// </summary>
    /// <param name="prop"></param>
    /// <returns></returns>
    public virtual bool IsShowButton(IMatch prop, BagItem currCell)
    {
        BagItem item = BattleController.GetCtrl<BagPackCtrl>().Match(prop);
        return dbData.bagShowUse == 1 && item != null && item.entity == this && currCell == item;
    }

    public string GetPropArg(int index)
    {
        if (index == 1)
        {
            return dbData.propArg1;
        }
        else if (index == 2)
        {
            return dbData.propArg2;
        }
        else
        {
            return dbData.propArg3;
        }
    }
    // public int dbValue
    // {
    //     get
    //     {
    //         return 0;
    //         // if (dbData.value.IsNullOrEmpty()) return 1;
    //         // string[] spite = dbData.value.Split(ConstKey.Spite0);
    //         // int id = BattleController.Instance.ctrlProcedure.mission.dbData.difficulte;
    //         // return spite[id].ToInt();
    //     }
    // }
    
    /// <summary>
    /// 在背包中如果点击某个按钮的逻辑
    /// </summary>
    /// <param name="count"></param>
    /// <param name="button"></param>
    /// <returns></returns>
    public virtual void OnButtonInBag(BagItem item)
    {
        AnalyticsEvent.SendEvent(AnalyticsType.UseProps, dbData.ID.ToString());
        BattleController.GetCtrl<BagPackCtrl>().ConsumeItem(item, 1);
    }


    public string modelName
    {
        get { return dbData.prefab; }
    }

    public void GetModel(Action<GameObject> callback)
    {
        if (_model != null)
        {
            callback?.Invoke(_model);
        }
        else
        {
            if (!isLoadModel)
            {
                isLoadModel = true;
                AssetLoad.LoadGameObject(string.Format(WatchModelPath,dbData.prefab), AEUICommpont.creatPoint3d, (go, arg) =>
                {
                    _model = go;
                    int count = modelCallbackCache?.Count ?? 0;
                    for (int i = 0; i < count; i++)
                    {
                        modelCallbackCache.Dequeue()(go);
                    }
                    callback?.Invoke(_model);
                    isLoadModel = false;
                });
            }
            else
            {
                if (modelCallbackCache == null) modelCallbackCache = new Queue<Action<GameObject>>();
                modelCallbackCache.Enqueue(callback);
            }
        }
    }

    public bool isGet => collectionStation == CollectionStation.NewGet || collectionStation == CollectionStation.Get;
    public CollectionStation collectionStation { get; set; }
    public CollectionType collectionType => (CollectionType) dbData.collectionType;
    public ICollectionData collectionData => dbData;
    public event Action<ICollection> onGetCollection;
    public void GetIcon(CollectionStation station, Action<Sprite> callback)
    {
        if (station != CollectionStation.locked)
        {
            GetIcon(TypeList.Collection, callback);
        }
        else
        {
            GetIcon(TypeList.Lock, callback);
        }
    }

    public void Unlock()
    {
        onGetCollection?.Invoke(this);
    }
}