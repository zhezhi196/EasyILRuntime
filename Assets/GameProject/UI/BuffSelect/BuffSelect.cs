using System.Net.Mime;
using System.Reflection.Emit;
using System;
using System.Collections.Generic;
//using Chapter2.UI;
using Module;
using SecondChapter;
using UnityEngine;
using Project.Data;
using ProjectUI;
using UnityEngine.UI;


    public class BuffSelect : UIViewBase
    {
        public Transform content;
        public UIBtnBase back;
        public UIBtnBase resetHP;
        public UIBtnBase resetProgress;
        public Text desText;
        public Text btnText;
        private bool isDream = false;

        private ElectrocutionChairProp _prop;
        protected override void OnChildStart()
        {
            base.OnChildStart();
            //content.localScale = Tools.GetScreenScale() * Vector3.one;
            back.AddListener(OnBack);
            resetHP.AddListener(ClickResetHP);
            resetProgress.AddListener(ClickResetProgress);
            //BattleController.GetCtrl<PlayerCtrl>().player.onBuffChange += OnPlayerBuffChange;
        }

        private void OnDestroy()
        {
            if (BattleController.GetCtrl<PlayerCtrl>() != null && Player.player != null)
            {
               // BattleController.GetCtrl<PlayerCtrl>().player.onBuffChange -= OnPlayerBuffChange;
            }
        }

        private void OnPlayerBuffChange(Buff buff, bool b)
        {
            //buffIcons.Find(x => x.buffID == buff.dbData.ID)?.icon.OnActive(b);
        }

        private void OnBack()
        {
            OnExit();
        }

        public override void Refresh(params object[] args)
        {
            _prop = (ElectrocutionChairProp) args[0];
            /*for (int i = 0; i < PlayerCtrl.Instance.player.buffList.Count; i++)
            {
                OnPlayerBuffChange(PlayerCtrl.Instance.player.buffList[i], true);
            }*/
        }

        public override void OnOpenStart()
        {
            base.OnOpenStart();
            BattleController.Instance.Pause(winName);
            if (!isDream)
            {
                desText.text = Language.GetContent("2804");
                btnText.text = Language.GetContent("2805");
            }
            else
            {
                desText.text = Language.GetContent("2808");
                btnText.text = Language.GetContent("2809");
            }
        }

        public override void OnCloseComplete()
        {
            base.OnCloseComplete();
            BattleController.Instance.Continue(winName);
        }

        private void ClickResetProgress()
        {
            GameDebug.Log("点击返回梦境");
            if (BattleController.currNode.id == 0)
            {
                GameDebug.Log("当前应该是测试模式？因为当前节点是0，理论上当前节点不可能在0！");
                return;
            }

            Transform flashPoint = null;
            if (_prop.belongNodeId == 0) //从梦境返回现实
            {
                flashPoint = BattleController.currNode.nodeParent.playerCreator.transform;
                isDream = false;
            }
            else //回到梦境
            {
                flashPoint = BattleController.GetNode(0).nodeParent.playerCreator.transform;
                isDream = true;
            }

            Player.player.transform.position = flashPoint.transform.position;
            Player.player.transform.rotation = flashPoint.transform.rotation;
            
            UIController.Instance.Back();
            var gameUi = UIController.Instance.Get("GameUI").viewBase as GameUI;
            gameUi.uiCanvas.alpha = 0;
            UICommpont.FreezeUI("electricChair");
            UIController.Instance.canPhysiceback = false;
            //播放动画
            Player.player.PlayWeakUp(() =>
            {
                UIController.Instance.canPhysiceback = true;
                gameUi.PlayFade(() =>
                {
                    UICommpont.UnFreezeUI("electricChair");
                });
            });

        }
        
        private void ClickResetHP()
        {
            GameDebug.Log("点击重获新生");
            //AdsData adaData = DataMgr.Instance.GetSqlService<AdsData>().WhereID(30601);
            var reward =  (RewardBag)Commercialize.GetRewardBag(30601);
            reward.GetReward((res) =>
            {
                if (res.result == IapResultMessage.Success)
                {
                    GameDebug.Log("点击重获新生+++");
                    CommonPopup.Popup(Language.GetContent("701"), Language.GetContent("2810"), null,
                        new PupupOption(() =>
                            {
                                OnBack();
                            },
                            Language.GetContent("702")));
                }
            }, 0);
            
        }
    }