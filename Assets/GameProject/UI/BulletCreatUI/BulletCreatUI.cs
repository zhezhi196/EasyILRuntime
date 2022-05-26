using System;
using System.Reflection.Emit;
using Module;
//using SecondChapter;
using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using Project.Data;

    public class BulletCreatUI: UIViewBase
    {
        public Text ownCount;
        private string ownCountString = "<size=65>{0}</size>/{1}";
        public GameObject elementObj;
        public Text element;
        public GameObject elementGroupObj;
        public Text elementGroup;
        public GameObject alloyObj;
        public Text alloy;
        public GameObject alloyGroupObj;
        public Text alloyGroup;
        public Text mackCount;

        public UIBtnBase back;
        public UIBtnBase make;
        public UIBtnBase groupMake;
        public Toggle[] SelectBullet;
        private int index = 0;
        public BulletEntity currentBullet;
        private int currentBulletMax = 100;
        private int haveBulletCount  = 0;
        public RawImage renderTexure;
        public GameObject makeEffect;
        //子弹ID
        public int[] dataIDs;
        public int[] weaponIDs;
        public List<BulletData> allData = new List<BulletData>();
        public List<WeaponData> allWeaponData = new List<WeaponData>();

        public Text createCountText;
        public Text createOneText;
        public UIScrollBase modelScroll;
        private const int ModelImageSize = 685;

        //public MoneyButton elementMoney;
        //public MoneyButton alloyMoney;
        //private MoneyButton currentOpen;
        protected override void OnChildStart()
        {
            back.AddListener(OnBack);
            make.AddListener(OnMake);
            groupMake.AddListener(GroupMake);
            for (int i = 0; i < SelectBullet.Length; i++)
            {
                int index = i;
                SelectBullet[i].onValueChanged.AddListener((bool b)=>
                {
                    if (b)
                        OnSelectBullet(index);
                });
            }

            for(int i = 0; i < dataIDs.Length; i ++)
            {
                allData.Add(DataMgr.Instance.GetSqlService<BulletData>().WhereID(dataIDs[i]));
            }
            for(int i = 0; i < weaponIDs.Length; i ++)
            {
                allWeaponData.Add(DataMgr.Instance.GetSqlService<WeaponData>().WhereID(weaponIDs[i]));
            }
            modelScroll.AddDrag((v1, v2, time) => UI3DShow.Instance.OnRotateModel(winName,v2));
            renderTexure.texture = RenderTextureTools.commonTexture;
            renderTexure.GetComponent<RectTransform>().sizeDelta = ModelImageSize * Tools.GetScreenScale();
        }

        public override void OnCloseComplete()
        {
            //elementObj.OnActive(false);
            //alloyObj.OnActive(false);
            UI3DShow.Instance.OnClose(winName);
        }

        public override void Refresh(params object[] args)
        {
            //BattleController.Instance.Pause(winName);
            if (model.direction == UIOpenDirection.Backward)
            {
                //UI3DShow.Instance.OnShow(winName,currentBullet);
                OnSelectBullet(index);
                return;
            }
            SelectBullet[index].isOn = false;
            SelectBullet[0].isOn = true;
            //OnSelectBullet(0);
        }


        private void OnSelectBullet(int obj)
        {
            index = obj;
            BulletType type = (BulletType) allData[index].type;

            currentBulletMax = allWeaponData[index].bulletBag.ToInt();
            haveBulletCount = BattleController.GetCtrl<BulletCtrl>().GetBullet(type).bagCount;

            ownCount.text = string.Format(ownCountString, haveBulletCount, currentBulletMax);

            //单次制造消耗
            element.text = allData[index].costCount.ToString();
            //制造一组消耗
            elementGroup.text = (allData[index].costCount.ToInt()*allData[index].createCount).ToString();
            createOneText.text = String.Format( Language.GetContent("1001"),1);
            createCountText.text = String.Format( Language.GetContent("1001"),allData[index].createCount);
            UI3DShow.Instance.OnShow(winName,BattleController.GetCtrl<BulletCtrl>().GetBullet(type));
        }

        private void OnMake()
        {
            // 消耗零件
            Cost cost = new Cost(MoneyInfo.ConvertType(allData[index].costType.ToInt()), allData[index].costCount.ToInt());
            // 背包中子弹数量达到最大
            if ( haveBulletCount >= currentBulletMax)
            {
                CommonPopup.Popup(Language.GetContent("701"), Language.GetContent("1009"),null,
                  new PupupOption(() => CommonPopup.Close(), Language.GetContent("702")));
                  return;
            }

            // 制造后子弹数量超过背包最大数量限制
            else if ( haveBulletCount + 1 > currentBulletMax)
            {
                CommonPopup.Popup(Language.GetContent("701"), Language.GetContent("1008"),null,
                    new PupupOption(null, Language.GetContent("703")),
                    new PupupOption(() =>
                    {
                         // 拥有零件数量大于等于制造消耗数量
                        if (MoneyInfo.Spend(0, cost))
                        {
                            int createBulletCount= BulletEntity.OwnBullet((BulletType) allData[index].type, BulletCreatType.Single,
                                1);
                            var conten = Commercialize.GetRewardContent(
                                BattleController.GetCtrl<BulletCtrl>().GetBullet((BulletType) allData[index].type)
                                    .dbData.ID, createBulletCount);
                            //RewardUI.OpenRewardUI(conten);
                            ShowMackEffect();
                            currentBulletMax = allWeaponData[index].bulletBag.ToInt();
                            haveBulletCount = BattleController.GetCtrl<BulletCtrl>().GetBullet((BulletType) allData[index].type).bagCount;
                            ownCount.text = string.Format(ownCountString, haveBulletCount, currentBulletMax);
                        }
                    }, Language.GetContent("702")));
                return;
            }

            // 拥有零件数量大于等于制造消耗数量
            if (MoneyInfo.Spend(0, cost))
            {
                int createBulletCount= BulletEntity.OwnBullet((BulletType) allData[index].type, BulletCreatType.Single,
                    1);
                var conten = Commercialize.GetRewardContent(
                    BattleController.GetCtrl<BulletCtrl>().GetBullet((BulletType) allData[index].type)
                        .dbData.ID, createBulletCount);
                //RewardUI.OpenRewardUI(conten);
                ShowMackEffect();
                currentBulletMax = allWeaponData[index].bulletBag.ToInt();
                haveBulletCount = BattleController.GetCtrl<BulletCtrl>().GetBullet((BulletType) allData[index].type).bagCount;
                ownCount.text = string.Format(ownCountString, haveBulletCount, currentBulletMax);
            }
            
        }

        private void GroupMake()
        {
            // 消耗零件
            Cost cost = new Cost(MoneyInfo.ConvertType(allData[index].costType.ToInt()), allData[index].costCount.ToInt()*allData[index].createCount);
            // 背包中子弹数量达到最大
            if ( haveBulletCount >= currentBulletMax)
            {
                CommonPopup.Popup(Language.GetContent("701"), Language.GetContent("1009"),null,
                  new PupupOption(() => CommonPopup.Close(), Language.GetContent("702")));
                  return;
            }

            // 制造后子弹数量超过背包最大数量限制
            else if ( haveBulletCount + allData[index].createCount > currentBulletMax)
            {
                CommonPopup.Popup(Language.GetContent("701"), Language.GetContent("1008"),null,
                    new PupupOption(null, Language.GetContent("703")),
                    new PupupOption(() =>
                    {
                         // 拥有零件数量大于等于制造消耗数量
                        if (MoneyInfo.Spend(0, cost))
                        {
                            int createBulletCount= BulletEntity.OwnBullet((BulletType) allData[index].type, BulletCreatType.Group,
                                1);
                            var conten = Commercialize.GetRewardContent(
                                BattleController.GetCtrl<BulletCtrl>().GetBullet((BulletType) allData[index].type)
                                    .dbData.ID, createBulletCount);
                            //RewardUI.OpenRewardUI(conten);
                            ShowMackEffect();
                            currentBulletMax = allWeaponData[index].bulletBag.ToInt();
                            haveBulletCount = BattleController.GetCtrl<BulletCtrl>().GetBullet((BulletType) allData[index].type).bagCount;
                            ownCount.text = string.Format(ownCountString, haveBulletCount, currentBulletMax);
                        }
                    }, Language.GetContent("702")));
                return;
            }
            
            // 拥有零件数量大于等于制造消耗数量
            if (MoneyInfo.Spend(0, cost))
            {
                int createBulletCount= BulletEntity.OwnBullet((BulletType) allData[index].type, BulletCreatType.Group,
                    1);
                var conten = Commercialize.GetRewardContent(
                    BattleController.GetCtrl<BulletCtrl>().GetBullet((BulletType) allData[index].type)
                        .dbData.ID, createBulletCount);
                //RewardUI.OpenRewardUI(conten);
                ShowMackEffect();
                currentBulletMax = allWeaponData[index].bulletBag.ToInt();
                haveBulletCount = BattleController.GetCtrl<BulletCtrl>().GetBullet((BulletType) allData[index].type).bagCount;
                ownCount.text = string.Format(ownCountString, haveBulletCount, currentBulletMax);
            }
        }

        public void ShowMackEffect()
        {
            if (makeEffect.activeSelf)
            {
                makeEffect.OnActive(false);
            }
            makeEffect.OnActive(true);
        }

        public override void OnExit(params object[] args)
        {
            makeEffect.OnActive(false);
            BattleController.Instance.Continue(winName);
            base.OnExit(args);
        }

        private void OnBack()
        {
            OnExit();
        }
    }