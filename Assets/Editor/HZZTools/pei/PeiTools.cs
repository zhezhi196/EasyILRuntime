using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Sirenix.OdinInspector.Editor;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine.Playables;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class PeiTools : Editor
{
    [MenuItem("GameObject/Pei/SelectCameraPoint", priority = 13)]
    private static void SelectCameraPoint()
    {
        Transform[] trans = Selection.activeTransform.GetComponentsInChildren<Transform>();
        Transform cameraTrans = null;
        if (trans.Length > 0)
        {
            for (int i = 0; i < trans.Length; i++)
            {
                if (trans[i].name == "CameraPoint")
                {
                    cameraTrans = trans[i];
                    break;
                }
            }
        }
        if (cameraTrans != null)
        {
            Selection.activeGameObject = cameraTrans.gameObject;
            EditorGUIUtility.PingObject(Selection.activeObject);
        }
    }
    [MenuItem("GameObject/Pei/SelectWeaponPoint", priority = 14)]
    private static void SelectWeaponPoint()
    {
        Transform[] trans = Selection.activeTransform.GetComponentsInChildren<Transform>();
        Transform cameraTrans = null;
        if (trans.Length > 0)
        {
            for (int i = 0; i < trans.Length; i++)
            {
                if (trans[i].name == "weapon")
                {
                    cameraTrans = trans[i];
                    break;
                }
            }
        }
        if (cameraTrans != null)
        {
            Selection.activeGameObject = cameraTrans.gameObject;
            EditorGUIUtility.PingObject(Selection.activeObject);
        }
    }
    [MenuItem("GameObject/Pei/LoadUI")]
    private static void LoadUIPrefab()
    {
        string[] dirs = Directory.GetDirectories($"Assets/Bundles/UI");
        for (int i = 0; i < dirs.Length; i++)
        {
            dirs[i] = dirs[i].Split('\\').Last();
            Debug.Log(dirs[i]);
        }
        OpenLoadUIWindow.names = dirs;
        OpenLoadUIWindow.parent = Selection.activeTransform;
        OpenLoadUIWindow.OpenWindow();
    }

    [MenuItem("Assets/Pei/CleanUpPlayableBind")]
    private static void CleanUpPlayableBind()
    {
        GameObject gob = Selection.activeGameObject as GameObject;
        if (gob != null)
        {
            var playable = gob.GetComponent<PlayableDirector>();
            CleanUpBind(playable);
        }
    }

    public static void CleanUpBind(PlayableDirector playable)
    {
        if (playable == null) return;
        Dictionary<UnityEngine.Object, UnityEngine.Object> bindings = new Dictionary<UnityEngine.Object, UnityEngine.Object>();
        foreach (var pb in playable.playableAsset.outputs)
        {
            var key = pb.sourceObject;
            var value = playable.GetGenericBinding(key);
            if (key != null && !bindings.ContainsKey(key))
            {
                bindings.Add(key, value);
            }
        }

        var dirSO = new UnityEditor.SerializedObject(playable);
        var sceneBindings = dirSO.FindProperty("m_SceneBindings");
        for (var i = sceneBindings.arraySize - 1; i >= 0; i--)
        {
            var binding = sceneBindings.GetArrayElementAtIndex(i);
            var key = binding.FindPropertyRelative("key");
            if (key.objectReferenceValue == null || !bindings.ContainsKey(key.objectReferenceValue))
                sceneBindings.DeleteArrayElementAtIndex(i);
        }
        dirSO.ApplyModifiedProperties();
    }
}

public class OpenLoadUIWindow : OdinEditorWindow
{
    public static string[] names;
    public static Transform parent;
    static EditorWindow window;
    public static void OpenWindow()
    {
        window = GetWindow<OpenLoadUIWindow>("加载UI到场景");
        window.Show();
    }
    [LabelText("选择UI"), ValueDropdown("names")]
    public string ui;
    [Button("加载UI")]
    public void LoadUI()
    {
        if (!string.IsNullOrEmpty(ui))
        {
            string p = string.Format("Assets/Bundles/UI/{0}/{1}.prefab", ui, ui);
            GameObject gameObj = AssetDatabase.LoadAssetAtPath<GameObject>(string.Format("Assets/Bundles/UI/{0}/{1}.prefab", ui,ui));
            GameObject o = PrefabUtility.InstantiatePrefab(gameObj, parent) as GameObject;
        }
    }
}
public class SceneHierarchy : EditorWindow
{
    Vector2 scrollPos;
    static string[] scenesGUIDs;
    static string[] sceneName;
    static EditorWindow window;
    [MenuItem("Tools/裴亚龙专用/scene目录 _%#&s")]
    public static void ShowWindow()
    {
        window = EditorWindow.GetWindow(typeof(SceneHierarchy), false, "场景");
        window.Show();
        RefreshScene();
    }

    //bool layoutDone = false;
    private void OnGUI()
    {
        if (GUILayout.Button("刷新"))
        {
            RefreshScene();
        }
        if (sceneName != null)
        {
            EditorGUILayout.BeginVertical();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos,GUILayout.Width(window.position.width-10), GUILayout.Height(window.position.height-10));
            for (int i = 0; i < sceneName.Length; i++)
            {
                int index = i;
                if (GUILayout.Button(sceneName[i]))
                {
                    OpenScene(index);
                }
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }
    }

    [ContextMenu("刷新窗口")]
    private static void RefreshScene()
    {
        scenesGUIDs = AssetDatabase.FindAssets("t:Scene", new[] { "Assets/Bundles" ,"Assets/Scene"});

        sceneName = new string[scenesGUIDs.Length];
        for (int i = 0; i < scenesGUIDs.Length; i++)
        {
            string[] path = AssetDatabase.GUIDToAssetPath(scenesGUIDs[i]).Split(char.Parse("/"));
            sceneName[i] = path[path.Length - 1];
            sceneName[i] = sceneName[i].Split(char.Parse("."))[0];
        }
    }

    private void OpenScene(int index)
    {
        Scene activeScene = SceneManager.GetActiveScene();
        if (activeScene.isDirty)
        {
            if (EditorUtility.DisplayDialog("提示", "是否保存场景", "保存", "不"))
            {
                EditorSceneManager.SaveOpenScenes();
                EditorSceneManager.OpenScene(AssetDatabase.GUIDToAssetPath(scenesGUIDs[index]));
                return;
            }
        }
        EditorSceneManager.OpenScene(AssetDatabase.GUIDToAssetPath(scenesGUIDs[index]));
    }
}