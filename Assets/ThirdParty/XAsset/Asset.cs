// Decompiled with JetBrains decompiler
// Type: xasset.Asset
// Assembly: xasset, Version=1.0.7365.4552, Culture=neutral, PublicKeyToken=null
// MVID: 4D869495-BD31-47B9-91C2-89825580533B
// Assembly location: E:\xasset-master\Assets\Plugins\XAsset\xasset.dll

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace xasset
{
    public class Asset : Reference, IEnumerator
    {
        private List<Object> _requires;
        public Type assetType;
        public string name;

        public LoadState loadState { get; protected set; }

        public Asset()
        {
            asset = null;
            loadState = LoadState.Init;
        }

        public virtual bool isDone
        {
            get { return true; }
        }

        public virtual float progress
        {
            get { return 1f; }
        }

        public virtual string error { get; protected set; }

        public string text { get; protected set; }

        public byte[] bytes { get; protected set; }

        public Object asset { get; internal set; }

        private bool checkRequires
        {
            get { return _requires != null; }
        }

        public void Require(Object obj)
        {
            if (_requires == null)
                _requires = new List<Object>();
            _requires.Add(obj);
            Retain();
        }

        public void Dequire(Object obj)
        {
            if (_requires == null || !_requires.Remove(obj))
                return;
            Release();
        }

        private void UpdateRequires()
        {
            for (int index = 0; index < _requires.Count; ++index)
            {
                if (!(_requires[index] != null))
                {
                    Release();
                    _requires.RemoveAt(index);
                    --index;
                }
            }

            if (_requires.Count != 0)
                return;
            _requires = null;
        }

        internal virtual void Load()
        {
            if (Assets.source == ResourceSource.ServerBundle || Assets.loadDelegate == null)
                return;
            asset = Assets.loadDelegate(name, assetType);
        }

        internal virtual void Unload()
        {
            if (asset == null)
                return;
            if (Assets.source != ResourceSource.ServerBundle && !(asset is GameObject))
                Resources.UnloadAsset(asset);
            asset = null;
        }

        internal bool Update()
        {
            if (checkRequires)
                UpdateRequires();
            if (!isDone)
                return true;
            if (onComplete == null)
                return false;
            try
            {
                onComplete(this);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }

            onComplete = null;
            return false;
        }

        public event Action<Asset> onComplete;
        public bool MoveNext()
        {
            return !isDone;
        }

        public void Reset()
        {
        }

        public object Current
        {
            get { return null; }
        }
    }
}