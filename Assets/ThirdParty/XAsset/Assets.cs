// Decompiled with JetBrains decompiler
// Type: xasset.Assets
// Assembly: xasset, Version=1.0.7365.4552, Culture=neutral, PublicKeyToken=null
// MVID: 4D869495-BD31-47B9-91C2-89825580533B
// Assembly location: E:\WorkSpace\UnityProject\FrameWork\FrameWork\Assets\Editor\XAsset\xasset.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.Video;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace xasset
{
    /// <summary>
    /// 只跟资源有关系
    /// </summary>
    public enum ResourceSource
    {
        InnerBundle,
        ServerBundle
    }

    public class Assets : MonoBehaviour
    {
        public static ResourceSource source = ResourceSource.ServerBundle;
        public static LoadDelegate loadDelegate = null;
        public static GetPlatformDelegate getPlatformDelegate = null;
        private static string[] _bundles = new string[0];
        private static readonly Dictionary<string, int> _bundleAssets = new Dictionary<string, int>();
        private static readonly List<Asset> _assets = new List<Asset>();
        private static readonly List<Asset> _unusedAssets = new List<Asset>();

        public static Dictionary<string, int> bundleAssets
        {
            get { return _bundleAssets; }
        }

        /// <summary>
        /// streamingAsset
        /// </summary>
        public static string dataPath { get; set; }

        /// <summary>
        /// "Android"
        /// </summary>
        public static string platform { get; private set; }

        /// <summary>
        /// E:/WorkSpace/UnityProject/FrameWork/FrameWork/Assets/StreamingAssets\AssetBundles\Android\
        /// </summary>
        public static string assetBundleDataPath { get; private set; }

        /// <summary>
        /// http://127.0.0.1:7888/Android\
        /// </summary>
        public static string downloadURL { get; set; }

        /// <summary>
        /// file://E:/WorkSpace/UnityProject/FrameWork/FrameWork/Assets/StreamingAssets\AssetBundles\Android\
        /// </summary>
        public static string assetBundleDataPathURL { get; private set; }

        /// <summary>
        /// C:/Users/dELL/AppData/LocalLow/DefaultCompany/FrameWork\AssetBundles\Android\
        /// </summary>
        public static string updatePath { get; private set; }

        private static string GetPlatformForAssetBundles(RuntimePlatform platform)
        {
            switch (platform)
            {
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:
                    return "OSX";
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                    return "Windows";
                case RuntimePlatform.IPhonePlayer:
                    return "iOS";
                case RuntimePlatform.Android:
                    return "Android";
                case RuntimePlatform.WebGLPlayer:
                    return "WebGL";
                default:
                    return null;
            }
        }

        /// <summary>
        /// Application.persistentDataPath+ path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetRelativeUpdatePath(string path)
        {
            return updatePath + path;
        }

        /// <summary>
        /// http://127.0.0.1:7888/Android\+Filename
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string GetDownloadURL(string filename)
        {
            return downloadURL + filename;
        }

        /// <summary>
        /// file://E:/WorkSpace/UnityProject/FrameWork/FrameWork/Assets/StreamingAssets\AssetBundles\Android\+filename
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string GetAssetBundleDataPathURL(string filename)
        {
            return assetBundleDataPathURL + filename;
        }

        public static void Initialize(Action onSuccess, Action<string> onError)
        {
            if (FindObjectOfType<Assets>() == null)
            {
                GameObject go = new GameObject(nameof(Assets)).AddComponent<Assets>().gameObject;
                go.transform.SetParent(GameObject.Find("GamePlay").transform);
            }

            InitPaths();
            if (source == ResourceSource.ServerBundle)
            {
                InitBundles(onSuccess, onError);
            }
            else if (onSuccess != null)
            {
                onSuccess();
            }

        }

        private static void InitBundles(Action onSuccess, Action<string> onError)
        {
            Bundles.OverrideBaseDownloadingUrl += Bundles_overrideBaseDownloadingURL;
            Bundles.Initialize(() => LoadAsync(AssetPath.AssetsManifestAsset, typeof(AssetsManifest)).onComplete += (Action<Asset>) (obj =>
            {
                AssetsManifest manifest = obj.asset as AssetsManifest;
                Configuration.channer = manifest.channel;
                if ((Object) manifest == (Object) null)
                {
                    Action<string> action = onError;
                    if (action == null)
                        return;
                    action("manifest == null");
                }
                else
                {
                    Debug.Log($"渠道版本号: {manifest.channel}");

                    if (manifest.channel == Channel.LocalDebug || Application.platform != RuntimePlatform.WindowsEditor)
                    {
                        if (Application.platform == RuntimePlatform.WindowsEditor)
                        {
                            downloadURL = string.Join("/", "http://127.0.0.1:7888", manifest.channel, platform) + "/";
                        }
                        else
                        {
                            downloadURL = string.Join("/", manifest.localServer, manifest.channel, platform) + "/";
                        }
                    }
                    else
                    {
                        downloadURL = string.Join("/", manifest.remoteServer, manifest.channel, platform) + "/";
                    }

                    Debug.Log("下载地址做成: " + downloadURL);

                    Bundles.activeVariants = manifest.activeVariants;
                    _bundles = manifest.bundles;

                    string[] dirs=new string[manifest.dirs.Length];
                    for (int i = 0; i < manifest.dirs.Length; i++)
                    {
                        dirs[i] = manifest.dirs[i].Replace("\\","/");
                    }
                    _bundleAssets.Clear();
                    int index = 0;
                    for (int length = manifest.assets.Length; index < length; ++index)
                    {
                        AssetData asset2 = manifest.assets[index];
                        _bundleAssets[string.Format("{0}/{1}", dirs[asset2.dir], asset2.name)] = asset2.bundle;
                    }

                    Action action = onSuccess;
                    if (action != null)
                        action();
                    obj.Release();
                }
            }), onError);
        }

        private static void InitPaths()
        {
            string str1 = string.Empty;
            if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor)
                str1 = "file://";
            else if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.WindowsPlayer)
                str1 = "file:///";
            if (string.IsNullOrEmpty(dataPath))
                dataPath = Application.streamingAssetsPath;
            platform = getPlatformDelegate != null ? getPlatformDelegate() : GetPlatformForAssetBundles(Application.platform);
            //string path2 = Path.Combine(AssetPath.AssetBundles, platform); todo
            string path2 = string.Join("/", AssetPath.AssetBundles, platform);
            
            //string str2 = Path.Combine(dataPath, path2); todo
            string str2 = string.Join("/", dataPath, path2);
            char directorySeparatorChar = Path.DirectorySeparatorChar;
            string str3 = directorySeparatorChar.ToString();
            assetBundleDataPath = str2 + str3;
            assetBundleDataPathURL = str1 + assetBundleDataPath;
            //string str4 = Path.Combine(Application.persistentDataPath, path2);

            string str4 = string.Join("/", Application.persistentDataPath, path2);// Path.Combine(, path2);
            directorySeparatorChar = Path.DirectorySeparatorChar;
            string str5 = directorySeparatorChar.ToString();
            updatePath = str4 + str5;
        }

        public static string[] GetAllDependencies(string path)
        {
            string assetBundleName;
            return GetAssetBundleName(path, out assetBundleName) ? Bundles.GetAllDependencies(assetBundleName) : null;
        }

        public static SceneAsset LoadScene(string path, bool async, bool addictive)
        {
            SceneAsset sceneAsset = async ? new SceneAssetAsync(path, addictive) : new SceneAsset(path, addictive);
            GetAssetBundleName(path, out sceneAsset.assetBundleName);
            sceneAsset.Load();
            sceneAsset.Retain();
            _assets.Add(sceneAsset);
            return sceneAsset;
        }

        public static void UnloadScene(string path)
        {
            int index = 0;
            for (int count = _assets.Count; index < count; ++index)
            {
                Asset asset = _assets[index];
                if (asset.name.Equals(path))
                {
                    Unload(asset);
                    break;
                }
            }
        }

        public static Asset Load(string path, Type type)
        {
            return Load(path, type, false);
        }

        public static Asset LoadAsync(string path, Type type)
        {
            return Load(path, type, true);
        }

        public static void Unload(Asset asset)
        {
            asset.Release();
            for (int index = 0; index < _unusedAssets.Count; ++index)
            {
                Asset unusedAsset = _unusedAssets[index];
                if (unusedAsset.name.Equals(asset.name))
                {
                    unusedAsset.Unload();
                    _unusedAssets.RemoveAt(index);
                    break;
                }
            }
        }

        private void Update()
        {
            for (int index = 0; index < _assets.Count; ++index)
            {
                Asset asset = _assets[index];
                if (!asset.Update() && asset.IsUnused())
                {
                    _unusedAssets.Add(asset);
                    _assets.RemoveAt(index);
                    --index;
                }
            }

            for (int index = 0; index < _unusedAssets.Count; ++index)
                _unusedAssets[index].Unload();
            _unusedAssets.Clear();
            Bundles.Update();
        }

        [Conditional("LOG_ENABLE")]
        private static void Log(string s)
        {
            Debug.Log(string.Format("[Assets]{0}", s));
        }

        private static Asset Load(string path, Type type, bool async)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("invalid path");
                return null;
            }

            int index = 0;
            for (int count = _assets.Count; index < count; ++index)
            {
                Asset asset = _assets[index];
                if (asset.name.Equals(path))
                {
                    asset.Retain();
                    return asset;
                }
            }

            string assetBundleName;
            Asset asset1 = !GetAssetBundleName(path, out assetBundleName)
                ? (!path.StartsWith("http://", StringComparison.Ordinal) && !path.StartsWith("https://", StringComparison.Ordinal) && (!path.StartsWith("file://", StringComparison.Ordinal) && !path.StartsWith("ftp://", StringComparison.Ordinal)) &&
                   !path.StartsWith("jar:file://", StringComparison.Ordinal)
                    ? new Asset()
                    : new WebAsset())
                : (async ? new BundleAssetAsync(assetBundleName) : new BundleAsset(assetBundleName));
            asset1.name = path;
            asset1.assetType = type;
            _assets.Add(asset1);
            asset1.Load();
            asset1.Retain();
            return asset1;
        }

        private static bool GetAssetBundleName(string path, out string assetBundleName)
        {
            if (path.Equals(AssetPath.AssetsManifestAsset))
            {
                assetBundleName = Path.GetFileNameWithoutExtension(path).ToLower();
                return true;
            }

            assetBundleName = null;
            int index;
            if (!_bundleAssets.TryGetValue(path, out index))
                return false;
            assetBundleName = _bundles[index];
            return true;
        }

        private static string Bundles_overrideBaseDownloadingURL(string bundleName)
        {
            return !File.Exists(GetRelativeUpdatePath(bundleName)) ? null : updatePath;
        }
    }
}