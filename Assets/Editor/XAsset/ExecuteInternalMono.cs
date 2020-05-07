// Decompiled with JetBrains decompiler
// Type: xasset.editor.ExecuteInternalMono
// Assembly: xasset.editor, Version=1.0.7365.26779, Culture=neutral, PublicKeyToken=null
// MVID: 36063514-8E9B-4EA6-8B40-CE7BAE230DF8
// Assembly location: E:\WorkSpace\UnityProject\FrameWork\FrameWork\Assets\Editor\XAsset\xasset.editor.dll

using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace xasset.editor
{
  internal class ExecuteInternalMono
  {
    private static readonly Regex UnsafeCharsWindows = new Regex("[^A-Za-z0-9/_\\-\\.\\:\\,\\/\\@\\\\]");
    private static readonly Regex UnescapeableChars = new Regex("[\\x00-\\x08\\x10-\\x1a\\x1c-\\x1f\\x7f\\xff]");
    private static readonly Regex Quotes = new Regex("\"");

    public static string PrepareFileName(string input)
    {
      if (Application.platform == RuntimePlatform.OSXEditor)
        return ExecuteInternalMono.EscapeCharsQuote(input);
      return ExecuteInternalMono.EscapeCharsWindows(input);
    }

    public static string EscapeCharsQuote(string input)
    {
      if (input.IndexOf('\'') == -1)
        return "'" + input + "'";
      if (input.IndexOf('"') == -1)
        return "\"" + input + "\"";
      return (string) null;
    }

    public static string EscapeCharsWindows(string input)
    {
      if (input.Length == 0)
        return "\"\"";
      if (ExecuteInternalMono.UnescapeableChars.IsMatch(input))
      {
        Debug.LogWarning((object) "Cannot escape control characters in string");
        return "\"\"";
      }
      if (ExecuteInternalMono.UnsafeCharsWindows.IsMatch(input))
        return "\"" + ExecuteInternalMono.Quotes.Replace(input, "\"\"") + "\"";
      return input;
    }

    public static ProcessStartInfo GetProfileStartInfoForMono(
      string monodistribution,
      string profile,
      string executable,
      string arguments,
      bool setMonoEnvironmentVariables)
    {
      string str1 = ExecuteInternalMono.PathCombine(monodistribution, "bin", "mono");
      string str2 = ExecuteInternalMono.PathCombine(monodistribution, "lib", "mono", profile);
      if (Application.platform == RuntimePlatform.WindowsEditor)
        str1 = ExecuteInternalMono.PrepareFileName(str1 + ".exe");
      ProcessStartInfo processStartInfo = new ProcessStartInfo()
      {
        Arguments = ExecuteInternalMono.PrepareFileName(executable) + " " + arguments,
        CreateNoWindow = true,
        FileName = str1,
        RedirectStandardError = true,
        RedirectStandardOutput = true,
        WorkingDirectory = Application.dataPath + "/..",
        UseShellExecute = false
      };
      if (setMonoEnvironmentVariables)
      {
        processStartInfo.EnvironmentVariables["MONO_PATH"] = str2;
        processStartInfo.EnvironmentVariables["MONO_CFG_DIR"] = ExecuteInternalMono.PathCombine(monodistribution, "etc");
      }
      return processStartInfo;
    }

    private static string PathCombine(params string[] parts)
    {
      string path1 = parts[0];
      for (int index = 1; index < parts.Length; ++index)
        path1 = Path.Combine(path1, parts[index]);
      return path1;
    }
  }
}
