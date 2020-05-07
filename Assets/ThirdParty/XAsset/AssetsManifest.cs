// Decompiled with JetBrains decompiler
// Type: xasset.AssetsManifest
// Assembly: xasset, Version=1.0.7365.4552, Culture=neutral, PublicKeyToken=null
// MVID: 4D869495-BD31-47B9-91C2-89825580533B
// Assembly location: E:\WorkSpace\UnityProject\FrameWork\FrameWork\Assets\Editor\XAsset\xasset.dll

using UnityEngine;

namespace xasset
{
    public enum Channel
    {
        LocalDebug,
        RemoteServer,
        AppStore,
        GooglePlay
    }
    public class AssetsManifest : ScriptableObject
    {
        public string remoteServer = "";
        public string localServer = "";
        public Channel channel;
        public string[] activeVariants = new string[0];
        [HideInInspector]
        public string[] bundles = new string[0];
        [HideInInspector]
        public string[] dirs = new string[0];
        [HideInInspector]
        public AssetData[] assets = new AssetData[0];
        
    }
}