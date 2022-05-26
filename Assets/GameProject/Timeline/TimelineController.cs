using System.Collections;
using System.Collections.Generic;
using Module;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using Sirenix.OdinInspector;
using UnityEngine.Timeline;

public class TimelineController : MonoBehaviour
{
    public TimeLineType type = TimeLineType.Story;
    public PlayableDirector playable;
    public Transform playerPoint;
    public bool startHide = true;
    public UnityAction onFinishEvent;
    protected Dictionary<string, PlayableBinding> bindingDict = new Dictionary<string, PlayableBinding>();
    protected bool isInit = false;
    public string key = "";
    public virtual bool canPlay
    {
        get { return true; }
    }
    private void Start()
    {
        if (startHide && !isInit)
            playable.gameObject.OnActive(false);
    }

    protected void Init()
    {
        foreach (var at in playable.playableAsset.outputs)
        {
            if (!bindingDict.ContainsKey(at.streamName))
            {
                bindingDict.Add(at.streamName, at);
            }
        }
        isInit = true;
    }

    public virtual void Play(Player player, Monster enemy, UnityAction callBack, params object[] args)
    {
        if (!isInit)
            Init();
        UIController.Instance.canPhysiceback = false;
    }

    /// <summary>
    /// timeline事件直接调用
    /// </summary>
    public virtual void OnComplete()
    {
        //Chapter2.Analytics.SendEvent(Chapter2.AnalyticsType.AnimationEnd, this);
        UIController.Instance.canPhysiceback = true;
        if (onFinishEvent != null)
        {
            onFinishEvent();
            onFinishEvent = null;
        }
        playable.gameObject.OnActive(false);
    }

    public void Pause()
    {
        playable.playableGraph.GetRootPlayable(0).SetSpeed(0);
    }
    [Button("继续播放")]
    public void Continue()
    {
        playable.playableGraph.GetRootPlayable(0).SetSpeed(1);
    }

#if UNITY_EDITOR
    [Button("打印轨道名")]
    public void PrintOutputName()
    {
        foreach (var at in playable.playableAsset.outputs)
        {
            Debug.Log(at.streamName);
        }
    }
    [Button("添加音效组件")]
    public void AddAudioComponent()
    {
        GameObject audio = new GameObject("Audio");
        audio.transform.SetParentZero(this.transform);
        audio.AddComponent<Audio>();
    }
#endif
}
