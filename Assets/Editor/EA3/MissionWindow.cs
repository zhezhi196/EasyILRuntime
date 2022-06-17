using System;
using System.Collections.Generic;
using System.IO;
using Module;
using Project.Data;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EditorModule
{
    public class MissionWindow: EditorWindow
    {
        #region Props
        
        [MenuItem("GameObject/HZZ/Props Creat", false, 13)]
        public static void CreatProps()
        {
            CreatPropsCreator();
        }
        
        public static void CreatPropsCreator()
        {
            int temid = GetPropsID();
            if (temid != -1)
            {
                GameObject go = new GameObject();
                PropsCreator creator = go.AddComponent<PropsCreator>();
                creator.id = temid;
                UnityEditor.Selection.activeObject = go;
                NodeParent parent = NodeParent.CreatParent();
                creator.loadNodeID = parent.node.id;
                creator.initNodeID = parent.node.id;
                if (!parent.node.nextNode.IsNullOrEmpty() && parent.node.nextNode[0] is TaskNode ts)
                {
                    creator.unloadNodeID = ts.id;
                }

                var propsParent = parent.transform.FindOrNew("PropsParent").gameObject.AddOrGetComponent<PropsParent>();
                go.transform.SetParent(propsParent.transform);
                if (NodeParent.lastEditorObj != null) go.transform.position = NodeParent.lastEditorObj.position;
                creator.OnModelChanged();
            }
            else
            {
                GameDebug.LogError("请设置ID");
            }
        }
        private static int GetPropsID()
        {
            PropsCreator[] editors = GameObject.FindObjectsOfType<PropsCreator>();
            PropsParent parent = GameObject.FindObjectOfType<PropsParent>();
            if (parent != null && parent.idPool.x == parent.idPool.y)
            {
                return -1;
            }
            int id = parent != null ? parent.idPool.x : 0;
            if (!editors.IsNullOrEmpty())
            {
                while (editors.Contains(ed => ed.id == id))
                {
                    id++;
                    if (parent != null && id > parent.idPool.y)
                    {
                        return -1;
                    }
                }
            }

            return id;
        }

        #endregion

        #region Monster

        [MenuItem("GameObject/HZZ/Monster Creat", false, 13)]
        public static void CreatMonster()
        {
            int temid = GetMonsterID();
            if (temid != -1)
            {
                GameObject go = new GameObject();
                MonsterCreator creator = go.AddComponent<MonsterCreator>();
                creator.id = temid;
                go.name = $"{creator.id} {creator.modeName}";
                Selection.activeObject = go;
                NodeParent parent = NodeParent.CreatParent();
                var propsParent = parent.transform.FindOrNew("MonsterParent").gameObject
                    .AddOrGetComponent<MonsterParent>();
                creator.loadNodeID = parent.node.id;
                if (!parent.node.nextNode.IsNullOrEmpty() && parent.node.nextNode[0] is TaskNode ts)
                {
                    creator.unloadNodeID = ts.id;
                }

                go.transform.SetParent(propsParent.transform);
                if (NodeParent.lastEditorObj != null) go.transform.position = NodeParent.lastEditorObj.position;
            }

        }

        private static int GetMonsterID()
        {
            MonsterCreator[] editors = GameObject.FindObjectsOfType<MonsterCreator>();
            MonsterParent parent = GameObject.FindObjectOfType<MonsterParent>();
            int id = parent != null ? parent.idPool.x : 0;
            if (parent != null && parent.idPool.x == parent.idPool.y)
            {
                return -1;
            }
            if (!editors.IsNullOrEmpty())
            {
                while (editors.Contains(ed => ed.id == id))
                {
                    id++;
                    if (parent != null && id > parent.idPool.y)
                    {
                        return -1;
                    }
                }
            }
            return id;
        }

        #endregion
        
        [MenuItem("GameObject/HZZ/Player Creat", false, 13)]
        public static void CreatPlayer()
        {
            GameObject go = new GameObject();
            PlayerCreator creator = go.AddComponent<PlayerCreator>();
            Selection.activeObject = go;
            NodeParent parent = NodeParent.CreatParent();
            go.name = "Player Born Point";
            go.transform.SetParent(parent.transform);
            if (NodeParent.lastEditorObj != null) go.transform.position = NodeParent.lastEditorObj.position;
            //UnityEditor.EditorUtility.SetDirty(go);
        }

        private MissionGraph editorData;
        public TaskNode tn;
        private int _index;


        public int index
        {
            get { return _index; }
            set { _index = value; }
        }

        public List<string> missionName = new List<string>();
        
        public List<string> sceneName= new List<string>();

        public List<string> nodeName = new List<string>();


        [MenuItem("Tools/策划工具/关卡编辑/打开场景编辑 _F3")]
        private static void OpenMissionEditor()
        {
            GetWindow<MissionWindow>();
        }
        
        [MenuItem("Tools/策划工具/关卡编辑/开始游戏 _F4")]
        public static void StartGame()
        {
            if (EditorSceneManager.GetActiveScene().name != "GameStart")
            {
                NodeParent[] all = GameObject.FindObjectsOfType<NodeParent>();

                if (EditorUtility.DisplayDialog("确认", "是否保存编辑内容?", "保存", "不保存"))
                {
                    if (!all.IsNullOrEmpty())
                    {
                        for (int i = 0; i < all.Length; i++)
                        {
                            all[i].Save();
                        }
                    
                        for (int i = 0; i < all.Length; i++)
                        {
                            GameObject.DestroyImmediate(all[i].gameObject);
                        }
                        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                        EditorSceneManager.SaveOpenScenes();
                    }
                    EditorSceneManager.OpenScene($"Assets/{ConstKey.GetFolder(AssetLoad.AssetFolderType.Bundle)}/{ConstKey.GetFolder(AssetLoad.AssetFolderType.Scenes)}/GameStart.unity");
                }
                else
                {
                    if (!all.IsNullOrEmpty())
                    {
                        for (int i = 0; i < all.Length; i++)
                        {
                            GameObject.DestroyImmediate(all[i].gameObject);
                        }
                        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                        EditorSceneManager.SaveOpenScenes();
                    }
            
                    EditorSceneManager.OpenScene($"Assets/{ConstKey.GetFolder(AssetLoad.AssetFolderType.Bundle)}/{ConstKey.GetFolder(AssetLoad.AssetFolderType.Scenes)}/GameStart.unity");
                }
                AssetDatabase.Refresh();
            }

        }

        private void OnEnable()
        {
            DirectoryInfo dir = new DirectoryInfo($"{Application.dataPath}/{ConstKey.GetFolder(AssetLoad.AssetFolderType.Bundle)}/LevelEditor");
            FileInfo[] files = dir.GetFiles("*.asset");
            for (int i = 0; i < files.Length; i++)
            {
                missionName.Add(files[i].Name.Replace(".asset", string.Empty));
            }

        }

        private void OnGUI()
        {
            GUILayout.BeginVertical();
            if (index == 0)
            {
                for (int i = 0; i < missionName.Count; i++)
                {
                    if (GUILayout.Button(missionName[i]))
                    {
                        nodeName.Clear();
                        editorData = AssetDatabase.LoadAssetAtPath<MissionGraph>($"Assets/{ConstKey.GetFolder(AssetLoad.AssetFolderType.Bundle)}/LevelEditor/{missionName[i]}.asset");
                        for (int j = 0; j < editorData.nodes.Count; j++)
                        {
                            if (editorData.nodes[j] is TaskNode)
                            {
                                nodeName.Add(editorData.nodes[j].name);
                            }
                        }
                        index++;
                        return;
                    }
                }
            }

            if (index == 1)
            {
                for (int i = 0; i < nodeName.Count; i++)
                {
                    if (GUILayout.Button(nodeName[i]))
                    {
                        sceneName.Clear();
                        for (int j = 0; j < editorData.nodes.Count; j++)
                        {
                            if (editorData.nodes[j].name == nodeName[i])
                            {
                                tn = editorData.nodes[j] as TaskNode;
                                if (tn != null)
                                {
                                    for (int k = 0; k < tn.nodeSetting.loadScene.Count; k++)
                                    {
                                        sceneName.Add(tn.nodeSetting.loadScene[k].ToString());
                                    }
                                }
                            }
                        }

                        index++;
                        return;
                    }
                }
            }

            if (index == 2)
            {
                for (int i = 0; i < Enum.GetNames(typeof(SceneName)).Length; i++)
                {
                    string name = Enum.GetNames(typeof(SceneName))[i];
                    string buttonName = sceneName.Contains(name) ? name : name + "(不包含)";
                    if (GUILayout.Button(buttonName))
                    {
                        EditorSceneManager.OpenScene($"Assets/{ConstKey.GetFolder(AssetLoad.AssetFolderType.Bundle)}/{ConstKey.GetFolder(AssetLoad.AssetFolderType.Scenes)}/{name}.unity");
                        NodeParent[] node = FindObjectsOfType<NodeParent>();
                        if (!node.IsNullOrEmpty())
                        {
                            for (int j = 0; j < node.Length; j++)
                            {
                                DestroyImmediate(node[j].gameObject);
                            }
                        }

                        Selection.activeObject = tn;

                        tn.Edit();
                        Close();
                    }
                }
            }
            GUILayout.EndVertical();
            GUILayout.Space(30);
            GUILayout.BeginVertical();
            if (index > 0&&GUILayout.Button("返回"))
            {
                index--;
            }
            GUILayout.Space(30);
            if (GUILayout.Button("保存"))
            {
                NodeParent[] node = GameObject.FindObjectsOfType<NodeParent>();
                if (!node.IsNullOrEmpty())
                {
                    for (int i = 0; i < node.Length; i++)
                    {
                        node[i].Save();
                    }
                }
            }


            if (GUILayout.Button("开始游戏"))
            {
                StartGame();
            }
            GUILayout.EndVertical();
        }
    }
}