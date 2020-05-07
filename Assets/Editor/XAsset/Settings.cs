// Decompiled with JetBrains decompiler
 // Type: xasset.editor.Settings
 // Assembly: xasset.editor, Version=1.0.7365.26779, Culture=neutral, PublicKeyToken=null
 // MVID: 36063514-8E9B-4EA6-8B40-CE7BAE230DF8
 // Assembly location: E:\WorkSpace\UnityProject\FrameWork\FrameWork\Assets\Editor\XAsset\xasset.editor.dll

using Module;
using UnityEngine;
 using UnityEngine.Serialization;

namespace xasset.editor
 {
     public class Settings : ScriptableObject
     {
         public ResourceSource resourceSource;
         public CodeSource codeSource;
         //public bool localServer;
     }
 }