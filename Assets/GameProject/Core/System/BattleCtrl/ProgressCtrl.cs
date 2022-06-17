using System;
using System.Collections.Generic;
using Module;
using Project.Data;
using UnityEngine;

public class ProgressCtrl : BattleSystem
{
    public const string saveKey = "ProgressIndex";

    public ProgressSO progress;
    public int curProgressIndex;
    public List<IProgressOption> curProgressOptions = new List<IProgressOption>();
    public List<IProgressOption> allProgress = new List<IProgressOption>();
    public bool showingTips = false;
    
    public RewardBag iapBag;
    
    public RunTimeAction sortProgress;

    public override void OnRestart(EnterNodeType enterType)
    {
        base.OnRestart(enterType);
        
        if (enterType == EnterNodeType.Restart)
        {
            curProgressIndex = 0;
            LocalFileMgr.SetInt(saveKey, curProgressIndex);
        }
        else
        {
            if (LocalFileMgr.ContainKey(saveKey))
            {
                curProgressIndex = LocalFileMgr.GetInt(saveKey);
            }
            else
            {
                curProgressIndex = 0;
                LocalFileMgr.SetInt(saveKey, curProgressIndex);
            }
        }
        
        iapBag = (RewardBag) Commercialize.GetRewardBag(DataMgr.CommonData(33001).ToInt());
        allProgress.Clear();
        sortProgress = new RunTimeAction(() =>
        {
            AssetLoad.PreloadAsset<ProgressSO>($"Progress/Progress_{BattleController.currNode.graph.name}.asset", (v) =>
            {
                progress = v.Result;
                
                for (int i = 0; i < MonsterCreator.creatList.Count; i++)
                {
                    if (progress.IsInProgress(MonsterCreator.creatList[i].key))
                    {
                        allProgress.Add(MonsterCreator.creatList[i]);
                    }
                }

                for (int i = 0; i < PropsCreator.editorList.Count; i++)
                {
                    if (progress.IsInProgress(PropsCreator.editorList[i].key))
                    {
                        allProgress.Add(PropsCreator.editorList[i]);
                    }
                }

                //记录当前的所有记录点Creator实例
                NextProgress(curProgressIndex);
            
                BattleController.Instance.NextFinishAction("sortProgress");
                sortProgress = null;
            });
        });
    }

    public bool TryNextProgress()
    {
        GameDebug.Log($"=======当前提示已经完成了，index为：{curProgressIndex}，尝试触发下一个提示===");
        var node = progress.GetCurNode(curProgressIndex);
        int max = curProgressOptions.Count;
        int completeCount = 0;
        for (int i = 0; i < curProgressOptions.Count; i++)
        {
            if (curProgressOptions[i].progressIsComplete)
            {
                completeCount++;
            }
        }

        bool complete = false;
        //得到是否是满了，满了尝试触发下一个提示节点
        if (progress.nodeList[curProgressIndex].nextLogic == ProgressNextLogic.And)
        {
            complete = completeCount == max;
        }
        else
        {
            complete = completeCount > 0;
        }
        
        if (complete)
        {
            curProgressIndex++;
            LocalFileMgr.SetInt(saveKey, curProgressIndex);
            
            if (IsAllComplete()) //是否完成了
            {
                return false;
            }
            NextProgress(curProgressIndex);
            return true;
        }

        return false;
    }

    private void NextProgress(int nextIndex)
    {
        curProgressOptions.Clear();
        var list = progress.GetCreatorListByIndex(nextIndex);
        if (list == null)
        {
            return;
        }
        GameDebug.Log($"=======下一个提示触发了 {nextIndex}");
        for (int i = 0; i < list.Count; i++)
        {
            for (int j = 0; j < allProgress.Count; j++)
            {
                if (list[i] == allProgress[j].key)
                {
                    curProgressOptions.Add(allProgress[j]);
                }
            }
        }

        //有可能这个进度也完成了，就继续往后跳
        var complete= GetCurProgressComplete();
        if (complete)
        {
            TryNextProgress();
        }
    }

    public bool IsAllComplete()
    {
        return curProgressIndex >= progress.nodeList.Count;
    }


    public Vector3 GetTipPos(int index)
    {
        return curProgressOptions[index].GetTipsPos();
    }
    
    public bool GetUITipsShouldShow(int index)
    {
        if (index >= curProgressOptions.Count)
        {
            return false;
        }
        
        return !curProgressOptions[index].progressIsComplete;
    }

    /// <summary>
    /// 当前节点
    /// </summary>
    /// <returns></returns>
    public bool GetCurProgressComplete()
    {
        if (IsAllComplete())
        {
            return true;
        }
        if (progress.nodeList[curProgressIndex].nextLogic == ProgressNextLogic.And)
        {
            int count = 0;
            for (int i = 0; i < curProgressOptions.Count; i++)
            {
                if (curProgressOptions[i].progressIsComplete) count++;
            }

            if (count == curProgressOptions.Count)
            {
                return true;
            }
            
            return false;
        }
        else
        {
            for (int i = 0; i < curProgressOptions.Count; i++)
            {
                if (curProgressOptions[i].progressIsComplete)
                    return true;
            }

            return false;
        }
    }
    
    

    public void ShowTips(bool skipIap)
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
                //尝试定位到最新的进度
                TryNextProgress();
                
                showingTips = true;
#if UNITY_EDITOR
                Debug.Log($"显示提示消息{curProgressIndex}");// {currProgress.gameObject}", currProgress.gameObject);

#else
                GameDebug.Log($"显示提示消息{currProgress.key}: {currProgress.gameObject}");
#endif
            }
        }, flag | IapRewardFlag.NoAudio);
    }
}