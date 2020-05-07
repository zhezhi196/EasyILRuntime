using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module
{
    public static class ApplicationPath
    {
        public static string GetDataPath()
        {
#if UNITY_ANDROID&&!UNITY_EDITOR
            return "jar:file://" + Application.dataPath + "!/assets/";
#elif UNITY_IOS&&!UNITY_EDITOR
            return Application.dataPath + "/Raw/";
#else
            return Application.dataPath;
#endif
        }

        public static string GetStreamingAssetsPath()
        {
#if UNITY_ANDROID&&!UNITY_EDITOR
            return "jar:file://" + Application.dataPath + "!/assets/";
#elif UNITY_IOS&&!UNITY_EDITOR
            return Application.dataPath + "/Raw/";
#else
            return Application.streamingAssetsPath;
#endif
        }
    }
}
