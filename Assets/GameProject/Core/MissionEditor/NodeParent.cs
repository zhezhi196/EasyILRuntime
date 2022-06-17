using Module;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class NodeParent: MonoBehaviour
{
    private static Transform _lastEditorObj;

    public static Transform lastEditorObj
    {
        get
        {
            if (_lastEditorObj == null) _lastEditorObj = GameObject.Find("CreatPoint")?.transform;
            return _lastEditorObj;
        }
    }
    
    [ReadOnly]
    public ProgressSO progressSO;
    [Button("读取提示点信息")]
    public void LoadProgressSO()
    {
        if (progressSO == null)
        {
            AssetLoad.PreloadAsset<ProgressSO>($"Progress/Progress_{node.graph.name}.asset", (v) =>
            {
                progressSO = v.Result;
            });
        }
    }
    
    private void Awake()
    {
        LoadProgressSO();
    }
    

    public TaskNode node;
    public static NodeParent CreatParent()
    {
        NodeParent parent = GameObject.FindObjectOfType<NodeParent>();
        if (parent == null)
        {
            parent = new GameObject("Editor").AddOrGetComponent<NodeParent>();
        }

        return parent;
    }
    public string prefab;

    private PlayerCreator _playerCreator;

    public PlayerCreator playerCreator
    {
        get
        {
            if (_playerCreator == null) _playerCreator = transform.GetComponentInChildren<PlayerCreator>();
            return _playerCreator;
        }
    }
    


#if UNITY_EDITOR

    [Button,HideInPrefabs]
    public void Save()
    {
        UnityEditor.PrefabUtility.SaveAsPrefabAssetAndConnect(gameObject, prefab, UnityEditor.InteractionMode.AutomatedAction);
        UnityEditor.PrefabUtility.SaveAsPrefabAsset(gameObject, prefab);
        UnityEditor.AssetDatabase.Refresh();
    }
    [Button,HideInPrefabs]
    public void LoadAllModel()
    {
        
    }
    [Button("查询发送事件")]
    private void SearchSendEvent(int eventId)
    {
        IMissionEditor[] allMissionEditor = transform.GetComponentsInChildren<IMissionEditor>(true);
        for (int i = 0; i < allMissionEditor.Length; i++)
        {
            if (allMissionEditor[i].sender == null)
                continue;
            for (int j = 0; j < allMissionEditor[i].sender.Length; j++)
            {
                for (int k = 0; k < allMissionEditor[i].sender[j].eventKey.Length; k++)
                {
                    if (allMissionEditor[i].sender[j].eventKey[k].eventKey == eventId)
                    {
                        if (allMissionEditor[i] is MonsterCreator monsterCreator)
                        {
                            Debug.Log($"怪物{monsterCreator.id} {monsterCreator.modeName}存在发送事件{eventId}", monsterCreator.gameObject);
                        }
                        else if (allMissionEditor[i] is PropsCreator propsCreator)
                        {
                            Debug.Log($"物品{propsCreator.id} {propsCreator.model}存在发送事件{eventId}", propsCreator.gameObject);
                        }
                    }
                }
            }
        }
    }
    [Button("查询接受事件")]
    private void SearchSendRegister(int eventId)
    {
        IMissionEditor[] allMissionEditor = transform.GetComponentsInChildren<IMissionEditor>(true);
        for (int i = 0; i < allMissionEditor.Length; i++)
        {
            for (int j = 0; j < allMissionEditor[i].sender.Length; j++)
            {
                for (int k = 0; k < allMissionEditor[i].sender[j].eventKey.Length; k++)
                {
                    if (allMissionEditor[i].receiver[j].eventKey==eventId)
                    {
                        if (allMissionEditor[i] is MonsterCreator monsterCreator)
                        {
                            Debug.Log($"怪物{monsterCreator.id} {monsterCreator.modeName}存在接受事件{eventId}", monsterCreator.gameObject);
                        }
                        else if (allMissionEditor[i] is PropsCreator propsCreator)
                        {
                            Debug.Log($"物品{propsCreator.id} {propsCreator.model}存在接受事件{eventId}", propsCreator.gameObject);
                        }
                    }
                }
            }
        }
    }

#endif

  
}