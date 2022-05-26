using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Module;

/// <summary>
/// 角色语音
/// 事件触发语音
/// </summary>
public class PlayerDialogCtrl : MonoBehaviour
{
    private AudioPlay currentAudio;
    private bool playing = false;

    public AudioContainer reloadContainer;
    public AudioContainer attackContainer;
    private void Start()
    {
        //注册事件
        EventCenter.Register(EventKey.WeaponReload, OnReload);
        EventCenter.Register<bool, MonsterPart>(EventKey.HitMonster, OnAttackMonster);
    }

    private void Update()
    {
        if (playing && currentAudio!=null)//播放语音时检查是否完成
        {
            if (currentAudio.isComplete)
            {
                currentAudio = null;
                playing = false;
            }
        }
    }
    //退出游戏打断音效
    private void OnExitGame()
    {
        if (playing)
        {
            StopAudio();
        }
    }
    private void OnTimeline()
    {
        //打断当前音效
        if (playing)
        {

        }
    }

    private void OnDestroy()
    {
        //注销事件
        EventCenter.UnRegister(EventKey.WeaponReload, OnReload);
        EventCenter.UnRegister<bool, MonsterPart>(EventKey.HitMonster, OnAttackMonster);
    }


    //换弹事件
    private void OnReload()
    {
        //最后一个弹夹语音
        //不是最后一个弹夹语音
        reloadContainer.Play(this.transform);
    }
    //受击事件
    private void OnHurt()
    { 
    
    }
    //击杀怪物
    private void OnAttackMonster(bool kill, MonsterPart part)
    {
        //近战击杀
        //枪杀
        attackContainer.Play(this.transform);
    }
    //恢复生命
    private void OnHpChange()
    { 
    
    }
    //看广告
    private void OnWatchAds()
    { 
        
    }

    private void PlayAudio(string audioName)
    {
        currentAudio = AudioPlay.Play(audioName).SetID("PlayerDialog");
        playing = true;
    }

    private void StopAudio()
    {
        currentAudio.Stop();
        currentAudio = null;
        playing = false;
    }
}
