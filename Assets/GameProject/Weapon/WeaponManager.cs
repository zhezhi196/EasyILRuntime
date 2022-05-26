using Module;
using Project.Data;
using System.Collections;
using System.Collections.Generic;

public class WeaponManager
{
    public static Dictionary<int,List<WeaponPart>> weaponAllPartDataDic = new Dictionary<int, List<WeaponPart>>();//各武器升级数据
    //public Dictionary<int, WeaponPart> weaponPartDic = new Dictionary<int, WeaponPart>();//武器部件
    public static Dictionary<int, WeaponEntity> weaponAllEntitys = new Dictionary<int, WeaponEntity>();//所有武器对象

    public static Dictionary<int, SkinEntity> allSkins = new Dictionary<int, SkinEntity>();//所有皮肤对象

    public static AttributeLanguage attributeLanguage;//武器属性多语言配置
    /// <summary>
    /// 武器数据初始化
    /// 初始化所有武器皮肤数据
    /// 初始化所有武器
    /// 初始化所有武器所有等级数据
    /// 武器存档初始化
    /// </summary>
    /// <param name="process"></param>
    /// <returns></returns>
    public static AsyncLoadProcess Init(AsyncLoadProcess process)
    {
        var weaponAllData = DataMgr.Instance.GetSqlService<WeaponData>().tableList;
        var weaponSaveData = DataMgr.Instance.GetSqlService<WeaponSaveData>().tableList;
        //var propservice = DataInit.Instance.GetSqlService<PropData>();
        var allSkinData = DataMgr.Instance.GetSqlService<SkinData>().tableList;

        //所有武器皮肤
        for (int i = 0; i < allSkinData.Count; i++)
        {
            var skin = new SkinEntity();
            skin.Init(allSkinData[i]);
            allSkins.Add(allSkinData[i].ID, skin);
        }
        //所有武器
        for (int i = 0; i < weaponAllData.Count; i++)
        {
            var weaponEntity =(WeaponEntity) WeaponEntity.GetEntity(weaponAllData[i].propId);
            weaponEntity.Init();
            weaponAllEntitys.Add(weaponAllData[i].ID, weaponEntity);
            weaponAllPartDataDic.Add(weaponAllData[i].ID,new List<WeaponPart>());
            //每个武器各部位升级部件数据
            List<WeaponPartData> weaponPartDatas = DataMgr.Instance.GetSqlService<WeaponPartData>().WhereList(data => data.weaponID == weaponAllData[i].ID);
            weaponPartDatas.Sort((a, b) => a.level.CompareTo(b.level));
            for (int j = 0; j < weaponPartDatas.Count; j++)
            {
                weaponAllPartDataDic[weaponAllData[i].ID].Add(new WeaponPart(weaponPartDatas[j]));
            }
        }
        //已拥有武器初始化
        if (weaponSaveData != null)
        {
            WeaponEntity weaponEntity;
            for (int j = 0; j < weaponSaveData.Count; j++)
            {
                weaponEntity = weaponAllEntitys[weaponSaveData[j].weaponID];
                weaponEntity.SaveInit(weaponSaveData[j]);
            }
        }
        AssetLoad.PreloadAsset<AttributeLanguage>("Config/AttributeLanguage.asset", (obj) =>
        {
            attributeLanguage = obj.Result;
        });
        return process;
    }

    /// <summary>
    /// 获得武器
    /// 竞速模式不更新数据库
    /// 游戏获得,普通模式获得,只有新获取武器更新数据库
    /// 武器可能没有可切换皮肤,装配的皮肤记录为0
    /// </summary>
    /// <param name="id">武器id</param>
    public static void AddWeapon(int id)
    {
        EventCenter.Dispatch<int>(EventKey.OnGetWeapon, id);
        if (BattleController.Instance.ctrlProcedure != null && BattleController.Instance.ctrlProcedure.mission.gameMode != GameMode.Main)//竞速模式保存武器数据
        {
            return;
        }
        if (weaponAllEntitys[id].collectionStation == CollectionStation.NewGet)//新获得武器记录到数据库
        {
            var service = DataMgr.Instance.GetSqlService<WeaponSaveData>();
            var weaponData = service.Where(data => data.weaponID == id);
            if (weaponData == null)
            {
                WeaponSaveData saveData = new WeaponSaveData()
                {
                    weaponID = weaponAllEntitys[id].weaponData.ID,
                    level = 0,
                    equipSkin = weaponAllEntitys[id].weaponSkins.Count > 0 ? weaponAllEntitys[id].weaponSkins[0].dbData.ID : 0
                };
                service.Insert(saveData);
                if (saveData.equipSkin != 0)//获取下初始皮肤记录到数据库
                {
                    weaponAllEntitys[id].weaponSkins[0].Acquire();
                    weaponAllEntitys[id].weaponSkins[0].Equip();
                }
            }
            GameDebug.Log("获得武器,更新数据库");
        }
    }

    /// <summary>
    /// 武器升级
    /// 竞速模式不保存到数据库
    /// 普通模式找到武器的存档数据更新
    /// </summary>
    /// <param name="id">武器id</param>
    /// <param name="up">提升等级</param>
    public static void UpgradeWeapon(int id,int up =1)
    {
        if (BattleController.Instance.ctrlProcedure != null && BattleController.Instance.ctrlProcedure.mission.gameMode != GameMode.Main)//竞速模式保存武器数据
        {
            return;
        }
        weaponAllEntitys[id].level += up;
        EventCenter.Dispatch<int, int>(EventKey.OnWeaponUpgrade, id, up);
        //写入数据库
        var service = DataMgr.Instance.GetSqlService<WeaponSaveData>();
        var weaponData = service.Where(data => data.weaponID == id);
        if (weaponData == null)
        {
            WeaponSaveData saveData = new WeaponSaveData()
            {
                weaponID = id,
                level = weaponAllEntitys[id].level
            };
            service.Insert(saveData);
        }
        else
        {
            service.Update((data => data.weaponID == id), "level", weaponAllEntitys[id].level);
        }
        GameDebug.Log("武器升级,更新数据库");
    }

    public static string GetAttName(Weapon w, string name)
    {
        if (attributeLanguage != null)
        {
            return Language.GetContent(attributeLanguage.GetAttKey(name, w));
        }
        return "";
    }
}
