using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Module;
using Sirenix.OdinInspector;
using UnityEngine.UI;

namespace ProjectUI
{
    public class BattleSettingUI : UIViewBase
    {
        public SettingCommon settingCommon;

        public UIBtnBase ysBtn;
        public UIBtnBase exitBtn;
        public UIBtnBase uiSetBtn;
        public UIBtnBase backBtn;
        

        protected override void OnChildStart()
        {
            settingCommon.Init();
            uiSetBtn.AddListener(OnUISetting);
            backBtn.AddListener(OnClickBackBtn);
            exitBtn.AddListener(OnClickExitBtn);
            if (Channel.isChina)
            {
                ysBtn.OnActive(true);
                ysBtn.AddListener(OnClickYSBtn);
            }
        }

        private void OnUISetting()
        {
            UIController.Instance.Open("DIYGameUI", UITweenType.None,settingCommon.enterMode == 1 ? 1f : 0.9f);
        }

        public override void Refresh(params object[] args)
        {
           settingCommon.Refresh((int)args[0]);
        }

      
        private void OnClickYSBtn()
        {
            SDK.SDKMgr.GetInstance().MyCommon.ShowPrivacyPolicy();
        }

        private void OnClickExitBtn()
        {
            CommonPopup.Popup(Language.GetContent("701"), Language.GetContent("712"), null,
                new PupupOption(() =>
                {
                    BattleController.Instance.Save();
                    BattleController.Instance.ExitBattle(OutGameStation.Break);
                }, Language.GetContent("713")),
                new PupupOption(() => {
                }, Language.GetContent("703")));
        }
        
        private void OnClickBackBtn()
        {
            UIController.Instance.Back();
        }
    }
}