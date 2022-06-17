using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Module;
/// <summary>
/// 角色timeline模型管理
/// </summary>
public class PlayerTimelineModel : MonoBehaviour
{
    public enum ShowModel
    { 
        None,
        Weapon,
        Morphine
    }

    public GameObject weaponModel;
    public GameObject morphineModel;
    public Animator _anim;
    public Transform camerTrans;
    /// <summary>
    /// 显示模型
    /// </summary>
    /// <param name="showWeapon">是否显示模型</param>
    public void Show(ShowModel showModel)
    {
        weaponModel.OnActive(showModel == ShowModel.Weapon);
        morphineModel.OnActive(showModel == ShowModel.Morphine);
        gameObject.OnActive(true);
    }

    public void Hide()
    {
        gameObject.OnActive(false);
    }
}
