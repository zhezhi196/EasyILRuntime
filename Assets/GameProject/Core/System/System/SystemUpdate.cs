using System.Collections;
using System.Collections.Generic;
using Module;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public static class SystemUpdate
{
    public static void Update()
    {
        Setting.Update();
        UIController.Instance.Update();
        Async.Update();
        Clock.Update();
        LocalSaveFile.Update();
        QueueRunMethod.Update();
        PredicateCallback.Update();
        TimeHelper.Update();
        BattleController.Instance.OnUpdate();
    }
}
