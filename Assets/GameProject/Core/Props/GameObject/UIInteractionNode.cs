using System;
using System.Collections;
using Module;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class UIInteractionNode : UIBtnBase, IPoolObject
{
    [Serializable]
    public struct IconInfo
    {
        public InterActiveStyle station;
        public GameObject icon;
        public string audio;
        public bool mute;
    }

    public ObjectPool pool { get; set; }
    [ReadOnly]public LookPoint lookPoint;
    public IconInfo[] icon;
    public GameObject circleIcon;

    public Label failTips;
    public Label showTips;
    public float maxShowTime = 1;
    public Image rayCastImage;
    
    private bool _isButton;
    private Coroutine _showCoroutine;
    protected override void DefaultListener()
    {
        if (lookPoint != null && lookPoint.canInteractive)
        {
            lookPoint.Interactive();
            if (!lookPoint.interactiveTips.IsNullOrEmpty())
            {
                failTips.gameObject.OnActive(true);
                failTips.text.text = lookPoint.interactiveTips;
                if (_showCoroutine != null)
                {
                    StopCoroutine(_showCoroutine);
                }
                _showCoroutine= StartCoroutine(WaitCloseDiscription());
            }
        }
    }

    private IEnumerator WaitCloseDiscription()
    {
        yield return new WaitForSeconds(maxShowTime);
        failTips.gameObject.OnActive(false);
    }

    public void UpdateNode(Transform parent)
    {
        if (lookPoint == null)
        {
            return;
        }
        
        transform.localScale = Vector3.one;
        transform.localPosition = UIController.Instance.Convert3DToUI(lookPoint.source.evCamera, lookPoint.transform.position);
        interactable = lookPoint.isButtonActive;
        transform.SetParent(parent);
        if (lookPoint.IsInVisiable() || !lookPoint.IsInVisiable(1.2f))
        {
            if (lookPoint.onCircle)
            {
                circleIcon.OnActive(true);
                _isButton = false;
                CloseIcon();
            }
            else
            {
                circleIcon.OnActive(false);
                SetIcon();

                if (!lookPoint.tips.IsNullOrEmpty())
                {
                    showTips.gameObject.OnActive(true);
                    showTips.text.text = lookPoint.tips;
                }
                else
                {
                    showTips.gameObject.OnActive(false);
                }
            }
        }
    }

    private void CloseIcon()
    {
        for (int i = 0; i < icon.Length; i++)
        {
            icon[i].icon.OnActive(false);
        }

        showTips.gameObject.OnActive(false);
        rayCastImage.raycastTarget = false;
    }

    private void SetIcon(InterActiveStyle station = InterActiveStyle.None)
    {
        if (station != InterActiveStyle.None)
        {
            SetIconItem(station);
        }
        else
        {
            if (lookPoint != null && lookPoint.target != null)
            {
                SetIconItem(lookPoint.interactiveStyle);
            }
        }

        rayCastImage.raycastTarget = true;
        transform.localScale = Vector3.one;
    }

    private void SetIconItem(InterActiveStyle station)
    {
        for (int i = 0; i < icon.Length; i++)
        {
            if (icon[i].station != station)
            {
                icon[i].icon.OnActive(false);
            }
            else
            {
                icon[i].icon.OnActive(true);
                if (icon[i].mute)
                {
                    config.flag = config.flag | UIButtonFlag.NoAudio;
                    config.audio = icon[i].audio;
                }
            }
        }
    }


    public void ReturnToPool()
    {
        lookPoint = null;
        // ObjectPool.ReturnToPool(this); //会产生栈溢出，放到外层回收了
        failTips.gameObject.OnActive(false);
        showTips.gameObject.OnActive(false);
        _isButton = false;
        transform.localScale = Vector3.one;
        StopAllCoroutines();
    }

    public void OnGetObjectFromPool()
    {
        transform.localScale = Vector3.one;
        SetIcon();
    }
}