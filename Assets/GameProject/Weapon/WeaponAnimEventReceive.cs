using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 武器动画事件接收中转
/// 因为手枪单枪双枪切换,Animtor和Weapon不在一个对象上
/// 增加的工具
/// </summary>
public class WeaponAnimEventReceive : MonoBehaviour, IWeaponAnimEvent
{
    public Weapon weapon;
    public virtual void Attack()
    {
        weapon?.Attack();
    }
    /// <summary>
    /// 开始换弹,播放换弹音效
    /// </summary>
    public virtual void StartReload()
    {
        weapon?.StartReload();
    }
    /// <summary>
    /// 换弹
    /// </summary>
    public virtual void Reload()
    {
        weapon?.Reload();
    }

    public virtual void ToAim()
    {
        weapon?.ToAim();
    }

    public virtual void ToNoAim()
    {
        weapon?.ToNoAim();
    }

    public virtual void SkillEvent(string key)
    {
        weapon?.SkillEvent(key);
    }
    public void PlayAudio(string audioName, Vector3 pos)
    {
        weapon?.PlayAudio(audioName,pos);
    }

    public void StopAnimAudio()
    {
        weapon?.StopAnimAudio();
    }

    public virtual void OnReloadExit()
    {
        //weapon?.OnReloadExit();
    }

    public void OnAnimStateEnter(string stateName)
    {
        weapon?.OnAnimStateEnter(stateName);
    }

    public void OnAnimStateExit(string stateName)
    {
        weapon?.OnAnimStateExit(stateName);
    }
}
