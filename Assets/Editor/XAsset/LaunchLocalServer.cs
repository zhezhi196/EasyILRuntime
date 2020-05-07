// Decompiled with JetBrains decompiler
// Type: xasset.editor.LaunchLocalServer
// Assembly: xasset.editor, Version=1.0.7365.26779, Culture=neutral, PublicKeyToken=null
// MVID: 36063514-8E9B-4EA6-8B40-CE7BAE230DF8
// Assembly location: E:\WorkSpace\UnityProject\FrameWork\FrameWork\Assets\Editor\XAsset\xasset.editor.dll

using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace xasset.editor
{
    internal class LaunchLocalServer : ScriptableSingleton<LaunchLocalServer>
    {
        [SerializeField] private int m_ServerPID;

        public static bool IsRunning()
        {
            if (instance.m_ServerPID == 0)
                return false;
            try
            {
                return !Process.GetProcessById(instance.m_ServerPID).HasExited;
            }
            catch
            {
                return false;
            }
        }

        public static void KillRunningAssetBundleServer()
        {
            try
            {
                if (instance.m_ServerPID == 0)
                    return;
                Process.GetProcessById(instance.m_ServerPID).Kill();
                instance.m_ServerPID = 0;
            }
            catch
            {
            }
        }

        public static void Run()
        {
            string fullPath = Path.GetFullPath(AssetPath.AssetServerPath);
            string str = Path.Combine(Environment.CurrentDirectory, AssetPath.AssetBundles);
            KillRunningAssetBundleServer();
            BuildScript.CreateAssetBundleDirectory();
            string arguments = string.Format("\"{0}\" {1}", str, Process.GetCurrentProcess().Id);
            ProcessStartInfo startInfoForMono = ExecuteInternalMono.GetProfileStartInfoForMono(MonoInstallationFinder.GetMonoInstallation("MonoBleedingEdge"), GetMonoProfileVersion(), fullPath, arguments, true);
            startInfoForMono.WorkingDirectory = str;
            startInfoForMono.UseShellExecute = false;
            Process process = Process.Start(startInfoForMono);
            if (process == null || process.HasExited || process.Id == 0)
                Debug.LogError("Unable Start AssetBundleServer process");
            else
                instance.m_ServerPID = process.Id;
        }

        private static string GetMonoProfileVersion()
        {
            return "3.5";
        }
    }
}