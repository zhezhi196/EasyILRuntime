using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Module;
using System;

public class TimelineCtrl : BattleSystem
{
    private string configPath = "Timeline/PlayerTimelineConfig.asset";
    public PlayerTimelineConfig timelineConfig;
    public Dictionary<string, TimelineController> storyTimeLine = new Dictionary<string, TimelineController>();//剧情动画
    private Dictionary<string, TimelineController> timelineDic = new Dictionary<string, TimelineController>();//暗杀,处决等动画
    public List<TimelineController> reviveTimeline = new List<TimelineController>();
    private const string deathStoryKey = "DeathTimeline";
    public TimelineController deathStory;

    public override void BattlePrepare(EnterNodeType enterType)
    {
        deathStory = null;
        AssetLoad.PreloadAsset<PlayerTimelineConfig>(configPath, (go) =>
        {
            timelineConfig = go.Result;
        });
    }

    public override void ExitBattle(OutGameStation station)
    {
        reviveTimeline.Clear();
    }

    public void GetDeathStory(Action<TimelineController> callback)
    {
        //死亡剧情动画初始化
        int index = LocalFileMgr.GetInt(deathStoryKey);
        if (index < timelineConfig.deathStory.Length)
        {
            AssetLoad.LoadGameObject<TimelineController>(timelineConfig.deathStory[index], BattleController.Instance.timelineRoot, (line, obj) =>
            {
                line.gameObject.OnActive(false);
                callback?.Invoke(line);
                LocalFileMgr.SetInt(deathStoryKey, index + 1);
            });
        }
        else {
            callback?.Invoke(null);
        }
    }

    //public void NextDeathStory()
    //{
    //    //死亡剧情动画初始化
    //    deathStory = null;
    //    int index = LocalFileMgr.GetInt(deathStoryKey);
    //    LocalFileMgr.SetInt(deathStoryKey,index + 1);
    //    if (index+1 < timelineConfig.deathStory.Length)
    //    {
    //        AssetLoad.LoadGameObject<TimelineController>(timelineConfig.deathStory[index], BattleController.Instance.timelineRoot, (line, obj) =>
    //        {
    //            line.gameObject.OnActive(false);
    //            deathStory = line;
    //        });
    //    }
    //}

    public void GetAssTimeline(Monster m, bool isKill, Action<TimelineController> callback)
    {
        string path = "";
        PlayerTimelineConfig.AssMonsterConfig config = timelineConfig.assMonsterConfigs.Find(c => c.monster == m.modelName);
        if (config != null)
        {
            if (!isKill)
            {
                path = config.noKillTimeline;
            }
            else
            {
                path = config.killTimeline;
            }
        }
        if (!string.IsNullOrEmpty(path))
        {
            if (timelineDic.ContainsKey(path))
            {
                callback?.Invoke(timelineDic[path]);
            }
            else
            {
                AssetLoad.LoadGameObject<TimelineController>(path, BattleController.Instance.timelineRoot, (line, obj) =>
                {
                    line.gameObject.OnActive(false);
                    timelineDic.Add(path, line);
                    callback?.Invoke(line);
                });
            }
        }
        else
        {
            callback?.Invoke(null);
        }
    }

    public void GetExcuteTimeline(Monster m, Action<TimelineController> callback)
    {
        string path = "";
        PlayerTimelineConfig.ExcuteMonsterConfig config = timelineConfig.excuteMonsterConfigs.Find(c => c.monster == m.modelName);
        if (config != null)
        {
            path = config.timeline;
        }
        if (!string.IsNullOrEmpty(path))
        {
            if (timelineDic.ContainsKey(path))
            {
                callback?.Invoke(timelineDic[path]);
            }
            else
            {
                AssetLoad.LoadGameObject<TimelineController>(path, BattleController.Instance.timelineRoot, (line, obj) =>
                {
                    line.gameObject.OnActive(false);
                    timelineDic.Add(path, line);
                    callback?.Invoke(line);
                });
            }
        }
        else
        {
            callback?.Invoke(null);
        }
    }

    public void GetGetoutTimeline(Monster m, Action<TimelineController> callback)
    {
        string path = "";
        PlayerTimelineConfig.GetOutConfig config = timelineConfig.getoutConfigs.Find(c => c.monster == m.modelName);
        if (config != null)
        {
            path = config.timeline;
        }
        if (!string.IsNullOrEmpty(path))
        {
            if (timelineDic.ContainsKey(path))
            {
                callback?.Invoke(timelineDic[path]);
            }
            else
            {
                AssetLoad.LoadGameObject<TimelineController>(path, BattleController.Instance.timelineRoot, (line, obj) =>
                {
                    line.gameObject.OnActive(false);
                    timelineDic.Add(path, line);
                    callback?.Invoke(line);
                });
            }
        }
        else
        {
            callback?.Invoke(null);
        }
    }

