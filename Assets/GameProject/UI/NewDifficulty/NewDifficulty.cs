using Module;
using SecondChapter;
using UnityEngine;
using UnityEngine.UI;

namespace Chapter2.UI
{
    public class NewDifficulty : UIViewBase
    {
        public UIBtnBase button;
        public UIBtnBase back;
        public Text title;
        private Mission mission;
        public Image adsIcon;
        public Label buttonText;
        public Label buttonTextConfirm;
        public Image icon;
        public Transform content;
        protected override void OnChildStart()
        {
            base.OnChildStart();
            button.AddListener(OnOk);
            back.AddListener(OnBack);
            content.localScale = Tools.GetScreenScale();
        }

        private void OnBack()
        {
            OnExit();
        }

        public override void Refresh(params object[] args)
        {
            base.Refresh(args);
            mission = args[0] as Mission;
            title.text = Language.GetContent(mission.dbData.title);
            if (mission.unlockIap.iapState == IapState.Normal)
            {
                adsIcon.gameObject.OnActive(true);
                buttonTextConfirm.gameObject.OnActive(false);
            }
            else
            {
                adsIcon.gameObject.OnActive(false);
                buttonTextConfirm.gameObject.OnActive(true);
                back.gameObject.OnActive(false);
            }

            mission.GetIcon(TypeList.Normal, sp => icon.sprite = sp);
        }

        private void OnOk()
        {
            mission.unlockIap.GetReward(res =>
            {
                if (res.result == IapResultMessage.Success)
                {
                    OnExit();
                    mission.GetIcon(TypeList.Normal,s =>
                    {
                        CommonPopup.Popup(Language.GetContent("701"), Language.GetContent(mission.dbData.unlockDes), s);
                    });
                    
                }
            });
        }
    }
}