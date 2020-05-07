// Decompiled with JetBrains decompiler
// Type: xasset.Bundle
// Assembly: xasset, Version=1.0.7365.4552, Culture=neutral, PublicKeyToken=null
// MVID: 4D869495-BD31-47B9-91C2-89825580533B
// Assembly location: E:\WorkSpace\UnityProject\FrameWork\FrameWork\Assets\Editor\XAsset\xasset.dll

using System.Collections.Generic;
using UnityEngine;

namespace xasset
{
    public class Bundle : Asset
    {
        public readonly List<Bundle> dependencies = new List<Bundle>();

        public AssetBundle assetBundle
        {
            get
            {
                return asset as AssetBundle;
            }
            internal set
            {
                asset = value;
            }
        }

        internal override void Load()
        {
            asset = AssetBundle.LoadFromFile(name);
            if (!(assetBundle == null))
                return;
            error = name + " LoadFromFile failed.";
        }

        internal override void Unload()
        {
            if (assetBundle == null)
                return;
            assetBundle.Unload(true);
            assetBundle = null;
        }
    }
}