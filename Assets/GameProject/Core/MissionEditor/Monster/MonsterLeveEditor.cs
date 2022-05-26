using System;
using System.Collections.Generic;
using System.Text;
using Module;
using Project.Data;
using Sirenix.OdinInspector;
using UnityEngine;

public enum PatrolType
{
    [LabelText("循环")]
    Loop,
    [LabelText("来回")]
    Pingpong,
    [LabelText("随机")]
    Random
}
[Serializable]
public class MonsterLeveEditor
{
    //潘宇需求
    // [Serializable]
    // public struct MonsterPartEditor
    // {
    //     [HorizontalGroup("MonsterPartEditor"),HideLabel]
    //     public MonsterPartType type;
    //     [HorizontalGroup("MonsterPartEditor"),HideLabel]
    //     public int dbData;
    // }
    #region Editor

#if UNITY_EDITOR
    [ReadOnly]
    public MonsterCreator creator;
    private Color campColor
    {
        get
        {
            if(camp== MonsterCamp.Enemy) return new Color(1, 0.5f, 0);
            if(camp== MonsterCamp.Friend) return Color.green;
            return Color.white;
        }
    }
    
    private void OnRemovePatrolPoint(int index)
    {
        if (patrolPoint[index] != null)
        {
            GameObject.DestroyImmediate(patrolPoint[index].gameObject);
        }

        patrolPoint.RemoveAt(index);
    }

    private void OnRemoveEscapePoint(int index)
    {
        if (escapePoint[index].gameObject != null)
            GameObject.DestroyImmediate(escapePoint[index].gameObject);
        escapePoint.RemoveAt(index);
    }
    
    private void CreatEscapePoint()
    {
        Transform tar = creator.transform.NewChild("escapePoint" + escapePoint.Count);
        escapePoint.Add(tar);
        UnityEditor.Selection.activeGameObject = tar.gameObject;
    }

    private void CreatPatrolPoint()
    {
        Transform tar = creator.transform.NewChild("patrolPoint" + patrolPoint.Count);
        patrolPoint.Add(tar);
        UnityEditor.Selection.activeGameObject = tar.gameObject;
    }

    private void SearchDB()
    {
        var data = (IMonsterAttribute<float>) DataMgr.Instance.GetSqlService<MonsterData>().WhereID(dataId);
        if (data != null)
        {
            StringBuilder builder = new StringBuilder();
            var propert = typeof(IMonsterAttribute<float>).GetProperties();
            for (int i = 0; i < propert.Length; i++)
            {
                builder.Append($"{propert[i].Name}={propert[i].GetValue(data)}\n");
            }

            UnityEditor.EditorUtility.DisplayDialog("属性", builder.ToString(), "Ok");
        }
        else
        {
            UnityEditor.EditorUtility.DisplayDialog("属性", $"查不到表{dataId}", "Ok");
        }
    }
#endif

    #endregion
    
    [LabelText("数据信息"),InlineButton("SearchDB","查看属性")] public int dataId;
    [LabelText("阵营"),GUIColor("campColor")] public MonsterCamp camp;
    [LabelText("动画控制器")] public RuntimeAnimatorController animatorController;
    [HideLabel,HorizontalGroup("Station"),BoxGroup("Station/出生信息")] public BornStation bornStation;
    [HideLabel,HorizontalGroup("Station"),BoxGroup("Station/重置信息"),InlineButton("CopyBorn","拷贝")] public BornStation resetStation;
    // [LabelText("视野角度")]
    // public float viewAngle = 90;
    // [LabelText("视野距离")]
    // public float viewDistance = 10;
    // [LabelText("额外听力范围")]
    // public float hearRange;
    [LabelText("技能")]
    public List<Skill> skills;
    //[LabelText("部位数据")]
    //public List<MonsterPartEditor> partEditor;

    [LabelText("巡逻类型"),HideInPrefabs]
    public PatrolType patrolType;
    [LabelText("巡逻停留时间"),HideInPrefabs]
    public float patrolWaitTime;
    [LabelText("巡逻点"), GUIColor(0.3767355f, 0.9622642f, 0.9507972f),
     ListDrawerSettings(CustomRemoveIndexFunction = "OnRemovePatrolPoint", CustomAddFunction = "CreatPatrolPoint"),HideInPrefabs]
    public List<Transform> patrolPoint = new List<Transform>();


    [LabelText("逃离点"), GUIColor(1f, 0.9215686f, 0.01568628f),ListDrawerSettings(CustomRemoveIndexFunction = "OnRemoveEscapePoint",CustomAddFunction = "CreatEscapePoint"),HideInPrefabs]
    public List<Transform> escapePoint= new List<Transform>();

    private void CopyBorn()
    {
        resetStation = bornStation;
    }
}