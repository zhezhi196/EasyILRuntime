using Module;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TeachingName
{
    MoveTeaching,//移动,旋转教学
    GiftTeaching,//天赋教学
    MeleeAttack,//近战教学
    CrouchTeaching,//下蹲教学
    TipTeaching,//提示教学
    RunTeaching,//跑步教学
    WeaponFire,//武器射击教学
    AssTeaching,//暗杀教学
    WeaponUpgrade,//武器升级
    WeaponChange,//武器切换
    StrengthTeaching,//体力教学
}

public class TeachingManager
{
    public static Dictionary<TeachingName, Teaching> teachingDic = new Dictionary<TeachingName, Teaching>();
    public static AsyncLoadProcess Init(AsyncLoadProcess process)
    {
        //添加教学到字典

        //循环字典,未完成教学初始化
        foreach (var item in teachingDic)
        {
            item.Value.InitTeaching();
        }
        return process;
    }

    public static bool TeachingIsStart(TeachingName name)
    {
        if (teachingDic.ContainsKey(name))
        {
            return teachingDic[name].teachState!= Teaching.TeachState.None;
        }
        GameDebug.Log("未查询到指定教学");
        return true;
    }

    public static bool CompleteTeaching(TeachingName name)
    {
        if (teachingDic.ContainsKey(name))
        {
            return teachingDic[name].IsComplete();
        }
        GameDebug.Log("未查询到指定教学");
        return true;
    }

    public static string GetTeachingKey(Teaching teaching)
    {
        return "Teaching" + teaching.teachingName.ToString();
    }
}
