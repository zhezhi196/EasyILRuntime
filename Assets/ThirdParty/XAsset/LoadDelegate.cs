// Decompiled with JetBrains decompiler
// Type: xasset.LoadDelegate
// Assembly: xasset, Version=1.0.7365.4552, Culture=neutral, PublicKeyToken=null
// MVID: 4D869495-BD31-47B9-91C2-89825580533B
// Assembly location: E:\WorkSpace\UnityProject\FrameWork\FrameWork\Assets\Editor\XAsset\xasset.dll

namespace xasset
{
    public delegate string GetPlatformDelegate();
    public delegate UnityEngine.Object LoadDelegate(string path, System.Type type);
    public delegate string OverrideDataPathDelegate(string bundleName);
}