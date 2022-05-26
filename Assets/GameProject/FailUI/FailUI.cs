using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Module;
using UnityEngine.UI;

namespace ProjectUI
{
    public class FailUI : UIViewBase
    {
        public UIBtnBase doneBtn;
        public Text difficulty;
        public Text timeText;
        public Text bulletText;
        public Text killText;
        public Text deatthText;
        private string[] diffuculte = new string[] {"301","302","303" };
        System.Action callBack;
        protected override void OnChildStart()
        {
            doneBtn.AddListener(OnClickDoneBtn);
        }
        public override void Refresh(params object[] args)
        {
            //AnalyticsEvent.SendEvent(AnalyticsType.GameEnd, null);//通关打点
            if (args.Length > 0)
            {
                callBack = args[0] as System.Action;
            }
            int h, m, s;
            Tools.Float2Time(BattleController.Instance.ctrlProcedure.gameTime, out h,out m,out s);
            difficulty.text = Language.GetContent(diffuculte[BattleController.Instance.ctrlProcedure.mission.difficulte.ToInt()]);
            timeText.text = string.Format(Language.GetContent("1507"), h, m, s);
            bulletText.text = BattleController.GetCtrl<PlayerCtrl>().fireCount.ToString();
            deatthText.text = BattleController.GetCtrl<PlayerCtrl>().deathCount.ToString();
            killText.text = BattleController.GetCtrl<MonsterCtrl>().DeadMonster.ToString();
        }
        private void OnClickDoneBtn()
        {
            //UIController.Instance.Back();
            gameObject.OnActive(false);
            callBack?.Invoke();
            callBack = null;
        }
    }
}