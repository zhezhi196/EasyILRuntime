using UnityEngine;

namespace Module
{
    public interface ILocationSettingUI : ILocaltionReadingUI
    {
        Rect uiRect { get; }
    }

    public interface ILocaltionReadingUI
    {
        string uiName { get; }
    }
}