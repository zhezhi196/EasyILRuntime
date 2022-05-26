using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Module;
using DG.Tweening;

namespace ProjectUI
{
    public class WeaponGiftSlot : MonoBehaviour
    {
        public GameUI gameUI;
        public List<GameUIWeaponSlot> weaponSlots = new List<GameUIWeaponSlot>();
        public UIBtnBase midBtn;
        public CanvasGroup midBtnCanvsa;
        public Text midCount;
        private int propID = 20032;
        private PropEntity morphineEntity;
        private bool refreshing = false;
        public CanvasGroup canvasGroup;

        private void Awake()
        {
            midBtnCanvsa = midBtn.transform.GetChild(0).GetComponent<CanvasGroup>();
        }
        private void Start()
        {
            for (int i = 0; i < weaponSlots.Count; i++)
            {
                int index = i;
                weaponSlots[i].toggle.onValueChanged.AddListener((b) =>
                {
                    if (b)
                    {
                        OnSelectWeapon(index);
                    }
                });
            }
            midBtn.AddListener(OnMinBtnClick);
            morphineEntity =  PropEntity.GetEntity(propID);
        }

        private void OnDisable()
        {
            DOTween.Kill(gameObject);
        }

        public void Show()
        {
            gameObject.OnActive(true);
            Refresh();
        }

        public void Close()
        {
            canvasGroup.alpha = 0;
            gameObject.OnActive(false);
        }

        public void Refresh()
        {
            Voter voter = new Voter(6, () =>{
                canvasGroup.DOFade(1, 0.3f).SetId(gameObject).SetUpdate(true);
            });
            refreshing = true;
            for (int i = 0; i < Player.player.weaponManager.weaponSolts.Count; i++)
            {
                if (i < weaponSlots.Count)
                {
                    weaponSlots[i].Refresh(Player.player.weaponManager.weaponSolts[i],()=> {
                        voter.Add();
                    });
                }
            }
            int propCount = BattleController.GetCtrl<BagPackCtrl>().GetBagItemNum(propID);
            midBtn.OnActive(propCount > 0);
            midBtn.interactable = Player.player.hp < Player.player.MaxHp;
            midBtnCanvsa.alpha = (Player.player.hp >= Player.player.MaxHp) ? 0.3f : 1f;
            midCount.text = propCount.ToString();
            refreshing = false;
        }

        private void OnSelectWeapon(int index)
        {
            if (!refreshing)
            {
                if (weaponSlots[index].weapon != null)
                {
                    //AudioPlay.PlayOneShot("qieHuanWuQi");
                    Player.player.ChangeWeapon(weaponSlots[index].weapon);
                }
                gameUI.CloseWeaponSelect();
                Module.GameDebug.Log("SelectWeapon:" + index);
            }
        }

        private void OnMinBtnClick()
        {
            ((HPEntity)morphineEntity).OnUse();
            gameUI.CloseWeaponSelect();
        }
    }
}
