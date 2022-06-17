using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Module;
using UnityEngine.Rendering.Universal;

public class DeathStoryTimeline : TimelineController
{
    public Camera _camera;
    public override void Play(Player player, AttackMonster enemy, UnityAction callBack, params object[] args)
    {
        base.Play(player, enemy, callBack, args);
        Player.player.gameObject.OnActive(false);
        playable.gameObject.OnActive(true);
        AudioPlay.defaultListener.enabled = true;
        onFinishEvent = callBack;
        playable.Play();
        _camera.GetComponent<UniversalAdditionalCameraData>().renderType = CameraRenderType.Overlay;
        CameraCtrl.AddCamera(_camera);
        BlackMaskChange.Instance.Close();
    }

    public override async void OnComplete()
    {
        AudioPlay.defaultListener.enabled = false;
        BlackMaskChange.Instance.Black();//黑屏
        Player.player.gameObject.OnActive(true);
        CameraCtrl.RemoveCamera(_camera);
        await Async.WaitforSecondsRealTime(0.3f, gameObject);
        base.OnComplete();
    }
}
