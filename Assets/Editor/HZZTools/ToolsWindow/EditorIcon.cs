// using System;
// using System.Collections.Generic;
// using Module;
// using Sirenix.OdinInspector;
// using Sirenix.OdinInspector.Editor;
// using UnityEditor;
// using UnityEngine;
//
// namespace EditorModule
// {
//     public class EditorIcon: OdinEditorWindow
//     {
//         [MenuItem("Tools/策划工具/编辑动态加载图")]
//         public static void OpenWindow()
//         {
//             GetWindow<EditorIcon>();
//         }
//         [ListDrawerSettings(Expanded = true)]
//         public List<IconAsset> asset;
//         private SpriteLoader assetIOcon;
//         
//         protected override void OnEnable()
//         {
//             base.OnEnable();
//             assetIOcon = AssetDatabase.LoadAssetAtPath<SpriteLoader>("Assets/Config/Icon.asset");
//             asset = assetIOcon.iconInfo;
//         }
//
//         private void OnDisable()
//         {
//             assetIOcon.ReleaseText();
//         }
//     }
// }