using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class TaskPredicate
{
    [HideInInspector]
    public string tag;
    [LabelText("@tag")]
    public List<TaskPredicataItem> enterPredicate;
}
