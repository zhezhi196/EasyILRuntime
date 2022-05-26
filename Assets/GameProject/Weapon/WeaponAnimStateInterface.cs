using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeaponAnimEvent
{
    void Attack();
    void StartReload();
    void Reload();
    void ToAim();
    void ToNoAim();
    void SkillEvent(string key);
    void PlayAudio(string audioName, Vector3 pos);
    void StopAnimAudio();
    //void OnReloadExit();
    void OnAnimStateEnter(string stateName);
    void OnAnimStateExit(string stateName);
}
