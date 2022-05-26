using System.Collections.Generic;
using Module;
using Sirenix.OdinInspector;
using UnityEngine;
[ExecuteInEditMode]
public class MonsterParent : MonoBehaviour
{
    public Vector2Int idPool;
    
#if UNITY_EDITOR

    private void Update()
    {
        if (Application.isPlaying)
        {
            transform.position = Vector3.zero;
            transform.eulerAngles = Vector3.zero;
        }
    }
    [Button]
    private void RereadPrefabLevel()
    {
        MonsterCreator[] creator = transform.GetComponentsInChildren<MonsterCreator>(true);
        for (int i = 0; i < creator.Length; i++)
        {
            creator[i].OnModelChanged();
        }
    }
    [Button]
    private void SetDestroyEvent(int eventID, int unloadId)
    {
        MonsterCreator[] creators = transform.GetComponentsInChildren<MonsterCreator>();
        for (int i = 0; i < creators.Length; i++)
        {
            creators[i].unloadNodeID = unloadId;
            if (!creators[i].eventRecivers.Contains(fd => fd.eventID == eventID))
            {
                creators[i].eventRecivers.Add(new EventReciverEditor(){eventID = eventID,count = 1,responseModels = new List<ReceiveLogical>(){new ReceiveLogical(){responseID = RunLogicalName.ForceDestroy}}});
            }
        }
        
    }
    [Button]
    public void ResetLevelID()
    {
        MonsterCreator[] creator = transform.GetComponentsInChildren<MonsterCreator>();
        for (int i = 0; i < creator.Length; i++)
        {
            creator[i].ReReadLevelWithConfig();
        }
    }
#endif

}