using System;
using System.Collections.Generic;
using Module;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameUI : UIViewBase
{
    public Slider playerHp;
    public Text angryTime;
    public Transform skillParent;
    public JoyStick joyStick;
    public Vector2 pointDown;
    public UIBtnBase setting;
    public UIBtnBase fight;
    public UIBtnBase fangyu;
    public UIBtnBase fanji;

    public void SwitchButtonStatas(PlayerPosture posture)
    {
        fight.gameObject.OnActive(posture == PlayerPosture.Normal || posture == PlayerPosture.NoAvoid);
        fangyu.gameObject.OnActive(posture == PlayerPosture.CanAvoid);
        fanji.gameObject.OnActive(posture == PlayerPosture.Avoid);
    }

    protected override void OnChildStart()
    {
        base.OnChildStart();
        setting.AddListener(OnSetting);
        fight.AddListener(OnFight);
        fangyu.AddListener(OnFangyu);
        fanji.AddListener(OnFanji);
        Player.player.onSwitchPosture += SwitchButtonStatas;
    }


    private void OnDestroy()
    {
        Player.player.onSwitchPosture -= SwitchButtonStatas;
    }

    private void OnFight()
    {
        Player.player.StartFight();
    }
    
    
    private void OnFanji()
    {
        Player.player.CounterAttack();
    }

    private void OnFangyu()
    {
        Player.player.Avoid();
    }

    private void OnSetting()
    {
        UIController.Instance.Open("BattleSettingUI", UITweenType.None);
    }

    public override void OnOpenStart()
    {
        var skillCtrl = Player.player.GetAgentCtrl<SkillCtrl>();
        Voter voter = new Voter(skillCtrl.allSkill.Count,
            () =>
            {
                skillParent.Sort<GameSkillShow>((a, b) => a.skillInstance.skillModle.SelectIndex.CompareTo(b.skillInstance.skillModle.SelectIndex));
            });
        for (int i = 0; i < skillCtrl.allSkill.Count; i++)
        {
            if (skillCtrl.allSkill[i] is PlayerSkillInstance temSKi)
            {
                LoadPrefab<GameSkillShow>("skillItem", skillParent, fd =>
                {
                    fd.SetSkill(temSKi);
                    voter.Add();
                });
            }
        }
    }

    protected override void Update()
    {
        base.Update();

        if (Player.player.levelIndex == 2)
        {
            angryTime.gameObject.OnActive(true);
            angryTime.text = "剩余时间: " + Player.player.angryTime.ToString("F0");
        }
        else
        {
            angryTime.gameObject.OnActive(false);
        }
    }
}
