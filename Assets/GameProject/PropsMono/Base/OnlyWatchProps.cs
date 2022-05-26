using System;
using Module;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// 只能交互打开看的道具
/// </summary>
public class OnlyWatchProps : PropsBase, IBag
{
    public enum WatchStyle
    {
        [LabelText("打开高模UI")]
        OpenWatch,
        [LabelText("仅显示tips")]
        InterShowTips,
    }
    
    [SerializeField, FoldoutGroup("物品信息"), LabelText("观察样式")]
    public WatchStyle watchStyle;
    public string modelName => entity.modelName;
    public override InterActiveStyle interactiveStyle
    {
        get { return InterActiveStyle.Watch; }
    }

    public PutToBagStyle buttonStyle
    {
        get { return PutToBagStyle.BackToHud; }
    }

    protected override bool OnInteractive(bool fromMonster = false)
    {
        //BattleController.GetCtrl<ProgressCtrl>().TryNextProgress(creator.id, CompleteProgressPredicate.Watch);
        if (watchStyle == WatchStyle.OpenWatch)
        {
            UIController.Instance.Open("BagInteractiveUI", UITweenType.None, this);
        }
        else if (watchStyle == WatchStyle.InterShowTips)
        {
            
        }

        return true;
    }
    
    public void OnButtonPutToBag()
    {
        UIController.Instance.Open("GameUI", UITweenType.None, OpenFlag.Insertion);
    }

    public virtual string GetText(string type)
    {
        return entity.GetText(type);
    }

    public void GetModel(Action<GameObject> callback)
    {
        entity.GetModel(callback);
    }
}