using System;
using System.Security.Cryptography;
using Module;
using UnityEngine;

/// <summary>
/// 交互放入背包的道具
/// </summary>
public class InteractiveToBag : PropsBase, IBag, IMatch
{
    public virtual PutToBagStyle buttonStyle
    {
        get { return PutToBagStyle.PutToBag; }
    }



    protected override bool OnInteractive(bool fromMonster = false)
    {
        var propsCtrl = BattleController.GetCtrl<PropsCtrl>();
        if (entity.dropType == DropType.AlwaysShow || !propsCtrl.propPopupedMarkList.Contains(entity))
        {
            if (entity.dropType != DropType.AlwaysShow)
            {
                propsCtrl.propPopupedMarkList.Add(entity);
            }
            UIController.Instance.Open("BagInteractiveUI", UITweenType.None, this);
            return true;
        }
        else
        {
            RunLogicalOnSelf(RunLogicalName.Destroy);
            entity.GetReward(count, creator.id, creator.matchInfo, 0);
            return true;
        }
    }

    public virtual void OnButtonPutToBag()
    {
        //BattleController.GetCtrl<ProgressCtrl>().TryNextProgress(creator.id, CompleteProgressPredicate.PutToBag);
        DestroyWhileUnActive();
        RunLogicalOnSelf(RunLogicalName.Destroy);
        entity.GetReward(count, creator.id, creator.matchInfo, 0);
        UIController.Instance.Open("GameUI", UITweenType.None, OpenFlag.Insertion);
        BattleController.Instance.Save(0);

        //处理特殊弹框
        if (entity.dropType == DropType.AlwaysShowAndPopup)
        {
            SpriteLoader.LoadIcon(entity.dbData.icon , (sprite) =>
            {
                CommonPopup.Popup("",Language.GetContent(entity.dbData.getDes),sprite);
            });
        }
    }

    public void OnMatchSuccess(IMatch[] target)
    {
    }

    public bool CanMatch(IMatch target)
    {
        if (matchObject.IsNullOrEmpty()) return false;
        for (int i = 0; i < matchObject.Length; i++)
        {
            if (matchObject[i].CanMatch(target))
            {
                return true;
            }
        }

        return false;
    }

    public virtual string GetText(string type)
    {
        return entity.GetText(type);
    }

    public string modelName => entity.modelName;
    public void GetModel(Action<GameObject> callback)
    {
        entity.GetModel(callback);
    }
}