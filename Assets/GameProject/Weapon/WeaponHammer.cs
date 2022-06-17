using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Module;

/// <summary>
/// 近战武器:锤子
/// 普通攻击,三连击;蓄力攻击;超重击
/// </summary>
public class WeaponHammer : Weapon
{
    public const string NorAnimName = "NorAttack{0}";
    public const string ChargeAnimName = "ChargeAttack";
    public const string StrongAnimName = "StrongeAttack";
    private Vector2[] checkPos = new Vector2[] { Vector2.zero, new Vector2(70, -70), new Vector2(-70, -70), new Vector2(70, 70), new Vector2(-70, 70) };
    public enum AttackType
    {
        None,
        NormalAttak,
        ChargeAttack,
        StrongAttack,
        CombeAttack,
    }
    [System.Serializable]
    public class CombeAttackTime
    {
        public float startTime = 0;
        public float endTime = 1;
    }
    [Header("锤子设置")]
    public float attackExpend = 20;
    public List<CombeAttackTime> combeAttack = new List<CombeAttackTime>();
    public float chargeAttack = 0.5f;//进入蓄力按键时长
    public float maxChargeTime = 1f;//最大蓄力时间
    private float damageRatio = 0;//伤害值比率
    public bool canCharge = false;//是否可以蓄力攻击
    public bool strongAttack = false;//强力攻击

    private float pressedTime = 0;
    private bool attackBtnDown = false;
    private AttackType attackType;
    private AnimatorStateInfo animSta;
    private int attackCount = 0;
    private bool weakAttack = false;//虚弱攻击
    private bool waitAttack = false;//等待攻击
    private bool needCheckAttack = false;

    public override bool CanAttack
    {
        get {
            return !waitAttack;
        }
    }

    protected override void Update()
    {
        base.Update();
        //蓄力攻击判定
        if (attackBtnDown&& canCharge && attackType == AttackType.None)
        {
            if (Time.time - pressedTime >= chargeAttack)
            {
                attackType = AttackType.ChargeAttack;
                _player.AddStation(Player.Station.MeleeAttack);
                _animator.CrossFade(ChargeAnimName, 0.1f);
            }
        }
        if (needCheckAttack)
        {
            AttackCheck(false);
        }
    }

