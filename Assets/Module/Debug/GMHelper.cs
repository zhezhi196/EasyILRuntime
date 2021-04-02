using UnityEngine;
using Module;

public static class GMHelper
{
    public static AsyncLoadProcess Init(AsyncLoadProcess process)
    {
        // DebugLogConsole.AddCommand("cache", "清除缓存", ClearCache);
        // DebugLogConsole.AddCommand("debug", "打开Inspector", ShowDetail);
        return process;
    }

    private static void ShowDetail()
    {
        GameObject hierarchy = GameObject.Find("Debug/IngameDebugConsole/Scroll View/Viewport/Content/RuntimeHierarchy");
        GameObject inspector = GameObject.Find("Debug/IngameDebugConsole/Scroll View/Viewport/Content/RuntimeInspector");
        hierarchy.SetActive(!hierarchy.activeSelf);
        inspector.SetActive(!inspector.activeSelf);
    }

    private static void ClearCache()
    {
        PlayerPrefs.DeleteAll();
    }
}
