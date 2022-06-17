using DG.Tweening;
using Module;
using UnityEngine;

/// <summary>
/// 双开门
/// </summary>
public class DoorUnlockDoubleProp : DoorLocked
{
    public override bool canInteractive => !IsAnimating && base.canInteractive;
    public float OpenAngle = 90;
    
    public GameObject leftDoor;
    public GameObject rightDoor;

    private Tween leftTween;
    private Tween rightTween;
    
    
    public override void Open(bool withAnimator)
    {
        base.Open(withAnimator);
        creator.isGet = true;
        // occlusion.open = true;
        if (withAnimator)
        {
            IsAnimating = true;
            if (leftTween != null && leftTween.IsActive())
            {
                leftTween.Kill();
                rightTween.Kill();
            }
            rightTween = rightDoor.transform.DOLocalRotate(new Vector3(0,OpenAngle,0), openDoorTime).SetEase(curve).OnComplete(() =>
            {
                IsAnimating = false;
                openTween = null;
                moveAudio.Stop();
            });
            leftTween = leftDoor.transform.DOLocalRotate(new Vector3(0, -OpenAngle, 0), openDoorTime).SetEase(curve);
            // AnalyticsEvent.SendEvent(AnalyticsType.DoorOpen, creator.id.ToString());
        }
        else
        {
            rightDoor.transform.localRotation = Quaternion.Euler(0,OpenAngle,0);
            leftDoor.transform.localRotation = Quaternion.Euler(0,-OpenAngle,0);
        }
    }

    protected override void Close(bool withAnimator)
    {
        base.Close(withAnimator);
        if (withAnimator)
        {
            IsAnimating = true;
            if (leftTween != null && leftTween.IsActive())
            {
                leftTween.Kill();
                rightTween.Kill();
            }
            rightTween = rightDoor.transform.DOLocalRotate(new Vector3(0,0,0), openDoorTime).SetEase(curve).OnComplete(() =>
            {
                IsAnimating = false;
                // occlusion.open = false;
                openTween = null;
            });
            leftTween = leftDoor.transform.DOLocalRotate(new Vector3(0, 0, 0), openDoorTime).SetEase(curve);
        }
        else
        {
            rightDoor.transform.localRotation = Quaternion.Euler(0,0,0);
            leftDoor.transform.localRotation = Quaternion.Euler(0,0,0);
            // occlusion.open = false;
        }
    }
    
    
}