    /// <summary>
    /// 是否命中
    /// </summary>
    /// <param name="first">攻击前检测</param>
    /// <returns>是否处决怪物</returns>
    private bool AttackCheck(bool isPreAttack)
    {
        for (int i = 0; i < checkPos.Length; i++)
        {
            Ray ray = _player.evCamera.ScreenPointToRay(FireRayPos + checkPos[i]);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, weaponArgs.attckDistance, MaskLayer.PlayerShot)) 
            {
                //GameDebug.LogFormat("WeaponHit:" + hit.collider.name);
                IHurtObject hurtObject = hit.collider.GetComponent<IHurtObject>();
                if (isPreAttack)//第一次检测,检测是否能击中怪物,怪物是否可以暗杀处决
                {
                    if (hurtObject != null && hurtObject is MonsterPart part)
                    {
                        if (!part.monster.isAlive)//如果怪物死了跳过
                            continue;
                        Damage damage = _player.CreateDamage(this, hurtObject);
                        if (weakAttack)
                        {
                            damage.damage *= 0.5f;//虚弱状态攻击力减半
                            damage.lagDamage *= 0.5f;
                        }
                        if (attackType == AttackType.ChargeAttack)
                        {
                            damage.damage *= damageRatio;//蓄力攻击加成
                            damage.lagDamage *= damageRatio;
                        }
                        //GameDebug.LogErrorFormat("MammerPreAttack:{0};Damage:{1};weakAttack{2}", part.monster.hp, damage.damage, weakAttack);
                        //如果怪物没死
                        if ((part.monster.hp - damage.damage) >= 0)
                        {
                            //继续普通攻击
                            return false;
                        }
                        //处决判定
                        if (part.monster.canExc)
                        {
                            Expend();
                            ExcuteMonster(part.monster, damage);
                            return true;
                        }
                        return false;
                    }
                }
                //攻击动画击中检测
                if (hurtObject != null)
                {
                    Damage damage = _player.CreateDamage(this, hurtObject);
                    damage.dir = hit.point - firePoint.position;
                    if (weakAttack)
                    {
                        damage.damage *= 0.5f;//虚弱状态攻击力减半
                        damage.lagDamage *= 0.5f;
                    }
                    if (attackType == AttackType.ChargeAttack)
                    {
                        damage.damage *= damageRatio;//蓄力攻击加成
                        damage.lagDamage *= damageRatio;
                    }
                    if (hurtObject is MonsterPart part)
                    {
                        //GameDebug.LogErrorFormat("MammerAttack:{0};Damage:{1};weakAttack{2}", part.monster.hp, damage.damage, weakAttack);
                        EventCenter.Dispatch(EventKey.HitMonster, (part.monster.hp - damage.damage) > 0, part);
                    }
                    hurtObject.OnHurt(_player, damage);
                    needCheckAttack = false;
                    //HitSomething(hurtObject, hit, weaponType);
                    return false;
                }
            }
        }
        return false;
    }


    public override void AttackBtnDown()
    {
        attackBtnDown = true;
        weakAttack = _player.ContainStation(Player.Station.Weak);
        _animator.SetFloat("AttSpped", weakAttack ? weaponAttribute.shotInterval * 0.5f : weaponAttribute.shotInterval);
        pressedTime = Time.time;
        if (strongAttack && attackType != AttackType.None)
        {
            attackType = AttackType.StrongAttack;
        }
    }

    public override void AttackBtnUp()
    {
        if (!attackBtnDown)
            return;
        attackBtnDown = false;

        if (waitAttack)//正在攻击状态,只进行连续判定
        {
            //连击判定,普通攻击,连续攻击状态下进行判定
            if (attackType == AttackType.NormalAttak || attackType == AttackType.CombeAttack)
            {
                animSta = _animator.GetCurrentAnimatorStateInfo(0);
                if (attackCount > 0 && animSta.IsTag("CombeAttack"))
                {
                    CombeAttackTime combeTime = combeAttack[attackCount - 1];
                    if (animSta.normalizedTime >= combeTime.startTime & animSta.normalizedTime <= combeTime.endTime)
                    {
                        if (!AttackCheck(true))
                        {
                            attackType = AttackType.CombeAttack;
                            PlayAttackAnim(attackType);
                        }
                    }
                }
            }
            return;
        }
        //超重击
        if (attackType == AttackType.StrongAttack && weakAttack)
        {
            //超重击动画
            if (!AttackCheck(true))
            {
                waitAttack = true;
                PlayAttackAnim(attackType);
                GameDebug.Log("WeaponHammer:StrongAttack");
            }
            return;
        }
        //蓄力攻击
        if (attackType == AttackType.ChargeAttack)
        {
            //蓄力攻击动画
            if (!AttackCheck(true))
            {
                waitAttack = true;
                float pressedContinueTime = Time.time - pressedTime - chargeAttack;
                damageRatio = 1f + Mathf.Clamp01(pressedContinueTime / maxChargeTime);//1-2蓄力系数
                GameDebug.Log("WeaponHammer:ChargeAttack");
                PlayAttackAnim(attackType);
            }
            return;
        }
        //普通攻击
        if (!AttackCheck(true))
        {
            waitAttack = true;
            attackType = AttackType.NormalAttak;
            PlayAttackAnim(attackType);
            GameDebug.Log("WeaponHammer:NormalAttak");
        }
    }

    private void PlayAttackAnim(AttackType type)
    {
        switch (type)
        {
            case AttackType.NormalAttak:
                _player.AddStation(Player.Station.MeleeAttack);
                _animator.CrossFade(string.Format(NorAnimName, 0), 0.1f);
                break;
            case AttackType.StrongAttack:
                _player.AddStation(Player.Station.MeleeAttack);
                _animator.CrossFade(StrongAnimName, 0.1f);
                break;
            case AttackType.ChargeAttack:
            case AttackType.CombeAttack:
                _animator.SetTrigger("Attack");
                break;
            default:
                break;
        }
    }
    //消耗体力
    private void Expend()
    {
        if (!weakAttack)
            Player.player.ChangeStrength(-(attackType == AttackType.ChargeAttack ? attackExpend * 2 : attackExpend));
    }
    //处决怪物
    private void ExcuteMonster(AttackMonster monster,Damage damage)
    {
        _player.ExcuteMonster(monster,damage);
        GameDebug.Log("WeaponHammer:ExcuteMonster");
    }
    /// <summary>
    /// 重置武器状态
    /// </summary>
    private void ResetState()
    {
        needCheckAttack = false;
        attackCount = 0;
        attackType = AttackType.None;
        _animator.ResetTrigger("Attack");
    }
    //开始攻击判定,攻击动画Event调用
    public void MeleeAttackStart()
    {
        Expend();
        _animator.ResetTrigger("Attack");
        needCheckAttack = true;
        attackCount += 1;
    }
    //结束攻击判定,攻击动画Event调用
    public void MeleeAttackEnd()
    {
        needCheckAttack = false;
    }
    //拿出武器
    public async override void TakeOut()
    {
        ResetState();
        waitAttack = false;
        this.gameObject.OnActive(true);
        if (_player.ContainStation(Player.Station.Aim))
        {
            _player.RemoveStation(Player.Station.Aim);
        }
        if (_player.ContainStation(Player.Station.Running))
        {
            SetRunStation(true);
        }
        _player.AddStation(Player.Station.WeaponChanging);
        await Async.WaitforSecondsRealTime(0.3f);
        _player.RemoveStation(Player.Station.WeaponChanging);
    }
    //退出攻击动画,攻击结束动画Event调用
    public void OnAttackExit()
    {
        waitAttack = false;
        _player.RemoveStation(Player.Station.MeleeAttack);
        GameDebug.Log("WeaponHammer:AttackExit");
        ResetState();
    }

    public override void OnAnimStateEnter(string stateName)
    {
        if (stateName == "Dodge")
        {
            waitAttack = false;
            ResetState();
        }
    }
}
