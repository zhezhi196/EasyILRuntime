using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Module;
using System;

namespace ProjectUI
{
    public class TopBtnsGroup : MonoBehaviour
    {
        public UIBtnBase upgradeBtn;
        public GameObject creatTip;
        public UIBtnBase bagBtn;
        public UIBtnBase setBtn;
        public UIBtnBase tipBtn;
        public UIBtnBase storeBtn;
        public UIBtnBase taskBtn;
        public RedPoint taskRedpoint;
        public UIBtnBase mapBtn;
        public GameObject mapTip;
        public UIBtnBase rewardBtn;
        public GameObject rewardTip;
        public ProgressTips progressTips;
        public GameObject tipRedpoint;

        public float spacing = 80f;
        public RectTransform btnGridTrans;
        private List<GameObject> btnObjs = new List<GameObject>();

        public void Start()
        {
            upgradeBtn.AddListener(OnUpgradeBtn);
            btnObjs.Add(upgradeBtn.gameObject);
            setBtn.AddListener(OnSetBtn);
            btnObjs.Add(setBtn.gameObject);
            bagBtn.AddListener(OnBagBtn);
            btnObjs.Add(bagBtn.gameObject);
            tipBtn.AddListener(OnTipBtn);
            btnObjs.Add(tipBtn.gameObject);
            storeBtn.AddListener(OnStoreBtn);
            btnObjs.Add(storeBtn.gameObject);
            taskBtn.AddListener(OnTaskBtn);
            btnObjs.Add(taskBtn.gameObject);
            mapBtn.AddListener(OnMapBtn);
            btnObjs.Add(mapBtn.gameObject);
            rewardBtn.AddListener(OnRewardBtn);
            btnObjs.Add(rewardBtn.gameObject);
            rewardBtn.OnActive(BattleController.GetCtrl<LimitRewardCtrl>().reward != null);//第一次打开主动刷新限时礼包
            // rewardBtn.OnActive(BattleController.GetCtrl<LimitRewardCtrl>().reward != null && TeachingManager.CompleteTeaching(TeachingName.WeaponFire));
            //刷新按钮显示状态
            // upgradeBtn.OnActive(TeachingManager.CompleteTeaching(TeachingName.GiftTeaching));
            // bagBtn.OnActive(TeachingManager.CompleteTeaching(TeachingName.TipTeaching));
            // //tipBtn.OnActive(TeachingManager.CompleteTeaching(TeachingName.TipTeaching));
            // storeBtn.OnActive(TeachingManager.CompleteTeaching(TeachingName.WeaponFire));
            // taskBtn.OnActive(TeachingManager.CompleteTeaching(TeachingName.WeaponFire));
            PropsBase.OnUnInteractiveProp += ChangeTipBtnTip;
            EventCenter.Register<bool>(EventKey.LimitReward,LimiRewardChange);
            EventCenter.Register<TeachingName>(EventKey.GameTeachStart, OnTeachingStart);
            EventCenter.Register<TeachingName>(EventKey.GameTeachComplete, OnTeachingComplete);
            EventCenter.Register(EventKey.AllWeaponEmpty, OnAllWeaponEmpty);
            EventCenter.Register<NodeBase>(EventKey.OnNodeEnter, OnNextNode);
            RefeshGridPos();
        }

        private void LateUpdate()
        {
            RefreshProgress();
        }

        private void OnEnable()
        {
            List<IRedPoint> redPoints = new List<IRedPoint>();
            redPoints.AddRange(AchievementManager.Instance.Achievements);
            redPoints.AddRange(DailyTaskManager.Instance.DailyTasks);
            taskRedpoint.SetTarget(redPoints.ToArray());
        }

        private void OnDestroy()
        {
            PropsBase.OnUnInteractiveProp -= ChangeTipBtnTip;
            EventCenter.UnRegister<bool>(EventKey.LimitReward, LimiRewardChange);
            EventCenter.UnRegister<TeachingName>(EventKey.GameTeachStart, OnTeachingStart);
            EventCenter.UnRegister<TeachingName>(EventKey.GameTeachComplete, OnTeachingComplete);
            EventCenter.UnRegister(EventKey.AllWeaponEmpty, OnAllWeaponEmpty);
            EventCenter.UnRegister<NodeBase>(EventKey.OnNodeEnter, OnNextNode);
        }

        private void OnUpgradeBtn()
        {
            UIController.Instance.Open("SaveAndGiftUI", UITweenType.None);
            creatTip.OnActive(false);
        }

        private void OnSetBtn()
        {
            UIController.Instance.Open("BattleSettingUI", UITweenType.None, 2);
        }

        private void OnBagBtn()
        {
            UIController.Instance.Open("BagUI", UITweenType.None);
        }

        public void OnTaskBtn()
        {
            UIController.Instance.Open("DailyTaskUI", UITweenType.None, "DailyTask");
        }

        public void OnStoreBtn()
        {
            Commercialize.OpenStore();
        }
        private void OnMapBtn()
        {
            mapTip.OnActive(false);
            UIController.Instance.Open("MapUI", UITweenType.None);
        }

        private void OnRewardBtn()
        {
            UIController.Instance.Open("LimitRewardUI", UITweenType.None);
            rewardTip.OnActive(false);
        }

        private void LimiRewardChange(bool b)
        {
            rewardBtn.OnActive(b);
            if (b)
            {
                rewardTip.OnActive(true);
            }
        }

        private void RefreshProgress()
        {
            progressTips.Refresh(ActiveTipsButton);
        }

