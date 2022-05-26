using System;
using System.Collections;
using System.Collections.Generic;
using Module;
using UnityEngine;

public class GMAntiCheat : MonoBehaviour
{
    //参数还原用
    private Dictionary<string, float> _cheatResetDic = new Dictionary<string, float>();
    
    bool openTools = false;
    float sizeK = 1f;
    Vector2 toolWidSize = new Vector2(1200, 500);
    
    private float btnWidth = 150;
    private float btnHeight = 50f;
    private float vSpacing = 0f;
    /// <summary>
    /// 获取一个按钮的rect
    /// </summary>
    /// <param name="v">行</param>
    /// <param name="h">列</param>
    /// <returns></returns>
    private Rect GetBtnRect(int v, int h)
    {
        return new Rect(btnWidth * h * sizeK, 30f + (btnHeight + vSpacing) * v * sizeK, btnWidth * sizeK, btnHeight * sizeK);
    }


    private void RecordCheat(string key , float value)
    {
        if (!_cheatResetDic.ContainsKey(key))
        {
            _cheatResetDic.Add(key,value);
        }
    }

    private void SetCurrency(MoneyType type , int v , string propertyName , bool record , string recordName)
    {
        var lastData = DataMgr.Instance.GetSqlService<PlayerSaveData>().tableList[0];
        var property = lastData.GetType().GetProperty(propertyName);
        float propertyValue = -1;
        if (propertyName == "cash" || propertyName == "gemstone")
        {
            var tar = MoneyInfo.GetMoneyEntity(type);
            tar.GMSetCount(v);
            propertyValue = EncryptionHelper.AesDecrypt((string)property.GetValue(lastData),PlayerInfo.pid).ToFloat();
        }

        if (record)
        {
            if (propertyValue == -1)
            {
                propertyValue = (float) property.GetValue(lastData);
            }
            
            RecordCheat(recordName, propertyValue);
        }
        
        if (property != null)
        {
            string value = EncryptionHelper.AesEncrypt(v.ToString(), PlayerInfo.pid);
            property.SetValue(lastData,value);
        }
        // DataMgr.Instance.GetSqlService<PlayerSaveData>().Update(lastData);
    }
    
