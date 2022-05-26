using Module;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AssExcuteTimeline : TimelineController
{
    public AssExcuteTimeline()
    {
        startHide = false;
    }
    public override void Play(Player player, Monster enemy, UnityAction callBack, params object[] args)
    {
        base.Play(player,enemy,callBack,args);
        playable.gameObject.OnActive(true);
        onFinishEvent = callBack;
        playable.Play();
        UIController.Instance.canPhysiceback = false;
        if (bindingDict.ContainsKey("PlayerAnim") && player != null)
            playable.SetGenericBinding(bindingDict["PlayerAnim"].sourceObject, player.timelineModel._anim);
        if (bindingDict.ContainsKey("MonsterAnim") && enemy != null)
            playable.SetGenericBinding(bindingDict["MonsterAnim"].sourceObject, enemy.animator);
        //transform.position = enemy.transform.position;
        //transform.rotation = enemy.transform.rotation;
        //player.transform.position = playerPoint.position;
        //player.transform.rotation = playerPoint.rotation;
    }
}
