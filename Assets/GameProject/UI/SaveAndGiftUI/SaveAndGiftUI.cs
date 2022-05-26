using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Module;
namespace ProjectUI
{
    public class SaveAndGiftUI : UIViewBase
    {
        public UIBtnBase backBtn;
        public UIBtnBase saveBtn;
        public UIBtnBase giftBtn;
        public UIBtnBase weaponBtn;
        public UIBtnBase bulletBtn;

        protected override void OnChildStart()
        {
            backBtn.AddListener(OnClickBackBtn);
            saveBtn.AddListener(OnClickSaveBtn);
            giftBtn.AddListener(OnClickGiftBtn);
            weaponBtn.AddListener(OnClickWeaponBtn);
            //weaponBtn.interactable = TeachingManager.CompleteTeaching(TeachingName.WeaponUpgrade);
            bulletBtn.AddListener(OnClickBulletBtn);
        }

        public override void Refresh(params object[] args)
        {
            //weaponBtn.interactable = Player.player.weaponManager.ownWeapon.Count > 0;
            bulletBtn.interactable = TeachingManager.CompleteTeaching(TeachingName.WeaponFire);
        }

        private void OnClickBackBtn()
        {
            UIController.Instance.Back();
        }

        private void OnClickSaveBtn()
        {
            BattleController.Instance.Save();
        }

        private void OnClickGiftBtn()
        {
            UIController.Instance.Open("GiftUI", UITweenType.None);
        }

        private void OnClickWeaponBtn()
        {
            UIController.Instance.Open("WeaponUpgradeUI", UITweenType.None);
        }

        private void OnClickBulletBtn()
        {
            UIController.Instance.Open("BulletCreatUI", UITweenType.None);
        }
    }
}