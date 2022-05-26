using System;
using Sirenix.OdinInspector;

[Serializable]
public class AnimationKeyValue
{
    [HorizontalGroup(),HideLabel]
    public string key;
    [HorizontalGroup(),HideLabel]
    public string animation;
}