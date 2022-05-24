using System;
using Project.Data;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class MonsterLevel
{
    [HideInPrefabAssets]
    public bool isInit;
    [SerializeField] private int dbId;
    public MonsterSkill[] skill;
    [HideInPrefabAssets]
    public MonsterAttribute levelAttribute;
    public MonsterData dbData;
    public void Init()
    {
        if (!isInit)
        {
            dbData = DataInit.Instance.GetSqlService<MonsterData>().WhereID(dbId);
            levelAttribute = new MonsterAttribute(dbData);
            for (int i = 0; i < skill.Length; i++)
            {
                skill[i] = GameObject.Instantiate(skill[i]);
            }
        }
    }
}