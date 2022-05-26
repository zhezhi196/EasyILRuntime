using Module;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
/// <summary>
/// 弩
/// </summary>
public class WeaponBow : Weapon
{
    [Header("弩设置")]
    public AnimationClip emptyClip;
    public Animator modelAnima;
    public GameObject arrowModel;
    private PlayableGraph _playableGraph;
    private bool initPlayable = false;
    public string arrowPrefabPath = "";
    protected ObjectPool arrowPool;
    public override bool CanReload
    {
        get
        {
            if (bulletCount < maxBulletCount
                && !_player.ContainStation(Player.Station.BowReloading)
                && !_player.ContainStation(Player.Station.WeaponChanging)
                && bullet.bagCount > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    private void Start()
    {
        //创建空仓动画
        if (!initPlayable)
        {
            CreatEmptyPlayable();
        }
        _player.onRemoveStation += PlayerRemoveStation;
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
            _player.AddStation(Player.Station.WaitingAttack);
            if (!_player.ContainStation(Player.Station.Aim))
            {
                ChangeToAim();
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
        arrowModel.OnActive(false);
        _player.RemoveStation(Player.Station.WaitingAttack);
        Physics.SyncTransforms();
        Vector2 random = UnityEngine.Random.insideUnitSphere * 5;
        Ray ray = _player.evCamera.ScreenPointToRay(FireRayPos + random);
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
                    //EventCenter.Dispatch(EventKey.HitMonster, (part.monster.hp - damage.hpDamage.damage) > 0, part);
                }
                hurtObject.OnHurt(_player, damage);
                HitSomething(hurtObject, hit,weaponType);//特效
            }
        }
        BulletChange(-1);
        cd = showInterval;
        if (allGrow < weaponArgs.maxAccurate)
            allGrow += weaponAttribute.recoilForce;
        if (bulletCount <= 0)
        {
            OpenEmptyAnim();
            ReloadBtnDown();
        }
        GameDebug.Log("Bow Fire");
        base.Attack();
    }

    public override void ReloadBtnDown()
    {
        if (!CanReload)
            return;
        else
            CloseEnmptyAnim();
        _player.AddStation(Player.Station.BowReloading);
        if (bulletCount == 0)
        {
            _animator.CrossFade(WeaponrAnimaKey.EmptyReload, 0.1f);
        }
        if (!String.IsNullOrEmpty(weaponArgs.reloadAudio))
        {
            PlayAudio(weaponArgs.reloadAudio, transform.position);
        }
    }

    public override void Reload()
    {
        if (!_player.ContainStation(Player.Station.BowReloading))
            return;
        BulletChange(bullet.Get(maxBulletCount));
        _player.RemoveStation(Player.Station.BowReloading);
    }
    public override void TakeOut()
    {
        base.TakeOut();
        if (bulletCount == 0)
        {
            CloseEnmptyAnim();
            OpenEmptyAnim();
        }
        //todo--是否换弹判定
    }
    protected override void OnGamePasue(bool b)
    {
        base.OnGamePasue(b);
        if (bulletCount == 0 && this!=null)
        {
            CloseEnmptyAnim();
            OpenEmptyAnim();
        }
    }
    #region 弩没有箭的状态
    private void CreatEmptyPlayable()
    {
        initPlayable = true;
        _playableGraph = PlayableGraph.Create("PlayableAnimation");
        var playableOut = AnimationPlayableOutput.Create(_playableGraph, "Animation", modelAnima);
        var clipPlayable = AnimationClipPlayable.Create(_playableGraph, emptyClip);
        playableOut.SetSourcePlayable(clipPlayable);
    }

    public void OpenEmptyAnim()
    {
        if (!initPlayable)
        {
            CreatEmptyPlayable();
        }
        _playableGraph.Play();
    }
    public void CloseEnmptyAnim()
    {
        arrowModel?.OnActive(true);
        if (!initPlayable)
        {
            CreatEmptyPlayable();
        }
        _playableGraph.Stop();
    }
    private void PlayerRemoveStation(Player.Station station)
    {
        if (station == Player.Station.BowReloading)
        {
            if (bulletCount == 0)
            {
                CloseEnmptyAnim();
                OpenEmptyAnim();
            }
        }
    } 
    #endregion
}
