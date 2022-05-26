using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Module;

/// <summary>
/// 武器展示,皮肤刷新
/// </summary>
public class WeaponModelShow : MonoBehaviour
{
    private WeaponEntity entity;
    public int weaponID = 0;
    public Renderer[] modelRenders;

    private void Start()
    {
        entity = WeaponManager.weaponAllEntitys[weaponID];
        if (entity == null)
        {
            GameDebug.LogErrorFormat("武器展示皮肤初始化错误:{0}", gameObject.name);
        }
        RefeshRender();
        EventCenter.Register<WeaponEntity>(EventKey.ChangeWeaponSkin, OnChangeSkin);
    }

    private void OnDestroy()
    {
        EventCenter.UnRegister<WeaponEntity>(EventKey.ChangeWeaponSkin, OnChangeSkin);
    }

    private void OnChangeSkin(WeaponEntity entity)
    {
        if (this.entity == entity)
        {
            RefeshRender();
        }
    }

    private void RefeshRender()
    {
        for (int i = 0; i < modelRenders.Length; i++)
        {
            modelRenders[i].material = entity.equipSkin.skinMat;
        }
    }
}
