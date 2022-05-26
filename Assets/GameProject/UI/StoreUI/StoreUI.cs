using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Module;
using Project.Data;
using UnityEngine.UI;

namespace ProjectUI
{
    public class StoreUI : UIViewBase
    {
        public UIBtnBase back;
        public ToggleTree toggleTree;
        public StoreButton[] toggleButton;
        private List<ShopItem> lastItem = new List<ShopItem>();
        public ScrollRect scrollRect;
        public int currindex;
        public GameObject gaojiBan;
        public UIBtnBase gaojiButton;
        public Text[] rewardDes;
        public Text gaojibanPrice;
        public String[] gaojiDesId;

        protected override void OnChildStart()
        {
            base.OnChildStart();
            back.AddListener(OnBack);

            for (int i = 0; i < toggleButton.Length; i++)
            {
                int index = i;
                toggleButton[i].AddListener(OnToggleButton, index);
            }
            scrollRect.content.transform.localScale=Tools.GetScreenScale();
            gaojiButton.AddListener(OnGaoji);
            gaojiIcon.localScale = Tools.GetScreenScale();
        }

        public Transform gaojiIcon;

        private void OnGaoji()
        {
            var rewardDb = DataMgr.Instance.GetSqlService<IapData>().Where(db => db.type == 5);
            var rewardIap = (RewardBag) Commercialize.GetRewardBag(rewardDb);
            rewardIap.GetReward(res =>
            {
                if (res.result == IapResultMessage.Success)
                {
                    UISequence seq = new UISequence();
                    for (int i = 0; i < rewardIap.content.Length; i++)
                    {
                        if (rewardIap.content[i].stationCode == 0)
                        {
                            seq.Add("RewardUI", UITweenType.None, OpenFlag.Inorder,rewardIap.content[i]);
                        }
                    }

                    if (seq.count > 0)
                    {
                        seq.Popup();
                    }
                }
            });
            

            toggleTree.NotifyToggleOn(0,true);
        }

        public override void Refresh(params object[] args)
        {
            toggleTree.NotifyToggleOn(currindex, true);
            var rewardDb = DataMgr.Instance.GetSqlService<IapData>().Where(db => db.type == 5);
            var rewardIap = Commercialize.GetRewardBag(rewardDb) as RewardBag;
            toggleButton[4].gameObject.OnActive(rewardIap.stationCode == 0);
        }


        private void OnToggleButton(ToggleTreeStation from, ToggleTreeStation to, int index)
        {
            if (to == ToggleTreeStation.Off)
            {
                toggleButton[index].Select(false);
            }
            else if (to == ToggleTreeStation.Unactive)
            {
                toggleButton[index].gameObject.OnActive(false);
            }
            else if (to == ToggleTreeStation.On)
            {
                if (index < 4)
                {
                    scrollRect.gameObject.OnActive(true);
                    gaojiBan.gameObject.OnActive(false);
                    currindex = index;
                    toggleButton[index].Select(true);
                    scrollRect.horizontalNormalizedPosition = 0;
                    for (int i = 0; i < lastItem.Count; i++)
                    {
                        ObjectPool.ReturnToPool(lastItem[i]);
                    }
                    lastItem.Clear();

                    List<IapData> allData = DataMgr.Instance.GetSqlService<IapData>().WhereList(fd => fd.type == index + 1);
                    List<AdsData> adsAllData = DataMgr.Instance.GetSqlService<AdsData>().WhereList(fd => (fd.adsArg1.ToInt() == 1 || fd.adsArg1.ToInt() == 3) && fd.adsArg2.ToInt() == index + 1);
                    //王浩20211229说 内购表和广告表现排广告再排内购,同类型的按照表中的level字段排序
                    Voter voter = new Voter(allData.Count + adsAllData.Count, () =>
                    {
                        scrollRect.content.Sort((a, b) =>
                        {
                            int aType = DataMgr.Instance.GetDbType(a.reward.iap.dbData.ID);
                            int bType = DataMgr.Instance.GetDbType(b.reward.iap.dbData.ID);

                            var idCompare = aType.CompareTo(bType);
                            if (idCompare != 0)
                            {
                                return -idCompare;
                            }
                            else
                            {
                                return a.reward.index.CompareTo(b.reward.index);
                            }
                        }, lastItem);
                    });

                    for (int i = 0; i < allData.Count; i++)
                    {
                        RewardBag rewardBag = (RewardBag) Commercialize.GetRewardBag(allData[i]);
                        SetItem(rewardBag, voter.Add);
                    }

                    for (int i = 0; i < adsAllData.Count; i++)
                    {
                        RewardBag rewardBag = (RewardBag) Commercialize.GetRewardBag(adsAllData[i]);
                        SetItem(rewardBag, voter.Add);
                    }
                }
                else
                {
                    scrollRect.gameObject.OnActive(false);
                    gaojiBan.gameObject.OnActive(true);
                    toggleButton[index].Select(true);
                    var rewardDb = DataMgr.Instance.GetSqlService<IapData>().Where(db => db.type == 5);
                    var rewardIap = Commercialize.GetRewardBag(rewardDb) as RewardBag;
                    toggleButton[index].gameObject.OnActive(rewardIap.stationCode == 0);
                    for (int i = 0; i < rewardIap.content.Length; i++)
                    {
                        rewardDes[i].gameObject.transform.parent.gameObject.OnActive(true);
                        rewardDes[i].text = String.Format(Language.GetContent(gaojiDesId[i]));
                    }

                    gaojibanPrice.text = (rewardIap.iap as Currency).price;
                }

                
            }
        }


        protected override void Update()
        {
            base.Update();
            DateTime nextDay = DateTime.Today.AddDays(1);
            TimeSpan span = nextDay - TimeHelper.now;
            //limitTime.text = string.Format("{0:00}小时{1:00}分钟{2:00}秒后刷新", span.Hours, span.Minutes, span.Seconds);
        }

        private void SetItem(RewardBag resultBag,Action callback)
        {
            if (resultBag.stationCode == 0)
            {
                LoadPrefab<ShopItem>("ShopItem", scrollRect.content, item =>
                {
                    item.SetItem(resultBag);
                    lastItem.Add(item);
                    callback?.Invoke();
                });
               /* if (resultBag.iap.dbData.ID == 30020)
                {
                    LoadPrefab<DailyShopItem>("DailyShopItem", scrollRect.content, item =>
                    {
                        item.SetItem(resultBag);
                        lastItem.Add(item);
                        callback?.Invoke();
                    });
                }
                else if (resultBag.iap.dbData.ID == 30021 || resultBag.iap.dbData.ID == 30022)
                {
                    LoadPrefab<AddCountBagOverSee>("AddCountBagOverSee", scrollRect.content, item =>
                    {
                        item.SetItem(resultBag);
                        lastItem.Add(item);
                        callback?.Invoke();
                    });
                }
                else
                {
                    LoadPrefab<ShopItem>("ShopItem", scrollRect.content, item =>
                    {
                        item.SetItem(resultBag);
                        lastItem.Add(item);
                        callback?.Invoke();
                    });
                }*/
            }
            else
            {
                callback?.Invoke();
            }
        }

        private void OnBack()
        {
            OnExit();
        }
    }
}