    private void OnGUI()
    {
        if (!GamePlay.Instance.GMUI)
            return;
        if (!openTools)
        {
            if (GUI.Button(new Rect(Screen.width / 2 - 50, 100f * sizeK, 100f * sizeK, 50 * sizeK), "封号测试"))
            {
                sizeK = Tools.GetScreenScale().x;
                openTools = true;
            }
        }
        else
        {
            GUI.BeginGroup(new Rect(Screen.width / 2 - toolWidSize.x * 0.5f * sizeK, Screen.height / 2 - toolWidSize.y * 0.5f * sizeK, toolWidSize.x * sizeK, toolWidSize.y * sizeK));
            GUI.Box(new Rect(0, 0, toolWidSize.x * sizeK, toolWidSize.y * sizeK), "等着封号吧！！");
            if (GUI.Button(new Rect(0, 0, 50 * sizeK, 30 * sizeK), "关闭"))
            {
                openTools = false;
            }
            if (GUI.Button(new Rect(60, 0, 50 * sizeK, 30 * sizeK), "还原属性"))
            {
                ResetCheat();
            }
            
            // if (GUI.Button(GetBtnRect(0, 0), "设置钞票为-100"))
            // {
            //     SetCurrency(MoneyType.Cash,-100,"cash",true,"cash");
            // }
            // if (GUI.Button(GetBtnRect(0, 1), "设置宝石为-100"))
            // {
            //     SetCurrency(MoneyType.Gemstone,-100,"gemstone",true,"gemstone");
            // }
            
            // if (GUI.Button(GetBtnRect(0, 2), "设置钞票总量为-100"))
            // {
            //     // var tar = MoneyInfo.GetMoneyEntity(MoneyType.Cash);
            //     // var lastData = DataMgr.Instance.GetSqlService<PlayerSaveData>().tableList[0];
            //     // lastData.totalCash = -100;
            //     // DataMgr.Instance.GetSqlService<PlayerSaveData>().Update(lastData);
            //     SetCurrency(MoneyType.Cash,-100,"totalCash",true,"totalCash");
            // }
            //
            // if (GUI.Button(GetBtnRect(0, 3), "设置钞票总量>1亿"))
            // {
            //     // var tar = MoneyInfo.GetMoneyEntity(MoneyType.Cash);
            //     // var lastData = DataMgr.Instance.GetSqlService<PlayerSaveData>().tableList[0];
            //     // lastData.totalCash = 110000000;
            //     // DataMgr.Instance.GetSqlService<PlayerSaveData>().Update(lastData);
            //     SetCurrency(MoneyType.Cash,110000000,"totalCash",true,"totalCash");
            // }
            //
            // if (GUI.Button(GetBtnRect(0, 4), "设置宝石总量-100"))
            // {
            //     // var tar = MoneyInfo.GetMoneyEntity(MoneyType.Gemstone);
            //     // var lastData = DataMgr.Instance.GetSqlService<PlayerSaveData>().tableList[0];
            //     // lastData.totalGemstone = -100;
            //     // DataMgr.Instance.GetSqlService<PlayerSaveData>().Update(lastData);
            //     SetCurrency(MoneyType.Gemstone,-100,"totalGemstone",true,"totalGemstone");
            // }
            //
            // if (GUI.Button(GetBtnRect(0, 5), "设置宝石总量>100W"))
            // {
            //     // var tar = MoneyInfo.GetMoneyEntity(MoneyType.Gemstone);
            //     // var lastData = DataMgr.Instance.GetSqlService<PlayerSaveData>().tableList[0];
            //     // lastData.totalGemstone = 1100000;
            //     // DataMgr.Instance.GetSqlService<PlayerSaveData>().Update(lastData);
            //     SetCurrency(MoneyType.Gemstone,1100000,"totalGemstone",true,"totalGemstone");
            // }
            //
            // if (GUI.Button(GetBtnRect(0, 6), "设置钞票消耗-100"))
            // {
            //     // var tar = MoneyInfo.GetMoneyEntity(MoneyType.Gemstone);
            //     // var lastData = DataMgr.Instance.GetSqlService<PlayerSaveData>().tableList[0];
            //     // lastData.consumCash = -100;
            //     // DataMgr.Instance.GetSqlService<PlayerSaveData>().Update(lastData);
            //     SetCurrency(MoneyType.Cash,-100,"consumCash",true,"consumCash");
            // }
            //
            // if (GUI.Button(GetBtnRect(0, 7), "设置宝石消耗-100"))
            // {
            //     // var tar = MoneyInfo.GetMoneyEntity(MoneyType.Gemstone);
            //     // var lastData = DataMgr.Instance.GetSqlService<PlayerSaveData>().tableList[0];
            //     // lastData.consumGemstone = -100;
            //     // DataMgr.Instance.GetSqlService<PlayerSaveData>().Update(lastData);
            //     SetCurrency(MoneyType.Gemstone,-100,"consumGemstone",true,"consumGemstone");
            // }
            if (GUI.Button(GetBtnRect(1, 0), "手枪备弹-65"))
            {
                var temp = BattleController.GetCtrl<BulletCtrl>().bulletList
                    .Find(b => b.bulletType == BulletType.Pistol);
                RecordCheat("Pistol_bag",temp.bagCount);
                
                temp.GMSetBulletCount(-65);
            }
            if (GUI.Button(GetBtnRect(1, 2), "手枪备弹100"))
            {
                var temp = BattleController.GetCtrl<BulletCtrl>().bulletList
                    .Find(b => b.bulletType == BulletType.Pistol);
                RecordCheat("Pistol_bag",temp.bagCount);
                
                temp.GMSetBulletCount(100);
            }
            
            if (GUI.Button(GetBtnRect(1, 3), "散弹枪备弹-65"))
            {
                var temp = BattleController.GetCtrl<BulletCtrl>().bulletList
                    .Find(b => b.bulletType == BulletType.ShotGun);
                RecordCheat("ShotGun_bag",temp.bagCount);
                
                temp.GMSetBulletCount(-65);
            }
            if (GUI.Button(GetBtnRect(1, 4), "散弹枪备弹100"))
            {
                var temp = BattleController.GetCtrl<BulletCtrl>().bulletList
                    .Find(b => b.bulletType == BulletType.ShotGun);
                RecordCheat("ShotGun_bag",temp.bagCount);
                temp.GMSetBulletCount(100);
            }
            
            if (GUI.Button(GetBtnRect(1, 5), "弩备弹-65"))
            {
                var temp = BattleController.GetCtrl<BulletCtrl>().bulletList
                    .Find(b => b.bulletType == BulletType.Arrow);
                RecordCheat("Arrow_bag",temp.bagCount);
                temp.GMSetBulletCount(-65);
            }
            if (GUI.Button(GetBtnRect(1, 6), "弩备弹100"))
            {
                var temp = BattleController.GetCtrl<BulletCtrl>().bulletList
                    .Find(b => b.bulletType == BulletType.Arrow);
                RecordCheat("Arrow_bag",temp.bagCount);
                temp.GMSetBulletCount(100);
            }
            
            if (GUI.Button(GetBtnRect(1, 7), "MP5备弹-65"))
            {
                var temp = BattleController.GetCtrl<BulletCtrl>().bulletList
                    .Find(b => b.bulletType == BulletType.Mp5);
                RecordCheat("Mp5_bag",temp.bagCount);
                temp.GMSetBulletCount(-65);
            }
            if (GUI.Button(GetBtnRect(2, 0), "MP5备弹200"))
            {
                var temp = BattleController.GetCtrl<BulletCtrl>().bulletList
                    .Find(b => b.bulletType == BulletType.Mp5);
                RecordCheat("Mp5_bag",temp.bagCount);
                temp.GMSetBulletCount(200);
            }
            
            if (GUI.Button(GetBtnRect(2, 3), "手枪子弹-65"))
            {
                var temp = Player.player.weaponManager.FindWeapon(WeaponType.HandGun);
                RecordCheat("HandGun_ammo", temp.bulletCount);
                temp.GMBulletChange(-65);
            }
            if (GUI.Button(GetBtnRect(2, 4), "手枪子弹200"))
            {
                var temp = Player.player.weaponManager.FindWeapon(WeaponType.HandGun);
                RecordCheat("HandGun_ammo", temp.bulletCount);
                temp.GMBulletChange(200);
            }
            
            if (GUI.Button(GetBtnRect(2, 5), "散弹枪子弹-65"))
            {
                var temp = Player.player.weaponManager.FindWeapon(WeaponType.ShotGun);
                RecordCheat("ShotGun_ammo", temp.bulletCount);
                temp.GMBulletChange(-65);
            }
            if (GUI.Button(GetBtnRect(2, 6), "散弹枪子弹200"))
            {
                var temp = Player.player.weaponManager.FindWeapon(WeaponType.ShotGun);
                RecordCheat("ShotGun_ammo", temp.bulletCount);
                temp.GMBulletChange(200);
            }
            
            if (GUI.Button(GetBtnRect(2, 7), "弩子弹-65"))
            {
                var temp = Player.player.weaponManager.FindWeapon(WeaponType.Bow);
                RecordCheat("Bow_ammo", temp.bulletCount);
                temp.GMBulletChange(-65);
            }
            if (GUI.Button(GetBtnRect(3, 0), "弩子弹200"))
            {
                var temp = Player.player.weaponManager.FindWeapon(WeaponType.Bow);
                RecordCheat("Bow_ammo", temp.bulletCount);
                temp.GMBulletChange(200);
            }
            
            if (GUI.Button(GetBtnRect(3, 1), "MP5子弹-65"))
            {
                var temp = Player.player.weaponManager.FindWeapon(WeaponType.Rifle);
                RecordCheat("Rifle_ammo", temp.bulletCount);
                temp.GMBulletChange(-65);
            }
            if (GUI.Button(GetBtnRect(3, 2), "MP5子弹200"))
            {
                var temp = Player.player.weaponManager.FindWeapon(WeaponType.Rifle);
                RecordCheat("Rifle_ammo", temp.bulletCount);
                temp.GMBulletChange(200);
            }
            
            if (GUI.Button(GetBtnRect(3, 3), "燃烧瓶子弹-65"))
            {
                var temp = Player.player.weaponManager.FindWeapon(WeaponType.FireBottle);
                RecordCheat("FireBottle_ammo", temp.bulletCount);
                temp.GMBulletChange(-65);
            }
            if (GUI.Button(GetBtnRect(3, 4), "燃烧瓶子弹200"))
            {
                var temp = Player.player.weaponManager.FindWeapon(WeaponType.FireBottle);
                RecordCheat("FireBottle_ammo", temp.bulletCount);
                temp.GMBulletChange(200);
            }
            
            if (GUI.Button(GetBtnRect(3, 5), "手套伤害999"))
            {
                RecordCheat("23001_hurt", Player.player.weaponManager.FindWeapon(23001).weaponAttribute.meleeAtt.value);
                Player.player.weaponManager.FindWeapon(23001).weaponAttribute.meleeAtt = new FloatField(999, FieldAesType.Xor);
            }
            if (GUI.Button(GetBtnRect(3, 6), "手枪伤害999"))
            {
                RecordCheat("23002_hurt", Player.player.weaponManager.FindWeapon(23002).weaponAttribute.gunAtt.value);
                Player.player.weaponManager.FindWeapon(23002).weaponAttribute.gunAtt = new FloatField(999, FieldAesType.Xor);
            }
            if (GUI.Button(GetBtnRect(3, 7), "散弹枪伤害999"))
            {
                RecordCheat("23003_hurt", Player.player.weaponManager.FindWeapon(23003).weaponAttribute.gunAtt.value);
                Player.player.weaponManager.FindWeapon(23003).weaponAttribute.gunAtt = new FloatField(999, FieldAesType.Xor);
            }
            if (GUI.Button(GetBtnRect(4, 0), "弩伤害999"))
            {
                RecordCheat("23004_hurt", Player.player.weaponManager.FindWeapon(23004).weaponAttribute.gunAtt.value);
                Player.player.weaponManager.FindWeapon(23004).weaponAttribute.gunAtt = new FloatField(999, FieldAesType.Xor);
            }
            if (GUI.Button(GetBtnRect(4, 1), "MP5伤害999"))
            {
                RecordCheat("23005_hurt", Player.player.weaponManager.FindWeapon(23005).weaponAttribute.gunAtt.value);
                Player.player.weaponManager.FindWeapon(23005).weaponAttribute.gunAtt = new FloatField(999, FieldAesType.Xor);
            }
            if (GUI.Button(GetBtnRect(4, 2), "燃烧瓶伤害999"))
            {
                RecordCheat("23006_hurt", Player.player.weaponManager.FindWeapon(23006).weaponAttribute.gunAtt.value);
                Player.player.weaponManager.FindWeapon(23006).weaponAttribute.gunAtt = new FloatField(999, FieldAesType.Xor);
            }
            
            if (GUI.Button(GetBtnRect(4, 3), "手套攻击速度9"))
            {
                RecordCheat("23001_interval", Player.player.weaponManager.FindWeapon(23001).weaponAttribute.shotInterval.value);
                Player.player.weaponManager.FindWeapon(23001).weaponAttribute.shotInterval = new FloatField(9, FieldAesType.Xor);
            }
            if (GUI.Button(GetBtnRect(4, 4), "手枪攻击速度9"))
            {
                RecordCheat("23002_interval", Player.player.weaponManager.FindWeapon(23002).weaponAttribute.shotInterval.value);
                Player.player.weaponManager.FindWeapon(23002).weaponAttribute.shotInterval = new FloatField(9, FieldAesType.Xor);
            }
            if (GUI.Button(GetBtnRect(4, 5), "散弹枪攻击速度9"))
            {
                RecordCheat("23003_interval", Player.player.weaponManager.FindWeapon(23003).weaponAttribute.shotInterval.value);
                Player.player.weaponManager.FindWeapon(23003).weaponAttribute.shotInterval = new FloatField(9, FieldAesType.Xor);
            }
            if (GUI.Button(GetBtnRect(4, 6), "弩攻击速度9"))
            {
                RecordCheat("23004_interval", Player.player.weaponManager.FindWeapon(23004).weaponAttribute.shotInterval.value);
                Player.player.weaponManager.FindWeapon(23004).weaponAttribute.shotInterval = new FloatField(9, FieldAesType.Xor);
            }
            if (GUI.Button(GetBtnRect(4, 7), "MP5攻击速度9"))
            {
                RecordCheat("23005_interval", Player.player.weaponManager.FindWeapon(23005).weaponAttribute.shotInterval.value);
                Player.player.weaponManager.FindWeapon(23005).weaponAttribute.shotInterval = new FloatField(9, FieldAesType.Xor);
            }
            
            if (GUI.Button(GetBtnRect(5, 0), "受击100&损失HP0"))
            {
                RecordCheat("hurtDamage", Player.player.hurtDamage);
                RecordCheat("hurtCount", Player.player.hurtCount);
                Player.player.hurtDamage = 0;
                Player.player.hurtCount = 100;
            }
            

            GUI.EndGroup();
        }
    }
    
