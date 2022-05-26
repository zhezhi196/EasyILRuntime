using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Module;
using UnityEngine;

/// <summary>
/// 藏尸柜
/// </summary>
public class CorpseCupboardProp : DoorUnlockTranslationProp
{
    public Transform door;
    
    
    public override void Open(bool withAnimator)
    {
        creator.isGet = true;
        IsAnimating = true;
        //门打开动画
        door.DOLocalRotate(new Vector3(0, -20, 0), 1).OnComplete(() =>
        {
            //抽屉抽出动画
            base.Open(withAnimator);
        });
    }
    
}
