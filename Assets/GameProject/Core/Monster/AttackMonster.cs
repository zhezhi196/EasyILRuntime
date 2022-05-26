/*普通攻击怪
 【腾讯文档】通用攻击怪 https://docs.qq.com/flowchart/DRFh0eXFDdmJNVnBI
 */

using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Module;
using RootMotion.Dynamics;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

[Serializable]
public struct SkinMeshInfo
{
    [HorizontalGroup(), HideLabel] public MonsterPartType type;
    [HorizontalGroup(), HideLabel] public MonsterDir partDir;
    [HorizontalGroup(), HideLabel] public SkinnedMeshRenderer skinRender;
}

public abstract class AttackMonster : Monster
{
    private float senserTime;

    [TabGroup("状态"), SerializeField] public float missPlayerTime;
    [TabGroup("状态"), SerializeField] protected float _seePlayerTime;
    // [TabGroup("状态"), SerializeField] protected FightLevel _fightLevel = FightLevel.Normal;
    //
    // public FightLevel fightLevel
    // {
    //     get { return _fightLevel; }
    //     set
    //     {
    //         _fightLevel = value;
    //         SendSwitchStationEvent();
    //     }
    // }

    [TabGroup("挂点")] public PuppetMaster puppetMaster;

    [LabelText("断肢")] [TabGroup("挂点"), SerializeField] [TabGroup("挂点")]
    public SkinMeshInfo[] skinRender;

    [TabGroup("信息")] //丢失掉玩家视野后多长时间会不跟踪玩家
    public float missPlayerDelayTime = 5;

    [TabGroup("信息")] public AfterAttackStyle afterStyle = AfterAttackStyle.Duizhi;

    [TabGroup("信息")] //看到玩家后多少秒转为战斗
    public virtual float seePlayerFightTime
    {
        get
        {
            if (eye.ContainPoint(0, Player.player.chasePoint))
            {
                return currentLevel.dbData.alertTime1;
            }

            return currentLevel.dbData.alertTime2;
        }
    }

    [TabGroup("信息")] public bool breakPart;
    [TabGroup("信息")] public bool smoothDampToDir = true;

    [TabGroup("挂点")] public MonsterPart[] monsterParts;
    [TabGroup("挂点")] public Transform excPoint;
    public override Vector3 faceDirection
    {
        get
        {
            if (simpleBtCtrl.currBehavior is Confrontation confront)
            {
                return confront.faceDirection;

            }
            else
            {
                return navmesh.steeringTarget - transform.position;
            }
        }
    }

    public override float stopMoveDistance
    {
        get
        {
            if (simpleBtCtrl != null && simpleBtCtrl.currBehavior is Confrontation) return 0.1f;
            return base.stopMoveDistance;
        }
    }
    private NavMoveCtrl _navMoveCtrl;

    public NavMoveCtrl navMoveCtrl
    {
        get
        {
            if (_navMoveCtrl == null)
            {
                _navMoveCtrl = GetAgentCtrl<NavMoveCtrl>();
            }

            return _navMoveCtrl;
        }
    }


    public override bool canAss
    {
        get
        {
            if (!ContainStation(MonsterStation.TimeLine))
            {
                if ((fightState != FightState.Fight || ContainStation(MonsterStation.Paralysis)) &&
                    Vector3.Dot(transform.forward, Player.player.chasePoint - transform.position) < 0 &&
                    canExc &&
                    toPlayerDistance <= Player.player.argConfig.assDistance * 1.5f &&
                    toPlayerAnger <= Player.player.argConfig.assAngle)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(assPoint.transform.position,
                        Player.player.eyePoint.position - assPoint.transform.position, out hit,
                        Player.player.argConfig.assDistance * 1.5f, MaskLayer.LookPointBlock))
                    {
                        if (hit.collider.gameObject.layer == MaskLayer.Playerlayer)
                        {
                            return true;
                        }
                    }
                }
            }


