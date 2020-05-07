// Decompiled with JetBrains decompiler
// Type: xasset.BundleAssetAsync
// Assembly: xasset, Version=1.0.7365.4552, Culture=neutral, PublicKeyToken=null
// MVID: 4D869495-BD31-47B9-91C2-89825580533B
// Assembly location: E:\WorkSpace\UnityProject\FrameWork\FrameWork\Assets\Editor\XAsset\xasset.dll

using System;
using System.IO;
using UnityEngine;

namespace xasset
{
  public class BundleAssetAsync : BundleAsset
  {
    private AssetBundleRequest _request;

    public BundleAssetAsync(string bundle)
      : base(bundle)
    {
    }

    public override bool isDone
    {
      get
      {
        if (error != null || bundle.error != null)
          return true;
        int index1 = 0;
        for (int count = bundle.dependencies.Count; index1 < count; ++index1)
        {
          if (bundle.dependencies[index1].error != null)
            return true;
        }
        switch (loadState)
        {
          case LoadState.Init:
            return false;
          case LoadState.LoadAssetBundle:
            if (!bundle.isDone)
              return false;
            int index2 = 0;
            for (int count = bundle.dependencies.Count; index2 < count; ++index2)
            {
              if (!bundle.dependencies[index2].isDone)
                return false;
            }
            if (bundle.assetBundle == null)
            {
              error = "assetBundle == null";
              return true;
            }
            _request = bundle.assetBundle.LoadAssetAsync(Path.GetFileName(name), assetType);
            loadState = LoadState.LoadAsset;
            goto case LoadState.LoadAsset;
          case LoadState.LoadAsset:
          case LoadState.Unload:
            if (loadState != LoadState.LoadAsset || !_request.isDone)
              return false;
            asset = _request.asset;
            loadState = LoadState.Loaded;
            return true;
          case LoadState.Loaded:
            return true;
          default:
            throw new ArgumentOutOfRangeException();
        }
      }
    }

    public override float progress
    {
      get
      {
        float progress = bundle.progress;
        if (bundle.dependencies.Count <= 0)
          return (float) (progress * 0.3f + (_request != null ? _request.progress * 0.7f : 0.0));
        int index = 0;
        for (int count = bundle.dependencies.Count; index < count; ++index)
        {
          Bundle dependency = bundle.dependencies[index];
          progress += dependency.progress;
        }
        return (float) (progress / (double) (bundle.dependencies.Count + 1) * 0.3f + (_request != null ? _request.progress * 0.7f : 0.0));
      }
    }

    internal override void Load()
    {
      bundle = Bundles.LoadAsync(assetBundleName);
      loadState = LoadState.LoadAssetBundle;
    }

    internal override void Unload()
    {
      _request = null;
      loadState = LoadState.Unload;
      base.Unload();
    }
  }
}
