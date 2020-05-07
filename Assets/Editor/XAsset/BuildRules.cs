// Decompiled with JetBrains decompiler
// Type: xasset.editor.BuildRules
// Assembly: xasset.editor, Version=1.0.7365.26779, Culture=neutral, PublicKeyToken=null
// MVID: 36063514-8E9B-4EA6-8B40-CE7BAE230DF8
// Assembly location: E:\WorkSpace\UnityProject\FrameWork\FrameWork\Assets\Editor\XAsset\xasset.editor.dll

using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace xasset.editor
{
  [CreateAssetMenu]
  public class BuildRules : ScriptableObject
  {
    public Dictionary<string, string> bundlesFromRules = new Dictionary<string, string>();
    public Dictionary<string, HashSet<string>> tracker = new Dictionary<string, HashSet<string>>();
    public Dictionary<string, string[]> conflicted = new Dictionary<string, string[]>();
    [NonSerialized]
    public List<string> duplicated = new List<string>();
    public BuildRule[] rules;

    public bool ValidateAsset(string asset)
    {
      if (!asset.StartsWith("Assets/"))
        return false;
      string extension = Path.GetExtension(asset);
      return !(extension == ".dll") && !(extension == ".cs") && (!(extension == ".meta") && !(extension == ".js")) && !(extension == ".boo");
    }

    public static bool IsScene(string asset)
    {
      return asset.EndsWith(".unity");
    }

    public void Track(string asset, string bundle)
    {
      if (!this.tracker.ContainsKey(asset))
        this.tracker.Add(asset, new HashSet<string>()
        {
          bundle
        });
      this.tracker[asset].Add(bundle);
      if (this.tracker[asset].Count > 1 && string.IsNullOrEmpty(AssetDatabase.GetImplicitAssetBundleName(asset)))
        this.duplicated.Add(asset);
      Type mainAssetTypeAtPath = AssetDatabase.GetMainAssetTypeAtPath(asset);
      Debug.Log((object) ("Track:" + asset + " : " + mainAssetTypeAtPath.ToString()));
    }

    public IEnumerable<string> CheckTracker(string asset)
    {
      if (this.tracker.ContainsKey(asset))
        return (IEnumerable<string>) this.tracker[asset];
      return (IEnumerable<string>) new HashSet<string>();
    }

    public void Apply()
    {
      this.tracker.Clear();
      this.duplicated.Clear();
      this.conflicted.Clear();
      this.bundlesFromRules.Clear();
      foreach (BuildRule rule in this.rules)
        this.MarkAssetsByRelativeDirWith(rule.searchPath, rule.searchPattern, rule.searchOption, rule.searchDirOnly, rule.assetBundleName);
      float num = 0.0f;
      foreach (KeyValuePair<string, string> bundlesFromRule in this.bundlesFromRules)
      {
        if (!EditorUtility.DisplayCancelableProgressBar(nameof (Apply), string.Format("{0}/{1}", (object) num, (object) this.bundlesFromRules.Count), num / (float) this.bundlesFromRules.Count))
        {
          AssetImporter.GetAtPath(bundlesFromRule.Key).assetBundleName = bundlesFromRule.Key.EndsWith(".shader") ? "shaders" : bundlesFromRule.Value;
          ++num;
        }
        else
          break;
      }
      EditorUtility.ClearProgressBar();
      AssetDatabase.Refresh();
      AssetDatabase.RemoveUnusedAssetBundleNames();
      foreach (string allAssetBundleName in AssetDatabase.GetAllAssetBundleNames())
      {
        string[] pathsFromAssetBundle = AssetDatabase.GetAssetPathsFromAssetBundle(allAssetBundleName);
        if (Array.Exists<string>(pathsFromAssetBundle, new Predicate<string>(BuildRules.IsScene)) && !Array.TrueForAll<string>(pathsFromAssetBundle, new Predicate<string>(BuildRules.IsScene)))
          this.conflicted.Add(allAssetBundleName, pathsFromAssetBundle);
        foreach (string dependency in AssetDatabase.GetDependencies(pathsFromAssetBundle, true))
        {
          if (this.ValidateAsset(dependency))
            this.Track(dependency, allAssetBundleName);
        }
      }
    }

    public void Optimazie()
    {
      foreach (string asset in this.duplicated)
        BuildRules.OptimazieAsset(asset);
      foreach (KeyValuePair<string, string[]> keyValuePair in this.conflicted)
      {
        foreach (string asset in keyValuePair.Value)
        {
          if (!BuildRules.IsScene(asset))
            BuildRules.OptimazieAsset(asset);
        }
      }
    }

    private static void OptimazieAsset(string asset)
    {
      AssetImporter atPath = AssetImporter.GetAtPath(asset);
      if (asset.EndsWith(".shader"))
      {
        atPath.assetBundleName = "shaders";
      }
      else
      {
        string str = Path.GetDirectoryName(asset) + "/" + Path.GetFileNameWithoutExtension(asset);
        atPath.assetBundleName = str.Replace("\\", "/").ToLower();
      }
    }

    private void MarkAssetsByRelativeDirWith(
      string path,
      string searchPattern = "*",
      SearchOption searchOption = SearchOption.AllDirectories,
      bool searchDir = false,
      string assetBundleName = null)
    {
      int startIndex = path.LastIndexOf('/') + 1;
      foreach (string path1 in searchDir ? Directory.GetDirectories(path, searchPattern, searchOption) : Directory.GetFiles(path, searchPattern, searchOption))
      {
        if (string.IsNullOrEmpty(assetBundleName))
        {
          string withoutExtension = Path.GetFileNameWithoutExtension(path1);
          assetBundleName = (Path.GetDirectoryName(path1).Substring(startIndex) + "/" + withoutExtension).Replace("\\", "/").ToLower();
        }
        this.bundlesFromRules[path1] = assetBundleName;
      }
    }
  }
}
