using System.Collections.Generic;
using Module;

public class MonsterCtrl : BattleSystem
{
    public RunTimeAction loadMonster;
    public RunTimeAction bornMonster;
    public List<Monster> exitMonster = new List<Monster>();
    
    public override void OnNodeEnter(TaskNode node)
    {
        loadMonster = new RunTimeAction(() =>
        {
            exitMonster.Clear();
            Voter voter = new Voter(node.gameEditorInfo.monsterCreators.Length, () =>
            {
                BattleController.Instance.NextFinishAction("loadMonster");
                loadMonster = null;
            });
            for (int i = 0; i < node.gameEditorInfo.monsterCreators.Length; i++)
            {
                node.gameEditorInfo.monsterCreators[i].LoadMonster(mos=>
                {
                    exitMonster.Add(mos);
                    voter.Add();
                });
            }
        });

        bornMonster = new RunTimeAction(() =>
        {
            for (int i = 0; i < exitMonster.Count; i++)
            {
                exitMonster[i].Born();
            }
            BattleController.Instance.NextFinishAction("bornMonster");
            bornMonster = null;
        });
    }
}