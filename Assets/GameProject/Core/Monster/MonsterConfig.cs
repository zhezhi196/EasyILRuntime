using System;
using Sirenix.OdinInspector;
using UnityEngine;
[CreateAssetMenu(menuName = "HZZ/怪物关卡配置")]
public class MonsterConfig : ScriptableObject
{
    [Serializable]
    public class MonsterConfigInfo
    {
        public int normalId;
        public MonsterMissionConfigInfo[] missionConfig;
    }
    [Serializable]
    public class MonsterMissionConfigInfo
    {
        [HorizontalGroup(),HideLabel]
        public int missionId;
        [HorizontalGroup(),HideLabel]
        public int targetId;
    }


    public MonsterConfigInfo[] config;
}