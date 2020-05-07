// Decompiled with JetBrains decompiler
// Type: xasset.Reference
// Assembly: xasset, Version=1.0.7365.4552, Culture=neutral, PublicKeyToken=null
// MVID: 4D869495-BD31-47B9-91C2-89825580533B
// Assembly location: E:\WorkSpace\UnityProject\FrameWork\FrameWork\Assets\Editor\XAsset\xasset.dll

namespace xasset
{
    public class Reference
    {
        public int refCount;

        public bool IsUnused()
        {
            return this.refCount <= 0;
        }

        public void Retain()
        {
            ++this.refCount;
        }

        public void Release()
        {
            --this.refCount;
        }
    }
}