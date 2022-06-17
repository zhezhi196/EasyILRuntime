using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


public enum ProgressNextLogic
{
    [LabelText("全部完成")]
    And,
    [LabelText("完成其中一个")]
    Or,
}

[Serializable]
public class ProgressNode
{
    [LabelText("完成逻辑")]
    public ProgressNextLogic nextLogic;
    [LabelText("配置Id列表")]
    public List<int> creatorIdList;
}


[CreateAssetMenu(fileName = "ProgressSO", menuName = "提示点配置", order = 0)]
public class ProgressSO : ScriptableObject
{
    public List<ProgressNode> nodeList = new List<ProgressNode>();

    public void ChangeCreatorId(int oldId , int newId)
    {
        for (int i = 0; i < nodeList.Count; i++)
        {
            for (int j = 0; j < nodeList[i].creatorIdList.Count; j++)
            {
                if (nodeList[i].creatorIdList[j] == oldId)
                {
                    nodeList[i].creatorIdList[j] = newId;
                }
            }
        }
    }

    public ProgressNode GetCurNode(int index)
    {
        return nodeList[index];
    }

    public List<int> GetCreatorListByIndex(int index)
    {
        if (index >= nodeList.Count)
        {
            return null;
        }

        return nodeList[index].creatorIdList;
    }

    
    
    public bool IsInProgress(int id)
    {
        for (int i = 0; i < nodeList.Count; i++)
        {
            for (int j = 0; j < nodeList[i].creatorIdList.Count; j++)
            {
                if (nodeList[i].creatorIdList[j] == id)
                {
                    return true;
                }
            }
        }

        return false;
    }
}