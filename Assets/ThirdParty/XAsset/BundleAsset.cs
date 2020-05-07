// Decompiled with JetBrains decompiler
// Type: xasset.BundleAsset
// Assembly: xasset, Version=1.0.7365.4552, Culture=neutral, PublicKeyToken=null
// MVID: 4D869495-BD31-47B9-91C2-89825580533B
// Assembly location: E:\WorkSpace\UnityProject\FrameWork\FrameWork\Assets\Editor\XAsset\xasset.dll

using System.IO;

namespace xasset
{
    public class BundleAsset : Asset
    {
        protected readonly string assetBundleName;
        protected Bundle bundle;

        public BundleAsset(string bundle)
        {
            assetBundleName = bundle;
        }

        internal override void Load()
        {
            bundle = Bundles.Load(assetBundleName);
            asset = bundle.assetBundle.LoadAsset(Path.GetFileName(name), assetType);
        }

        internal override void Unload()
        {
            if (bundle != null)
            {
                bundle.Release();
                bundle = null;
            }
            asset = null;
        }
    }
}