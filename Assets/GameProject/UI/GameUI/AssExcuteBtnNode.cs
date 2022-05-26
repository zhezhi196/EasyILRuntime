using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Module;

public class AssExcuteBtnNode : MonoBehaviour
{
    public string assetPath = "UI/GameUI/AssBtn.prefab";
    public Dictionary<AttackMonster, AssExcuteBtn> assMonsterDic = new Dictionary<AttackMonster, AssExcuteBtn>();

    private void Start()
    {
        EventCenter.Register<AttackMonster, TimeLineType>(EventKey.MonsterTimeLine, OnMonsterTimeline);
    }

    void Update()
    {
        if (!isActiveAndEnabled && Player.player.canAssMonster)
            return;
        RefeshAllMonster();
        RefeshDic();
    }

    private void OnDestroy()
    {
        EventCenter.UnRegister<AttackMonster, TimeLineType>(EventKey.MonsterTimeLine, OnMonsterTimeline);
    }

    void OnMonsterTimeline(AttackMonster monster, TimeLineType timeLineType)
    {
        if (monster != null)
        {
            TryRemoveMonster(monster);
        }
    }

    private void RefeshAllMonster()
    {
        MonsterCtrl ctrl = BattleController.GetCtrl<MonsterCtrl>();
        if (ctrl != null)
        {
            for (int i = 0; i < ctrl.exitMonster.Count; i++)
            {
                if (ctrl.exitMonster[i].canAss)
                {
                    TryAddMonster(ctrl.exitMonster[i] as AttackMonster);
                }
            }
        }
    }
    private List<AttackMonster> removeMonster = new List<AttackMonster>();
    private void RefeshDic()
    {
        removeMonster.Clear();
        foreach (KeyValuePair<AttackMonster, AssExcuteBtn> item in assMonsterDic)
        {
            if (item.Key == null || !item.Key.canAss || !item.Key.isAlive)
            {
                removeMonster.Add(item.Key);
            }
            else
            {
                if (item.Value != null)
                    item.Value.UpdateNode();
            }
        }
        for (int i = 0; i < removeMonster.Count; i++)
        {
            TryRemoveMonster(removeMonster[i]);
        }
    }

    public void TryAddMonster(AttackMonster monster)
    {
        if (!assMonsterDic.ContainsKey(monster))
        {
            assMonsterDic.Add(monster, null);
            AssetLoad.LoadGameObject<AssExcuteBtn>(assetPath, transform, (btn, obj) =>
            {
                if (assMonsterDic.ContainsKey(monster))
                {
                    btn.monster = monster;
                    //if (monster is Cat)
                    //{
                    //    btn.btnBase.image.sprite = btn.icons[1];
                    //}
                    //else
                    //{
                    //    btn.btnBase.image.sprite = btn.icons[0];
                    //}
                    btn.SetBtnPos();
                    btn.gameObject.OnActive(true);
                    assMonsterDic[monster] = btn;
                }
                else
                {
                    btn.ResetBtn();
                }
            });
        }
    }

    public void TryRemoveMonster(AttackMonster monster)
    {
        if (assMonsterDic.ContainsKey(monster))
        {
            assMonsterDic[monster]?.ResetBtn();
            assMonsterDic.Remove(monster);
        }
    }
}
