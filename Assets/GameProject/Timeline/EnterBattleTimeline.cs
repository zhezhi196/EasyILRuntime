using Module;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

public class EnterBattleTimeline : TimelineController
{
    public Camera _camera;
    public string audioName;
    public override async void Play(Player player, AttackMonster enemy, UnityAction callBack, params object[] args)
    {
        base.Play(player, enemy, callBack, args);
        if (!string.IsNullOrEmpty(audioName))
            AudioPlay.PlayBackGroundMusic(audioName);
        _camera.GetComponent<UniversalAdditionalCameraData>().renderType = CameraRenderType.Overlay;
        onFinishEvent = callBack;
        BlackMaskChange.Instance.Black();//黑屏
        Player.player.AddStation(Player.Station.Story);
        Player.player.gameObject.OnActive(false);
        AudioPlay.defaultListener.enabled = true;
        playable.gameObject.OnActive(true);
        CameraCtrl.AddCamera(_camera);
        await Async.WaitforSecondsRealTime(0.3f, gameObject);
        playable.Play();
        BlackMaskChange.Instance.Close();
        UIController.Instance.canPhysiceback = false;
    }

    public override async void OnComplete()
    {
        AudioPlay.defaultListener.enabled = false;
        BlackMaskChange.Instance.Black();//黑屏
        Player.player.gameObject.OnActive(true);
        Player.player.RemoveStation(Player.Station.Story);
        CameraCtrl.RemoveCamera(_camera);
        await Async.WaitforSecondsRealTime(0.3f, gameObject);
        BlackMaskChange.Instance.Close();
        base.OnComplete();
    }
}
