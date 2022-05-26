using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Module;
public class WeaponBrick : Weapon
{
    [Header("砖头设置")]
    public Transform throwPoint;
    public string throwModelPath = "";

    public override void Init(Player p, bool readData)
    {
        base.Init(p, readData);
        ObjectPool.Cache(throwModelPath, 1);
    }
    public override void AttackBtnDown()
    {
        _animator.SetTrigger(WeaponrAnimaKey.Fire);
    }

    public override void Attack()
    {
        //Ray ray = _player.evCamera.ScreenPointToRay(FireRayPos);
        //RaycastHit hit;
        if (_player.ContainStation(Player.Station.Stealth))
            _player.RemoveStation(Player.Station.Stealth);
        //if (Physics.Raycast(ray, out hit, 1f, MaskLayer.PlayerShot))
        //{
        //    //AssetLoad.LoadGameObject<WeaponBrickThrow>(throwModelPath, null, (t, obj) =>
        //    //{
        //    //    t.Throw(hit, weaponAttribute.attRange);
        //    //});
        //}
       
        Vector3 target = Vector3.zero;
        target = _player.cameraCtrl.pCamera.transform.position + _player.cameraCtrl.pCamera.transform.forward * 20;
        AssetLoad.LoadGameObject<WeaponBrickThrow>(throwModelPath, null, (t, obj) =>
        {
            t.transform.position = throwPoint.position;
            t.Throw(throwPoint, target, this);
        });
        //切换到上一个武器
        bulletCount -= 1;
        if (bullet.bagCount <= 0)
        {
            _player.RemvoveSlotWeapon(this);
        }
        else
        {
            bullet.Get(1);
            bulletCount = 1;
        }
    }


    public async override void TakeOut()
    {
        this.gameObject.OnActive(true);
        //if (bulletCount <= 0)
        //{
        //    bullet.Get(1);
        //    bulletCount = 1;
        //}
        if (Player.player.ContainStation(Player.Station.Running))
        {
            SetRunStation(true);
        }
        _player.RemoveStation(Player.Station.Aim);
        _player.AddStation(Player.Station.WeaponChanging);
        await Async.WaitforSecondsRealTime(0.3f);
        _player.RemoveStation(Player.Station.WeaponChanging);
    }
}
