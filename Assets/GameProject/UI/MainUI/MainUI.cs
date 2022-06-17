using Module;
using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
namespace ProjectUI
{
    public class MainUI : UIViewBase
    {
        public GameObject mainPanel;
        public UIBtnBase continueBtn;
        public UIBtnBase newgameBtn;
        public UIBtnBase setBtn;
        //public UIBtnBase backBtn;
        //public GameObject otherPanel;

        public UIBtnBase collectionBtn;
        public RedPoint collectiontip;
        public GameObject levelPanel;
        public UIBtnBase levelBackBtn;
        public UIBtnBase storebtn;
        public UIBtnBase dailyTaskBtn;
        public RedPoint tasktip;
        public UIBtnBase achieveBtn;
        public RedPoint achievetip;
        public UIBtnBase[] rankBtn;
        [Header("其他")]
        public UIBtnBase ageTipBtn;
        public UIBtnBase clearData;
        public UIBtnBase moreGameBtn;
        public GameObject qqLabel;
        [Header("额外内容")]
        public UIBtnBase otherBtn;
        public GameObject lockIcon;
        public Text otherLable;
        public GameObject otherLableLight;

        protected override void OnChildStart()
        {
            base.OnChildStart();
            continueBtn.AddListener(OnContinue);
            newgameBtn.AddListener(ClickNewGame);
            setBtn.AddListener(OnClickSetBtn);
            otherBtn.AddListener(OpenOtherPanel);
            //backBtn.AddListener(CloseOtherPanel);
            levelBackBtn.AddListener(BackBtn);
            collectionBtn.AddListener(OpenCollection);
            storebtn.AddListener(OpenStore);
            dailyTaskBtn.AddListener(OpenDailyTask);
            achieveBtn.AddListener(OpenAchievement);
            
            if (Channel.isChina)
            {
                ageTipBtn.OnActive(true);
                clearData.OnActive(true);
                ageTipBtn.AddListener(OnAgeTipBtn);
                clearData.AddListener(OnClearData);
                /*if (Channel.channel == ChannelType.Oppo)
                {
                    qqLabel.OnActive(true);
                    moreGameBtn.OnActive(true);
                    moreGameBtn.AddListener(OpenMoreGame);
                }*/
            }
            for (int i = 0; i < rankBtn.Length; i++)
            {
                int index = i;
                rankBtn[i].AddListener(OnRankButton,index);
            }
            if (Mission.missionList.Find(m => m.dbData.ID == 17001).station.ToInt() > 2)
            {
                lockIcon.OnActive(false);
                otherLable.SetAlpha(1f);
                otherLableLight.OnActive(true);
            }
            //AudioPlay.PlayBackGroundMusic("BGM_aiji");
            HttpCache.Instance.login.Login(() => {});
            HttpCache.Instance.login.GetTime();
            Commercialize.QueryPurchases();
        }

        private void OpenMoreGame()
        {
            //SDK.SDKMgr.GetInstance().MyCommon.JumpLeisureSubject();
        }

        private void OnEnable()
        {
            achievetip.SetTarget(AchievementManager.Instance.Achievements.ToArray());
            tasktip.SetTarget(DailyTaskManager.Instance.DailyTasks.ToArray());
            collectiontip.SetTarget(Collection.collectionList.ToArray());
        }

        private void OnRankButton(int obj)
        {
            UIController.Instance.Open("RankUI", UITweenType.None, obj);
        }

        private void OpenAchievement()
        {
            UIController.Instance.Open("DailyTaskUI", UITweenType.None,"Achievement");
        }
        //private void AchievChange(Achievement arg1, AchievementReward arg2)
        //{
        //    achievetip.OnActive(false);
        //    for (int i = 0; i < Achievement.allAchievement.Count; i++)
        //    {
        //        if (Achievement.allAchievement[i].currReward.station == TaskStation.CompleteUnReward)
        //        {
        //            achievetip.OnActive(true);
        //            break;
        //        }
        //    }
        //}

