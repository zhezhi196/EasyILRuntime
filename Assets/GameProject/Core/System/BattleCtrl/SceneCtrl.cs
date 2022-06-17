using System;
using System.Collections.Generic;
using DG.Tweening;
using Module;
using UnityEngine;

public class SceneCtrl : BattleSystem
{
    public List<string> currScene = new List<string>();
    public RunTimeAction unloadScene;
    public RunTimeAction loadScene;
    public string saveKey = "SceneLoad";
    public RunTimeAction LoadComplete;

    public override void StartBattle(EnterNodeType enterType)
    {
        LoadComplete = new RunTimeAction(() =>
        {
            Loading.Close(UILoading.uiLoading, "EnterBattle");
            BattleController.Instance.NextFinishAction("LoadComplete");
            LoadComplete = null;

        });
    }

    public void SwitchFog(TaskNode tak,int index,float fadeTime)
    {
        if (index > tak.nodeSetting.frog.Length)
        {
            return;
        }
        fogSetting setting = tak.nodeSetting.frog[index];
        RenderSettings.fog = setting.fog;

        if (setting.fog)
        {
            DOTween.To(() => RenderSettings.fogColor, value => RenderSettings.fogColor = value, setting.fogColor, fadeTime).SetEase(setting.fadeCurve);
            RenderSettings.fogMode = setting.fogMode;
            if (setting.fogMode == FogMode.Linear)
            {
                DOTween.To(() => RenderSettings.fogStartDistance, value => RenderSettings.fogStartDistance = value, setting.Start, fadeTime).SetEase(setting.fadeCurve);
                DOTween.To(() => RenderSettings.fogEndDistance, value => RenderSettings.fogEndDistance = value, setting.End, fadeTime).SetEase(setting.fadeCurve);
            }
            else
            {
                DOTween.To(() => RenderSettings.fogDensity, value => RenderSettings.fogDensity = value, setting.density, fadeTime).SetEase(setting.fadeCurve);
            }
        }
    }

    public override void OnNodeEnter(NodeBase node, EnterNodeType enterType)
    {
        if (node is TaskNode tak)
        {
            SwitchFog(tak, 0, 2);
            if (!tak.nodeSetting.bgm.IsNullOrEmpty())
            {
                // AudioPlay.PlayBackGroundMusic(tak.nodeSetting.bgm);
                AudioManager.PlayMusic(tak.nodeSetting.bgm);
            }
        }

        if (enterType == EnterNodeType.Restart || enterType == EnterNodeType.NextNode || enterType == EnterNodeType.SkipNode)
        {
            if (node is TaskNode task)
            {
                if (!task.nodeSetting.loadScene.IsNullOrEmpty())
                {
                    loadScene = new RunTimeAction(() =>
                    {
                        LoadScenes(enterType, task.nodeSetting.loadScene, () =>
                        {
                            BattleController.Instance.NextFinishAction("loadScene");
                            loadScene = null;
                        });
                    });
                }

                if (!task.nodeSetting.unloadScene.IsNullOrEmpty())
                {
                    unloadScene = new RunTimeAction(() =>
                    {
                        UnLoadScenes(enterType, task.nodeSetting.unloadScene, () =>
                        {
                            BattleController.Instance.NextFinishAction("unloadScene");
                            unloadScene = null;
                        });
                    });
                }
            }
        }
        else if (enterType == EnterNodeType.FromSave)
        {
            string sceneStr = LocalSave.Read(LocalSave.savePath , saveKey);
            string[] loadSceneStr = sceneStr.Split(ConstKey.Spite0);
            Voter voter = new Voter(loadSceneStr.Length, () =>
            {
                BattleController.Instance.NextFinishAction("loadScene");
                loadScene = null;
            });
            loadScene = new RunTimeAction(() =>
            {
                for (int i = 0; i < loadSceneStr.Length; i++)
                {
                    LoadScene(loadSceneStr[i], voter.Add);
                }
            });
        }
    }

    public void LoadScenes(EnterNodeType nodeType, List<SceneSetting> scenes, Action callback)
    {
        if (!scenes.IsNullOrEmpty())
        {
            Voter voter = new Voter(scenes.Count, callback);
            for (int i = 0; i < scenes.Count; i++)
            {
                if ((scenes[i].runtimeLoad || nodeType == EnterNodeType.SkipNode) && !currScene.Contains(scenes[i].sceneName.ToString()))
                {
                    LoadScene(scenes[i].sceneName.ToString(), voter.Add);
                }
                else
                {
                    voter.Add();
                }
            }
        }
        else
        {
            callback?.Invoke();
        }
    }
    
    public void LoadScenes(string[] scenes, Action callback)
    {
        if (!scenes.IsNullOrEmpty())
        {
            Voter voter = new Voter(scenes.Length, callback);
            for (int i = 0; i < scenes.Length; i++)
            {
                LoadScene(scenes[i], voter.Add);
            }
        }
        else
        {
            callback?.Invoke();
        }
    }

    public void UnLoadScenes(EnterNodeType nodeType, List<SceneSetting> scenes, Action callback)
    {
        if (!scenes.IsNullOrEmpty())
        {
            Voter voter = new Voter(scenes.Count, callback);
            for (int i = 0; i < scenes.Count; i++)
            {
                if (scenes[i].runtimeLoad || nodeType == EnterNodeType.SkipNode)
                {
                    UnLoadScene(scenes[i].sceneName.ToString(), voter.Add);
                }
                else
                {
                    voter.Add();
                }
            }
        }
        else
        {
            callback?.Invoke();
        }
    }
    
    public void UnLoadScenes(string[] scenes, Action callback)
    {
        if (!scenes.IsNullOrEmpty())
        {
            Voter voter = new Voter(scenes.Length, callback);
            for (int i = 0; i < scenes.Length; i++)
            {
                UnLoadScene(scenes[i], voter.Add);
            }
        }
        else
        {
            callback?.Invoke();
        }
    }


    public void LoadScene(string scene, Action callback)
    {
        if (!currScene.Contains(scene))
        {
            currScene.Add(scene);
            GameScene.LoadAdditive(scene.ToString(), callback);
        }
        else
        {
            GameDebug.LogError($"当前已加载{scene}场景!!!!!!请确认是否提前加载,此后逻辑不会往下走");
        }
    }

    public void UnLoadScene(string scene, Action callback)
    {
        if (currScene.Contains(scene))
        {
            currScene.Remove(scene);
            GameScene.UnLoad(scene, callback);
        }
        else
        {
            GameDebug.LogError($"当前不包含{scene}场景!!!!!!请确认是否已卸载此场景.后续逻辑不会走");
        }
    }

    public override void Save()
    {
        string sceneStr = string.Join(ConstKey.Spite0.ToString(), currScene);
        LocalSave.Write(LocalSave.savePath , saveKey, sceneStr,"currentScene");
    }
}