            return false;
        }
    }

    protected override void Start()
    {
        base.Start();
        EventCenter.Register<bool>(EventKey.PlayerHide, OnPlayerHide);
    }
    public void OnPlayerHide(bool isHide)
    {
        if (isHide)
        {
            if (isSeePlayer)
            {
                AddStation(MonsterStation.IsSeePlayHide);
            }
        }
        else
        {
            RemoveStation(MonsterStation.IsSeePlayHide);
        }
    }
    public void ExitConfrontation()
    {
        GetAgentCtrl<SimpleBehaviorCtrl>().SwitchBehavior(new Chase());
    }
    public override Damage OnHurt(Damage damage)
    {
        MonsterPart part = monsterParts.Find(fd => fd.partType == MonsterPartType.Body);
        return OnHurt(part, damage);
    }

    protected override void AddCtrl()
    {
        ctrlList.Add(new AnimatorCtrl(this));
        ctrlList.Add(new AudioCtrl(this));
        if (!creator.notNav)
        {
            ctrlList.Add(new NavMoveCtrl(this));
        }

        ctrlList.Add(new SimpleBehaviorCtrl(this));
        ctrlList.Add(new SkillCtrl(this));
        ctrlList.Add(new ParalysisCtrl(this));
        ctrlList.Add(new BuffCtrl(this));

    }

    public bool Ass(int damage)
    {
        if (currentLevel.dbData.assKill == 1)
        {
            return true;
        }
        else
        {
            var res = CanDead(damage);
            return res;
        }
    }

    public void AssEnd(int damage, int type)
    {
        //直接死
        MonsterPart prt = monsterParts.Find(part => part.partType == MonsterPartType.Head);
        if (currentLevel.dbData.assKill == 1 || CanDead(damage))
        {
            if (type == 0)
            {
                OnDead(prt, DeadType.Ass1, new Damage());
            }
            else if (type == 1)
            {
                OnDead(prt, DeadType.Ass2, new Damage());
            }
        }
        else
        {
            OnHurt(prt, new Damage() {damage = damage});
            PlayAnimation("ass_miss", 3);
        }
    }

    public void ExcEnd(int damage)
    {
        //直接死
        MonsterPart prt = monsterParts.Find(part => part.partType == MonsterPartType.Head);
        if (currentLevel.dbData.assKill == 1 || CanDead(damage))
        {
            OnDead(prt, DeadType.Exc, new Damage());
        }
        else
        {
            OnHurt(prt, new Damage() {damage = damage});
        }
    }

    protected void BreakPart(MonsterPart part)
    {
        if (part.partType != MonsterPartType.Body)
        {
            var skin = skinRender.Find(fd => fd.type == part.partType && fd.partDir == part.partDir);
            if (skin.skinRender != null)
            {
                skin.skinRender.gameObject.OnActive(false);
            }
            else
            {
                GameDebug.LogError("无法断指");
            }
            EffectPlay.Play("duanzhi_cixue", part.bloodParent ? part.bloodParent : part.transform);
        }
    }

    public override Damage OnHurt(ITarget target, Damage damage)
    {
        var result = base.OnHurt(target, damage);
        if (canHurt && !CanDead(damage))
        {
            Chase();
            ExitConfrontation();
            //GetAgentCtrl<SkillCtrl>().ResetSkillCD();

        }

        return result;
    }

    public void Week(float time, float attDown, float moveDown)
    {
        buffCtrl.AddBuff<WeekBuff>(BuffType.Restart, new BuffOption(time, null, 1), attDown, moveDown);

    }

    protected override void OnHearTarget(Vector3 arg2, object[] arg3)
    {
        if (canHear)
        {
            string tag = arg3[0].ToString();
            if (tag == "Chase")
            {
                Chase();
            }
            else if (tag == "Check")
            {
                GetAgentCtrl<SimpleBehaviorCtrl>().SwitchBehavior(new CheckTarget(), arg2);
                LookAtPoint(arg2);
            }
            else if (tag == "yuandiCheck")
            {
                GetAgentCtrl<SimpleBehaviorCtrl>().SwitchBehavior(new CheckTarget(), transform.position);
            }
        }
    }

    /// <summary>
    /// 追捕
    /// </summary>
    public virtual void Chase()
    {
        if (SwitchFightState(FightState.Fight))
        {
            RemoveStation(MonsterStation.Sleep);
            RemoveStation(MonsterStation.Paraller);
            RemoveStation(MonsterStation.Wait);
            StopCheck();
            GetAgentCtrl<SimpleBehaviorCtrl>().SwitchBehavior(new Chase());
        }
    }

    /// <summary>
    /// 丢失玩家
    /// </summary>
    protected virtual void LosePlayer()
    {
        if (fightState == FightState.Fight && !ContainStation(MonsterStation.Attack))
        {
            if (SwitchFightState(FightState.Alert))
            {
                missPlayerTime = missPlayerDelayTime;
                _seePlayerTime = 0;
                GetAgentCtrl<SimpleBehaviorCtrl>().SwitchBehavior(new CheckTarget(), memeryPosition);
                RemoveStation(MonsterStation.IsSeePlayHide);
            }
        }
    }

    public override bool SwitchFightState(FightState state)
    {
        if (state == FightState.Normal)
        {
            _seePlayerTime = 0;
            missPlayerTime = missPlayerDelayTime;
        }
        else if (state == FightState.Fight)
        {
            _seePlayerTime = seePlayerFightTime;
            missPlayerTime = 0;
        }

        return base.SwitchFightState(state);
    }


    protected override void Update()
    {
        base.Update();
        if (Mathf.Abs(navMoveCtrl.moveDirection.x) > 0.1f)
        {
            animator.SetFloat("moveDir", navMoveCtrl.moveDirection.x);
        }
        else
        {
            animator.SetFloat("moveDir", 0);
        }

        if (!isAlive) return;
        senserTime += TimeHelper.deltaTime;
        float triggerTime = 0.2f;
        if (senserTime >= triggerTime)
        {
            senserTime = senserTime - triggerTime;
            if (fightState == FightState.Fight && !isTired)
            {
                monsterCtrl.TrySensorMonster(this, currentLevel.dbData.roarRange);
            }
        }

        if (Player.player != null)
        {
            if (canSee)
            {
                if (isSeePlayer && !isTired)
                {
                    missPlayerTime = 0;
                    _seePlayerTime += GetDelatime(false);
                    _seePlayerTime = Mathf.Clamp(_seePlayerTime, 0, seePlayerFightTime + 1);
                    if (fightState == FightState.Normal)
                    {
                        //都现从normal到alert
                        SwitchFightState(FightState.Alert);
                    }

                    if (_seePlayerTime >= seePlayerFightTime)
                    {
                        if (fightState == FightState.Alert)
                        {
                            //从alert到fight
                            Chase();
                        }

                        memeryPosition = Player.player.chasePoint;
                    }
                }
                else
                {
                    if (!isParalysis)
                    {
                        missPlayerTime += GetDelatime(false);
                    }

                    missPlayerTime = Mathf.Clamp(missPlayerTime, 0, missPlayerDelayTime + 1);
                    if (missPlayerTime >= missPlayerDelayTime || Player.player.ContainStation(Player.Station.Stealth))
                    {
                        //从fight到normal
                        //长时间看不到玩家,移除fight状态
                        LosePlayer();
                    }
                    else
                    {
                        //在看不到玩家的若干秒,还会追踪到玩家的位置
                        memeryPosition = Player.player.chasePoint;
                        if (fightState == FightState.Alert)
                        {
                            SwitchFightState(FightState.Normal);
                        }
                    }

                    _seePlayerTime = 0;
                }
            }
        }
    }


    public override bool OnDead(ITarget target, DeadType deadType, Damage damage)
    {
        if (initStation == InitStation.Normal)
        {
            //BattleController.GetCtrl<ProgressCtrl>().TryNextProgress(creator.id, CompleteProgressPredicate.KillMonster);
            if (waitCoroutine != null)
            {
                StopCoroutine(waitCoroutine);
            }

            BreakAction();
            GetAgentCtrl<BuffCtrl>().ClearBuff();
            initStation = InitStation.PreDead;
            if (creator.dropId != 0)
            {
                drop = new Drop(creator.dropId);
            }
            else
            {
                drop = new Drop(currentLevel.dbData.dropId);
            }

            string deadAnimation = "die_body";
            MonsterPart part = target as MonsterPart;
            if (deadType == DeadType.Ass1)
            {
                deadAnimation = "die_ass1";
            }
            else if (deadType == DeadType.Ass2)
            {
                deadAnimation = "die_ass2";
            }
            else if (deadType == DeadType.Exc)
            {
                deadAnimation = "die_exc";
            }
            // else
            // {
            //     if (part is HeadPart)
            //     {
            //         deadAnimation = "die_head";
            //     }
            // }

            if (breakPart && deadType != DeadType.Ass1 && deadType != DeadType.Ass2 && deadType != DeadType.Exc)
            {
                //if ((!Channel.isChina || Channel.isIOS))
                {
                    BreakPart(part);
                }
            }


            var deadStation = PlayAnimation(deadAnimation, 3, AnimationFlag.ForceFadeOut | AnimationFlag.NotAutoOut);
            if (deadStation != null)
            {
                deadStation.onStationChange +=
                    st =>
                    {
                        if (st.isComplete)
                        {
                            drop.GetDropBag(creator.matchInfo, bag => { bag.transform.position = transform.position; });
                            initStation = InitStation.Dead;
                            if (puppetMaster != null)
                            {
                                if (deadType != DeadType.Shot)
                                {
                                    puppetMaster.state = PuppetMaster.State.Frozen;
                                    puppetMaster.mode = PuppetMaster.Mode.Disabled;
                                    puppetMaster.gameObject.OnActive(false);
                                }
                            }
                        }
                    };
            }
            for (int i = 0; i < ctrlList.Count; i++)
            {
                ctrlList[i].OnAgentDead();
            } 

            StopAllAudio();
            //Push(transform.position - Player.player.chasePoint, 1);
            creator.TrySendEvent(SendEventCondition.Dead);
            BattleController.Instance.TryNextNode(PredicateType.KillMonster, creator.id.ToString());
            BattleController.GetCtrl<MonsterCtrl>().TryRemoveMonster(this);
            OnMonsterDead?.Invoke(this, part, deadType, damage);
            SetLayer(true);
            StartCoroutine(RongJie());
            BattleController.GetCtrl<MonsterCtrl>().DeadMonster++;
            AnalyticsEvent.SendEvent(AnalyticsType.KillMonster, creator.id.ToString());

            if (puppetMaster != null)
            {
                if (deadType == DeadType.Shot)
                {
                    puppetMaster.state = PuppetMaster.State.Dead;
                    puppetMaster.mode = PuppetMaster.Mode.Active;
                    DOTween.To(() => puppetMaster.mappingWeight, value => puppetMaster.mappingWeight = value, 2, Player.player.currentWeapon.weaponArgs==null?0.1f:Player.player.currentWeapon.weaponArgs.animationToRagdollTime);
                    if (Player.player.currentWeapon.weaponArgs != null)
                    {
                        StartCoroutine(WaitAddForce(part, damage));
                    }
                }
            }

            return true;
        }

        return false;
    }

    private IEnumerator WaitAddForce(MonsterPart part,Damage damage)
    {
        yield return new WaitForSeconds(Player.player.currentWeapon.weaponArgs.forceDelay);
        part.rig.AddForceAtPosition(damage.dir * damage.force, damage.hitPoint);
        yield return new WaitForSeconds(puppetMaster.stateSettings.killDuration);
        puppetMaster.state = PuppetMaster.State.Frozen;
        //puppetMaster.state = PuppetMaster.State.Frozen;
        puppetMaster.transform.GetChild(0).gameObject.OnActive(false);
    }


    public SkinnedMeshRenderer GetPartSkin(MonsterPartType type)
    {
        return skinRender.Find(fd => fd.type == type).skinRender;
    }


    public override void ResetToBorn()
    {
        if (_initStation > InitStation.Normal)
        {
            for (int i = 0; i < skinRender.Length; i++)
            {
                skinRender[i].skinRender.material.SetFloat("_Alpha", 0);
            }
        }

        base.ResetToBorn();
    }

    protected virtual IEnumerator RongJie()
    {
        //20220110 潘宇要求尸体消失时间改为3秒
        yield return new WaitForSeconds(rongjieWaitSecond);
        WaitForEndOfFrame frame = new WaitForEndOfFrame();
        BeginRongjie();
        float rongjiePower = 0;
        while (rongjiePower <= 1)
        {
            yield return frame;
            rongjiePower += TimeHelper.deltaTime;
            for (int i = 0; i < skinRender.Length; i++)
            {
                skinRender[i].skinRender.material.SetFloat("_Alpha", rongjiePower);
            }
        }

        if (deadDestroy)
        {
            AssetLoad.Destroy(gameObject);
        }
        else
        {
            gameObject.OnActive(false);
        }
    }

    protected virtual void BeginRongjie()
    {
    }

    public bool HurtPlayer(PlayerDamage damage, Bounds damageBounds, Transform damagePoint)
    {
        for (int i = 0; i < Player.player.PlayerParts.Count; i++)
        {
            if (Tools.ContainRotateBounds(damageBounds, damagePoint, Player.player.PlayerParts[i].transform.position))
            {
                Player.player.PlayerParts[i].OnHurt(damage);
                if (i == 0)
                {
                    return true;
                }
            }
        }

        return false;
    }
    
    public bool HurtPlayer(PlayerDamage damage, float raduis,Vector3 damagePoint)
    {
        for (int i = 0; i < Player.player.PlayerParts.Count; i++)
        {
            DrawTools.DrawCircle(damagePoint, Quaternion.LookRotation(transform.up), raduis, Color.green);
            if (Player.player.PlayerParts[i].transform.position.Distance(damagePoint) <= raduis)
            {
                Player.player.PlayerParts[i].OnHurt(damage);
                if (i == 0)
                {
                    return true;
                }
            }
        }

        return false;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        EventCenter.UnRegister<bool>(EventKey.PlayerHide, OnPlayerHide);

    }
