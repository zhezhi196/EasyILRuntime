using Module;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EndTimeline : TimelineController
{
    public bool updateCamera = false;
    public Camera[] _camera;
    private bool isPlay = false;
    public override async void Play(Player player, AttackMonster enemy, UnityAction callBack, params object[] args)
    {
        base.Play(player, enemy, callBack, args);
        onFinishEvent = callBack;
        playable.gameObject.OnActive(true);
        for (int i = 0; i < _camera.Length; i++)
        {
            CameraCtrl.AddCamera(_camera[i]);
        }
        await Async.WaitforSecondsRealTime(0.3f, gameObject);
        playable.Play();
        BlackMaskChange.Instance.Close();
        isPlay = true;
    }

    public override void OnComplete()
    {
        BlackMaskChange.Instance.Black();
        for (int i = 0; i < _camera.Length; i++)
        {
            CameraCtrl.RemoveCamera(_camera[i]);
        }
        isPlay = false;
        base.OnComplete();
    }

    private void LateUpdate()
    {
        if (isPlay&&updateCamera)
            CameraCtrl.trans.rotation = _camera[0].transform.rotation;
    }

    public void OnDestroy()
    {
        if (updateCamera)
            CameraCtrl.Reset();
        Async.StopAsync(gameObject);
    }
}
