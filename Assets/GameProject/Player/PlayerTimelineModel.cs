using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Module;
/// <summary>
/// 角色timeline模型管理
/// </summary>
public class PlayerTimelineModel : MonoBehaviour
{
    public GameObject weaponModel;
    public Animator _anim;
    public Transform camerTrans;
    public void Show(bool showWeapon)
    {
        weaponModel.OnActive(showWeapon);
        gameObject.OnActive(true);
    }

    public void Hide()
    {
        gameObject.OnActive(false);
    }
}
