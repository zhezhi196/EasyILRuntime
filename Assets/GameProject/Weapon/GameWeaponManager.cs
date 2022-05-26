using Module;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
public class GameWeaponManager : MonoBehaviour
{
    [System.Serializable]
    public class WeaponSlot
    {
        public WeaponType type;
        public Weapon weapon;
    }
    [Header("无武器控制器")]
    public Weapon defualtWeapon;
    public List<Weapon> weaponList = new List<Weapon>();
    [ReadOnly]
    public List<Weapon> ownWeapon = new List<Weapon>();
    [BoxGroup("武器槽配置")]
    public List<WeaponSlot> weaponSolts = new List<WeaponSlot>();
    public Action<int> OnGetNewWeapon;

    public Weapon FindWeapon(WeaponType type)
    {
        return weaponList.Find(w => w.weaponType == type);
    }

    public Weapon FindWeapon(int id)
    {
        return ownWeapon.Find(w => w.weaponID == id);
    }

    public Weapon OnGetWeapon(int id)
    {
        Weapon w = weaponList.Find(wp => wp.weaponID == id);
        if (w != null)
        {
            if (ownWeapon.Contains(w))//已经获取的武器，增加子弹
            {
                if (w.weaponType == WeaponType.Thrown)//投掷物拾取
                {
                    if (!w.entity.isGet)
                    {
                        w.entity.collectionStation = CollectionStation.NewGet;
                    }
                    if (w.bulletCount == 0)//当前没有持有投掷物，放入武器位
                    {
                        w.bulletCount = 1;
                        return AddToSolt(w) ? w : null;
                    }
                    else {//todo--当前持有投掷物，扔掉手里的
                        //w.bullet.OwnBullet(BulletCreatType.Single, 1);
                    }
                }
                else if (w.weaponType != WeaponType.MeleeWeapon)//非近战武器,非投掷武器
                {
                    w.bullet.OwnBullet(BulletCreatType.Single, w.weaponArgs.getBulletCount);
                }
                return null;
            }
            //首次获取武器,添加到武器位,添加到已获取武器表,添加子弹
            ownWeapon.Add(w);
            if (w.weaponType == WeaponType.MeleeWeapon)//近战武器
            {
                //if (BattleController.Instance.ctrlProcedure.mode == GameMode.Main)
                //{
                //    //Player.player.PlayStoryTimeline("ShouTao", null);
                //}
                w.bulletCount =1;
                return AddToSolt(w) ? w : null; ;
            }
            if (w.weaponType == WeaponType.Thrown)//投掷武器
            {
                if (!w.entity.isGet)
                    w.entity.collectionStation = CollectionStation.NewGet;
                w.bulletCount = 1;
                return AddToSolt(w) ? w : null;
            }
            //首次获得弹夹带子弹
            if (w.weaponArgs.firstBulletCount > w.maxBulletCount)
            {
                w.bulletCount = w.maxBulletCount;
                w.bullet.OwnBullet(BulletCreatType.Single, w.weaponArgs.firstBulletCount - w.maxBulletCount);
            }
            else
            {
                w.bulletCount = w.weaponArgs.firstBulletCount;
            }
            OnGetNewWeapon?.Invoke(id);
            return AddToSolt(w)?w:null;
        }
        return null;
    }
    //装备武器到武器槽
    public bool AddToSolt(Weapon w)
    {
        if (w.weaponType == WeaponType.Thrown)
        {
            if (w.bulletCount == 0)
            {
                return false;
            }
        }

        for (int i = 0; i < weaponSolts.Count; i++)
        {
            if (weaponSolts[i].type == w.weaponType)
            {
                weaponSolts[i].weapon = w;
                return true;
            }
        }
        return false;
    }

    public Weapon RemoveSoltWeapon(Weapon w)
    {
        for (int i = 0; i < weaponSolts.Count; i++)
        {
            if (weaponSolts[i].weapon == w)
            {
                weaponSolts[i].weapon = null;
            }
        }
        for (int i = 0; i < weaponSolts.Count; i++)
        {
            if (weaponSolts[i].weapon != null)
            {
                return weaponSolts[i].weapon;
            }
        }
        return null;
    }
    public bool AllWeaponEmpty()
    {
        if (ownWeapon.Count <= 0)
            return false;
        for (int i = 0; i < ownWeapon.Count; i++)
        {
            if (ownWeapon[i].bulletCount > 0 || ownWeapon[i].bullet?.bagCount > 0)
            {
                return false;
            }
        }
        EventCenter.Dispatch(EventKey.AllWeaponEmpty);
        return true;
    }
}
