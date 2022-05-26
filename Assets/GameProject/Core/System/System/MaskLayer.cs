using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MaskLayer
{
    /// <summary>
    /// 物品交互的格挡层
    /// </summary>
    public static int LookPointBlock = LayerMask.GetMask("Player", "Props", "Ground", "Wall", "Door", "Monster","Default");

    /// <summary>
    /// 格挡层
    /// </summary>
    public static int obstacal = LayerMask.GetMask( "Ground", "Wall", "Door","Props","Default");

    /// <summary>
    /// 玩家层
    /// </summary>
    public static int Playerlayer = 10;

    /// <summary>
    /// 玩家射击判定层
    /// </summary>
    public static int PlayerShot = LayerMask.GetMask("Monster", "Props", "Ground", "Wall", "Door", "MonsterAttackBall","Default");

    /// <summary>
    /// 无效层,只对obstacal有碰撞,其他都没有任何碰撞
    /// </summary>
    public static int unActiveThing = 16;

    /// <summary>
    /// 怪物层
    /// </summary>
    public static int monster = 11;
    
    /// <summary>
    /// 玩家阻碍
    /// </summary>
    public static int playerBlock = 19;
    
    /// <summary>
    /// 死亡的怪物层
    /// </summary>
    public static int deadMonster = 20;
    /// <summary>
    /// 门
    /// </summary>
    public static int door = LayerMask.GetMask("Door", "TransparentObj");
}