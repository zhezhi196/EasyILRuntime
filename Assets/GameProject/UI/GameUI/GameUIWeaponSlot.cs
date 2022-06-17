using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Module;

public class GameUIWeaponSlot : MonoBehaviour
{
    public Toggle toggle;
    public Image weaponIcon;
    public GameObject bulletRoot;
    public Text weaponName;
    public Text bulletCount;
    public Text bulletBagCount;
    public Action<bool> onSelectWeapon;
    [Sirenix.OdinInspector.ReadOnly]
    public Weapon weapon;
    private Sprite norSprite;
    void Start()
    {
        Image tImage = this.GetComponent<Image>();
        tImage.alphaHitTestMinimumThreshold = 0.1f;
        norSprite = weaponIcon.sprite;
    }

    public void Refresh(GameWeaponManager.WeaponSlot slot,Action callBack)
    {
        if (slot.weapon == null ||(slot.weapon.weaponType == WeaponType.Thrown&&slot.weapon.bulletCount ==0))//燃烧瓶为0表现为未获取
        {
            toggle.interactable = false;
            weaponName.gameObject.OnActive(false);
            bulletRoot.OnActive(false);
            callBack?.Invoke();
            //if(norSprite!=null)
            //{
            //    weaponIcon.sprite = norSprite;
            //}
            weaponIcon.SetAlpha(0.1f);
        }
        else {
            toggle.interactable = true;
            weapon = slot.weapon;
            if (Player.player.currentWeapon == slot.weapon)
            {
                toggle.isOn = true;
            }
            SpriteLoader.LoadIcon(slot.weapon.entity.weaponData.icon, (s) =>
            {
                weaponIcon.sprite = s;
                callBack?.Invoke();
            });
            weaponIcon.SetAlpha(1f);
            weaponName.text = Language.GetContent(slot.weapon.weaponName);
            weaponName.gameObject.OnActive(true);
            bulletCount.text = slot.weapon.bulletCount.ToString();
            bulletBagCount.text = slot.weapon.bullet==null?"1":slot.weapon.bullet.bagCount.ToString();
            bulletRoot.OnActive(weapon.weaponType != WeaponType.MeleeWeapon);//需求修改：近战武器不显示子弹
        }
    }

    void OnValueChange(bool b)
    {
        onSelectWeapon?.Invoke(b);
    }

    private void OnDisable()
    {
        toggle.isOn = false;
    }
}
