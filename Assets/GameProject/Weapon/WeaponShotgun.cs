using Module;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 霰弹枪
/// 特殊处理霰弹枪的换弹动画
/// </summary>
public class WeaponShotgun : Weapon
{
    [Header("霰弹枪设置")]
    public int attackCount = 5;
    private int reloadCount = 0;
    private bool reloading = false;
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
            else {
               _animator.CrossFade("collimationIdle", 0.2f);
            }
            _animator.SetTrigger(WeaponrAnimaKey.Fire);
            return;
        }
        if (cd <= 0 && bulletCount <= 0 )
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
        _player.RemoveStation(Player.Station.WaitingAttack);
        EffectPlay.Play(fireEffect, firePoint);//射击特效
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
                    HitSomething(hurtObject, hit,weaponType);//特效
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
        if (bulletCount == 0)
        {
            _animator.CrossFade("EmptyReload", 0.1f);
        }
        else
        {
            _animator.CrossFade("Reload", 0.1f);
        }
    }

    public override void Reload()
    {
        if (!System.String.IsNullOrEmpty(weaponArgs.reloadAudio))
        {
            PlayAudio(weaponArgs.reloadAudio, transform.position);
        }
        BulletChange(1);
        bullet.Get(1);
        reloadCount -= 1;
        _animator.SetInteger(WeaponrAnimaKey.ReloadCount, reloadCount);
        if (reloadCount >= maxBulletCount)
        {
            _player.RemoveStation(Player.Station.ShotGunReload);
        }
        //base.Reload();
    }

    public override void ToAim()
    {
        base.ToAim();
        //reloading = false;
    }

    public override void ToNoAim()
    {
        base.ToNoAim();
        //reloading = false;
    }

    public override void TakeBack()
    {
        base.TakeBack();
        //reloading = false;
    }
}
