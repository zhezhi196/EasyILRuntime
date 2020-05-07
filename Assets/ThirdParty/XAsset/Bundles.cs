// Decompiled with JetBrains decompiler
// Type: xasset.Bundles
// Assembly: xasset, Version=1.0.7365.4552, Culture=neutral, PublicKeyToken=null
// MVID: 4D869495-BD31-47B9-91C2-89825580533B
// Assembly location: E:\WorkSpace\UnityProject\FrameWork\FrameWork\Assets\Editor\XAsset\xasset.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace xasset
{
  public static class Bundles
  {
    private static readonly int MAX_LOAD_SIZE_PERFREME = 3;
    private static readonly List<Bundle> _bundles = new List<Bundle>();
    private static readonly List<Bundle> _unusedBundles = new List<Bundle>();
    private static readonly List<Bundle> _ready2Load = new List<Bundle>();
    private static readonly List<Bundle> _loading = new List<Bundle>();

    public static string[] activeVariants { get; set; }

    private static AssetBundleManifest manifest { get; set; }

    public static event OverrideDataPathDelegate OverrideBaseDownloadingUrl;

    public static string[] GetAllDependencies(string bundle)
    {
      return (Object) manifest == (Object) null ? null : manifest.GetAllDependencies(bundle);
    }

    public static void Initialize(Action onSuccess, Action<string> onError)
    {
      Bundle request = Load(Assets.platform, true, true);
      request.onComplete += (Action<Asset>) (_param1 =>
      {
        if (request.error != null && onError != null)
        {
          onError(request.error);
        }
        else
        {
          manifest = request.assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
          request.assetBundle.Unload(false);
          request.assetBundle = null;
          request.Release();
          request = null;
          if (onSuccess == null)
            return;
          onSuccess();
        }
      });
    }

    public static Bundle Load(string assetBundleName)
    {
      return Load(assetBundleName, false, false);
    }

    public static Bundle LoadAsync(string assetBundleName)
    {
      return Load(assetBundleName, false, true);
    }

    public static void Unload(Bundle bundle)
    {
      bundle.Release();
      for (int index = 0; index < _unusedBundles.Count; ++index)
      {
        Bundle unusedBundle = _unusedBundles[index];
        if (unusedBundle.name.Equals(bundle.name))
        {
          unusedBundle.Unload();
          _unusedBundles.RemoveAt(index);
          break;
        }
      }
    }

    public static void Unload(string assetBundleName)
    {
      int index = 0;
      for (int count = _bundles.Count; index < count; ++index)
      {
        Bundle bundle = _bundles[index];
        if (bundle.name.Equals(assetBundleName))
        {
          Unload(bundle);
          break;
        }
      }
    }

    private static void UnloadDependencies(Bundle bundle)
    {
      for (int index = 0; index < bundle.dependencies.Count; ++index)
        bundle.dependencies[index].Release();
      bundle.dependencies.Clear();
    }

    private static void LoadDependencies(Bundle bundle, string assetBundleName, bool asyncRequest)
    {
      string[] allDependencies = manifest.GetAllDependencies(assetBundleName);
      if (allDependencies.Length == 0)
        return;
      for (int index = 0; index < allDependencies.Length; ++index)
      {
        string assetBundleName1 = allDependencies[index];
        bundle.dependencies.Add(Load(assetBundleName1, false, asyncRequest));
      }
    }

    [Conditional("LOG_ENABLE")]
    private static void Log(string s)
    {
      Debug.Log("[Bundles]" + s);
    }

    private static Bundle Load(
      string assetBundleName,
      bool isLoadingAssetBundleManifest,
      bool asyncMode)
    {
      if (string.IsNullOrEmpty(assetBundleName))
      {
        Debug.LogError("assetBundleName == null");
        return null;
      }
      if (!isLoadingAssetBundleManifest)
      {
        if (manifest == null)
        {
          Debug.LogError("Please initialize AssetBundleManifest by calling Bundles.Initialize()");
          return null;
        }
        assetBundleName = RemapVariantName(assetBundleName);
      }
      string str = GetDataPath(assetBundleName) + assetBundleName;
      int index = 0;
      for (int count = _bundles.Count; index < count; ++index)
      {
        Bundle bundle = _bundles[index];
        if (bundle.name.Equals(str))
        {
          bundle.Retain();
          return bundle;
        }
      }
      Bundle bundle1;
      if (str.StartsWith("http://", StringComparison.Ordinal) || str.StartsWith("https://", StringComparison.Ordinal) || str.StartsWith("file://", StringComparison.Ordinal) || str.StartsWith("ftp://", StringComparison.Ordinal))
        bundle1 = new WebBundle
        {
          hash = ((Object) manifest != (Object) null ? manifest.GetAssetBundleHash(assetBundleName) : new Hash128()),
          cache = !isLoadingAssetBundleManifest
        };
      else
        bundle1 = asyncMode ? new BundleAsync() : new Bundle();
      bundle1.name = str;
      _bundles.Add(bundle1);
      if (MAX_LOAD_SIZE_PERFREME > 0 && (bundle1 is BundleAsync || bundle1 is WebBundle))
        _ready2Load.Add(bundle1);
      else
        bundle1.Load();
      if (!isLoadingAssetBundleManifest)
        LoadDependencies(bundle1, assetBundleName, asyncMode);
      bundle1.Retain();
      return bundle1;
    }

    private static string GetDataPath(string bundleName)
    {
      if (OverrideBaseDownloadingUrl == null)
        return Assets.assetBundleDataPath;
      foreach (OverrideDataPathDelegate invocation in OverrideBaseDownloadingUrl.GetInvocationList())
      {
        string str = invocation(bundleName);
        if (str != null)
          return str;
      }
      return Assets.assetBundleDataPath;
    }

    internal static void Update()
    {
      if (MAX_LOAD_SIZE_PERFREME > 0)
      {
        if (_ready2Load.Count > 0 && _loading.Count < MAX_LOAD_SIZE_PERFREME)
        {
          for (int index = 0; index < Math.Min(MAX_LOAD_SIZE_PERFREME - _loading.Count, _ready2Load.Count); ++index)
          {
            Bundle bundle = _ready2Load[index];
            if (bundle.loadState == LoadState.Init)
            {
              bundle.Load();
              _loading.Add(bundle);
              _ready2Load.RemoveAt(index);
              --index;
            }
          }
        }
        for (int index = 0; index < _loading.Count; ++index)
        {
          Bundle bundle = _loading[index];
          if (bundle.loadState == LoadState.Loaded || bundle.loadState == LoadState.Unload)
          {
            _loading.RemoveAt(index);
            --index;
          }
        }
      }
      for (int index = 0; index < _bundles.Count; ++index)
      {
        Bundle bundle = _bundles[index];
        if (!bundle.Update() && bundle.IsUnused())
        {
          _unusedBundles.Add(bundle);
          UnloadDependencies(bundle);
          _bundles.RemoveAt(index);
          --index;
        }
      }
      for (int index = 0; index < _unusedBundles.Count; ++index)
        _unusedBundles[index].Unload();
      _unusedBundles.Clear();
    }

    private static string RemapVariantName(string assetBundleName)
    {
      string[] bundlesWithVariant = manifest.GetAllAssetBundlesWithVariant();
      string str1 = assetBundleName.Split('.')[0];
      int num1 = int.MaxValue;
      int index1 = -1;
      for (int index2 = 0; index2 < bundlesWithVariant.Length; ++index2)
      {
        string[] strArray = bundlesWithVariant[index2].Split('.');
        string str2 = strArray[0];
        string str3 = strArray[1];
        if (!(str2 != str1))
        {
          int num2 = Array.IndexOf(activeVariants, str3);
          if (num2 == -1)
            num2 = 2147483646;
          if (num2 < num1)
          {
            num1 = num2;
            index1 = index2;
          }
        }
      }
      if (num1 == 2147483646)
        Debug.LogWarning("Ambiguous asset bundle variant chosen because there was no matching active variant: " + bundlesWithVariant[index1]);
      return index1 != -1 ? bundlesWithVariant[index1] : assetBundleName;
    }
  }
}
