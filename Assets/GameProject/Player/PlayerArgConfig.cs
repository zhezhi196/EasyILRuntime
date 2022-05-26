using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class PlayerStationConfig
{
    public Player.Station inStation;
    //public Player.Station outStation;
    public Player.Station[] outStations;
    //[Button("CopyToList")]
    //private void CopyToList()
    //{
    //    var result = new List<Player.Station>();
    //    foreach (Player.Station item in Player.Station.GetValues(typeof(Player.Station)))
    //    {
    //        if ((System.Convert.ToInt32(item) & System.Convert.ToInt32(outStation)) > 0)
    //        {
    //            result.Add(item);
    //        }
    //    }
    //    outStations = result.ToArray();
    //}
}


/// <summary>
/// 玩家固定参数配置
/// </summary>
[CreateAssetMenu(menuName ="Player/参数配置")]
public class PlayerArgConfig : ScriptableObject
{
    [LabelText("站立高度")] public float standHeight = 1.72f;
    [LabelText("下蹲高度")] public float squatHeight = 0.8f;
    [LabelText("转动速度")] public float rotateSpeed = 0.16f;
    [LabelText("瞄准转动速度")] public float aimRotateSpeed = 0.05f;
    [LabelText("体力恢复速度"),Range(0,1)] public float strengthRecover = 0.01f;
    [LabelText("体力跑步消耗")] public float runStrengthExpend = 15f;
    [LabelText("精力恢复速度")] public float energyRecover = 15f;
    [LabelText("闪避体力消耗")] public float dodgeExpend = 10;

    [FoldoutGroup("音效设置"), LabelText("下蹲移动脚步声间隔")]
    public float squatWalkAudioTime = 1f;
    [FoldoutGroup("音效设置"), LabelText("下蹲移动脚步声")]
    public string[] squatWalkAudio;
    [FoldoutGroup("音效设置"), LabelText("站立移动脚步声间隔")]
    public float walkAudioTime = 1f;
    [FoldoutGroup("音效设置"), LabelText("跑步移动脚步声间隔")]
    public float runAudioTime = 1f;
    [FoldoutGroup("音效设置"), LabelText("站立移动脚步声")]
    public string[] walkAudio;
    [FoldoutGroup("音效设置"), LabelText("受击音效间隔")]
    public float hurtAudioTime = 0.6f;
    [FoldoutGroup("音效设置"), LabelText("受击音效")]
    public string[] hurtAudio;
    [FoldoutGroup("音效设置"), LabelText("脚步范围")]
    public float stepRange = 0.5f;//下蹲脚步声范围
    [FoldoutGroup("音效设置"), LabelText("下蹲脚步范围")]
    public float squatStepRange = 0.5f;//下蹲脚步声范围
    [FoldoutGroup("音效设置"), LabelText("跑步脚步范围")]
    public float runStepRange = 0.5f;//下蹲脚步声范围

    [FoldoutGroup("暗杀处决"),LabelText("暗杀距离")]
    public float assDistance = 3f;//暗杀距离
    [FoldoutGroup("暗杀处决"),LabelText("暗杀角度")]
    public float assAngle = 60f;//暗杀范围角度,半角度
    #region PlayerStation
    [FoldoutGroup("状态排斥列表"), LabelText("增加一个Station前,当前Agent身上不能有的Station列表")]
    public List<PlayerStationConfig> rejectStationConfig = new List<PlayerStationConfig>();
    [FoldoutGroup("关联列表"), LabelText("一个Station的父Station,若添加必有父Station,若父buff删除必删除子Station")]
    public List<PlayerStationConfig> baseStationConfig = new List<PlayerStationConfig>();

    public PlayerStationConfig GetRejectStation(Player.Station station)
    {
        for (int i = 0; i < rejectStationConfig.Count; i++)
        {
            if (rejectStationConfig[i].inStation == station)
            {
                return rejectStationConfig[i].outStations.Length <= 0 ? null : rejectStationConfig[i];
            }
        }
        return null;
    }

    public PlayerStationConfig GetBaseStation(Player.Station station)
    {
        for (int i = 0; i < baseStationConfig.Count; i++)
        {
            if (baseStationConfig[i].inStation == station)
            {
                return baseStationConfig[i];
            }
        }
        return null;
    } 
    #endregion
}
