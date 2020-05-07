// Decompiled with JetBrains decompiler
// Type: xasset.WebBundle
// Assembly: xasset, Version=1.0.7365.4552, Culture=neutral, PublicKeyToken=null
// MVID: 4D869495-BD31-47B9-91C2-89825580533B
// Assembly location: E:\WorkSpace\UnityProject\FrameWork\FrameWork\Assets\Editor\XAsset\xasset.dll

using UnityEngine;

namespace xasset
{
    public class WebBundle : Bundle
    {
        private WWW _request;
        public bool cache;
        public Hash128 hash;

        public override string error
        {
            get
            {
                return _request != null ? _request.error : null;
            }
        }

        public override bool isDone
        {
            get
            {
                if (loadState == LoadState.Init)
                    return false;
                if (_request == null || loadState == LoadState.Loaded)
                    return true;
                if (_request.isDone)
                {
                    assetBundle = _request.assetBundle;
                    loadState = LoadState.Loaded;
                }
                return _request.isDone;
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
            _request = cache ? WWW.LoadFromCacheOrDownload(name, hash) : new WWW(name);
            loadState = LoadState.LoadAssetBundle;
        }

        internal override void Unload()
        {
            if (_request != null)
            {
                _request.Dispose();
                _request = null;
            }
            loadState = LoadState.Unload;
            base.Unload();
        }
    }
}