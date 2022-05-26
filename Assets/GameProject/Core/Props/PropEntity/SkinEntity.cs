using Module;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Data;

public class SkinEntity : IRewardObject
{
    public enum SkinStation
    {
        UnGet,
        Get,
        Equip,
        NewGet
    }
    public SkinStation collectionStation { get; set; }
    public SkinData dbData { get; set; }
    public WeaponEntity weaponEntity;
    public Material skinMat;

    public int stationCode
    {
        get { return collectionStation == SkinStation.UnGet ? 0 : 1; }
    }

    public void GetIcon(string type, Action<Sprite> callback)
    {
        if (type == TypeList.Normal)
        {
            SpriteLoader.LoadIcon(dbData.icon, callback);
        }
        else if (type == TypeList.Achievement|| type == TypeList.High)
        {
            SpriteLoader.LoadIcon(dbData.AchievementIcon, callback);
        }
    }

    public float GetReward(float rewardCount, RewardFlag flag)
    {
        Acquire();
        return rewardCount;
    }

    public string GetText(string type)
    {
        if (type == TypeList.Title)
        {
            return Language.GetContent(dbData.title.ToString());
        }
        else if (type == TypeList.GetDes)
        {
            return Language.GetContent(dbData.getDes.ToString());
        }

        return string.Empty;
    }

    public void Init(SkinData data)
    {
        this.dbData = data;
        
        var skinservice = DataMgr.Instance.GetSqlService<SkinSaveData>().Where(d => d.skinID == this.dbData.ID);
        if (skinservice == null)
        {
            collectionStation = SkinStation.UnGet;
            if (dbData.lockDes == 0)
            {
                //说明是默认皮肤
                Acquire();
                collectionStation = SkinStation.Equip;
            }
        }
        else
        {
            collectionStation = skinservice.station.ToEnum<SkinStation>();
        }

        //if (data.adsUnlock != 0)
        //{
        //    ads = Commercialize.GetGameIap(data.adsUnlock);
        //}
    }

    /// <summary>
    /// 预加载皮肤
    /// </summary>
    public void PreloadMat()
    {
        MatLoader.LoadMaterial(dbData.matPath, (m) =>
        {
            if (m != null)
            {
                skinMat = m;
            }
            else {
                GameDebug.LogError("不能找到武器皮肤 ID=" + dbData.ID);
            }
        });
    }

    /// <summary>
    /// 装配皮肤
    /// </summary>
    public void Equip()
    {
        collectionStation = SkinStation.Equip;
        var skinservice = DataMgr.Instance.GetSqlService<SkinSaveData>();
        skinservice.Update((d => d.skinID == dbData.ID), "station", collectionStation.ToInt());
    }
    /// <summary>
    /// 卸载皮肤
    /// </summary>
    public void Unload()
    {
        collectionStation = SkinStation.Get;
        var skinservice = DataMgr.Instance.GetSqlService<SkinSaveData>();
        skinservice.Update((d => d.skinID == dbData.ID), "station", collectionStation.ToInt());
    }
    /// <summary>
    /// 获取皮肤
    /// </summary>
    public void Acquire()
    {
        //更新数据库
        if (collectionStation == SkinStation.UnGet)
        {
            collectionStation = SkinStation.NewGet;
            var skinservice = DataMgr.Instance.GetSqlService<SkinSaveData>();
            var skinData = skinservice.Where(d => d.skinID == dbData.ID);
            if (skinData == null)
            {
                SkinSaveData skinsaveData = new SkinSaveData()
                {
                    skinID = dbData.ID,
                    station = collectionStation.ToInt(),
                };
                skinservice.Insert(skinsaveData);
            }
            //WeaponManager.Instance.AddWeaponSkin(this);
            GameDebug.Log("获得武器皮肤:" + dbData.ID);
        }
    }
}
