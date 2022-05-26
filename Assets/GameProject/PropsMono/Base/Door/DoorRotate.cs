using DG.Tweening;
using UnityEngine;

/// <summary>
/// 可以旋转的门
/// 只定义开关逻辑
/// </summary>
public class DoorRotate : DoorLocked
{
    public float OpenAngle = 90;
    public override void Open(bool withAnimator)
    {
        base.Open(withAnimator);
        creator.isGet = true;
        // occlusion.open = true;
        if (withAnimator)
        {
            IsAnimating = true;
            if (openTween != null && openTween.IsActive())
            {
                openTween.Kill();
            }
            openTween = animDoorGo.transform.DOLocalRotate(new Vector3(0,OpenAngle,0), openDoorTime).SetEase(curve).OnComplete(() =>
            {
                IsAnimating = false;
                openTween = null;
                moveAudio?.Stop();
            });
            // AnalyticsEvent.SendEvent(AnalyticsType.DoorOpen, creator.id.ToString());
        }
        else
        {
            animDoorGo.transform.localRotation = Quaternion.Euler(0,OpenAngle,0);
        }
    }

    protected override void Close(bool withAnimator)
    {
        base.Close(withAnimator);
        if (withAnimator)
        {
            IsAnimating = true;
            if (openTween != null && openTween.IsActive())
            {
                openTween.Kill();
            }
            openTween = animDoorGo.transform.DOLocalRotate(new Vector3(0,0,0), openDoorTime).SetEase(curve).OnComplete(() =>
            {
                IsAnimating = false;
                // occlusion.open = false;
                openTween = null;
            });
        }
        else
        {
            animDoorGo.transform.localRotation = Quaternion.Euler(0,0,0);
            // occlusion.open = false;
        }
    }
}