        private void OpenDailyTask()
        {
            UIController.Instance.Open("DailyTaskUI", UITweenType.None,"DailyTask");
        }

        //private void TaskChange(DailyTask arg1, TaskStation arg2)
        //{
        //    tasktip.OnActive(false);
        //    for (int i = 0; i < DailyTask.dailyTask.Count; i++)
        //    {
        //        if (DailyTask.dailyTask[i].station == TaskStation.CompleteUnReward)
        //        {
        //            tasktip.OnActive(true);
        //            break;
        //        }
        //    }
        //}

        private void OpenStore()
        {
            Commercialize.OpenStore();
        }

        private void OpenOtherPanel()
        {
            if (Mission.missionList.Find(m => m.dbData.ID == 17001).isComplete)
            {
                UIController.Instance.Open("ArtUI", UITweenType.Fade);
            }
            else {
                CommonPopup.Popup(Language.GetContent("701"), Language.GetContent("725"), null,
                    new PupupOption(null, Language.GetContent("702")));
            }
        }

        //private void CloseOtherPanel()
        //{
        //    mainPanel.OnActive(true);
        //    otherPanel.OnActive(false);
        //}

        public override void Refresh(params object[] args)
        {
            if (!LocalSave.hasSave)
            {
                continueBtn.gameObject.OnActive(false);
            }
            GlobleTrigger.Trigger("MissionComplete",
                arg => { UIController.Instance.Popup("NewDifficulty", UITweenType.None, arg); });
        }

        private void OnContinue()
        {
            string missionid = LocalSaveFile.GetString(LocalSave.savePath, "BattleMission");
            BattleController.Instance.EnterBattle(Mission.missionList.Find(fd => fd.dbData.ID.ToString() == missionid),
                EnterNodeType.FromSave);
        }

        private void ClickNewGame()
        {
            //初次游戏开场动画
            if (GamePlay.Instance.firstGame && !LocalFileMgr.ContainKey("UITimeline"))
            {
                //BattleController.Instance.EnterBattle(Mission.missionList.Find(m => m.dbData.ID == 17001), EnterNodeType.Restart);   
                this.gameObject.OnActive(false);
                UISceneCtrl.Instance.PlayTimeline(() => {
                    LocalFileMgr.SetInt("UITimeline", 1);
                    BattleController.Instance.EnterBattle(Mission.missionList.Find(m => m.dbData.ID == 17001), EnterNodeType.Restart);
                });
            }
            else {
                levelPanel.OnActive(true);
            }
        }

        private void OnClickSetBtn()
        {
            UIController.Instance.Open("SettingUI", UITweenType.None);
        }

        private void BackBtn()
        {
            levelPanel.OnActive(false);
        }

        private void OpenCollection()
        {
            UIController.Instance.Open("CollectionUI", UITweenType.None);
        }

        private void OnAgeTipBtn()
        {
            UIController.Instance.Popup("AgeTipPopup", UITweenType.Fade);
        }

        private void OnClearData()
        {
            PupupOption option = new PupupOption(() =>
            {
                UIController.Instance.canPhysiceback = false;
                CommonPopup.Close();
                LocalFileMgr.RemoveAllKey();
                DataMgr.Instance.DeleteData();
                LocalSave.DeleteFile();
                CommonPopup.Popup("提醒", "请重新启动游戏",null, new PupupOption(() =>
                {
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
                }, "确定"));
            }, "确定");
            PupupOption option2 = new PupupOption(() =>
            {
                CommonPopup.Close();
            }, "取消");
            CommonPopup.Popup("提示", "删除存档将导致所有数据丢失,且无法找回.\n你确定要这么做么?",null, option, option2);
        }

        public override void OnExit(params object[] args)
        {
            CommonPopup.Popup(Language.GetContent("701"), Language.GetContent("711"), null,
                new PupupOption(() => { Application.Quit(); }, Language.GetContent("1112")),
                new PupupOption(null, Language.GetContent("703")));
        }
    }
}
