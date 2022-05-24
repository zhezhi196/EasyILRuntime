using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public enum SceneName
{
    Islet,
    Town
}
[Serializable]
public class NodeSetting
{
    [LabelText("加载的场景")]
    public List<SceneName> loadScene;
    [LabelText("卸载的场景")]
    public List<SceneName> unloadScene;
    [LabelText("背景音乐")]
    public string bgm;
    [HideLabel]
    public FogSetting frog;

}
[Serializable]
public class FogSetting
{
    public bool fog;
    [ShowIf("fog")]
    public Color fogColor;
    [ShowIf("fog")] public FogMode fogMode = FogMode.Linear;
    [ShowIf("fog"),ShowIf("@this.fogMode==FogMode.Linear")]
    public float Start;
    [ShowIf("fog"),ShowIf("@this.fogMode==FogMode.Linear")]
    public float End;
    [ShowIf("@this.fogMode!=FogMode.Linear")]
    public float density;
}