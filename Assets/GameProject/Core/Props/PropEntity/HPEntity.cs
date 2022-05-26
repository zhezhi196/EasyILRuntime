using Module;
using Project.Data;

public class HPEntity : PropEntity
{
    public HPEntity(PropData dbData) : base(dbData)
    {
    }

    public override void OnButtonInBag(BagItem item)
    {
        OnUse();
    }

    public void OnUse()
    {
        BattleController.GetCtrl<BagPackCtrl>().ConsumeItem(dbData.ID, 1);
        //角色恢复逻辑
        Player.player.UseMorphine(int.Parse(GetPropArg(BattleController.Instance.ctrlProcedure.mission.dbData.difficulte + 1)));
        AnalyticsEvent.SendEvent(AnalyticsType.UseProps, dbData.ID.ToString(), false);
    }

    public override float GetReward(float rewardCount, int editorid,string[] match, RewardFlag flag)
    {
        float resu = base.GetReward(rewardCount,editorid, match, flag);
        if (resu >= 0.9f)
        {
            BattleController.Instance.Save(0);
        }

        return resu;
    }

    public override bool IsShowButton(IMatch prop, BagItem currCell)
    {
        return Player.player.hp < Player.player.playerAtt.hp;
    }
}