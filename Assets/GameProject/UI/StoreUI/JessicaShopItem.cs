using System;
using System.Collections.Generic;
using Module;
using Project.Data;
using UnityEngine;
using UnityEngine.UI;

public class JessicaShopItem : UIBtnBase, IPoolObject
{
    public ObjectPool pool { get; set; }
    public Image icon;
    public Text title;
    public Text off;
    public Image costIcon;
    public Text costCount;
    public IRewardObject reward;
    public GameObject adsButton;
    public GameObject BuyButton;
    public int index;
    public Image rewardIcon;
    public Text rewardCount;
    public Text manxueDes;

    public GameReward ga;
    public JessicaStore storeUI;
    public void ReturnToPool()
    {
        ObjectPool.ReturnToPool(this);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        storeUI = transform.GetComponentInParent<JessicaStore>();
    }

    public void OnGetObjectFromPool()
    {
    }

    protected override void DefaultListener()
    {
        if (this.reward is GameReward gameReward)
        {
            var coun = gameReward.GetReward(1, 0);
            if (coun > 0)
            {
                RewardUI.OpenRewardUI(gameReward);
                if (storeUI.type == 1)
                {
                    AnalyticsEvent.SendEvent(AnalyticsType.BuyHideJessica, gameReward.dbData.ID.ToString(),false);
                }
                else
                {
                    AnalyticsEvent.SendEvent(AnalyticsType.BuyJessica, gameReward.dbData.ID.ToString(),false);
                }
            }
        }
        else if (this.reward is IapBag iapBag)
        {
            if (ga.dbData.ID == DataMgr.CommonData(33013).ToInt()|| ga.dbData.ID== DataMgr.CommonData(33014).ToInt())
            {
                if (Player.player.hp < Player.player.MaxHp)
                {
                    iapBag.GetReward(res =>
                    {
                        if (res.result == IapResultMessage.Success)
                        {
                            Player.player.ChangeHp(float.MaxValue);
                            CommonPopup.Popup(Language.GetContent("701"), Language.GetContent("1910"), null);
                            if (Player.player.hp >= Player.player.MaxHp)
                            {
                                interactable = false;
                            }
                        }
                    });
                }
            }
            else
            {
                if (iapBag.stationCode == 2)
                {
                    iapBag.content[0].reward.GetReward(0, 0);
                }
                else
                {
                    iapBag.GetReward(res =>
                    {
                        if (res.result == IapResultMessage.Success)
                        {
                            RewardUI.OpenRewardUI(reward);
                            string s = LocalFileMgr.GetString(storeUI.localWriteKey);
                            string[] st = s.Split(ConstKey.Spite0);
                            List<string> sdd = new List<string>(st);
                            sdd.Remove(ga.dbData.ID.ToString());
                            LocalFileMgr.SetString(storeUI.localWriteKey,string.Join(ConstKey.Spite0.ToString(), sdd.ToArray()));
                        }
                    });
                }
            }
        }
    }

    public void SetItem(int index, GameReward reward, Action callback)
    {
        ga = reward;
        this.index = index;
        IRewardBag bag = reward;
        if (((JessicaData) reward.dbData).adsId != 0)
        {
            bag = Commercialize.GetRewardBag(((JessicaData) reward.dbData).adsId);
        }
        title.text = bag.GetText(TypeList.Title);
        if (reward.dbData.ID == DataMgr.CommonData(33013).ToInt()|| reward.dbData.ID== DataMgr.CommonData(33014).ToInt())
        {
            interactable = Player.player.hp < Player.player.MaxHp;
            reward.GetIcon(TypeList.Normal, sp =>
            {
                icon.sprite = sp;
                callback?.Invoke();
            });

            rewardIcon.gameObject.OnActive(false);
            manxueDes.gameObject.OnActive(true);
            manxueDes.text = Language.GetContent(reward.dbData.des);
            
            if (bag is GameReward gameReward)
            {
                adsButton.OnActive(false);
                BuyButton.OnActive(true);
                costCount.text = gameReward.cost[0].cost.ToString();
            }
            else
            {
                adsButton.OnActive(true);
                BuyButton.OnActive(false);
            }
        }
        else
        {
            interactable=true;
            //20220111 王浩:杰西卡商店的icon用杰西卡数据表中的ICON字段,奖励都用物品表中的minIcon字段
            Voter voter = new Voter(2, callback);
            reward.GetIcon(TypeList.Normal, sp =>
            {
                icon.sprite = sp;
                voter.Add();
            });
            rewardIcon.gameObject.OnActive(true);
            manxueDes.gameObject.OnActive(false);
            bag.GetIcon(TypeList.Cost, sp =>
            {
                costIcon.sprite = sp;
            });

            if (bag.content[0].reward is WeaponEntity|| bag.content[0].reward is SkinEntity)
            {
                rewardIcon.gameObject.OnActive(false);
                voter.Add();
            }
            else
            {
                rewardIcon.gameObject.OnActive(true);
                reward.GetIcon(TypeList.MiniIcon, sp =>
                {
                    rewardIcon.sprite = sp;
                    voter.Add();
                });
            }

            if (reward.dbData.rewardCount.IsNullOrEmpty())
            {
                rewardCount.text = bag.GetText(TypeList.rewardCount);
            }
            else
            {
                rewardCount.text = reward.GetText(TypeList.rewardCount);
            }
        
            if (bag is GameReward gameReward)
            {
                adsButton.OnActive(false);
                BuyButton.OnActive(true);
                costCount.text = gameReward.cost[0].cost.ToString();
            }
            else
            {
                adsButton.OnActive(true);
                BuyButton.OnActive(false);
            }
        }
        if (Math.Abs(reward.dbData.off) > 0.0001)
        {
            off.transform.parent.gameObject.OnActive(true);
            off.text = reward.dbData.off.ToString("P0") + "\n"+Language.GetContent("1007");
        }
        else
        {
            off.transform.parent.gameObject.OnActive(false);
        }
        this.reward = bag;
    }
}