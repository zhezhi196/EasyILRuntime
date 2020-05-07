// Decompiled with JetBrains decompiler
// Type: xasset.SceneAssetAsync
// Assembly: xasset, Version=1.0.7365.4552, Culture=neutral, PublicKeyToken=null
// MVID: 4D869495-BD31-47B9-91C2-89825580533B
// Assembly location: E:\WorkSpace\UnityProject\FrameWork\FrameWork\Assets\Editor\XAsset\xasset.dll

using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace xasset
{
  public class SceneAssetAsync : SceneAsset
  {
    private AsyncOperation _request;

    public SceneAssetAsync(string path, bool addictive)
      : base(path, addictive)
    {
    }

    public override float progress
    {
      get
      {
        if (bundle == null)
          return _request == null ? 0.0f : _request.progress;
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

    public override bool isDone
    {
      get
      {
        switch (loadState)
        {
          case LoadState.LoadAssetBundle:
            if (bundle == null || bundle.error != null)
              return true;
            int index1 = 0;
            for (int count = bundle.dependencies.Count; index1 < count; ++index1)
            {
              if (bundle.dependencies[index1].error != null)
                return true;
            }
            if (!bundle.isDone)
              return false;
            int index2 = 0;
            for (int count = bundle.dependencies.Count; index2 < count; ++index2)
            {
              if (!bundle.dependencies[index2].isDone)
                return false;
            }
            _request = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
            loadState = LoadState.LoadAsset;
            goto case LoadState.LoadAsset;
          case LoadState.LoadAsset:
          case LoadState.Unload:
            if (loadState != LoadState.LoadAsset || !_request.isDone)
              return false;
            loadState = LoadState.Loaded;
            return true;
          case LoadState.Loaded:
            return true;
          default:
            throw new ArgumentOutOfRangeException();
        }
      }
    }

    internal override void Load()
    {
      if (!string.IsNullOrEmpty(assetBundleName))
      {
        bundle = Bundles.LoadAsync(assetBundleName);
        loadState = LoadState.LoadAssetBundle;
      }
      else
      {
        _request = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
        loadState = LoadState.LoadAsset;
      }
    }

    internal override void Unload()
    {
      base.Unload();
      _request = null;
    }
  }
}