    public void GetDeathTimeline(Monster m, Action<TimelineController> callback)
    {
        string path = "";
        PlayerTimelineConfig.DeathConfig config = timelineConfig.deathConfigs.Find(c => c.monster == m.modelName);
        if (config != null)
        {
            path = config.timeline;
        }
        if (!string.IsNullOrEmpty(path))
        {
            if (timelineDic.ContainsKey(path))
            {
                callback?.Invoke(timelineDic[path]);
            }
            else
            {
                AssetLoad.LoadGameObject<TimelineController>(path, BattleController.Instance.timelineRoot, (line, obj) =>
                {
                    line.gameObject.OnActive(false);
                    timelineDic.Add(path, line);
                    callback?.Invoke(line);
                });
            }
        }
        else
        {
            callback?.Invoke(null);
        }
    }

    public void GetMonsterShowTimeline(Monster m, Action<TimelineController> callback)
    {
        string path = "";
        PlayerTimelineConfig.MonserShowConfig config = timelineConfig.monsterShowConfigs.Find(c => c.monster == m.modelName);
        if (config != null)
        {
            path = config.timeline;
        }
        if (!string.IsNullOrEmpty(path))
        {
            if (timelineDic.ContainsKey(path))
            {
                callback?.Invoke(timelineDic[path]);
            }
            else
            {
                AssetLoad.LoadGameObject<TimelineController>(path, BattleController.Instance.timelineRoot, (line, obj) =>
                {
                    line.gameObject.OnActive(false);
                    timelineDic.Add(path, line);
                    callback?.Invoke(line);
                });
            }
        }
        else
        {
            callback?.Invoke(null);
        }
    }

    public bool AddStoryTimeline(string key, TimelineController timeline)
    {
        if (!storyTimeLine.ContainsKey(key))
        {
            storyTimeLine.Add(key, timeline);
            return true;
        }
        return false;
    }

    public void RemoveStoryTimeline(string key)
    {
        if (storyTimeLine.ContainsKey(key))
        {
            storyTimeLine.Remove(key);
        }
    }

    public bool AddReviveTimeline(TimelineController timeline)
    {
        if (!reviveTimeline.Contains(timeline))
        {
            reviveTimeline.Add(timeline);
            return true;
        }
        return false;
    }
    public void RemoveReviveTimeline(TimelineController timeline)
    {
        if (reviveTimeline.Contains(timeline))
            reviveTimeline.Remove(timeline);
    }
    /// <summary>
    /// 查找玩家复活动画
    /// </summary>
    /// <param name="vpos">Player Pos</param>
    /// <returns></returns>
    public TimelineController GetReviveAnimtion(Vector3 vpos)
    {
        TimelineController controller = null;
        float dis = 1000f;
        for (int i = 0; i < reviveTimeline.Count; i++)
        {
            if (Vector3.Distance(reviveTimeline[i].transform.position, vpos) < dis)
            {
                controller = reviveTimeline[i];
                dis = Vector3.Distance(reviveTimeline[i].transform.position, vpos);
            }
        }
        return controller;
    }

    public void GetEndTimeline(GameDifficulte difficulte, Action<TimelineController> callback)
    {
        string path = "";
        PlayerTimelineConfig.EndAnimConfig config = timelineConfig.endAnimConfig.Find(c => c.difficulte == difficulte);
        if (config != null)
        {
            path = config.timeline;
        }
        if (!string.IsNullOrEmpty(path))
        {
            if (timelineDic.ContainsKey(path))
            {
                callback?.Invoke(timelineDic[path]);
            }
            else
            {
                AssetLoad.LoadGameObject<TimelineController>(path, BattleController.Instance.timelineRoot, (line, obj) =>
                {
                    line.gameObject.OnActive(false);
                    timelineDic.Add(path, line);
                    callback?.Invoke(line);
                });
            }
        }
        else
        {
            callback?.Invoke(null);
        }
    }

    public TimelineController GetStoryTimeline(string key)
    {
        if (storyTimeLine.ContainsKey(key))
        {
            return storyTimeLine[key];
        }
        return null;
    }
}
