// Decompiled with JetBrains decompiler
// Type: xasset.editor.BuildScript
// Assembly: xasset.editor, Version=1.0.7365.26779, Culture=neutral, PublicKeyToken=null
// MVID: 36063514-8E9B-4EA6-8B40-CE7BAE230DF8
// Assembly location: E:\WorkSpace\UnityProject\FrameWork\FrameWork\Assets\Editor\XAsset\xasset.editor.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEditor;
using UnityEngine;

namespace xasset.editor
{
  public static class BuildScript
  {
    public static string overloadedDevelopmentServerURL = "";

    public static void Clear()
    {
      string[] assetBundleNames = AssetDatabase.GetAllAssetBundleNames();
      for (int index = 0; index < assetBundleNames.Length; ++index)
      {
        string assetBundleName = assetBundleNames[index];
        if (!EditorUtility.DisplayCancelableProgressBar("Clear AssetBundles ", string.Format("{0}/{1}", index, assetBundleNames.Length), index * 1f / assetBundleNames.Length))
          AssetDatabase.RemoveAssetBundleName(assetBundleName, true);
        else
          break;
      }
      EditorUtility.ClearProgressBar();
    }

    public static void CopyAssetBundlesTo(string outputPath)
    {
      if (!Directory.Exists(outputPath))
        Directory.CreateDirectory(outputPath);
      string platformName = GetPlatformName();
      string str1 = Path.Combine(Path.Combine(Environment.CurrentDirectory, AssetPath.AssetBundles), GetManifest().channel.ToString(), platformName);
      if (!Directory.Exists(str1))
        Debug.Log("No assetBundle output folder, try to build the assetBundles first.");
      string str2 = Path.Combine(outputPath, platformName);
      if (Directory.Exists(str2))
        FileUtil.DeleteFileOrDirectory(str2);
      FileUtil.CopyFileOrDirectory(str1, str2);
    }

    public static string GetPlatformName()
    {
      return GetPlatformForAssetBundles(EditorUserBuildSettings.activeBuildTarget);
    }

    private static string GetPlatformForAssetBundles(BuildTarget target)
    {
      switch (target)
      {
        case BuildTarget.StandaloneOSX:
          return "OSX";
        case BuildTarget.StandaloneWindows:
        case BuildTarget.StandaloneWindows64:
          return "Windows";
        case BuildTarget.iOS:
          return "iOS";
        case BuildTarget.Android:
          return "Android";
        case BuildTarget.WebGL:
          return "WebGL";
        default:
          return null;
      }
    }

    private static string[] GetLevelsFromBuildSettings()
    {
      return EditorBuildSettings.scenes.Select(scene => scene.path).ToArray();
    }

    private static string GetAssetBundleManifestFilePath()
    {
      return Path.Combine(Path.Combine(AssetPath.AssetBundles, GetPlatformName()), GetPlatformName()) + ".manifest";
    }

