// Decompiled with JetBrains decompiler
// Type: xasset.editor.BuildRulesEditor
// Assembly: xasset.editor, Version=1.0.7365.26779, Culture=neutral, PublicKeyToken=null
// MVID: 36063514-8E9B-4EA6-8B40-CE7BAE230DF8
// Assembly location: E:\WorkSpace\UnityProject\FrameWork\FrameWork\Assets\Editor\XAsset\xasset.editor.dll

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace xasset.editor
{
  [CustomEditor(typeof (BuildRules))]
  public class BuildRulesEditor : UnityEditor.Editor
  {
    public override void OnInspectorGUI()
    {
      BuildRules target = this.target as BuildRules;
      base.OnInspectorGUI();
      if (GUILayout.Button("收集"))
        target.Apply();
      if (GUILayout.Button("优化"))
        target.Optimazie();
      if (GUILayout.Button("清理"))
        BuildScript.Clear();
      EditorGUILayout.HelpBox("【规则说明】\n    - SearchPath：搜索路径\n    - SearchPattern：搜索模式\n    - SearchDir：是否只对文件夹搜索\n    - SearchOption：搜索选项顶层 or 递归\n    - AssetBundleName：搜索的资源全部用这个名字打包\n【操作说明】\n    - 收集：根据规则收集要打包的资源，收集后会把冗余和冲突的资源资源展示在对应的 Duplicated or Conflicted 栏目下\n    - 优化：对于收集到的 Duplicated 资源，进行独立打包\n    - 清理：对所有设置了 AssetBundleName的 资源进行清理\n【注意事项】：\n    - 所有shader放到一个包\n    - 场景文件不可以和非场景文件放到一个包\n    - 预知体通常单个文件一个包", MessageType.None);
      EditorGUILayout.LabelField("Duplicated", target.duplicated.Count.ToString());
      foreach (string str1 in target.duplicated)
      {
        IEnumerable<string> strings = target.CheckTracker(str1);
        string str2 = str1 + "\nIs auto-included in multiple bundles:\n";
        foreach (string str3 in strings)
          str2 = str2 + str3 + ", ";
        string message = str2.Substring(0, str2.Length - 2);
        EditorGUILayout.ObjectField(AssetDatabase.LoadAssetAtPath(str1, AssetDatabase.GetMainAssetTypeAtPath(str1)), AssetDatabase.GetMainAssetTypeAtPath(str1), false);
        EditorGUILayout.HelpBox(message, MessageType.Warning);
      }
      EditorGUILayout.LabelField("Conflicted", target.conflicted.Count.ToString());
      foreach (KeyValuePair<string, string[]> keyValuePair in target.conflicted)
      {
        EditorGUILayout.HelpBox(keyValuePair.Key + "\nThis bundle has pulled in scenes and non-scene assets.  A bundle must only have one type or the other.", MessageType.Error);
        foreach (string str in keyValuePair.Value)
        {
          EditorGUILayout.ObjectField(AssetDatabase.LoadAssetAtPath(str, AssetDatabase.GetMainAssetTypeAtPath(str)), AssetDatabase.GetMainAssetTypeAtPath(str), false);
          string fileName = Path.GetFileName(str);
          EditorGUILayout.HelpBox(!BuildRules.IsScene(str) ? fileName + " Is included in a bundle with a scene. Scene bundles must have only one or more scene assets." : fileName + " Is a scene that is in a bundle with non-scene assets. Scene bundles must have only one or more scene assets.", MessageType.Warning);
        }
      }
    }
  }
}
