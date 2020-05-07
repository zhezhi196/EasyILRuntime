// Decompiled with JetBrains decompiler
// Type: xasset.editor.MonoInstallationFinder
// Assembly: xasset.editor, Version=1.0.7365.26779, Culture=neutral, PublicKeyToken=null
// MVID: 36063514-8E9B-4EA6-8B40-CE7BAE230DF8
// Assembly location: E:\WorkSpace\UnityProject\FrameWork\FrameWork\Assets\Editor\XAsset\xasset.editor.dll

using System.IO;
using UnityEditor;
using UnityEngine;

namespace xasset.editor
{
    internal static class MonoInstallationFinder
    {
        private static string GetFrameWorksFolder()
        {
            string applicationPath = EditorApplication.applicationPath;
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform != RuntimePlatform.OSXEditor)
                return Path.Combine(Path.GetDirectoryName(applicationPath), "Data");
            return Path.Combine(applicationPath, Path.Combine("Contents", "Frameworks"));
        }

        public static string GetProfileDirectory(BuildTarget target, string profile)
        {
            return Path.Combine(MonoInstallationFinder.GetMonoInstallation(), Path.Combine("lib", Path.Combine("mono", profile)));
        }

        public static string GetMonoInstallation()
        {
            return MonoInstallationFinder.GetMonoInstallation("Mono");
        }

        public static string GetMonoInstallation(string monoName)
        {
            return Path.Combine(MonoInstallationFinder.GetFrameWorksFolder(), monoName);
        }
    }
}