#if UNITY_EDITOR

    protected override void EditorInit()
    {
        base.EditorInit();
        var tempskinRender = transform.GetComponentsInChildren<SkinnedMeshRenderer>();
        skinRender = new SkinMeshInfo[tempskinRender.Length];
        for (int i = 0; i < skinRender.Length; i++)
        {
            skinRender[i] = new SkinMeshInfo() {skinRender = tempskinRender[i]};
        }

        puppetMaster = transform.GetComponentInChildren<PuppetMaster>();

        //查找部位,部位加碰撞体
        HeadPart[] headPart = AddMonsterPart<HeadPart>("Bip001 Head");
        ArmPart[] Larm = AddMonsterPart<ArmPart>("L UpperArm", "L Forearm", "L Hand");
        ArmPart[] Rarm = AddMonsterPart<ArmPart>("R UpperArm", "R Forearm", "R Hand");
        LegPart[] Lleg = AddMonsterPart<LegPart>("L Thigh", "L Calf", "L Foot");
        LegPart[] Rleg = AddMonsterPart<LegPart>("R Thigh", "R Calf", "R Foot");
        BodyPart[] bodyParts = AddMonsterPart<BodyPart>("Pelvis", "Spine1");
        List<MonsterPart> part = new List<MonsterPart>();
        for (int i = 0; i < Larm.Length; i++)
        {
            Larm[i].partDir = MonsterDir.Left;
        }

        for (int i = 0; i < Rarm.Length; i++)
        {
            Larm[i].partDir = MonsterDir.Right;
        }

        for (int i = 0; i < Lleg.Length; i++)
        {
            Larm[i].partDir = MonsterDir.Left;
        }

        for (int i = 0; i < Rleg.Length; i++)
        {
            Larm[i].partDir = MonsterDir.Right;
        }

        part.AddRange(headPart);
        part.AddRange(Larm);
        part.AddRange(Rarm);
        part.AddRange(Lleg);
        part.AddRange(Rleg);
        part.AddRange(bodyParts);
        monsterParts = part.ToArray();
        if (puppetMaster != null)
        {
            Transform[] wristCheck = puppetMaster.transform.GetNameChildren("Twist", true);
            for (int i = 0; i < wristCheck.Length; i++)
            {
                var check = wristCheck[i].gameObject.AddOrGetComponent<TwistCheck>();
                check.target = check.transform.parent.GetChild(0);
            }
        }

        
        //加音效
        List<AgentAudio> temp = new List<AgentAudio>();
        if (headPart.Length > 0)
        {
            var t = headPart[0].gameObject.AddOrGetComponent<AgentAudio>();
            t.key = "Head";
            temp.Add(t);
            var t2 = transform.gameObject.AddOrGetComponent<AgentAudio>();
            temp.Add(t2);
            t2.key = "Foot";
            var t3 = transform.gameObject.AddOrGetComponent<AgentAudio>();
            temp.Add(t3);
            t2.key = "Attack";
            _agentAudio = temp.ToArray();
        }

        assPoint = transform.FindOrNew("assPoint");
        excPoint = transform.FindOrNew("excPoint");
    }

    private T[] AddMonsterPart<T>(params string[] name) where T : MonsterPart
    {
        List<T> result = new List<T>();

        for (int i = 0; i < name.Length; i++)
        {
            List<Transform> parts = new List<Transform>();
            for (int j = 0; j < name.Length; j++)
            {
                parts = GetNameMusical(name[j], ref parts);
            }

            for (int j = 0; j < parts.Count; j++)
            {
                T R = parts[j].gameObject.AddOrGetComponent<T>();
                R.EditorInit(this);
                result.Add(R);
            }
        }

        return result.ToArray();
    }

    public override void StopAttack()
    {
        
    }
    private List<Transform> GetNameMusical(string name, ref List<Transform> temp)
    {
        if (puppetMaster != null)
        {
            for (int i = 0; i < puppetMaster.muscles.Length; i++)
            {
                if (puppetMaster.muscles[i].joint.gameObject.name.Contains(name))
                {
                    temp.Add(puppetMaster.muscles[i].joint.transform);
                }
            }
        }

        return temp;
    }
    
    [Button]
    public virtual void SetPart()
    {
        for (int i = 0; i < monsterParts.Length; i++)
        {
            if (monsterParts[i].gameObject.name.Contains(" L "))
            {
                monsterParts[i].partDir = MonsterDir.Left;
            }
            else if (monsterParts[i].gameObject.name.Contains(" R "))
            {
                monsterParts[i].partDir = MonsterDir.Right;

            }
            else
            {
                monsterParts[i].partDir = MonsterDir.Mid;
            }
        }
    }
#endif
}