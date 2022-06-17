using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace EditorModule
{
    public enum GetComponentType
    {
        [LabelText("音效播放器")] AudioSource,
    }

    [Serializable]
    public class GetPrefabComponentUnit
    {
        [HideLabel,InlineButton("Select","选中")]
        public GameObject gameObject;
        
        private void Select()
        {
            EditorGUIUtility.PingObject(gameObject);
            AssetDatabase.OpenAsset(gameObject);
            UnityEditor.Selection.activeObject = gameObject;
        }
        
    }
    public class GetPrefabComponentWindow : OdinEditorWindow
    {
        [SerializeField,LabelText("获取类型")]
        private GetComponentType type;

        [MenuItem("Tools/策划工具/获取拥有某个组件的所有预制体")]
        private static void OpenWindiow()
        {
            GetWindow<GetPrefabComponentWindow>();
        }
        [SerializeField,LabelText("筛选过的队列")]
        List<GetPrefabComponentUnit> filteredList = new List<GetPrefabComponentUnit>();

        [Button("搜索")]
        private void OnSearch()
        {
            var rawObjList = GetAllPrefabs();
            Type componentType = GetComponentType();
            filteredList = new List<GetPrefabComponentUnit>(); 

            foreach (var gameObject in rawObjList)
            {
                if (gameObject.GetComponentsInChildren(componentType).Length > 0)
                {
                    filteredList.Add(new GetPrefabComponentUnit(){gameObject = gameObject});
                }
            }

            if (filteredList.Count == 0)
            {
                EditorUtility.DisplayDialog("", "没找到相关预制体","退出");
            }
        }

        [Button("更改所有AudioSource的Rolloff")]
        private void OnChangeAudioSourceRolloff()
        {
            if (type != EditorModule.GetComponentType.AudioSource || filteredList.Count == 0)
            {
                EditorUtility.DisplayDialog("", "请确认选择的是AudioSource","退出");
                return;
            }

            foreach (var unit in filteredList)
            {
                var comp = unit.gameObject.GetComponentsInChildren<AudioSource>();
                foreach (var audioSource in comp)
                {
                    audioSource.rolloffMode = AudioRolloffMode.Linear;
                }
            }
            EditorUtility.DisplayDialog("", "更改完成！","退出");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private Type GetComponentType()
        {
            switch (type)
            {
                case EditorModule.GetComponentType.AudioSource:
                    return typeof(AudioSource);
                default: return null;
            }
        }
        
        
        private List<GameObject> GetAllPrefabs()
        {
            List<GameObject> prefabs = new List<GameObject>();
            var resourcesPath = Application.dataPath;
            var absolutePaths = System.IO.Directory.GetFiles(resourcesPath, "*.prefab", System.IO.SearchOption.AllDirectories);
            for (int i = 0; i < absolutePaths.Length; i++)
            {
                EditorUtility.DisplayProgressBar("获取预制体……", "获取预制体中……", (float)i/absolutePaths.Length);

                string path = "Assets" + absolutePaths[i].Remove(0, resourcesPath.Length);
                path = path.Replace("\\", "/");
                GameObject prefab = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
                if (prefab != null)
                    prefabs.Add(prefab);
                else
                    Debug.Log("预制体不存在！ "+path);
            }
            EditorUtility.ClearProgressBar();
            return prefabs;
        }
    }
}