    public static void BuildStandalonePlayer()
    {
      string str = EditorUtility.SaveFolderPanel("Choose Location of the Built Game", "", "");
      if (str.Length == 0)
        return;
      string[] fromBuildSettings = GetLevelsFromBuildSettings();
      if (fromBuildSettings.Length == 0)
      {
        Debug.Log("Nothing to build.");
      }
      else
      {
        string buildTargetName = GetBuildTargetName(EditorUserBuildSettings.activeBuildTarget);
        if (buildTargetName == null)
          return;
        BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
          scenes = fromBuildSettings,
          locationPathName = str + buildTargetName,
          assetBundleManifestPath = GetAssetBundleManifestFilePath(),
          target = EditorUserBuildSettings.activeBuildTarget,
          options = EditorUserBuildSettings.development ? BuildOptions.Development : BuildOptions.None
        });
      }
    }

    public static string CreateAssetBundleDirectory()
    {
      AssetsManifest manifest = GetManifest();

      string path = Path.Combine(AssetPath.AssetBundles, manifest.channel.ToString(),GetPlatformName());
      if (!Directory.Exists(path))
        Directory.CreateDirectory(path);
      Debug.Log($"Build assetbundle channel: {manifest.channel} to path {path}");
      return path;
    }

    private static Dictionary<string, string> GetVersions(AssetBundleManifest manifest)
    {
      return manifest.GetAllAssetBundles().ToDictionary(item => item, item => manifest.GetAssetBundleHash(item).ToString());
    }

    private static void LoadVersions(string versionsTxt, IDictionary<string, string> versions)
    {
      if (versions == null)
        throw new ArgumentNullException(nameof (versions));
      if (!File.Exists(versionsTxt))
        return;
      using (StreamReader streamReader = new StreamReader(versionsTxt))
      {
        string str;
        while ((str = streamReader.ReadLine()) != null)
        {
          if (!(str == string.Empty))
          {
            string[] strArray = str.Split(':');
            if (strArray.Length > 1)
              versions.Add(strArray[0], strArray[1]);
          }
        }
      }
    }

    private static void SaveVersions(string versionsTxt, Dictionary<string, string> versions)
    {
      if (File.Exists(versionsTxt))
        File.Delete(versionsTxt);
      using (StreamWriter streamWriter = new StreamWriter(versionsTxt))
      {
        foreach (KeyValuePair<string, string> version in versions)
          streamWriter.WriteLine(version.Key + ":" + version.Value);
        streamWriter.Flush();
        streamWriter.Close();
      }
    }

    public static void RemoveUnusedAssetBundleNames()
    {
      AssetDatabase.RemoveUnusedAssetBundleNames();
    }

    public static void SetAssetBundleNameAndVariant(
      string assetPath,
      string bundleName,
      string variant)
    {
      AssetImporter atPath = AssetImporter.GetAtPath(assetPath);
      if (atPath == null)
        return;
      atPath.assetBundleName = bundleName;
      atPath.assetBundleVariant = variant;
    }

    public static void BuildManifest()
    {
      AssetsManifest manifest = GetManifest();
      AssetDatabase.RemoveUnusedAssetBundleNames();
      string[] assetBundleNames = AssetDatabase.GetAllAssetBundleNames();
      List<string> stringList = new List<string>();
      List<AssetData> assetDataList = new List<AssetData>();
      for (int index = 0; index < assetBundleNames.Length; ++index)
      {
        foreach (string path in AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleNames[index]))
        {
          string dir = Path.GetDirectoryName(path);
          int num = stringList.FindIndex(o => o.Equals(dir));
          if (num == -1)
          {
            num = stringList.Count;
            stringList.Add(dir);
          }
          assetDataList.Add(new AssetData
          {
            bundle = index,
            dir = num,
            name = Path.GetFileName(path)
          });
        }
      }
      manifest.bundles = assetBundleNames;
      manifest.dirs = stringList.ToArray();
      manifest.assets = assetDataList.ToArray();
      string assetPath = AssetDatabase.GetAssetPath(manifest);
      string lower = Path.GetFileNameWithoutExtension(assetPath).ToLower();
      SetAssetBundleNameAndVariant(assetPath, lower, null);
      EditorUtility.SetDirty(manifest);
      AssetDatabase.SaveAssets();
      AssetDatabase.Refresh();
    }

    public static void BuildAssetBundles()
    {
      string outputPath = CreateAssetBundleDirectory();
      AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
      string versionsTxt = outputPath + "/versions.txt";
       Dictionary<string, string> dictionary = new Dictionary<string, string>();
      LoadVersions(versionsTxt, dictionary);
      Dictionary<string, string> buildVersions = GetVersions(manifest);
      List<string> stringList = new List<string>();
      foreach (KeyValuePair<string, string> keyValuePair in buildVersions)
      {
        bool flag = true;
        string str;
        if (dictionary.TryGetValue(keyValuePair.Key, out str) && str.Equals(keyValuePair.Value))
          flag = false;
        if (flag)
          stringList.Add(keyValuePair.Key);
      }
      if (stringList.Count > 0)
      {
        using (StreamWriter streamWriter = new StreamWriter(File.Open(outputPath + "/updates.txt", FileMode.Append)))
        {
          streamWriter.WriteLine(DateTime.Now.ToFileTime() + ":");
          foreach (string str in stringList)
            streamWriter.WriteLine(str);
          streamWriter.Flush();
          streamWriter.Close();
        }
        SaveVersions(versionsTxt, buildVersions);
      }
      else
        Debug.Log("nothing to update.");
      string[] ignoredFiles = new string[4]
      {
        GetPlatformName(),
        "versions.txt",
        "updates.txt",
        "manifest"
      };
      List<string> list = Directory.GetFiles(outputPath, "*", SearchOption.AllDirectories).Select(t => new
      {
        t,
        file = t.Replace('\\', '/').Replace(outputPath.Replace('\\', '/') + "/", "")
      }).Where(_param1 =>
      {
        if (!_param1.file.EndsWith(".manifest", StringComparison.Ordinal))
          return !Array.Exists(ignoredFiles, s => s.Equals(_param1.file));
        return false;
      }).Where(_param1 => !buildVersions.ContainsKey(_param1.file)).Select(_param1 => _param1.t).ToList();
      foreach (string path in list)
      {
        if (File.Exists(path))
        {
          File.Delete(path);
          File.Delete(path + ".manifest");
        }
      }
      list.Clear();
    }

    private static string GetBuildTargetName(BuildTarget target)
    {
      string str = PlayerSettings.productName + "_" + PlayerSettings.bundleVersion;
      switch (target)
      {
        case BuildTarget.StandaloneOSX:
          return "/" + str + ".app";
        case BuildTarget.StandaloneWindows:
        case BuildTarget.StandaloneWindows64:
          return "/" + str + PlayerSettings.Android.bundleVersionCode + ".exe";
        case BuildTarget.iOS:
        case BuildTarget.WebGL:
          return "";
        case BuildTarget.Android:
          return "/" + str + PlayerSettings.Android.bundleVersionCode + ".apk";
        default:
          Debug.Log("Target not implemented.");
          return null;
      }
    }

    private static T GetAsset<T>(string path) where T : ScriptableObject
    {
      T obj = AssetDatabase.LoadAssetAtPath<T>(path);
      if (obj == null)
      {
        obj = ScriptableObject.CreateInstance<T>();
        AssetDatabase.CreateAsset(obj, path);
        AssetDatabase.SaveAssets();
      }
      return obj;
    }

    public static Settings GetSettings()
    {
      return GetAsset<Settings>(AssetPath.AssetsSettingAsset);
    }

    public static AssetsManifest GetManifest()
    {
      return GetAsset<AssetsManifest>(AssetPath.AssetsManifestAsset);
    }

    public static string GetServerURL()
    {
      string str1;
      if (!string.IsNullOrEmpty(overloadedDevelopmentServerURL))
      {
        str1 = overloadedDevelopmentServerURL;
      }
      else
      {
        string str2 = "";
        foreach (IPAddress address in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
        {
          if (address.AddressFamily == AddressFamily.InterNetwork)
          {
            str2 = address.ToString();
            break;
          }
        }
        str1 = "http://" + str2 + ":7888/";
      }
      return str1;
    }
  }
}
