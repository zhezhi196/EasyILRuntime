using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventKey
{
    public const string piao = "piao";
    public const string uiLoadingKey = "uiLoadingKey";
    public const string CompleteMission = "CompleteMission";
    public const string ScreenOrientationChange = "ScreenOrientationChange";
    public const string GamePause = "GamePause";

    public const string OnGetWeapon = "OnGetWeapon";
    public const string OnWeaponUpgrade = "OnWeaponUpgrade";
    public const string OnPlayerHurt = "OnPlayerHurt";
    public const string WeaponBulletChange = "WeaponBulletChange";
    public const string ChangeGift = "ChangeGift";
    public const string HitMonster = "HitMonster";
    public const string OnWeaponFire = "OnWeaponFire";
    public const string MonsterTimeLine = "MonsterTimeLine";
    public const string MonsterEndTimeLine = "MonsterEndTimeLine";
    public const string OnPause = "OnPause";
    public const string OnPlayerDeath = "OnPlayerDeath";
    public const string AllWeaponEmpty = "AllWeaponEmpty";
    public const string OnPlayerComplete = "OnPlayerComplete";//enterbattle player准备完成
    public const string PlayerHide = "PlayerHide";//玩家进出躲藏点
    public const string WeaponReload = "WeaponReload";//武器换弹事件
    public const string PerfectDodge = "PerfectDodge";//完美闪避

    public const string AddAssExMonster = "AddAssExMonster";
    public const string OnStudyGift = "OnStudyGift";
    public const string DropProps = "DropProps";
    public const string ShowTextTip = "ShowTextTip";//gameui显示文字提示,能量不足,子弹不足
    public const string ChangeWeaponSkin = "ChangeWeaponSkin";
    public const string BulletCreat = "BulletCreat";
    public const string LimitReward = "LimitReward";

    public const string GameTeachStart = "GameTeachStart";//教学开始
    public const string GameTeachComplete = "GameTeachComplete";//教学结束
    public const string EnterBattle = "EnterBattle";
    public const string TeachEvent = "TeachEvent";
    public const string OnNodeEnter = "OnNodeEnter";
    
    //小游戏
    public const string WireRefresh = "WireRefresh";
    public const string WireRefreshHighLight = "RefreshHighLight";
    public const string WireBatteryChange = "BatteryChange";
    public const string WireConnectCanOpen = "WireConnectCanOpen";
    
    //音效事件
    public const string OnOpenCloseRoomPortal = "OnOpenCloseRoomPortal";
    
    //获取了某个道具发事件
    public const string OnGetProp = "OnGetProp";
}