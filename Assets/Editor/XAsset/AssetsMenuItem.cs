// Decompiled with JetBrains decompiler
// Type: xasset.editor.AssetsMenuItem
// Assembly: xasset.editor, Version=1.0.7365.26779, Culture=neutral, PublicKeyToken=null
// MVID: 36063514-8E9B-4EA6-8B40-CE7BAE230DF8
// Assembly location: E:\WorkSpace\UnityProject\FrameWork\FrameWork\Assets\Editor\XAsset\xasset.editor.dll

using System;
using System.IO;
using Module;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace xasset.editor
{
    public static class AssetsMenuItem
    {
        private const string KBuildClear = "AssetBundles/Clear";
        private const string KBuildManifest = "AssetBundles/Build Manifest";
        private const string KBuildAssetBundles = "AssetBundles/Build AssetBundles";
        private const string KBuildPlayer = "AssetBundles/Build Player";
        private const string KMarkAssetsWithDir = "AssetBundles/按目录标记";
        private const string KMarkAssetsWithFile = "AssetBundles/按文件标记";
        private const string KMarkAssetsWithName = "AssetBundles/按名称标记";
        private const string KCopyPath = "Copy Path";
        private const string KMarkAssets = "标记资源";
        private const string KCopyToStreamingAssets = "AssetBundles/拷贝到StreamingAssets";
        public static string assetRootPath;

        [InitializeOnLoadMethod]
        private static void OnInitialize()
        {
            EditorUtility.ClearProgressBar();
            Settings settings = BuildScript.GetSettings();
            AssetsManifest manifest = BuildScript.GetManifest();
            if (settings.resourceSource == ResourceSource.ServerBundle && Application.platform == RuntimePlatform.WindowsEditor && manifest.channel == Channel.LocalDebug)
            {
                if (!LaunchLocalServer.IsRunning())
                    LaunchLocalServer.Run();
                Assets.dataPath = string.Empty;
            }
            else
            {
                if (LaunchLocalServer.IsRunning())
                    LaunchLocalServer.KillRunningAssetBundleServer();
                Assets.dataPath = Environment.CurrentDirectory;
            }

            Assets.source = settings.resourceSource;
            Assets.getPlatformDelegate = BuildScript.GetPlatformName;
            Assets.loadDelegate = AssetDatabase.LoadAssetAtPath;
            HotFixManager.codeSource = settings.codeSource;
        }

        [MenuItem("AssetBundles/openCache")]
        private static void OpenPersidata()
        {
            System.Diagnostics.Process.Start(Application.persistentDataPath);
        }

        [MenuItem("AssetBundles/Clear")]
        private static void Clear()
        {
            BuildScript.Clear();
        }

//        [MenuItem("AssetBundles/Build Manifest")]
//        private static void BuildManifest()
//        {
//            BuildScript.BuildManifest();
//        }
//
        [MenuItem("Setting/Build AssetBundles")]
        private static void BuildAssetBundles()
        {
            BuildScript.BuildManifest();
            BuildScript.BuildAssetBundles();
        }

//        [MenuItem("AssetBundles/Build Player")]
//        private static void BuildStandalonePlayer()
//        {
//            BuildScript.BuildStandalonePlayer();
//        }

//        [MenuItem("AssetBundles/拷贝到StreamingAssets")]
//        private static void CopyAssetBundles()
//        {
//            BuildScript.CopyAssetBundlesTo(Path.Combine(Application.streamingAssetsPath, "AssetBundles"));
//            AssetDatabase.Refresh();
//        }

        [MenuItem("AssetBundles/按目录标记")]
        private static void MarkAssetsWithDir()
        {
            BuildScript.GetSettings();
            AssetsManifest manifest = BuildScript.GetManifest();
            Object[] filtered = Selection.GetFiltered<Object>(SelectionMode.DeepAssets);
            for (int index = 0; index < filtered.Length; ++index)
            {
                string assetPath = AssetDatabase.GetAssetPath(filtered[index]);
                if (!Directory.Exists(assetPath) && !assetPath.EndsWith(".cs", StringComparison.CurrentCulture))
                {
                    if (!EditorUtility.DisplayCancelableProgressBar("标记资源", assetPath, index * 1f / filtered.Length))
                    {
                        string str = Path.GetDirectoryName(assetPath).Replace("\\", "/") + "_g";
                        BuildScript.SetAssetBundleNameAndVariant(assetPath, str.ToLower(), null);
                    }
                    else
                        break;
                }
            }

            EditorUtility.SetDirty(manifest);
            AssetDatabase.SaveAssets();
            EditorUtility.ClearProgressBar();
        }

        [MenuItem("AssetBundles/按文件标记")]
        private static void MarkAssetsWithFile()
        {
            BuildScript.GetSettings();
            AssetsManifest manifest = BuildScript.GetManifest();
            Object[] filtered = Selection.GetFiltered<Object>(SelectionMode.DeepAssets);
            for (int index = 0; index < filtered.Length; ++index)
            {
                string assetPath = AssetDatabase.GetAssetPath(filtered[index]);
                if (!Directory.Exists(assetPath) && !assetPath.EndsWith(".cs", StringComparison.CurrentCulture))
                {
                    if (!EditorUtility.DisplayCancelableProgressBar("标记资源", assetPath, index * 1f / filtered.Length))
                    {
                        string directoryName = Path.GetDirectoryName(assetPath);
                        string withoutExtension = Path.GetFileNameWithoutExtension(assetPath);
                        if (directoryName != null)
                        {
                            string path1 = directoryName.Replace("\\", "/") + "/";
                            if (withoutExtension != null)
                            {
                                
                                //string str = Path.Combine(path1, withoutExtension).Replace("\\", "/"); todo
                                string str = string.Join("/", path1, withoutExtension);//Path.Combine(path1, withoutExtension).Replace("\\", "/");
                                BuildScript.SetAssetBundleNameAndVariant(assetPath, str.ToLower(), null);
                            }
                        }
                    }
                    else
                        break;
                }
            }

            EditorUtility.SetDirty(manifest);
            AssetDatabase.SaveAssets();
            EditorUtility.ClearProgressBar();
        }

        [MenuItem("AssetBundles/按名称标记")]
        private static void MarkAssetsWithName()
        {
            BuildScript.GetSettings();
            Object[] filtered = Selection.GetFiltered<Object>(SelectionMode.DeepAssets);
            AssetsManifest manifest = BuildScript.GetManifest();
            for (int index = 0; index < filtered.Length; ++index)
            {
                string assetPath = AssetDatabase.GetAssetPath(filtered[index]);
                if (!Directory.Exists(assetPath) && !assetPath.EndsWith(".cs", StringComparison.CurrentCulture))
                {
                    if (!EditorUtility.DisplayCancelableProgressBar("标记资源", assetPath, index * 1f / filtered.Length))
                    {
                        string withoutExtension = Path.GetFileNameWithoutExtension(assetPath);
                        BuildScript.SetAssetBundleNameAndVariant(assetPath, withoutExtension.ToLower(), null);
                    }
                    else
                        break;
                }
            }

            EditorUtility.SetDirty(manifest);
            AssetDatabase.SaveAssets();
            EditorUtility.ClearProgressBar();
        }

        [MenuItem("AssetBundles/Copy Path")]
        private static void CopyPath()
        {
            string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            EditorGUIUtility.systemCopyBuffer = assetPath;
            Debug.Log(assetPath);
        }
    }
}