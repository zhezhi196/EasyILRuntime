using Module;
using UnityEditor;
using UnityEngine;
using Tools = Module.Tools;

namespace EditorModule
{
    public class MissionCtrl
    {
        [MenuItem("GameObject/HZZ/Monster Creat", false, 13)]
        public static void CreatMonster()
        {
            NodeEditorInfo editorInfo = GameObject.FindObjectOfType<NodeEditorInfo>();
            if (editorInfo != null)
            {
                MonsterCreator[] allMonster = editorInfo.transform.GetComponentsInChildren<MonsterCreator>(true);
                int nextId = Tools.GetID(allMonster);
                GameObject newGo = new GameObject(nextId.ToString());
                newGo.transform.SetParent(editorInfo.transform.FindOrNew("Monster"));
                MonsterCreator newCreator = newGo.AddComponent<MonsterCreator>();
                newCreator.ID = nextId;
                Selection.activeObject = newGo;
            }
            else
            {
                GameDebug.LogError("Editor 不能为空");
            }
        }
        [MenuItem("GameObject/HZZ/Player Creat", false, 13)]
        public static void CreatPlayer()
        {
            NodeEditorInfo editorInfo = GameObject.FindObjectOfType<NodeEditorInfo>();
            if (editorInfo != null)
            {
                GameObject newGo = new GameObject("PlayerCreator");
                newGo.transform.SetParent(editorInfo.transform.FindOrNew("Player"));
                PlayerCreator newCreator = newGo.AddComponent<PlayerCreator>();
                Selection.activeObject = newGo;
            }
            else
            {
                GameDebug.LogError("Editor 不能为空");
            }
        }
        
        [MenuItem("GameObject/HZZ/Props Creat", false, 13)]
        public static void CreatProps()
        {
            NodeEditorInfo editorInfo = GameObject.FindObjectOfType<NodeEditorInfo>();
            if (editorInfo != null)
            {
                PropsCreator[] allMonster = editorInfo.transform.GetComponentsInChildren<PropsCreator>(true);
                int nextId = Tools.GetID(allMonster);
                GameObject newGo = new GameObject(nextId.ToString());
                newGo.transform.SetParent(editorInfo.transform.FindOrNew("Props"));
                PropsCreator newCreator = newGo.AddComponent<PropsCreator>();
                newCreator.ID = nextId;
                Selection.activeObject = newGo;
            }
            else
            {
                GameDebug.LogError("Editor 不能为空");
            }
        }
    }
}