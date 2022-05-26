using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Module;
using System.Text;
using System;

/// <summary>
/// 游戏ui地图管理
/// 地图初始化,存档
/// 地图更新
/// 获取地图探索遮罩
/// </summary>
public class MapCtrl : BattleSystem,ILocalSave
{
    public string localFileName => LocalSave.savePath;
    public string localGroup { get; }
    public string localUid { get; }
    public string GetWriteDate()
    {
        // throw new NotImplementedException();
        return string.Empty;
    }
}