    private void ResetCheat()
    {
        float v = 0;
        // if (_cheatResetDic.TryGetValue("cash",out v))
        // {
        //     SetCurrency(MoneyType.Cash,(int)v,"cash",false,"cash");
        // }
        //
        // if (_cheatResetDic.TryGetValue("gemstone",out v))
        // {
        //     SetCurrency(MoneyType.Cash,(int)v,"gemstone",false,"gemstone");
        // }
        //
        // if (_cheatResetDic.TryGetValue("totalCash",out v))
        // {
        //     SetCurrency(MoneyType.Cash,(int)v,"totalCash",false,"totalCash");
        // }
        //
        // if (_cheatResetDic.TryGetValue("totalGemstone",out v))
        // {
        //     SetCurrency(MoneyType.Cash,(int)v,"totalGemstone",false,"totalGemstone");
        // }
        //
        // if (_cheatResetDic.TryGetValue("consumCash",out v))
        // {
        //     SetCurrency(MoneyType.Cash,(int)v,"consumCash",false,"consumCash");
        // }
        //
        // if (_cheatResetDic.TryGetValue("consumGemstone",out v))
        // {
        //     SetCurrency(MoneyType.Cash,(int)v,"consumGemstone",false,"consumGemstone");
        // }
        //
        // if (_cheatResetDic.TryGetValue("consumGemstone",out v))
        // {
        //     SetCurrency(MoneyType.Cash,(int)v,"consumGemstone",false,"consumGemstone");
        // }
        
        
        if (_cheatResetDic.TryGetValue("Pistol_bag",out v))
        {
            var temp = BattleController.GetCtrl<BulletCtrl>().bulletList.Find(b => b.bulletType == BulletType.Pistol);
            temp.GMSetBulletCount(-temp.bagCount);
        }
        
        if (_cheatResetDic.TryGetValue("ShotGun_bag",out v))
        {
            var temp = BattleController.GetCtrl<BulletCtrl>().bulletList.Find(b => b.bulletType == BulletType.ShotGun);
            temp.GMSetBulletCount(-temp.bagCount);
        }
        
        if (_cheatResetDic.TryGetValue("Arrow_bag",out v))
        {
            var temp = BattleController.GetCtrl<BulletCtrl>().bulletList.Find(b => b.bulletType == BulletType.Arrow);
            temp.GMSetBulletCount(-temp.bagCount);
        }
        
        if (_cheatResetDic.TryGetValue("Mp5_bag",out v))
        {
            var temp = BattleController.GetCtrl<BulletCtrl>().bulletList.Find(b => b.bulletType == BulletType.Mp5);
            temp.GMSetBulletCount(-temp.bagCount);
        }
        
        if (_cheatResetDic.TryGetValue("HandGun_ammo",out v))
        {
            Player.player.weaponManager.FindWeapon(WeaponType.HandGun).GMBulletChange((int)v);
        }
        if (_cheatResetDic.TryGetValue("Bow_ammo",out v))
        {
            Player.player.weaponManager.FindWeapon(WeaponType.Bow).GMBulletChange((int)v);
        }
        if (_cheatResetDic.TryGetValue("ShotGun_ammo",out v))
        {
            Player.player.weaponManager.FindWeapon(WeaponType.ShotGun).GMBulletChange((int)v);
        }
        if (_cheatResetDic.TryGetValue("Rifle_ammo",out v))
        {
            Player.player.weaponManager.FindWeapon(WeaponType.Rifle).GMBulletChange((int)v);
        }
        if (_cheatResetDic.TryGetValue("FireBottle_ammo",out v))
        {
            Player.player.weaponManager.FindWeapon(WeaponType.FireBottle).GMBulletChange((int)v);
        }
        
        
        if (_cheatResetDic.TryGetValue("23001_hurt",out v))
        {
            Player.player.weaponManager.FindWeapon(23001).weaponAttribute.meleeAtt = new FloatField(v, FieldAesType.Xor);
        }
        if (_cheatResetDic.TryGetValue("23002_hurt",out v))
        {
            Player.player.weaponManager.FindWeapon(23002).weaponAttribute.gunAtt = new FloatField(v, FieldAesType.Xor);
        }
        if (_cheatResetDic.TryGetValue("23003_hurt",out v))
        {
            Player.player.weaponManager.FindWeapon(23003).weaponAttribute.gunAtt = new FloatField(v, FieldAesType.Xor);
        }
        if (_cheatResetDic.TryGetValue("23004_hurt",out v))
        {
            Player.player.weaponManager.FindWeapon(23004).weaponAttribute.gunAtt = new FloatField(v, FieldAesType.Xor);
        }
        if (_cheatResetDic.TryGetValue("23005_hurt",out v))
        {
            Player.player.weaponManager.FindWeapon(23005).weaponAttribute.gunAtt = new FloatField(v, FieldAesType.Xor);
        }
        if (_cheatResetDic.TryGetValue("23006_hurt",out v))
        {
            Player.player.weaponManager.FindWeapon(23006).weaponAttribute.gunAtt = new FloatField(v, FieldAesType.Xor);
        }
        
        
        if (_cheatResetDic.TryGetValue("23001_hurt",out v))
        {
            Player.player.weaponManager.FindWeapon(23001).weaponAttribute.shotInterval = new FloatField(v, FieldAesType.Xor);
        }
        if (_cheatResetDic.TryGetValue("23002_hurt",out v))
        {
            Player.player.weaponManager.FindWeapon(23002).weaponAttribute.shotInterval = new FloatField(v, FieldAesType.Xor);
        }
        if (_cheatResetDic.TryGetValue("23003_hurt",out v))
        {
            Player.player.weaponManager.FindWeapon(23003).weaponAttribute.shotInterval = new FloatField(v, FieldAesType.Xor);
        }
        if (_cheatResetDic.TryGetValue("23004_hurt",out v))
        {
            Player.player.weaponManager.FindWeapon(23004).weaponAttribute.shotInterval = new FloatField(v, FieldAesType.Xor);
        }
        if (_cheatResetDic.TryGetValue("23005_hurt",out v))
        {
            Player.player.weaponManager.FindWeapon(23005).weaponAttribute.shotInterval = new FloatField(v, FieldAesType.Xor);
        }
        
        if (_cheatResetDic.TryGetValue("hurtDamage",out v))
        {
            Player.player.hurtDamage = v;
        }
        
        if (_cheatResetDic.TryGetValue("hurtCount",out v))
        {
            Player.player.hurtCount = (int)v;
        }
        
        _cheatResetDic = new Dictionary<string, float>();

    }

}
