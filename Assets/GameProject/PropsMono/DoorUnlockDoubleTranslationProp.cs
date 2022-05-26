using DG.Tweening;
using Module;
using UnityEngine;

/// <summary>
/// 双开推拉门
/// </summary>
public class DoorUnlockDoubleTranslationProp : DoorTranslation
{
    public override bool canInteractive => !IsAnimating && base.canInteractive;
    
    public GameObject leftDoor;
    public GameObject rightDoor;

    private Tween leftTween;
    private Tween rightTween;
    
    public override void Open(bool withAnimator)
    {
        base.Open(withAnimator);
        creator.isGet = true;
        // occlusion.open = true;

        Vector3 moveEnd = GetMoveDir(true);
        if (withAnimator)
        {
            IsAnimating = true;
            if (openTween != null && openTween.IsActive())
            {
                openTween.Kill();
            }
            leftTween = leftDoor.transform.DOLocalMove(moveEnd, openDoorTime).SetEase(curve).OnComplete(() =>
            {
                IsAnimating = false;
                openTween = null;
                moveAudio.Stop();
            });
            rightTween = rightDoor.transform.DOLocalMove(-moveEnd , openDoorTime);
            // AnalyticsEvent.SendEvent(AnalyticsType.DoorOpen, creator.id.ToString());
        }
        else
        {
            leftDoor.transform.localPosition = moveEnd; //new Vector3(0,animDoorGo.transform.localPosition.y,animDoorGo.transform.localPosition.z);
            rightDoor.transform.localPosition = -moveEnd;
        }
    }

    protected override void Close(bool withAnimator)
    {
        base.Close(withAnimator);
        
        Vector3 moveEnd = GetMoveDir(false);
        if (withAnimator)
        {
            IsAnimating = true;
            if (openTween != null && openTween.IsActive())
            {
                openTween.Kill();
            }
            leftTween = leftDoor.transform.DOLocalMove(moveEnd, openDoorTime).SetEase(curve).OnComplete(() =>
            {
                IsAnimating = false;
                openTween = null;
                moveAudio.Stop();
            });
            rightTween = rightDoor.transform.DOLocalMove(-moveEnd , openDoorTime);
            // AnalyticsEvent.SendEvent(AnalyticsType.DoorOpen, creator.id.ToString());
        }
        else
        {
            leftDoor.transform.localPosition = moveEnd; //new Vector3(0,animDoorGo.transform.localPosition.y,animDoorGo.transform.localPosition.z);
            rightDoor.transform.localPosition = -moveEnd;
        }
    }
    
    
    protected override Vector3 GetMoveDir(bool open)
    {
        var distance = open ? moveDistance : 0;
        Vector3 moveEnd = leftDoor.transform.localPosition;
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