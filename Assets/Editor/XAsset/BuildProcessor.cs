// Decompiled with JetBrains decompiler
// Type: xasset.editor.BuildProcessor
// Assembly: xasset.editor, Version=1.0.7365.26779, Culture=neutral, PublicKeyToken=null
// MVID: 36063514-8E9B-4EA6-8B40-CE7BAE230DF8
// Assembly location: E:\WorkSpace\UnityProject\FrameWork\FrameWork\Assets\Editor\XAsset\xasset.editor.dll

using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace xasset.editor
{
  public class BuildProcessor : IPreprocessBuild, IOrderedCallback, IPostprocessBuild
  {
    public void OnPostprocessBuild(BuildTarget target, string path)
    {
      if (target != BuildTarget.iOS || Environment.OSVersion.Platform != PlatformID.MacOSX)
        return;
      foreach (string file in Directory.GetFiles(Path.Combine(Application.dataPath, "Editor/XAsset/Shells"), "*.sh", SearchOption.AllDirectories))
      {
        if (file != null)
        {
          string destFileName = Path.Combine(path, Path.GetFileName(file));
          File.Copy(file, destFileName, true);
        }
      }
      string[] strArray = PlayerSettings.applicationIdentifier.Split('.');
      string str1 = strArray[strArray.Length - 1];
      string str2 = EditorUserBuildSettings.development ? "develop" : "release";
      string str3 = string.Format("{0}-{1}-{2}", (object) str1, (object) PlayerSettings.bundleVersion, (object) str2);
      string str4 = EditorUserBuildSettings.iOSBuildConfigType.ToString();
      Process.Start("/bin/bash", Path.Combine(path, "OpenTerminal.sh") + " " + path + " " + str3 + " " + str2 + " " + str1 + " " + str4);
    }

    public void OnPreprocessBuild(BuildTarget target, string path)
    {
      BuildScript.CopyAssetBundlesTo(Path.Combine(Application.streamingAssetsPath, "AssetBundles"));
      string path1 = Path.Combine(Path.Combine(Application.streamingAssetsPath, "AssetBundles"), BuildScript.GetPlatformName());
      if (!Directory.Exists(path1))
        return;
      foreach (string file in Directory.GetFiles(path1, "*.manifest", SearchOption.AllDirectories))
      {
        FileInfo fileInfo = new FileInfo(file);
        if (fileInfo.Exists)
          fileInfo.Delete();
      }
      foreach (string file in Directory.GetFiles(path1, "*.meta", SearchOption.AllDirectories))
        new FileInfo(file).Delete();
    }

    public int callbackOrder
    {
      get
      {
        return 0;
      }
    }
  }
}
