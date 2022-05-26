using System;
using System.Collections.Generic;
using Module;
using UnityEngine;

public class Collection : IModelObject, IRedPoint
{
    public static List<Collection> collectionList = new List<Collection>();

    public static AsyncLoadProcess Init(AsyncLoadProcess process)
    {
        process.IsDone = false;
        //查找对应表中带有仓库的表
        for (int i = 0; i < PropEntity.entityList.Count; i++)
        {
            if (PropEntity.entityList[i].dbData.isCollection == 1)
            {
                collectionList.Add(new Collection(PropEntity.entityList[i]));
            }
        }
        
        collectionList.Sort((a, b) =>a.target.collectionData.collectionIndex.CompareTo(b.target.collectionData.collectionIndex));
        process.SetComplete();
        return process;
    }

    public ICollection target { get; }

    public CollectionStation station
    {
        get { return target.collectionStation; }
        set { SetStation(value); }
    }

    public bool redPointIsOn =>station == CollectionStation.NewGet;
    public string modelName => target.modelName;

    public event Action onSwitchStation;
    

    public void SetStation(CollectionStation value)
    {
        SqlService<PropSaveData> service = DataMgr.Instance.GetSqlService<PropSaveData>();
        if (service != null)
        {
            if (!service.tableList.Contains(s => s.targetID == target.collectionData.ID))
            {
                service.Insert(new PropSaveData() {targetID = target.collectionData.ID, station = (int) value});
            }
            else
            {
                service.Update(s => s.targetID == target.collectionData.ID, "station", (int) value);
            }
        }

        CollectionStation last = target.collectionStation;
        target.collectionStation = value;
        if (last == CollectionStation.NewGet && value == CollectionStation.Get)
        {
            onSwitchStation?.Invoke();
        }
        else if (value == CollectionStation.NewGet)
        {
            onSwitchStation?.Invoke();
        }
    }

    public Collection(ICollection target)
    {
        this.target = target;

        PropSaveData saveData = DataMgr.Instance.GetSqlService<PropSaveData>().Where(s => s.targetID == target.collectionData.ID);
        if (saveData != null)
        {
            target.collectionStation = (CollectionStation) saveData.station;
        }
        else
        {
            target.collectionStation = CollectionStation.UnGet;
        }

        if (station == CollectionStation.UnGet)
        {
            this.target.onGetCollection += OnGetCollection;
        }
    }

    private async void OnGetCollection(ICollection obj)
    {
        if (station == CollectionStation.UnGet && target == obj)
        {
            station = CollectionStation.NewGet;
            this.target.onGetCollection -= OnGetCollection;
        }

        //如果是特殊的prop，需要存到playerpref里面，不会清除数据
        if (obj is PermanentEntity temp)
        {
            string[] array = LocalFileMgr.GetStringArray("PermanentEntities");
            bool isFind = false;
            if (array != null)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    if(array[i].Split(ConstKey.Spite1)[0].ToInt() == temp.dbData.ID)
                    {
                        isFind = true;
                    }
                }
            }
            if (!isFind)
            {
                var ctrl = BattleController.GetCtrl<BagPackCtrl>();

                await Async.WaitUntil(() =>
                {
                    return ctrl.bagList.Find((v) => v.entity.dbData.ID == temp.dbData.ID) != null;
                });
                
                var find = ctrl.bagList.Find((v) => v.entity.dbData.ID == temp.dbData.ID);
                LocalFileMgr.AddStringArray("PermanentEntities" , find.GetWriteDate());
            }
        }
    }

    public void GetModel(Action<GameObject> callback)
    {
        target.GetModel(callback);
    }

}