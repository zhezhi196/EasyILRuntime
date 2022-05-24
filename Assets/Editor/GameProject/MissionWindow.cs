using System;
using System.Collections.Generic;
using System.IO;
using Module;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace EditorModule
{
    public class MissionWindow : EditorWindow
    {
        [MenuItem("Tools/策划工具/关卡编辑/打开场景编辑 _F3")]
        private static void OpenMissionEditor()
        {
            GetWindow<MissionWindow>();
        }

        public List<string> missionName = new List<string>();
        public List<string> nodeName = new List<string>();
        public List<string> sceneName = new List<string>();
        public int index;
        private MissionGraph selectMission;
        public TaskNode selectNode;

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
                        selectMission = AssetDatabase.LoadAssetAtPath<MissionGraph>($"Assets/{ConstKey.GetFolder(AssetLoad.AssetFolderType.Bundle)}/LevelEditor/{missionName[i]}.asset");
                        for (int j = 0; j < selectMission.nodes.Count; j++)
                        {
                            if (selectMission.nodes[j] is TaskNode)
                            {
                                nodeName.Add(selectMission.nodes[j].name);
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
                        for (int j = 0; j < selectMission.nodes.Count; j++)
                        {
                            if (selectMission.nodes[j].name == nodeName[i])
                            {
                                selectNode = selectMission.nodes[j] as TaskNode;
                                if (selectNode != null)
                                {
                                    for (int k = 0; k < selectNode.nodeSetting.loadScene.Count; k++)
                                    {
                                        sceneName.Add(selectNode.nodeSetting.loadScene[k].ToString());
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
                        NodeEditorInfo[] node = FindObjectsOfType<NodeEditorInfo>();
                        if (!node.IsNullOrEmpty())
                        {
                            for (int j = 0; j < node.Length; j++)
                            {
                                DestroyImmediate(node[j].gameObject);
                            }
                        }

                        Selection.activeObject = selectNode;

                        selectNode.CreatNewInfo();
                        Close();
                    }
                }
            }

            GUILayout.EndVertical();
            GUILayout.Space(30);
            GUILayout.BeginVertical();
            if (index > 0 && GUILayout.Button("返回"))
            {
                index--;
            }

            GUILayout.Space(30);
            if (GUILayout.Button("保存"))
            {
                NodeEditorInfo[] node = GameObject.FindObjectsOfType<NodeEditorInfo>();
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
        [MenuItem("Tools/策划工具/关卡编辑/开始游戏 _F4")]
        public static void StartGame()
        {
            if (EditorSceneManager.GetActiveScene().name != "GameStart")
            {
                NodeEditorInfo[] all = GameObject.FindObjectsOfType<NodeEditorInfo>();

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
    }
}