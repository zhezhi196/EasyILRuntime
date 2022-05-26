using UnityEngine;
using Sirenix.OdinInspector;
using Module;

/// <summary>
/// 改变ui位置
/// 当屏幕朝向改变时,重新计算位置
/// </summary>
public class UIFitTools_Pos_Dynamic : MonoBehaviour
{
    public enum FitType
    {
        Pos_Left,
        Pos_Right,
    }

    /// <summary>
    /// 初始位置
    /// </summary>
    Vector3 startAnchoredPosition3D;

    RectTransform rectTransform;
    public FitType fitType;
    [LabelText("偏移的比率"), Range(0, 1)]
    public float ratio = 1f;
    [LabelText("开启PC端模拟"), BoxGroup("PC测试数据")]
    public bool simulation = false;
    [LabelText("屏幕方向"), BoxGroup("PC测试数据"), ShowIf("simulation")]
    public ScreenOrientation orientation = ScreenOrientation.LandscapeLeft;
    [LabelText("屏幕安全宽度"), BoxGroup("PC测试数据"), ShowIf("simulation")]
    public float safeWidth = 2000;
    [LabelText("屏幕总宽度"), BoxGroup("PC测试数据"), ShowIf("simulation")]
    public float width = 2160;
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        startAnchoredPosition3D = rectTransform.anchoredPosition3D;

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

        if (orientation == ScreenOrientation.LandscapeLeft)
        {
            SetPos(fitType == FitType.Pos_Left);
        }
        else if (orientation == ScreenOrientation.LandscapeRight)
        {
            SetPos(fitType == FitType.Pos_Right);
        }

        EventCenter.Register<ScreenOrientation>(EventKey.ScreenOrientationChange, OnScreenOrientationChange);
    }

    [Sirenix.OdinInspector.Button("改变朝向")]
    private void OnScreenOrientationChange(ScreenOrientation so)
    {
        if (so == ScreenOrientation.LandscapeLeft)
        {
            SetPos(fitType == FitType.Pos_Left);
        }
        else if (so == ScreenOrientation.LandscapeRight)
        {
            SetPos(fitType == FitType.Pos_Right);
        }
    }
    private void SetPos(bool needMove)
    {
        rectTransform = GetComponent<RectTransform>();
        var offset = (width - safeWidth) * ratio;
        if (needMove)
        {
            switch (fitType)
            {
                case FitType.Pos_Left:
                    rectTransform.anchoredPosition3D = new Vector3(startAnchoredPosition3D.x + offset, rectTransform.anchoredPosition3D.y, 0);
                    break;
                case FitType.Pos_Right:
                    rectTransform.anchoredPosition3D = new Vector3(startAnchoredPosition3D.x - offset, rectTransform.anchoredPosition3D.y, 0);
                    break;
            }
        }
        else
        {
            rectTransform.anchoredPosition3D = new Vector3(startAnchoredPosition3D.x, rectTransform.anchoredPosition3D.y, 0);
        }
    }
    private void OnDestroy()
    {
        EventCenter.UnRegister<ScreenOrientation>(EventKey.ScreenOrientationChange, OnScreenOrientationChange);
    }
}
