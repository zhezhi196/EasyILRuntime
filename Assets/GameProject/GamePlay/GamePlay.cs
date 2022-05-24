using System.Collections;
using Module;
using UnityEngine;

public class GamePlay: GameEntry<GamePlay>, IConfig
{
    IEnumerator Start()
    {
        yield return Setting.Init(process);Log("设置完毕");
        yield return DataInit.Instance.Init(process); Log("数据表");
        yield return PlayerInfo.Init(process);Log("玩家信息");
        yield return GameInfo.Init(process);Log("游戏信息");
        yield return Language.Init(process); Log("多语言");
        yield return SpriteLoader.Init(process); Log("图片");
        yield return Mission.Init(process);Log("关卡");
        yield return PlayerSkill.Init(process);Log("玩家技能");
        yield return Money.Init(process);Log("经济系统");
        yield return Talent.Init(process);Log("天赋系统");
        
        yield return StartGame(process);
    }
    
    private AsyncLoadProcess StartGame(AsyncLoadProcess asyncLoadProcess)
    {
        asyncLoadProcess.IsDone = false;
        GameScene.Load("UIScene", () =>
        {
            UIController.Instance.Open("MainUI", UITweenType.Fade);
            asyncLoadProcess.SetComplete();
        });
        return asyncLoadProcess;
    }

    protected override void Update()
    {
        base.Update();
        PlayerSkill.Update();
        
        if (Input.GetKeyDown(KeyCode.T))
        {
            //Config.SetChannel(ChannelType.googlePlay);
            //UIController.Instance.Open("SelectSkillUI", UITweenType.None);
        }
        else if (Input.GetKeyDown(KeyCode.U))
        {
            Player.player.OnTransform(Player.player.levelIndex == 0 ? 1 : 0);
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
        BattleController.Instance.Pause("ApplicationPause");
    }

    public override void OnContinue()
    {
        BattleController.Instance.Continue("ApplicationPause");

    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        DataInit.Instance.Dispose();
    }
}