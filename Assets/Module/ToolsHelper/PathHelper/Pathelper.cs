using UnityEngine;

namespace Module
{
    public static class Pathelper
    {
        public static string PersistentDataPath()
        {
            return Application.persistentDataPath;
        }

        public static string StreamAssets()
        {
#if UNITY_EDITOR
            return Application.streamingAssetsPath;
#else
#if UNITY_ANDROID
            return "jar:file://" + Application.dataPath + "!/assets/";
#elif UNITY_IOS3
            return Application.dataPath + "/Raw/";
#else
            return Application.dataPath + "/StreamingAssets/";
#endif
#endif
        }
    }
}