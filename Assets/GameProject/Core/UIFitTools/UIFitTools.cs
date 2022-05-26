using Module;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// 对容器类ui的适配,修改容器位置大小
/// </summary>
public class UIFitTools : MonoBehaviour
{
    public enum FitType
    {
        Fit_Left,
        Fit_Right,
        Fit_Both,
    }
    public FitType fitType;
    [LabelText("偏移的比率"), Range(0, 1)]
    public float ratio = 1f;
    [LabelText("强制全拉伸式"),Tooltip("暂时这样写,以后修改")]
    public bool isStretchAll = false;

    RectTransform rectTransform;

    [LabelText("开启PC端模拟"), BoxGroup("PC测试数据")]
    public bool simulation = false;
    [LabelText("屏幕方向"), BoxGroup("PC测试数据"), ShowIf("simulation")]
    public ScreenOrientation orientation = ScreenOrientation.LandscapeLeft;
    [LabelText("屏幕安全宽度"), BoxGroup("PC测试数据"), ShowIf("simulation")]
    public float safeWidth = 2000;
    [LabelText("屏幕总宽度"), BoxGroup("PC测试数据"), ShowIf("simulation")]
    public float width = 2160;

    private Vector2 StartAnchoredPosition;
    private Vector2 StartSizeDelta;

    void Start()
    {
        if (simulation)         //如果开启模拟,PC端使用模拟数据
        {
#if !UNITY_EDITOR
            orientation = Screen.orientation;
            safeWidth = RootCanvas.Instance.SafeWidth;
            width = RootCanvas.Instance.Width;
#endif
        }
        else
        {
            orientation = Screen.orientation;
            safeWidth = RootCanvas.Instance.SafeWidth;
            width = RootCanvas.Instance.Width;
        }

        rectTransform = GetComponent<RectTransform>();
        StartAnchoredPosition = rectTransform.anchoredPosition;
        StartSizeDelta = rectTransform.sizeDelta;
        SetSize(orientation);
        EventCenter.Register<ScreenOrientation>(EventKey.ScreenOrientationChange, OnScreenOrientationChange);
    }

    private void SetSize(ScreenOrientation so)
    {
        //float width = RootCanvas.Instance.Width;
        //if (width <= 2160)
        //{
        //    return;
        //}

        if(rectTransform.anchorMin == Vector2.zero && rectTransform.anchorMax == Vector2.one)
        {
            Fit_StretchAll(so);
        }
        else
        {
            if (isStretchAll)
            {
                Fit_StretchAll(so);
            }
            else
            {
                Fit_Center(so);
            }
        }
    }
    [Sirenix.OdinInspector.Button("Change")]
    private void OnScreenOrientationChange(ScreenOrientation so)
    {
        if (so == ScreenOrientation.LandscapeLeft)
        {
            SetSize(ScreenOrientation.LandscapeLeft);
        }
        else if (so == ScreenOrientation.LandscapeRight)
        {
            SetSize(ScreenOrientation.LandscapeRight);
        }
    }
    private void OnDestroy()
    {
        EventCenter.UnRegister<ScreenOrientation>(EventKey.ScreenOrientationChange,OnScreenOrientationChange);
    }
    #region 适配方案
    private void Fit_StretchAll(ScreenOrientation so)  //全拉伸式的适配方案
    {
        if (so == ScreenOrientation.LandscapeLeft)
        {
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.offsetMin = Vector2.zero;
            if (fitType != FitType.Fit_Right)
            {
                rectTransform.offsetMin = new Vector2((width - safeWidth)* ratio, 0);
            }
        }
        else if (so == ScreenOrientation.LandscapeRight)
        {
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.offsetMin = Vector2.zero;
            if (fitType != FitType.Fit_Left)
            {
                rectTransform.offsetMax = new Vector2((safeWidth - width) * ratio, 0);
            }
        }
    }
    private void Fit_Center(ScreenOrientation so)  //中心式的适配方案
    {
        if (so == ScreenOrientation.LandscapeLeft)
        {
            ResetRect();
            if(fitType != FitType.Fit_Right)
            {
                rectTransform.sizeDelta = new Vector2(safeWidth, rectTransform.sizeDelta.y);
                rectTransform.anchoredPosition = new Vector2(StartAnchoredPosition.x + (width - safeWidth) / 2, StartAnchoredPosition.y);
            }
        }
        else if (so == ScreenOrientation.LandscapeRight)
        {
            ResetRect();
            if(fitType != FitType.Fit_Left)
            {
                rectTransform.sizeDelta = new Vector2(safeWidth, rectTransform.sizeDelta.y);
                rectTransform.anchoredPosition = new Vector2(StartAnchoredPosition.x - (width - safeWidth) / 2, StartAnchoredPosition.y);
            }
        }
    }
#endregion
    private void ResetRect()
    {
        rectTransform.sizeDelta = StartSizeDelta;
        rectTransform.anchoredPosition = StartAnchoredPosition;
    }
}
