using System;
using System.Collections;
using IngameDebugConsole;
using Module;
using SDK;
using Sirenix.OdinInspector;
using UnityEngine;

public class GamePlay : GameEntry<GamePlay>, IConfig
{
    IEnumerator Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep; //永不息屏
        
        SDK.SDKMgr.GetInstance().InitALLSDK(); //初始化sdk

        if (debugManager != null)
        {
            debugManager.gameObject.OnActive(GMUI);
        }
        
#if UNITY_ANDROID && !UNITY_EDITOR&&SDK
        string javaStr = SDK.SDKMgr.GetInstance().MyCommon.GetKeyStoreMD5();
        if (javaStr != GameConfig.userKey)
        {
            yield break;
        }
#endif
        SDKMgr.GetInstance().MyCommon.HideSplash();
        Loading.Open(UILoading.uiLoading,"AppStart");
        yield return AudioManager.Init(process); Log("音效");
        yield return Setting.Init(process); Log("设置完毕");
        yield return DataMgr.Instance.Init(process); Log("数据表");
        yield return PlayerInfo.Init(process);Log("玩家信息");
        yield return Language.Init(process); Log("多语言");
        yield return GameInfo.Init(process);Log("游戏信息");
        yield return Commercialize.Init(process); Log("经济系统");
        //yield return Analytics.Init(process); Log("打点");
        yield return SpriteLoader.Init(process); Log("图片");
        yield return Mission.Init(process); Log("关卡");
        yield return PropEntity.Init(process); Log("物品");
        yield return LocalSave.Init(process); Log("存档");
        yield return GameService.Init(process); Log("服务");
        yield return WeaponManager.Init(process); Log("玩家武器");
        yield return Collection.Init(process);Log("仓库");
        yield return DailyTaskManager.Instance.Init(process); Log("每日任务");
        yield return AchievementManager.Instance.Init(process); Log("成就");
        yield return StartGame(process);
    }
    private AsyncLoadProcess StartGame(AsyncLoadProcess asyncLoadProcess)
    {
        asyncLoadProcess.IsDone = false;
        GameScene.Load("UIScene", () =>
        {
            var seq = new RunTimeSequence();
            //seq.OnComplete(asyncLoadProcess.SetComplete);
            //实名认证
            seq.Add(new RunTimeAction(() =>
            {
                Log("实名");
                ShiMing(seq);
            }));
            //防沉迷
            seq.Add(new RunTimeAction(() =>
            {
                Log("防沉迷");
                FangChenMi(seq);
            }));
            seq.OnComplete(() =>
            {
                //if (firstGame)
                //{
                //    Log("第一次游戏");
                //    AppStartUI();
                //}
                //else
                //{
                //    UIController.Instance.Open("MainUI", UITweenType.Fade);
                //    Loading.Close(UILoading.uiLoading, "AppStart");
                //}
                UIController.Instance.Open("MainUI", UITweenType.Fade);
                Loading.Close(UILoading.uiLoading, "AppStart");
                asyncLoadProcess.SetComplete();
            });
            seq.NextAction();
        });
        return asyncLoadProcess;
    }

    private void ShiMing(RunTimeSequence seq)
    {
        if (SDKMgr.GetInstance().MyAntiaddictionSDKBase.IsRegistered())
        {
            seq.NextAction();
        }
        else
        {
            SDKMgr.GetInstance().MyAntiaddictionSDKBase.ShowRealNameDialog(new Action<string>(
                (s) =>
                {
                    if (s == "0")
                    {
                        seq.NextAction();
                    }
                    else
                    {
                        GameDebug.Log("实名认证,认证失败退出游戏");
                        SDKMgr.GetInstance().MyCommon.ExitGame();
                    }
                }));
        }
    }

    private void FangChenMi(RunTimeSequence seq)
    {
        var psave = DataMgr.Instance.GetSqlService<PlayerSaveData>();
        //检测是否成年人
        if (psave.WhereID(1).userType != "1")
        {
            if (SDKMgr.GetInstance().MyAntiaddictionSDKBase.GetUserType() == 0)
            {
                psave.Update(d => d.ID == 1, "userType", "1");
                seq.NextAction();
            }
            else
            {
                Loading.Close(UILoading.uiLoading, "AppStart");
                CommonPopup.Popup("贴心小提示", "亲爱的玩家,根据国家防沉迷政策,您今日剩余的游戏时长为0,快去休息一下吧~", null, new PupupOption()
                {
                    action = () =>
                    {
                        GameDebug.Log("防沉迷退出游戏");
                        SDKMgr.GetInstance().MyCommon.ExitGame();
                    },
                    title = "退出游戏"
                });
            }
        }
        else
        {
            seq.NextAction();
        }
    }

    /// <summary>
    ///开始游戏闪屏,带适龄提示
    /// </summary>
    private void AppStartUI()
    {
        // UIController.Instance.Open("OpenAppUI", UITweenType.None, new System.Action(() =>
        // {
        //     UIController.Instance.Open("MainUI", UITweenType.Fade);
        // }));
        //UIController.Instance.Open("MainUI", UITweenType.Fade);
    }

    public UnityEngine.Rendering.Universal.ForwardRendererData renderData;
    public bool firstGame = true;//初次游戏开场动画,测试用
    public bool GMUI = true;//gm工具开关
    public DebugLogManager debugManager;

    protected override void Update()
    {
        base.Update();
        SystemUpdate.Update();
        if (Input.GetKeyDown(KeyCode.T))
        {

            //UIController.Instance.Popup("AdsFail", UITweenType.None);
        }
        else if (Input.GetKeyDown(KeyCode.U))
        {
            GMUI = !GMUI;
            if (!GMUI)
            {
                UIViewBase view = UIController.Instance.Get("GameUI").viewBase;
                Transform[] ggg = view.transform.GetComponentsInChildren<Transform>(true);
                for (int i = 0; i < ggg.Length; i++)
                {
                    if (ggg[i].gameObject.name != "RightJoyStick" && ggg[i].gameObject != view.gameObject)
                    {
                        ggg[i].gameObject.OnActive(false);
                    }
                }
                for (int i = 0; i < ggg.Length; i++)
                {
                    if (ggg[i].gameObject.name == "RightJoyStick")
                    {
                        ggg[i].gameObject.transform.parent.gameObject.OnActive(true);
                    }
                }
                
            }
        }
        else if (Input.GetKeyDown(KeyCode.Y))
        {
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {

        }


    }

    public override void Back()
    {
        if (UIController.Instance.currentUI.viewBase.canPhysicExit && UIController.Instance.canPhysiceback)
        {
            UIController.Instance.Back();
            OnBack?.Invoke();
        }
    }

    public override void OnPause()
    {
#if !UNITY_EDITOR
        BattleController.Instance.Pause("ApplicationPause");
#endif
    }

    public override void OnContinue()
    {
#if !UNITY_EDITOR
        BattleController.Instance.Continue("ApplicationPause");
#endif
    }
    [Button]
    public void ChangeLanguage()
    {
        if (Language.currentLanguage == SystemLanguage.English)
        {
            Language.ChangeLanguage(SystemLanguage.Chinese);
        }
        else if (Language.currentLanguage == SystemLanguage.Chinese)
        {
            Language.ChangeLanguage(SystemLanguage.English);
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        DataMgr.Instance.Dispose();
    }
}