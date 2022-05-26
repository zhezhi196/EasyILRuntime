using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FirstGameTimeline : TimelineController
{
    public override void Play(Player player, Monster enemy, UnityAction callBack, params object[] args)
    {
        onFinishEvent = callBack;
        playable.Play();
    }
}
