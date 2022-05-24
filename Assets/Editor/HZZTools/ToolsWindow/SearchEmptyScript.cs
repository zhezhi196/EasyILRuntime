using System.Collections.Generic;
using System.IO;
using Module;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

namespace EditorModule
{
    public class SearchEmptyScript: OdinEditorWindow
    {
        int go_count = 0, components_count = 0, missing_count = 0;
        public GameObject[] testObject;
        public bool allAssets;
        [HideIf("allAssets")]
        public string[] path;
        
        [MenuItem("Tools/程序工具/查找空脚本")]
        public static void Open()
        {
            GetWindow<SearchEmptyScript>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            path = new[] {Application.dataPath + $"/{ConstKey.GetFolder(AssetLoad.AssetFolderType.Bundle)}"};
        }

        [Button]
        public void CheckAll()
        {
            List<GameObject> go = new List<GameObject>();
            if (!allAssets)
            {
                for (int i = 0; i < path.Length; i++)
                {
                    go.AddRange(CheckDir(path[i]));
                }
            }
            else
            {
                go.AddRange(CheckDir(Application.dataPath));
            }
            
            FindInSelected(go.ToArray());
        }
        [Button]
        public void CheckSelect()
        {
            GameObject[] go = ExtendUtil.IsNullOrEmpty(testObject) ? Selection.gameObjects : testObject;
            FindInSelected(go);
        }

        private List<GameObject> CheckDir(string dir)
        {
            List<GameObject> result = new List<GameObject>();
            
            DirectoryInfo tar = new DirectoryInfo(dir);
            FileInfo[] fileInfo = tar.GetFiles("*.prefab", SearchOption.AllDirectories);
            for (int i = 0; i < fileInfo.Length; i++)
            {
                string p = fileInfo[i].FullName.Substring(Application.dataPath.Length, fileInfo[i].FullName.Length-Application.dataPath.Length);
                string resultpath = "Assets" + p;
                result.Add(AssetDatabase.LoadAssetAtPath<GameObject>(resultpath));
            }

            return result;
        }

        private void FindInSelected(GameObject[] go)
        {
            go_count = 0;
            components_count = 0;
            missing_count = 0;
            foreach (GameObject g in go)
            {
                if (g != null)
                {
                    FindInGO(g);
                }
            }
            Debug.Log(string.Format("Searched {0} GameObjects, {1} components, found {2} missing", go_count, components_count, missing_count));
            AssetDatabase.Refresh();
        }

        private void FindInGO(GameObject g)
        {
            go_count++;
            Component[] components = g.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                components_count++;
                if (components[i] == null)
                {
                    missing_count++;
                    string s = g.name;
                    Transform t = g.transform;
                    while (t.parent != null)
                    {
                        s = t.parent.name + "/" + s;
                        t = t.parent;
                    }
                    DestroyImmediate(components[i]);
                    Debug.Log(s + " has an empty script attached in position: " + i, g);
                }
            }
            // Now recurse through each child GO (if there are any):
            foreach (Transform childT in g.transform)
            {
                //Debug.Log("Searching " + childT.id  + " " );
                FindInGO(childT.gameObject);
            }
        }
    }
}