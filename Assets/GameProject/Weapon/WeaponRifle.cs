using Module;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 步枪,可连续射击
/// </summary>
public class WeaponRifle : Weapon
{
    private bool fireBtnDown;

    public override bool CanAttack
    {
        get {
            return cd <= 0 && bulletCount > 0 ;
        }
    }

    public override void AttackBtnDown()
    {
        base.AttackBtnDown();
        if (bulletCount <= 0 && CanReload)
        {
            //换弹
            ReloadBtnDown();
            return;
        }

        if (CanAttack)
        {
            fireBtnDown = true;
            _player.AddStation(Player.Station.WaitingAttack);
            if (!_player.ContainStation(Player.Station.Aim))
            {
                ChangeToAim();
            }
            cd = 1f;
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
                if (!String.IsNullOrEmpty(weaponArgs.emptyFireAuido))
                {
                    PlayAudio(weaponArgs.emptyFireAuido, transform.position);
                }
            }
        }
    }

    public override void AttackBtnUp()
    {
        base.AttackBtnUp();
        fireBtnDown = false;
    }

    public override void Attack()
    {
        if (bulletCount <= 0)//子弹小于0不能射击
            return;
        if (!fireBtnDown)
        {
            _player.RemoveStation(Player.Station.WaitingAttack);
        }
        EffectPlay.Play(fireEffect, firePoint);//射击特效
        _animator.ResetTrigger(WeaponrAnimaKey.Fire);
        Vector2 random = Vector2.zero;
        if (allGrow > 0)
            random = UnityEngine.Random.insideUnitSphere * allGrow;
        cd = showInterval;//射击间隔
        Physics.SyncTransforms();
        Ray ray = _player.cameraCtrl.evCamera.ScreenPointToRay(FireRayPos+ random);//+随机范围
        Physics.SyncTransforms();//点击射击按钮刷新物理
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 2000, MaskLayer.PlayerShot))
        {
            GameDebug.LogFormat("WeaponHit:" + hit.collider.name);
            IHurtObject hurtObject = hit.collider.GetComponent<IHurtObject>();
            if (hurtObject != null)
            {
                var damage = _player.CreateDamage(this, hurtObject);
                damage.dir = hit.point - firePoint.position;
                if (hurtObject is MonsterPart part)
                {
                    EventCenter.Dispatch(EventKey.HitMonster, (part.monster.hp - damage.damage) > 0, part);
                }
                hurtObject.OnHurt(_player, damage);
                HitSomething(hurtObject, hit,weaponType);//特效
            }
        }
        BulletChange(-1);
        if (allGrow < weaponArgs.maxAccurate)
            allGrow += weaponAttribute.recoilForce;
        if (bulletCount <= 0)
        {
            if (fireBtnDown)
            {
                _player.RemoveStation(Player.Station.WaitingAttack);
                fireBtnDown = false;
            }
            //尝试换弹
            ReloadBtnDown();
        }
        base.Attack();
    }

    protected override void Update()
    {
        base.Update();
        if (CanAttack && fireBtnDown)
        {
            _animator.SetTrigger(WeaponrAnimaKey.Fire);
            cd = 1f;
        }
    }
    public override void Reload()
    {
        int getCount = 0;
        if (bulletCount <= 0)
        {
            getCount = bullet.Get(maxBulletCount);
        }
        else
        {
            getCount = bullet.Get(maxBulletCount - bulletCount + 1);
        }
        BulletChange(getCount);
        base.Reload();
    }
}
