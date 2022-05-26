using System;
using Module;
using Project.Data;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 记录武器数据
/// 武器可能没有皮肤,没有皮肤时不可指定装配皮肤,切换皮肤
/// </summary>
public class WeaponEntity : PropEntity
{
    public WeaponData weaponData;
    public WeaponSaveData saveData;
    public List<SkinEntity> weaponSkins = new List<SkinEntity>();
    public SkinEntity equipSkin;
    public int level;

    public override bool canAddMap
    {
        get { return !isGet; }
    }

    public override bool showProgress
    {
        get { return !isGet; }
    }

    public override int stationCode
    {
        get
        {
            if (weaponData.ID == 23006) return 0;
            return isGet ? 1 : 0;
        }
    }

    public WeaponEntity(PropData dbData) : base(dbData)
    {
        weaponData = DataMgr.Instance.GetSqlService<WeaponData>().Where(s => s.propId == dbData.ID);
    }

    public void Init()
    {
        //皮肤对象初始化,对比皮肤的weaponid是否和武器的id相同
        foreach (var item in WeaponManager.allSkins)
        {
            if (item.Value.dbData.weaponID == weaponData.ID)
            {
                SkinEntity e = item.Value;
                e.PreloadMat();
                weaponSkins.Add(e);
                e.weaponEntity = this;
            }
        }
        //指定默认皮肤
        if (weaponSkins.Count > 0)
        {
            equipSkin = weaponSkins[0];
        }
    }

    public override void GetIcon(string type, Action<Sprite> callback)
    {
        if (type == TypeList.Normal || type == TypeList.Achievement)
        {
            SpriteLoader.LoadIcon(weaponData.icon, callback);
        }
        else if (type == TypeList.Collection )
        {
            SpriteLoader.LoadIcon(weaponData.WarehouseIcon, callback);
        }
        else if (type == TypeList.High)
        {
            SpriteLoader.LoadIcon(dbData.highIcon, callback);
        }
        else if (type == TypeList.MiniIcon)
        {
            SpriteLoader.LoadIcon(dbData.miniIcon, callback);
        }
        else
        {
            base.GetIcon(type, callback);
        }
    }

    public override float GetReward(float rewardCount, int editorid,string[] match, RewardFlag flag)
    {
        var temp = base.GetReward(rewardCount,editorid, match, flag);
        if (temp > 0.9f)
        {
            WeaponManager.AddWeapon(weaponData.ID);
        }

        return temp;
    }

    /// <summary>
    /// 武器表现初始化,装配,数据等
    /// </summary>
    /// <param name="data">武器保存的数据</param>
    public void SaveInit(WeaponSaveData data)
    {
        //武器状态.等级初始化
        saveData = data;
        level = data.level;
        //武器皮肤的初始化,武器皮肤的初始化,设置装配的皮肤
        for (int i = 0; i < weaponSkins.Count; i++)
        {
            if (weaponSkins[i].collectionStation ==SkinEntity.SkinStation.Equip)
            {
                equipSkin = weaponSkins[i];
                break;
            }
        }
    }

    public void ChangeSkin(SkinEntity skin)
    {
        if (equipSkin == skin)
            return;
        equipSkin?.Unload();
        equipSkin = skin;
        equipSkin.Equip();
        EventCenter.Dispatch<WeaponEntity>(EventKey.ChangeWeaponSkin, this);
    }
}