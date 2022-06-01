using UnityEditor;
using UnityEngine;

namespace Module
{
    public static class Pathelper
    {
#if UNITY_EDITOR
        public static string FullAssetPath(Object tar)
        {
            return FullAssetPath(AssetDatabase.GetAssetPath(tar));
        }
        public static string FullAssetPath(string path)
        {
            path = path.Substring(7, path.Length - 7);
            return $"{Application.dataPath}/{path}";
        }

        public static string GetReleativePath(string path)
        {
            return "Assets/" + path.Substring(Application.dataPath.Length + 1, path.Length - Application.dataPath.Length - 1).Replace("\\", "/");
        }

        public static string GetAssetPath(string windowPath)
        {
            return windowPath.Substring(Application.dataPath.Length - 6, windowPath.Length - Application.dataPath.Length + 6);
        }
#endif
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