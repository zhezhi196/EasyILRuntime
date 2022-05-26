using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Module;
using UnityEngine.UI;
/// <summary>
/// 教学遮罩
/// 检测鼠标是否点击在指定RectTransform内
/// 使用方法OpenMask=>AddClickAction=>ClearAction=>CloseMask
/// </summary>
public class TeachingMaskUI : MonoBehaviour, IPointerClickHandler
{
    public static TeachingMaskUI _instance;
    public static TeachingMaskUI Instance
    {
        get
        {
            return _instance;
        }
    }
    public Action<Vector2> onPointDown;
    public Action clickRt;
    private RectTransform rect;
    private RectTransform m_RT;
    public Transform maskParent;
    public int maskChildIndex = 0;
    public Image maskImage;
    public RectTransform MRT
    {
        get {
            if (m_RT == null)
            {
                m_RT = this.GetComponent<RectTransform>();
            }
            return m_RT;
        }
    }

    private void Awake()
    {
        _instance = this;
        gameObject.OnActive(false);
        maskParent = transform.parent;
        maskChildIndex = transform.GetSiblingIndex();
        maskImage = GetComponent<Image>();
        maskImage.SetAlpha(0.9f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onPointDown?.Invoke(eventData.position);
        if (InRect(eventData.position))
            clickRt?.Invoke();
    }
    /// <summary>
    /// 打开教学遮罩
    /// </summary>
    public void OpenMask()
    {
        this.gameObject.OnActive(true);
    }
    /// <summary>
    /// 关闭教学遮罩
    /// </summary>
    public void CloseMask()
    {
        this.gameObject.OnActive(false);
    }
    /// <summary>
    /// 遮罩遮罩不透明度
    /// </summary>
    /// <param name="f"></param>
    public void SetAlpha(float f)
    {
        maskImage.SetAlpha(Mathf.Clamp01(f));
    }
    /// <summary>
    /// 注册点击回调时间
    /// </summary>
    /// <param name="callback">回调</param>
    /// <param name="rt">点击判定区域</param>
    public void AddClickAction(Action callback,RectTransform rt = null)
    {
        clickRt += callback;
        rect = rt;
    }
    /// <summary>
    /// 清除回调
    /// </summary>
    public void ClearAction()
    {
        onPointDown = null;
        clickRt = null;
        rect = null;
    }
    /// <summary>
    /// 检测是否点击在区域内
    /// </summary>
    /// <param name="pos">鼠标点击屏幕位置</param>
    /// <returns></returns>
    public bool InRect(Vector2 pos)
    {
        if (rect == null)
            return true;
        return RectTransformUtility.RectangleContainsScreenPoint(rect, pos, CameraCtrl.Instance.uiCamera);    
    }
}
