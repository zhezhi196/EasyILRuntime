using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
[Serializable]
public struct SceneSetting
{
    [HorizontalGroup(),LabelText("正常游戏需要加载")]
    public bool runtimeLoad;
    [HorizontalGroup(),HideLabel]
    public SceneName sceneName;

}
public enum SceneName
{
    [LabelText("测试场景")]
    DebugScene,
    Scenes00,
    Scenes01,
    Scenes02,
    Scenes03,
    Scenes04,
    
}
[Serializable]
public class NodeSetting
{
    [LabelText("加载的场景")]
    public List<SceneSetting> loadScene;
    [LabelText("卸载的场景")]
    public List<SceneSetting> unloadScene;
    [LabelText("背景音乐")]
    public string bgm;

    public fogSetting[] frog;
    // [MultiLineProperty][HideLabel][Title("描述")]
    // public string des;
}

[Serializable]
public class fogSetting
{
    public bool fog;
    [ShowIf("fog")]
    public AnimationCurve fadeCurve;
    [ShowIf("fog")]
    public Color fogColor;
    [ShowIf("fog")] public FogMode fogMode = FogMode.Linear;
    [ShowIf("fog")]
    public float Start;
    [ShowIf("fog")]
    public float End;
    [ShowIf("fog")]
    public float density;
}
