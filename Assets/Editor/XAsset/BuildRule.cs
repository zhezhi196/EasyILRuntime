// Decompiled with JetBrains decompiler
// Type: xasset.editor.BuildRule
// Assembly: xasset.editor, Version=1.0.7365.26779, Culture=neutral, PublicKeyToken=null
// MVID: 36063514-8E9B-4EA6-8B40-CE7BAE230DF8
// Assembly location: E:\WorkSpace\UnityProject\FrameWork\FrameWork\Assets\Editor\XAsset\xasset.editor.dll

using System;
using System.IO;

namespace xasset.editor
{
    [Serializable]
    public class BuildRule
    {
        public SearchOption searchOption = SearchOption.AllDirectories;
        public string searchPath;
        public string searchPattern;
        public string assetBundleName;
        public bool searchDirOnly;
    }
}