using System;
using Module;
using Project.Data;
using UnityEngine;
using Random = UnityEngine.Random;

public class Drop: IBag
{
    public IRewardObject reward;
    public int count;
    public DropData dbData;
    public string[] match;
    public float plus;

    public Drop(int id)
    {
        dbData = DataMgr.Instance.GetSqlService<DropData>().WhereID(id);
        if (dbData == null)
        {
            GameDebug.LogError($"{id}没有掉落物,请检查表");
            return;
        }
        string[] rewardSpite = dbData.rewardID.Split(ConstKey.Spite0);
        string[] minCount = dbData.min.Split(ConstKey.Spite0);
        string[] maxCount = dbData.max.Split(ConstKey.Spite0);
        string[] weight = dbData.weight.Split(ConstKey.Spite0);
        int index = RandomHelper.RandomWeight(weight.ToFloatArray());
        reward = Commercialize.GetReward(rewardSpite[index].ToInt());
        count = GetDropCount(minCount[index].ToInt(), maxCount[index].ToInt());
    }

    private int GetDropCount(int min,int max)
    {
        return Random.Range(min, max + 1);
    }

    public void GetDropBag(float plus, string[] match, Action<DropBag> callback)
    {
        if (dbData != null)
        {
            if (!RandomHelper.RandomValue(dbData.noDropRate))
            {
                this.match = match;
                this.plus = plus;
                AssetLoad.LoadGameObject<DropBag>("Props/DropBag.prefab", null, (go, arg) =>
                {
                    go.SetDrop(this);
                    callback?.Invoke(go);
                });
            }
        }
    }

    public void GetDropBag(string[] match, Action<DropBag> callback)
    {
        GetDropBag(1, match, callback);
    }

    public void GetReward()
    {
        if (reward is PropEntity propEntity)
        {
            if (propEntity.dropType == DropType.FirstShow)
            {
                var propsCtrl = BattleController.GetCtrl<PropsCtrl>();

                if (!propsCtrl.propPopupedMarkList.Contains(propEntity))
                {
                    propsCtrl.propPopupedMarkList.Add(propEntity);
                    UIController.Instance.Open("BagInteractiveUI", UITweenType.None, this);
                }
            }
            else if (propEntity.dropType == DropType.AlwaysShow || propEntity.dropType == DropType.AlwaysShowAndPopup)
            {
                UIController.Instance.Open("BagInteractiveUI", UITweenType.None, this);

            }
            
            propEntity.GetReward((count * plus), 0, match, RewardFlag.NoAudio);
        }
    }

    public string GetText(string type)
    {
        return reward.GetText(type);
    }

    public string modelName
    {
        get
        {
            if (reward is PropEntity entity)
            {
                return entity.modelName;
            }

            return null;
        }
    }
    public void GetModel(Action<GameObject> callback)
    {
        if (reward is PropEntity entity)
        {
            entity.GetModel(callback);
        }
    }

    public PutToBagStyle buttonStyle => PutToBagStyle.PutToBag;
    public void OnButtonPutToBag()
    {
        UIController.Instance.Open("GameUI", UITweenType.None, OpenFlag.Insertion);
        
        if (reward is PropEntity entity)
        {
            if (entity.dropType == DropType.AlwaysShowAndPopup)
            {
                SpriteLoader.LoadIcon(entity.dbData.icon , (sprite) =>
                {
                    CommonPopup.Popup("",Language.GetContent(entity.dbData.getDes),sprite);
                });
            }
        }
    }
}