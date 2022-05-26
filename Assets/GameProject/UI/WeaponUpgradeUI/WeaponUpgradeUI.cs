using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Module;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using System.Reflection;

/// <summary>
/// 武器升级UI
/// </summary>
public class WeaponUpgradeUI : UIViewBase
{
    [System.Serializable]
    public class ToggleConfig
    {
        public Toggle toggle;
        public WeaponType weaponType;
        [ReadOnly]
        public Weapon weapon;
        public Transform freeze;
    }
    public UIBtnBase backBtn;
    public Text weaponName;
    public Text weaponLevel;
    public Transform leftGroup;
    [BoxGroup("武器数据条")]
    public List<UpgradeSlider> upgradeSliders = new List<UpgradeSlider>();
    public RawImage previewImage;
    public UIScrollBase modelScroll;
    public Transform partGroup;
    public Text nextLevel;
    public Text upgradeCost;
    public UIBtnBase upgradeBtn;
    public GameObject upEffect;
    public float effectTime;
    private int effectCount = 0;
    [BoxGroup("下一级增加属性")]
    public List<PartDataItem> partDataItems = new List<PartDataItem>();
    [BoxGroup("武器选择配置")]
    public List<ToggleConfig> toggleConfigs = new List<ToggleConfig>();

    private int currentIndex = -1;
    private WeaponPart nextPart;
    protected override void OnChildStart()
    {
        backBtn.AddListener(BackBtn);
        upgradeBtn.AddListener(UpgradeWeapon);
        leftGroup.localScale = Tools.GetScreenScale()*0.9f;
        previewImage.transform.localScale = Tools.GetScreenScale();
        previewImage.texture = RenderTextureTools.commonTexture;
        modelScroll.AddDrag((v1, v2, time) => UI3DShow.Instance.OnRotateModel(winName, v2));
        //partGroup.localScale = Tools.GetScreenScale();
        for (int i = 0; i < toggleConfigs.Count; i++)
        {
            int ii = i;
            toggleConfigs[ii].toggle.onValueChanged.AddListener((b) =>
            {
                toggleConfigs[ii].toggle.graphic.gameObject.OnActive(b);
                if (b)
                {
                    SelectWeapon(ii);
                }
            });
        }
    }

    public override void Refresh(params object[] args)
    {
        if (model.direction != UIOpenDirection.Backward) currentIndex = -1 ;
        if (currentIndex == -1)
        {
            List<Weapon> upgreadWeapons = Player.player.weaponManager.ownWeapon;
            for (int i = 0; i < toggleConfigs.Count; i++)
            {
                if (toggleConfigs[i].weapon == null)
                {
                    Weapon w = upgreadWeapons.Find(x => x.weaponType == toggleConfigs[i].weaponType);
                    toggleConfigs[i].toggle.interactable = w != null;
                    toggleConfigs[i].freeze.gameObject.OnActive(w == null);
                    toggleConfigs[i].weapon = w;
                }
            }
            for (int i = 0; i < toggleConfigs.Count; i++)
            {
                if (toggleConfigs[i].toggle.interactable)
                {
                    if (toggleConfigs[i].toggle.isOn)
                    {
                        SelectWeapon(i);
                    }
                    else {
                        toggleConfigs[i].toggle.isOn = true;
                    }
                   
                    break;
                }
            }
        }
        else {
            SelectWeapon(currentIndex);
        }
    }

    public override void OnCloseComplete()
    {
        UI3DShow.Instance.OnClose(winName);
        //currentIndex = -1;
    }

    private void SelectWeapon(int index)
    {
        currentIndex = index;
        nextPart = null;
        weaponName.text = Language.GetContent(toggleConfigs[index].weapon.weaponName);
        weaponLevel.text =string.Format(Language.GetContent("210"), toggleConfigs[index].weapon.level);
        UI3DShow.Instance.OnShow(winName, toggleConfigs[index].weapon.entity);
        //刷新武器当前数据展示
        for (int i = 0; i < upgradeSliders.Count; i++)
        {
            if (i < toggleConfigs[index].weapon.weaponUpFileds.Count)
            {
                upgradeSliders[i].Refesh(toggleConfigs[index].weapon, toggleConfigs[index].weapon.weaponUpFileds[i]);
                upgradeSliders[i].gameObject.OnActive(true);
            }
            else{
                upgradeSliders[i].gameObject.OnActive(false);
            }
        }
        //不刷新下一等级成长属性,只显示升级按钮
        if (toggleConfigs[index].weapon.level < WeaponManager.weaponAllPartDataDic[toggleConfigs[index].weapon.weaponID].Count)
        {
            nextPart = WeaponManager.weaponAllPartDataDic[toggleConfigs[index].weapon.weaponID][toggleConfigs[index].weapon.level];
            PropertyInfo[] nextProperty = nextPart.attribute.GetType().GetProperties();
            int showCount = 0;
            for (int i = 0; i < toggleConfigs[index].weapon.weaponUpFileds.Count; i++)
            {
                float updata = nextProperty[toggleConfigs[index].weapon.weaponUpFileds[i].index].GetValue(nextPart.attribute).ToFloat();
                if (updata != 0)
                {
                    partDataItems[showCount].Refesh(WeaponManager.GetAttName(toggleConfigs[index].weapon, toggleConfigs[index].weapon.weaponUpFileds[i].filed), updata);
                    partDataItems[showCount].gameObject.OnActive(true);
                    showCount += 1;
                }
            }
            for (int i = showCount; i < partDataItems.Count; i++)
            {
                partDataItems[i].gameObject.OnActive(false);
            }
            nextLevel.text ="Level "+nextPart.data.level.ToString() ;
            upgradeCost.text = nextPart.data.cost.ToString();
            partGroup.gameObject.OnActive(true);
        }
        else {
            partGroup.gameObject.OnActive(false);
        }
    }

    private void UpgradeWeapon()
    {
        Cost cost = new Cost(MoneyInfo.ConvertType(nextPart.data.costType), nextPart.data.cost);
        if (MoneyInfo.Spend(0, cost))
        {
            AnalyticsEvent.SendEvent(AnalyticsType.WeaponUpdate, toggleConfigs[currentIndex].weapon.weaponID.ToString(),false);
            WeaponManager.UpgradeWeapon(toggleConfigs[currentIndex].weapon.weaponID);
            for (int i = 0; i < upgradeSliders.Count; i++)
            {
                upgradeSliders[i].Upgrad();
            }
            //AudioPlay.PlayOneShot("shengji_cg");
            //AudioPlay.PlayOneShot("Gift").SetIgnorePause(true);
            Upgrad();
            SelectWeapon(currentIndex);
        }
    }

    private void BackBtn()
    {
        UIController.Instance.Back();
    }
    private void OnDisable()
    {
        upEffect.OnActive(false);
        effectCount = 0;
        Async.StopAsync(gameObject);
    }

    public void Upgrad()
    {
        if (upEffect.activeInHierarchy)
        {
            effectCount += 1;
        }
        else
        {
            upEffect.OnActive(true);
            EffectClock();
        }
    }

    private async void EffectClock()
    {
        await Async.WaitforSecondsRealTime(effectTime, gameObject);
        upEffect.OnActive(false);
        if (effectCount > 0)
        {
            effectCount -= 1;
            upEffect.OnActive(true);
            EffectClock();
        }
    }
}
