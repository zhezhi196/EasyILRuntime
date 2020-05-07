// Decompiled with JetBrains decompiler
// Type: xasset.editor.ManifestInspector
// Assembly: xasset.editor, Version=1.0.7365.26779, Culture=neutral, PublicKeyToken=null
// MVID: 36063514-8E9B-4EA6-8B40-CE7BAE230DF8
// Assembly location: E:\WorkSpace\UnityProject\FrameWork\FrameWork\Assets\Editor\XAsset\xasset.editor.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace xasset.editor
{
  [CustomEditor(typeof (AssetsManifest))]
  public class ManifestInspector : Editor
  {
    private DateTime _lastModify = DateTime.MinValue;
    private Vector2 _scrollPosition = Vector2.zero;
    private string _manifestStr;
    private int _startLine;
    private const int DisplayLineNum = 60;
    private List<int> _lineIndex;

    private void OnEnable()
    {
      _lineIndex = new List<int>();
      BuildCahe();
    }

    private void OnDisable()
    {
      _manifestStr = null;
    }

    private void ResetManifest()
    {
      BuildScript.Clear();
      AssetsManifest manifest = BuildScript.GetManifest();
      manifest.assets = new AssetData[0];
      manifest.dirs = new string[0];
      manifest.bundles = new string[0];
      EditorUtility.SetDirty(manifest);
      AssetDatabase.SaveAssets();
      BuildCahe();
    }

    private void BuildCahe()
    {
      _lastModify = File.GetLastWriteTime(AssetPath.AssetsManifestAsset);
      AssetsManifest manifest = BuildScript.GetManifest();
      StringBuilder stringBuilder = new StringBuilder(512);
      for (int index = 0; index < manifest.bundles.Length; ++index)
        stringBuilder.AppendLine(string.Format("bundle[{0}]={1}", index, manifest.bundles[index]));
      stringBuilder.AppendLine();
      for (int index = 0; index < manifest.dirs.Length; ++index)
        stringBuilder.AppendLine(string.Format("dir[{0}]={1}", index, manifest.dirs[index]));
      stringBuilder.AppendLine();
      for (int index = 0; index < manifest.assets.Length; ++index)
      {
        AssetData asset = manifest.assets[index];
        string str = string.Format("asset[{0}] = bundle:{1}, dir:{2}, name:{3}", (object) index, (object) asset.bundle, (object) asset.dir, (object) asset.name);
        stringBuilder.AppendLine(str);
      }
      _manifestStr = stringBuilder.ToString();
      _lineIndex.Clear();
      _lineIndex.Add(-1);
      for (int index = 0; index < _manifestStr.Length; ++index)
      {
        if (_manifestStr[index] == '\n')
          _lineIndex.Add(index);
      }
    }

    public override void OnInspectorGUI()
    {
      if (_lastModify != File.GetLastWriteTime(AssetPath.AssetsManifestAsset))
        BuildCahe();
      serializedObject.UpdateIfRequiredOrScript();
      EditorGUILayout.PropertyField(serializedObject.FindProperty("remoteServer"), true);
      EditorGUILayout.PropertyField(serializedObject.FindProperty("localServer"), true);
      EditorGUILayout.PropertyField(serializedObject.FindProperty("channel"), true);
      EditorGUILayout.PropertyField(serializedObject.FindProperty("activeVariants"), true);
      serializedObject.ApplyModifiedProperties();
      GUILayout.Space(5f);
      GUILayout.BeginHorizontal();
      if (GUILayout.Button("<<", EditorStyles.label, GUILayout.MaxWidth(40f)))
        _startLine -= 60;
      _startLine = (int) GUILayout.HorizontalSlider(_startLine, 0.0f, _lineIndex.Count - 60 - 1);
      if (GUILayout.Button(">>", EditorStyles.label, GUILayout.MaxWidth(40f)))
        _startLine += 60;
      if (GUILayout.Button("clear", EditorStyles.toolbarButton, GUILayout.MaxWidth(60f)) && EditorUtility.DisplayDialog("Clear manifest!", "Do you really want to  clear the manifest?", "OK", "Cancel"))
        ResetManifest();
      GUILayout.Space(2f);
      if (GUILayout.Button("refresh", EditorStyles.toolbarButton, GUILayout.MaxWidth(60f)))
        BuildCahe();
      GUILayout.EndHorizontal();
      GUILayout.Space(5f);
      int val2 = _lineIndex.Count - 1;
      _startLine = Math.Max(Math.Min(_startLine, val2 - 60), 0);
      int startLine = _startLine;
      int index = Math.Min(_startLine + 60, val2);
      _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
      GUI.enabled = false;
      EditorGUILayout.TextArea(_manifestStr.Substring(_lineIndex[startLine] + 1, _lineIndex[index] - _lineIndex[startLine] - 1));
      GUI.enabled = true;
      GUILayout.EndScrollView();
    }
  }
}
