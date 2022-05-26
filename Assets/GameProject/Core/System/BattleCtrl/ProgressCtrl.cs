using System;
using System.Collections.Generic;
using Module;
using Project.Data;
using UnityEngine;

public class ProgressCtrl : BattleSystem
{
    public RewardBag iapBag;
    public List<IProgressOption> allProgress = new List<IProgressOption>();
    private IProgressOption _currProgress;

    public IProgressOption currProgress
    {
        get { return _currProgress; }
        set { _currProgress = value; }
    }
    public RunTimeAction sortProgress;

    public bool isComplete
    {
        get
        {
            for (int i = 0; i < allProgress.Count; i++)
            {
                if (!allProgress[i].progressIsComplete) return false;
            }
            return true;
        }
    }
    public override void OnRestart(EnterNodeType enterType)
    {
        base.OnRestart(enterType);
        iapBag = (RewardBag) Commercialize.GetRewardBag(DataMgr.CommonData(33001).ToInt());
        allProgress.Clear();
        sortProgress = new RunTimeAction(() =>
        {
            for (int i = 0; i < MonsterCreator.creatList.Count; i++)
            {
                if (MonsterCreator.creatList[i].progress.index != -1)
                {
                    allProgress.Add(MonsterCreator.creatList[i]);
                }
            }

            for (int i = 0; i < PropsCreator.editorList.Count; i++)
            {
                if (PropsCreator.editorList[i].extuil.progress.index != -1)
                {
                    allProgress.Add(PropsCreator.editorList[i]);
                }
            }

            allProgress.Sort((a, b) => a.progressOption.index.CompareTo(b.progressOption.index));
            BattleController.Instance.NextFinishAction("sortProgress");
            sortProgress = null;
        });
    }

    public override void OnUpdate()
    {
        if (currProgress != null && currProgress.progressIsComplete)
        {
            currProgress = null;
        }
    }

    public void ShowTips(bool skipIap, Action callback)
    {
        IapRewardFlag flag = 0;
        if (skipIap)
        {
            flag = flag | IapRewardFlag.Free | IapRewardFlag.NoAnalysis;
        }

        iapBag.GetReward(s =>
        {
            if (s.result == IapResultMessage.Success)
            {
                for (int i = 0; i < allProgress.Count; i++)
                {
                    var tar = allProgress[i];
                    if (!tar.progressIsComplete)
                    {
                        currProgress = tar;
#if UNITY_EDITOR
                        Debug.Log($"显示提示消息{currProgress.key}: {currProgress.gameObject}",currProgress.gameObject);

#else
                        GameDebug.Log($"显示提示消息{currProgress.key}: {currProgress.gameObject}");
#endif
                        break;
                    }
                }
                callback?.Invoke();
            }
        }, flag | IapRewardFlag.NoAudio);
    }
}