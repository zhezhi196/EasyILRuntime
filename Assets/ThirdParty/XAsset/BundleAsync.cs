// Decompiled with JetBrains decompiler
// Type: xasset.BundleAsync
// Assembly: xasset, Version=1.0.7365.4552, Culture=neutral, PublicKeyToken=null
// MVID: 4D869495-BD31-47B9-91C2-89825580533B
// Assembly location: E:\WorkSpace\UnityProject\FrameWork\FrameWork\Assets\Editor\XAsset\xasset.dll

using UnityEngine;

namespace xasset
{
    public class BundleAsync : Bundle
    {
        private AssetBundleCreateRequest _request;

        public override bool isDone
        {
            get
            {
                if (loadState == LoadState.Init)
                    return false;
                if (loadState == LoadState.Loaded)
                    return true;
                if (loadState == LoadState.LoadAssetBundle && _request.isDone)
                {
                    asset = _request.assetBundle;
                    if (_request.assetBundle == null)
                        error = string.Format("unable to load assetBundle:{0}", name);
                    loadState = LoadState.Loaded;
                }
                return _request == null || _request.isDone;
            }
        }

        public override float progress
        {
            get
            {
                return _request != null ? _request.progress : 0.0f;
            }
        }

        internal override void Load()
        {
            _request = AssetBundle.LoadFromFileAsync(name);
            if (_request == null)
                error = name + " LoadFromFile failed.";
            else
                loadState = LoadState.LoadAssetBundle;
        }

        internal override void Unload()
        {
            if (_request != null)
                _request = null;
            loadState = LoadState.Unload;
            base.Unload();
        }
    }
}