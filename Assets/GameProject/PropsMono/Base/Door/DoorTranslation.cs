using DG.Tweening;
using UnityEngine;

public enum MoveDirection
{
    X,
    Y,
    Z,
}
/// <summary>
/// 可以平移的门
/// 只定义开关逻辑
/// </summary>
public class DoorTranslation : DoorLocked
{
    public float moveDistance = 1;
    public MoveDirection dir;
    public override void Open(bool withAnimator)
    {
        base.Open(withAnimator);
        creator.isGet = true;
        // occlusion.open = true;

        if (animDoorGo != null)
        {
            Vector3 moveEnd = GetMoveDir(true);
            if (withAnimator)
            {
                IsAnimating = true;
                if (openTween != null && openTween.IsActive())
                {
                    openTween.Kill();
                }
                openTween = animDoorGo.transform.DOLocalMove(moveEnd, openDoorTime).SetEase(curve).OnComplete(() =>
                {
                    IsAnimating = false;
                    openTween = null;
                    moveAudio.Stop();
                });
                // AnalyticsEvent.SendEvent(AnalyticsType.DoorOpen, creator.id.ToString());
            }
            else
            {
                animDoorGo.transform.localPosition = moveEnd; //new Vector3(0,animDoorGo.transform.localPosition.y,animDoorGo.transform.localPosition.z);
            }
        }
        
       
    }

    protected override void Close(bool withAnimator)
    {
        base.Close(withAnimator);


        if (animDoorGo != null)
        {
            Vector3 moveEnd = GetMoveDir(false);
            if (withAnimator)
            {
                IsAnimating = true;
                if (openTween != null && openTween.IsActive())
                {
                    openTween.Kill();
                }
            
                openTween = animDoorGo.transform.DOLocalMove(moveEnd, openDoorTime).SetEase(curve).OnComplete(() =>
                {
                    IsAnimating = false;
                    // occlusion.open = false;
                    openTween = null;
                });
            }
            else
            {
                animDoorGo.transform.localPosition = moveEnd; //new Vector3(0,animDoorGo.transform.localPosition.y,animDoorGo.transform.localPosition.z);
                // occlusion.open = false;
            }
        }
    }

    protected virtual Vector3 GetMoveDir(bool open)
    {
        var distance = open ? moveDistance : 0;
        Vector3 moveEnd = animDoorGo.transform.localPosition;
        if (dir == MoveDirection.X)
        {
            moveEnd = new Vector3(distance , moveEnd.y , moveEnd.z);
        }
        else if (dir == MoveDirection.Y)
        {
            moveEnd = new Vector3(moveEnd.x, distance , moveEnd.z);
        }
        else if (dir == MoveDirection.Z)
        {
            moveEnd = new Vector3(moveEnd.x, moveEnd.y ,distance);
        }

        return moveEnd;
    }
}