using UnityEngine;

/// <summary>
/// ui位置适配
/// 在游戏开始时适配一次,屏幕朝向改变时不重新计算位置
/// </summary>
public class UIFitTools_Pos : MonoBehaviour
{
    public enum FitType
    {
        Pos_Left,
        Pos_Right,
    }
    RectTransform rectTransform;
    public FitType fitType;
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        var offset = (RootCanvas.Instance.Width - RootCanvas.Instance.SafeWidth) / 2;

        switch (fitType)
        {
            case FitType.Pos_Left:
                if (rectTransform.anchorMin.x == rectTransform.anchorMax.x)
                {
                    rectTransform.anchoredPosition3D = new Vector3(rectTransform.anchoredPosition3D.x + offset, rectTransform.anchoredPosition3D.y, 0);
                }
                else
                {
                    rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x + offset, rectTransform.offsetMin.y);
                }
                break;
            case FitType.Pos_Right:
                if (rectTransform.anchorMin.x == rectTransform.anchorMax.x)
                {
                    rectTransform.anchoredPosition3D = new Vector3(rectTransform.anchoredPosition3D.x - offset, rectTransform.anchoredPosition3D.y, 0);
                }
                else
                {
                    rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x + offset, rectTransform.offsetMax.y);
                }
                break;
        }
    }
}
