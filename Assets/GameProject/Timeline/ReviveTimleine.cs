using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Module;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class ReviveTimleine : TimelineController
{
    public TimelineAsset[] timelineAssets;
    private const string saveKey="ReviveTimelinePlay";
    private void Start()
    {
        var crtl = BattleController.GetCtrl<TimelineCtrl>();
        if (crtl != null)
        {
            bool b = BattleController.GetCtrl<TimelineCtrl>().AddReviveTimeline(this);
            if (!b)
            {
                GameDebug.LogFormat("复活动画初始化错误:{0}");
            }
        }
    }
    public override void Play(Player player, AttackMonster enemy, UnityAction callBack, params object[] args)
    {
        base.Play(player, enemy, callBack, args);
        int count = LocalFileMgr.GetInt(saveKey);
        playable.playableAsset = timelineAssets[count];
        playable.gameObject.OnActive(true);
        onFinishEvent = callBack;
        playable.Play();
        if (count == 0)
            LocalFileMgr.SetInt(saveKey, 1);
        UIController.Instance.canPhysiceback = false;
        playable.SetGenericBinding((playable.playableAsset as TimelineAsset).GetOutputTrack(1), player.timelineModel._anim);
        //if (bindingDict.ContainsKey("PlayerAnim") && player != null)
        //    playable.SetGenericBinding(bindingDict["PlayerAnim"].sourceObject, player.timelineModel._anim);
    }

    public override void OnComplete()
    {
        //Chapter2.Analytics.SendEvent(Chapter2.AnalyticsType.AnimationEnd, this);
        UIController.Instance.canPhysiceback = true;
        if (onFinishEvent != null)
        {
            onFinishEvent();
            onFinishEvent = null;
        }
    }

    public void OnDestroy()
    {
        if (BattleController.GetCtrl<TimelineCtrl>() != null)
            BattleController.GetCtrl<TimelineCtrl>().RemoveReviveTimeline(this);
        Async.StopAsync(gameObject);
    }
}
