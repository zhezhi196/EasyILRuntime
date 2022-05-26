using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Module;

/// <summary>
/// 双管霰弹枪
/// 先使用右侧子弹
/// </summary>
public class WeaponDoubleBarrelShotgun : Weapon
{
    [Header("霰弹枪设置")]
    public int attackCount = 5;
    [Tooltip("0左边，1右边")]
    public GameObject[] h_BulletMods;
    [Tooltip("0左边，1右边")]
    public GameObject[] w_BulletMods;
    private int reloadCount = 0;
    private int fireCount = 0;
    public override bool CanAttack
    {
        get
        {
            if (cd <= 0 && bulletCount > 0
                && !_player.ContainStation(Player.Station.WaitingAttack))
                return true;
            else
                return false;
        }
    }
    public override bool CanReload
    {
        get
        {
            if (bulletCount < maxBulletCount
                && !_player.ContainStation(Player.Station.ShotGunReload)
                && !_player.ContainStation(Player.Station.WeaponChanging)
                && bullet.bagCount > 0)// &&!reloading
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public override void AttackBtnDown()
    {
        if (bulletCount <= 0 && CanReload)
        {
            //换弹
            ReloadBtnDown();
            return;
        }

        if (CanAttack)
        {
            //reloading = false;
            _player.AddStation(Player.Station.WaitingAttack);
            if (!_player.ContainStation(Player.Station.Aim))
            {
                ChangeToAim();
            }
            else
            {
                _animator.CrossFade("collimationIdle", 0.2f);
            }
            _animator.SetTrigger(WeaponrAnimaKey.Fire);
            return;
        }
        if (cd <= 0 && bulletCount <= 0)
        {
            if (bullet.bagCount <= 0)
                //EventCenter.Dispatch(EventKey.ShowTextTip, GameUITips.TextTipType.Bullet);
                if (_player.ContainStation(Player.Station.Aim))
                {
                    //武器空仓音效
                    if (!System.String.IsNullOrEmpty(weaponArgs.emptyFireAuido))
                    {
                        PlayAudio(weaponArgs.emptyFireAuido, transform.position);
                    }
                }
        }
    }

    public override void Attack()
    {
        if (bulletCount <= 0)//子弹小于0不能射击
            return;
        fireCount += 1;
        _player.RemoveStation(Player.Station.WaitingAttack);
        //EffectPlay.Play(fireEffect, firePoint);//射击特效
        //todo--射线检测
        for (int i = 0; i < attackCount; i++)
        {
            Vector2 random = Random.insideUnitSphere * weaponArgs.diffusionArea;
            //if (allGrow > 0)
            //    random = Random.insideUnitSphere* allGrow;
            Physics.SyncTransforms();
            Ray ray = _player.evCamera.ScreenPointToRay(FireRayPos + random);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 2000, MaskLayer.PlayerShot))
            {
                GameDebug.LogFormat("WeaponHit:" + hit.collider.name);
                IHurtObject hurtObject = hit.collider.GetComponent<IHurtObject>();
                if (hurtObject != null)
                {
                    var damage = _player.CreateDamage(this, hurtObject);
                    //damage.hpDamage.damage /= attackCount;
                    if (hurtObject is MonsterPart part)
                    {
                        //EventCenter.Dispatch(EventKey.HitMonster, (part.monster.hp - damage.hpDamage.damage) > 0, part);
                    }
                    hurtObject.OnHurt(_player, damage);
                    HitSomething(hurtObject, hit, weaponType);//特效
                }
            }
        }
        //播放射击音效
        BulletChange(-1);
        if (bulletCount <= 0)
            ReloadBtnDown();
        cd = showInterval;
        if (allGrow < weaponArgs.maxAccurate)
            allGrow += weaponAttribute.recoilForce;
        GameDebug.Log("Shotgun Fire");
        base.Attack();
    }

    public override void ReloadBtnDown()
    {
        if (!CanReload)
            return;
        //reloading = true;
        _player.AddStation(Player.Station.ShotGunReload);
        reloadCount = maxBulletCount - bulletCount;
        if (bullet.bagCount < reloadCount)
        {
            reloadCount = bullet.bagCount;
        }
        _animator.SetInteger(WeaponrAnimaKey.ReloadCount, reloadCount);
        if (fireCount <= 0 && bulletCount!=0)
        {
            _animator.CrossFade("AddReload", 0.1f);
        }
        else
        {
            _animator.CrossFade(bulletCount == 0 ? "EmptyReload" : "Reload", 0.1f);
        }
    }
    public override void Reload()
    {
        if (!System.String.IsNullOrEmpty(weaponArgs.reloadAudio))
        {
            PlayAudio(weaponArgs.reloadAudio, transform.position);
        }
        fireCount = 0;
        BulletChange(1);
        //切换模型显示
        if (bulletCount == 1)
        {
            w_BulletMods[1].OnActive(true);
        }
        if (bulletCount == 2)
        {
            w_BulletMods[0].OnActive(true);
        }
        bullet.Get(1);
        reloadCount -= 1;
        if (reloadCount <= 0)
        {
            _player.RemoveStation(Player.Station.ShotGunReload);
            OnReloadExit();
        }
    }
    public void OnReloadExit()
    {
        //隐藏手里子弹模型
        for (int i = 0; i < h_BulletMods.Length; i++)
        {
            h_BulletMods[i].OnActive(false);
        }
    }

    public override void TakeOut()
    {
        for (int i = 0; i < h_BulletMods.Length; i++)
        {
            h_BulletMods[i].OnActive(false);
        }
        base.TakeOut();
    }

    //倒出子弹，0,两个,1左边
    public void HideModel()
    {
        if (bulletCount == 0)
        {
            for (int i = 0; i < w_BulletMods.Length; i++)
            {
                w_BulletMods[i].OnActive(false);
            }
        }
        else {
            w_BulletMods[0].OnActive(false);
        }
        //显示手里子弹模型
        for (int i = 0; i < h_BulletMods.Length; i++)
        {
            h_BulletMods[i].OnActive(true);
        }
    }
    //装上子弹，1右边,2,左边
    //public void ShowBullet(int type)
    //{
    //    if (type == 1)
    //    {
    //        w_BulletMods[1].OnActive(true);
    //        h_BulletMods[1].OnActive(false);
    //    }
    //    if (type == 2)
    //    {
    //        w_BulletMods[0].OnActive(true);
    //        h_BulletMods[0].OnActive(false);
    //    }
    //}
}
