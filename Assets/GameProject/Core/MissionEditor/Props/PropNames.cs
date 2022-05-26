using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


[Serializable]
public class PropNameUnit
{
    [HorizontalGroup("1",LabelWidth = 13),LabelText("中")]
    public string chineseName;
    [HorizontalGroup("1",LabelWidth = 13),LabelText("英")]
    public string className;
}

[CreateAssetMenu(fileName = "PropNames", menuName = "道具名称配置", order = 0)]
public class PropNames : ScriptableObject
{
    [SerializeField]
    public List<PropNameUnit> nameList;
}