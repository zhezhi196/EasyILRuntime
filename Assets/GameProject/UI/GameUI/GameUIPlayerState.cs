using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Module;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;

public class GameUIPlayerState : MonoBehaviour
{
    public GameObject[] eyes;

    private int monsterState = 0;
    private string hpDownEffectPath = "UI/GameUI/HpDownEffect.prefab";
    [ReadOnly]
    public float iMaxLife = 600f;
    public Image lifeIcon;
    public Image maxLife;
    public Image currentLife;
    public RectTransform maxlifeLine;
    private float lifeVelocity = 0.0f;
    private float playerLowLife = 50f;
    public Color lowColor = Color.white;
    public Color normalColor = Color.white;
    [ReadOnly]
    public float iMaxEnergy = 240f;
    public Image maxEnergy;
    public Image currentEnergy;
    public RectTransform maxEnergyLine;
    public float changeSpeed = 0.1f;
    private float energyVelocity = 0.0f;
    public Color energyLowColor = Color.white;
    public Color energyNormalColor = Color.white;
    public GameObject lowEnergyEffect;

    private void Awake()
    {
        //hpRectTrans = hpSlider.GetComponent<RectTransform>();
        //hpSlider.value = Player.player.hp / Player.player.MaxHp;
        //hpGrayFill.fillAmount = Player.player.hp / Player.player.MaxHp;
        //hpNormalSize = hpRectTrans.sizeDelta;
        ////energyRectTrans = energySlider.GetComponent<RectTransform>();
        ////energySlider.value = Player.player.energy / Player.player.playerAtt.energy;
        ////energyNormalSize = energyRectTrans.sizeDelta;
        //strengthRectTrans = strengthSlider.GetComponent<RectTransform>();
        //strengthNormalSize = strengthRectTrans.sizeDelta;
       
        ////Player.player.onEnerggrChange += OnPlayerEnergyChange;
       
        iMaxLife = DataMgr.CommonData(33025).ToInt() * 2;
        iMaxEnergy = DataMgr.CommonData(33026).ToInt() * 4;
        Player.player.onHpChange += OnPlayerHpChange;
        Player.player.onStrengthChange += OnPlayerStrengthChange;
        Player.player.onAddStation += OnPlayerAddStation;
        Player.player.onRemoveStation += OnPlayerRemoveStation;
    }

    private void OnEnable()
    {
        Refesh();
    }

    public void Refesh()
    {
        //新体力能量槽
        maxLife.fillAmount = Player.player.MaxHp / iMaxLife;
        currentLife.fillAmount = Player.player.hp / iMaxLife;
        maxlifeLine.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(0, 180, 1 - Player.player.MaxHp / (iMaxLife * 0.5f)));
        maxEnergy.fillAmount = Player.player.MaxStrength / iMaxEnergy;
        currentEnergy.fillAmount = Player.player.strength / iMaxEnergy;
        maxEnergyLine.localEulerAngles = new Vector3(0, 0, -Mathf.Lerp(0, 180, 1 - Player.player.MaxStrength / (iMaxEnergy * 0.5f)));
        //重新打开Gameui是数值改变刷新
        float hV = Player.player.hp / iMaxLife;
        if (currentLife.fillAmount != hV)
        {
            OnPlayerHpChange(hpChangeValue);
        }
        float eV = Player.player.energy / iMaxEnergy;
        if (currentEnergy.fillAmount != eV)
        {
            OnPlayerEnergyChange(energyChangeValue);
        }
    }

    private void Update()
    {
        monsterState = 0;
        MonsterCtrl ctrl = BattleController.GetCtrl<MonsterCtrl>();
        if (ctrl != null)
        {
            for (int i = 0; i < ctrl.exitMonster.Count; i++)
            {
                if (monsterState == 0 && ctrl.exitMonster[i].showUiState == FightState.Alert)
                {
                    monsterState = 1;
                }
                else if (ctrl.exitMonster[i].showUiState == FightState.Fight)
                {
                    monsterState = 2;
                }

                if (monsterState == 2)
                {
                    break;
                }
            }

            for (int i = 0; i < eyes.Length; i++)
            {
                eyes[i].OnActive(i == monsterState);
            }
        }
    }

    private Tweener hpTweener;
    private float hpChangeValue = 0;
    private void OnPlayerHpChange(float change)
    {
        if (!gameObject.activeInHierarchy)//如果没显示不更新动画
        {
            hpChangeValue = change;
            return;
        }
        if (hpTweener != null)
        {
            hpTweener.Kill();
        }
       
        if (change < 0)
        {
            currentLife.fillAmount = Player.player.hp / iMaxLife;
        }
        else {
            //增长动画
            float newValue = Player.player.hp / iMaxLife;
            hpTweener = currentLife.DOFillAmount(newValue, 1f).SetUpdate(true);
        }
    }

    private Tweener energyTweener;
    private float energyChangeValue = 0;
    private void OnPlayerEnergyChange(float change)
    {
        if (!gameObject.activeInHierarchy)//如果没显示不更新动画
        {
            energyChangeValue = change;
            return;
        }

        if(energyTweener!=null)
        {
            energyTweener.Kill();
        }
        //if (change < 0)
        //{
        //    energySlider.value = Player.player.energy / Player.player.playerAtt.energy;
        //    AssetLoad.LoadGameObject<HpDownEffect>(energyDownEffectPath, energyRectTrans, (effect, obj) =>
        //    {
        //        float width = energyRectTrans.sizeDelta.x * (Mathf.Abs(change) / Player.player.playerAtt.energy);
        //        RectTransform rect = effect.GetComponent<RectTransform>();
        //        rect.anchoredPosition = new Vector2(energyRectTrans.sizeDelta.x * energySlider.value, rect.anchoredPosition.y);//+ width * 0.5f
        //        rect.sizeDelta = new Vector2(width, rect.sizeDelta.y);
        //        effect.gameObject.OnActive(true);
        //        effect.Play();
        //    });
        //}
        //else { 
        //    //增长动画
        //    float newValue = Player.player.energy / Player.player.playerAtt.energy;
        //    energyTweener = energySlider.DOValue(newValue, 1f).SetUpdate(true);
        //}
    }

    private void OnPlayerStrengthChange(float change)
    {
        currentEnergy.fillAmount = Player.player.strength / iMaxEnergy;
    }
    private void OnPlayerRemoveStation(Player.Station obj)
    {
        if (obj == Player.Station.Weak)
        {
            currentEnergy.color = energyNormalColor;
            lowEnergyEffect.OnActive(false);
        }
    }

    private void OnPlayerAddStation(Player.Station obj)
    {
        if (obj == Player.Station.Weak)
        {
            currentEnergy.color = energyLowColor;
            lowEnergyEffect.OnActive(true);
        }

    }
}
