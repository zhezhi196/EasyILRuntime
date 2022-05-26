using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Module;
using UnityEngine.Events;

public class StoryTimeline : TimelineController
{
    public Transform endPoint;
    public Player _player;
    private void Start()
    {
        bool b = BattleController.GetCtrl<TimelineCtrl>().AddStoryTimeline(key, this);
        if (!b)
        {
            GameDebug.LogFormat("剧情动画初始化错误:{0}", key);
        }
        if (startHide)
            gameObject.OnActive(false);
    }

    public override async void Play(Player player, Monster enemy, UnityAction callBack, params object[] args)
    {
        AnalyticsEvent.SendEvent(AnalyticsType.PlayAnimation, key);
        base.Play(player, enemy, callBack, args);
        _player = player;
        playable.gameObject.OnActive(true);
        onFinishEvent = callBack;
        BlackMaskChange.Instance.Black();//黑屏
        await Async.WaitforSecondsRealTime(0.3f,gameObject);
        playable.Play();
        BlackMaskChange.Instance.Close();
        UIController.Instance.canPhysiceback = false;
        if (bindingDict.ContainsKey("PlayerAnim") && player != null)
            playable.SetGenericBinding(bindingDict["PlayerAnim"].sourceObject, player.timelineModel._anim);
    }

    public override async void OnComplete()
    {
        BlackMaskChange.Instance.Black();//黑屏
        _player.characterController.enabled = false;
        _player.transform.position = endPoint.transform.position;
        _player.transform.rotation = endPoint.transform.rotation;
        _player.characterController.enabled = true;
        await Async.WaitforSecondsRealTime(0.3f, gameObject);
        BlackMaskChange.Instance.Close();
        base.OnComplete();
    }

    public void OnDestroy()
    {
        if (BattleController.GetCtrl<TimelineCtrl>() != null)
            BattleController.GetCtrl<TimelineCtrl>().RemoveStoryTimeline(key);
        Async.StopAsync(gameObject);
    }
}