        private void ActiveTipsButton(bool boo)
        {
            if (tipBtn.gameObject.activeInHierarchy == !boo)//&& TeachingManager.CompleteTeaching(TeachingName.TipTeaching)
            {
                tipBtn.gameObject.OnActive(boo);
                RefeshGridPos();
            }
        }

        //提示按钮提示
        private void ChangeTipBtnTip(bool b)
        {
            //tipBtnTip.OnActive(b);
            tipRedpoint.OnActive(b);
            //if (showCreatTip)
            //{
            //    creatTip.OnActive(!b);
            //}
        }
        //private void AchievChange(Achievement arg1, AchievementReward arg2)
        //{
        //    RefeshTaskRedPoint();
        //}
        //private void TaskChange(DailyTask arg1, TaskStation arg2)
        //{
        //    RefeshTaskRedPoint();
        //}

        //private void RefeshTaskRedPoint()
        //{
        //    taskRedpoint.OnActive(false);
        //    for (int i = 0; i < DailyTask.dailyTask.Count; i++)
        //    {
        //        if (DailyTask.dailyTask[i].station == TaskStation.CompleteUnReward)
        //        {
        //            taskRedpoint.OnActive(true);
        //            break;
        //        }
        //    }
        //    if (taskRedpoint.activeSelf)
        //        return;
        //    for (int i = 0; i < Achievement.allAchievement.Count; i++)
        //    {
        //        if (Achievement.allAchievement[i].currReward.station == TaskStation.CompleteUnReward)
        //        {
        //            taskRedpoint.OnActive(true);
        //            break;
        //        }
        //    }
        //}

        private void OnTipBtn()
        {
            PropsBase.isCountTime = false;
            ChangeTipBtnTip(false);
            string showKey = "ShowTips";//首次免费
            //PupupOption option = default;
            if (!LocalFileMgr.ContainKey(showKey) || BattleController.GetCtrl<ProgressCtrl>().iapBag.iapState == IapState.Skip)
            {
                CommonPopup.Popup(Language.GetContent("701"), Language.GetContent("714"), null,
                        new PupupOption(null, Language.GetContent("703")), 
                        new PupupOption(() =>
                        {
                            LocalFileMgr.Record(showKey);
                            BattleController.GetCtrl<ProgressCtrl>().ShowTips(true, null);
                        }, Language.GetContent("2213")));
            }
            else
            {
                //SDK.SDKMgr.GetInstance().MyAdSDK.EntryRewardVideoAdScene(string.Empty);//广告统计
                SpriteLoader.LoadIcon("GameUI_guanggao",sp =>
                  {
                      CommonPopup.Popup(Language.GetContent("701"), Language.GetContent("714"), null,
                          new PupupOption(null, Language.GetContent("703")), 
                          new PupupOption(() => BattleController.GetCtrl<ProgressCtrl>().ShowTips(false, null), Language.GetContent("715"), sp));
                  });
                //option = new PupupOption(() => BattleController.GetCtrl<ProgressCtrl>().ShowTips(false,null), "获取提示");
            }

            //BattleController.Instance.Pause(gameObject.name);
            //SpriteLoader.LoadIcon("kanguanggao",
            //    sp =>
            //    {
            //        CommonPopup.Popup(Language.GetContent("701"), "观看广告以获取提示", sp,
            //            new PupupOption(null, Language.GetContent("703")), option);
            //    });
        }

        public void RefeshGridPos()
        {
            int hideCount = 0;
            for (int i = 0; i < btnObjs.Count; i++)
            {
                if (!btnObjs[i].activeSelf)
                {
                    hideCount += 1;
                }
            }
            btnGridTrans.anchoredPosition = (new Vector2(spacing * hideCount,0));
        }

        private void OnTeachingStart(TeachingName teachingName)
        {
            switch (teachingName)
            {
                case TeachingName.MoveTeaching:
                    break;
                case TeachingName.GiftTeaching:
                    upgradeBtn.OnActive(true);
                    break;
                case TeachingName.MeleeAttack:
                    break;
                case TeachingName.CrouchTeaching:
                    break;
                case TeachingName.TipTeaching:
                    bagBtn.OnActive(true);
                    tipBtn.OnActive(true);
                    break;
                case TeachingName.RunTeaching:
                    break;
                case TeachingName.WeaponFire:
                    break;
                case TeachingName.AssTeaching:
                    break;
                case TeachingName.WeaponUpgrade:
                    break;
                case TeachingName.WeaponChange:
                    break;
                case TeachingName.StrengthTeaching:
                    break;
                default:
                    break;
            }
        }

        private void OnTeachingComplete(TeachingName teachingName)
        {
            switch (teachingName)
            {
                case TeachingName.MoveTeaching:
                    break;
                case TeachingName.GiftTeaching:
                    break;
                case TeachingName.MeleeAttack:
                    break;
                case TeachingName.CrouchTeaching:
                    break;
                case TeachingName.TipTeaching:
                    break;
                case TeachingName.RunTeaching:
                    break;
                case TeachingName.WeaponFire:
                    storeBtn.OnActive(true);
                    taskBtn.OnActive(true);
                    rewardBtn.OnActive(BattleController.GetCtrl<LimitRewardCtrl>().reward != null);
                    break;
                case TeachingName.AssTeaching:
                    break;
                case TeachingName.WeaponUpgrade:
                    break;
                case TeachingName.WeaponChange:
                    break;
                case TeachingName.StrengthTeaching:
                    break;
                default:
                    break;
            }
        }

        private void OnAllWeaponEmpty()
        {
            if (!creatTip.activeSelf)
            {
                creatTip.OnActive(true);
            }
        }

        private void OnNextNode(NodeBase node)
        {
            mapTip.OnActive(true);
        }
    }
}
