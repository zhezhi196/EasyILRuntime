using System;
using System.Collections;
using System.Collections.Generic;
using GameBuff;
using Module;
using SimBehavior;
using Sirenix.OdinInspector;
using StationMachine;
using UnityEngine;
using UnityEngine.AI;
using Rampage = SimBehavior.Rampage;
using Transform = UnityEngine.Transform;

public enum PlayerPosture
{
    Normal,
    NoAvoid,
    CanAvoid,
    Avoid,
}
public class Player : Agent, IStateMechineObject, IAgentAudioObject, IBuffObject, INavMoveObject, ISkillObject, IStationObject<PlayerStation>,ISimpleBehaviorObject,ISensorTarget
{
    #region GameIn

    private static Player _player;
    private static IBasement _baseMent;
    private bool _isChangeAtt;
    protected IPlayerTarget _target;

    public static Player player
    {
        get
        {
            if (_player == null) _player = BattleController.GetCtrl<PlayerCtrl>().player;
            return _player;
        }
    }

    public Vector3 center
    {
        get
        {
            return transform.position + new Vector3(0, 1, 0);
        }
    }

    public override bool isVisiable => true;

    public static IBasement baseMent
    {
        get
        {
            if (_baseMent == null)
            {
                var all = BattleController.GetCtrl<PlayerCtrl>().baseMent;
                for (int i = 0; i < all.Count; i++)
                {
                    if (all[i] is IBasement result)
                    {
                        _baseMent = result;
                        break;
                    }
                }
            }

            return _baseMent;
        }
    }

    public bool isChangeAtt
    {
        get { return _isChangeAtt; }
        set
        {
            _isChangeAtt = value;
            if (value)
            {
                onChangedAtt?.Invoke();
            }
        }
    }

    public bool canHurt
    {
        get { return !ContainStation(PlayerStation.Transform); }
    }

    #endregion

    [SerializeField, TabGroup("状态")] private PlayerStation _playerStation;
    [SerializeField, TabGroup("状态")] private FloatField _anger;

    public float angryTime;
    [TabGroup("状态")] public int levelIndex;

    [TabGroup("状态")]
    public Morphology currLevel
    {
        get { return morphology[levelIndex]; }
    }

    [TabGroup("状态"), SerializeField,LabelText("最终属性")] private List<PlayerSkillInstance> skill;
    [TabGroup("状态"), SerializeField,LabelText("最终属性")] private GameAttribute _finalAttribute;


    [TabGroup("状态"), SerializeField] public float toTargetDistance;
    [TabGroup("状态"), SerializeField] public float toTargetAngle;
    [TabGroup("状态")] public PlayerPosture posture;

    public float anger
    {
        get { return _anger; }
        set
        {
            _anger = new FloatField(value);
            onChangeAnger?.Invoke(value);
        }
    }

    [SerializeField, TabGroup("挂点")] private NavMeshAgent _navMesh;
    [SerializeField, TabGroup("挂点")] private AgentAudio[] _audio;
    [SerializeField, TabGroup("挂点")] public PlayerCamera camera;
    [SerializeField, TabGroup("挂点")] public AnimationCallback _animationCallback;
    
    [LabelText("行走距离"), TabGroup("信息")] public float walkDistance = 1;
    [SerializeField,LabelText("选择范围")]
    public Bounds selectBound;
    [TabGroup("挂点")]
    public Transform selectTransfor;

    [LabelText("形态"), TabGroup("信息")] public Morphology[] morphology;
    public virtual IPlayerTarget target => _target;
    public GameAttribute finalAtt
    {
        get
        {
            if (isChangeAtt)
            {
                _finalAttribute = GetAttribute(levelIndex);
                isChangeAtt = false;
            }

            return _finalAttribute;
        }
    }
    

    
    public PlayerStation station => _playerStation;
    public Animator animator => currLevel.animator;
    public AnimationCallback animationCallback => _animationCallback;
    public bool canReceiveEvent => isAlive;
    public NavMeshAgent navmesh => _navMesh;
    public override AgentType type => currLevel.agentType;
    public float rotateToMove => 90;
    public Vector3 moveDirection => navmesh.steeringTarget - transform.position;

    public override string logName => "Player";

    public bool canReleaseSkill
    {
        get
        {
            return isAlive && target != null && !ContainStation(PlayerStation.Attack) && !ContainStation(PlayerStation.Transform);
        }
    }
    public float stopMoveDistance
    {
        get
        {
            if (isAlive)
            {
                var skillCtrl = GetAgentCtrl<SkillCtrl>();
                if (skillCtrl != null && skillCtrl.readyRelease is AttackSkill atsk)
                {
                    return atsk.maxHurtDistance;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }
    }


    public float rotateSpeed
    {
        get
        {
            if (moveSpeed == 0) return 0;
            return finalAtt.rotateSpeed;
        }
    }

    public float moveSpeed
    {
        get
        {
            if (!isAlive || ContainStation(PlayerStation.Transform) || ContainStation(PlayerStation.Attack) ||
                ContainStation(PlayerStation.Roar)) return 0;
            
            return finalAtt.moveSpeed;
        }
    }



    public bool canCtrlPlayer
    {
        get { return isAlive && !ContainStation(PlayerStation.Rampage); }
    }

    public bool canTransform
    {
        get { return !ContainStation(PlayerStation.Transform); }
    }

    public bool isSenserable
    {
        get { return initStation == InitStation.Normal; }
    }

    public event Action<ISensorTarget> onDisable;

    public event Action<float> onChangeAnger;
    public event Action onChangedAtt;
    public event Action<PlayerStation> OnAddStation; 
    public event Action<PlayerStation> OnRemoveStation;
    public event Action<PlayerPosture> onSwitchPosture; 

    public void Creat(PlayerCreator creator, Action callback)
    {
        if (initStation == InitStation.UnActive)
        {
            _initStation = InitStation.OnlyCreat;
            transform.position = creator.transform.position;
            transform.rotation = creator.transform.rotation;
            AddCtrl();
            for (int i = 0; i < morphology.Length; i++)
            {
                levelIndex = i;
                morphology[i].Creat(this);
            }

            //ctrlList.ForEach(ct => ct.OnCreat());
            Voter voter = new Voter(SkillSlot.slots.Count, callback);
            for (int i = 0; i < SkillSlot.slots.Count; i++)
            {
                var tempSlot = SkillSlot.slots[i];
                if (tempSlot.isGet)
                {
                    if (tempSlot.dbData.type == 0)
                    {
                        tempSlot.LoadSkillInstance(lst =>
                        {
                            skill = lst;
                            voter.Add();
                        });
                    }
                    else
                    {
                        voter.Add();
                    }
                }
                else
                {
                    voter.Add();
                }
            }
        }
    }

    protected override void AddCtrl()
    {
        ctrlList.Add(new StateMachineCtrl(this, new Jump(0, "tiao"), new Attack1(0, "atk1"), new Attack2(0, "atk2"),
            new Attack3(0, "atk3"), new Attack4(0, "atk4"), new Tiaoxin(0, "tiaoxin"),
            new StationMachine.Transform(0, "transform"), new StationMachine.Rampage(0, "bigshow"),
            new Dead(0, "shuimian"), new GeDang(0, "gedang"), new FanJi(0, "fanji")));
        ctrlList.Add(new PlayerSkillCtrl(this));
        ctrlList.Add(new NavMoveCtrl(this));
        ctrlList.Add(new PlayerBuffCtrl(this));
        ctrlList.Add(new SimpleBehaviorCtrl(this));
    }
    

    public bool SwitchTarget(ITarget tar)
    {
        if (this.target != tar)
        {
            this._target = tar as IPlayerTarget;
            OnSwitchStation();
            animator.SetBool("hasTarget", this.target != null);
            return true;
        }

        return false;
    }

    public void Born()
    {
        if (initStation >= InitStation.OnlyCreat)
        {
            _initStation = InitStation.Normal;
            ResetBornValue();
            var skillCtrl = GetAgentCtrl<SkillCtrl>();
            skillCtrl.SetSkill(skill.ToArray());
            OnTransform(0, true);
            StartFight();
            SetLog();
        }
    }

    public void HurtMonster(Monster monster, params Damage[] damage)
    {
        for (int i = 0; i < damage.Length; i++)
        {
            monster.OnHurt(damage[i]);
        }
    }

    public void SwitchPosture(PlayerPosture posture)
    {
        if (this.posture != posture)
        {
            this.posture = posture;
            OnSwitchStation();
            onSwitchPosture?.Invoke(posture);
        }
    }

    private void ResetBornValue()
    {
        _hp = finalAtt.hp;
    }

    public bool SwitchStation(PlayerStation station)
    {
        if (this._playerStation != station)
        {
            this._playerStation = station;
            OnSwitchStation();
            return true;
        }

        return false;
    }

    public bool ContainStation(PlayerStation station)
    {
        return (this.station & station) == station;
    }

    public bool AddStation(PlayerStation station)
    {
        if (ContainStation(station)) return false;
        this._playerStation = this.station | station;
        if ((station & PlayerStation.Jump) != 0)
        {
            navmesh.enabled = false;
        }
        OnSwitchStation();
        LogFormat("添加状态{0}", station);
        OnAddStation?.Invoke(station);
        return true;
    }

    public bool RemoveStation(PlayerStation station)
    {
        if (!ContainStation(station)) return false;
        this._playerStation = this.station & ~station;
        if ((station & PlayerStation.Jump) != 0)
        {
            navmesh.enabled = true;
        }
        OnSwitchStation();
        LogFormat("移除状态{0}", station);
        OnRemoveStation?.Invoke(station);
        return true;
    }

    public void PreAttack(int attack, params Damage[] damge)
    {
        for (int i = 0; i < morphology.Length; i++)
        {
            var mm = morphology[i];
            for (int j = 0; j < mm.attCollider.Length; j++)
            {
                mm.attCollider[j].PreAttack(attack, damge);
            }
        }
    }

    /// <summary>
    /// 属性加成:(玩家变身属性 * (1 + 天赋属性) + 玩家武器属性 + 玩家技能基础属性) * (1 + 玩家变身增长属性 + 玩家技能增长属性 + buff属性)
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public GameAttribute GetAttribute(int index)
    {
        BuffCtrl ctrl = GetAgentCtrl<BuffCtrl>();
        GameAttribute buffAtt = new GameAttribute(0);
        for (int i = 0; i < ctrl.buffList.Count; i++)
        {
            if (ctrl.buffList[i] is GameAttributeBuff ga)
            {
                buffAtt += ga.Attribute;
            }
        }

        GameAttribute baseAttribute = morphology[index].finalAttribute + GetSkillAttribute(0);
        //这里规定所有的buff的属性必须要是成长属性
        GameAttribute growupAttribute = morphology[index].growupAttribute + GetSkillAttribute(1) + buffAtt;
        return baseAttribute * (1 + growupAttribute);
    }
    
    public GameAttribute GetSkillAttribute(int type)
    {
        GameAttribute result = new GameAttribute(0);
        for (int i = 0; i < SkillSlot.slots.Count; i++)
        {
            var tempSlot = SkillSlot.slots[i];
            if (tempSlot.isGet)
            {
                if(tempSlot.dbData.type != 0)
                {
                    for (int j = 0; j < tempSlot.skills.Count; j++)
                    {
                        if (tempSlot.skills[j].dbData.type == 3)
                        {
                            if (tempSlot.skills[j].dbData.weapon == currLevel.weapon.id)
                            {
                                var temBaseAtt = tempSlot.dbData.plus * AttributeHelper.GetAttributeByType(tempSlot.skills[j].dbData)[type];
                                result += temBaseAtt;
                            }
                        }
                    }
                }
            }
        }

        return result;
    }

    public void ChangeAngerDetatime(bool isUp)
    {
        if (isAlive && !ContainStation(PlayerStation.Transform))
        {
            float tempAnger = anger;
            if (isUp)
            {
                float upSpeed = Mathf.Clamp(finalAtt.angerUpSpeed, 0, float.MaxValue);
                tempAnger += upSpeed * GetDelatime(false);
            }
            else
            {
                float downSpeed = Mathf.Clamp(finalAtt.angerDownSpeed, 0, float.MaxValue);
                tempAnger -= downSpeed * GetDelatime(false);
            }

            float maxAnger = finalAtt.anger;
            tempAnger = Mathf.Clamp(tempAnger, 0, maxAnger);

            if (tempAnger >= maxAnger)
            {
                //如果玩家处于第一状态
                if (levelIndex == 0)
                {
                    OnTransform(1);
                }
                else
                {
                    //玩家狂暴
                    Rampage();
                }
            }
            else if (tempAnger <= 0)
            {
                //玩家恢复人形
                OnTransform(0);
            }
            else
            {
                anger = tempAnger;
            }
        }
    }

    /// <summary>
    /// 玩家狂暴
    /// </summary>
    public void Rampage()
    {
        if (levelIndex != 2 && AddStation(PlayerStation.Transform))
        {
            GetAgentCtrl<SimpleBehaviorCtrl>().SwitchBehavior(new Rampage());
            levelIndex = 2;
            GetAgentCtrl<StateMachineCtrl>().Play<StationMachine.Rampage>().onEnd += complete =>
            {
                if (complete)
                {
                    RemoveStation(PlayerStation.Transform);
                }
            };
        }

        angryTime -= GetDelatime(false);
        if (angryTime <= 0.5f)
        {
            GetAgentCtrl<SimpleBehaviorCtrl>().SwitchBehavior(null);
            OnTransform(0);
        }
    }

    public void OnTransform(int index)
    {
        OnTransform(index, false);
    }

    public void OnTransform(int index, bool born)
    {
        if (index != levelIndex && canTransform && AddStation(PlayerStation.Transform))
        {
            SkillCtrl skillCtrl = GetAgentCtrl<SkillCtrl>();
            skillCtrl.RemoveSkill(currLevel.defaultWeapon.normalAttack.name);
            var lastLevel = currLevel;
            levelIndex = index;
            GetAgentCtrl<StateMachineCtrl>().BeforeChangeContrller();
            currLevel.OnEnter(lastLevel);
            angryTime = GetAttribute(2).angryTime;
            //ctrlList.ForEach(ct => ct.OnTransform(index));
            SwitchMoveStyle(MoveStyle.Idle);
            SwitchPosture(PlayerPosture.Normal);
            if (!born)
            {
                GetAgentCtrl<StateMachineCtrl>().Play<StationMachine.Transform>().onEnd += complete =>
                {
                    RemoveStation(PlayerStation.Transform);
                };
            }
            else
            {
                RemoveStation(PlayerStation.Transform);
            }

            skillCtrl.AddSkill(Instantiate(currLevel.defaultWeapon.normalAttack));

            if (levelIndex == 0)
            {
                anger = new FloatField(0);
            }
            else
            {
                anger = new FloatField(finalAtt.anger * 0.5f);
            }
            isChangeAtt = true;
        }
    }

    public AgentAudio GetAudio(string name)
    {
        for (int i = 0; i < _audio.Length; i++)
        {
            if (_audio[i].name == name)
            {
                return _audio[i];
            }
        }

        return null;
    }

    public float GetAudioVolume(AgentAudio tar)
    {
        throw new NotImplementedException();
    }

    public bool GetAudioActive(AgentAudio tar)
    {
        throw new NotImplementedException();
    }


    public void OnAddBuff(Buff buff)
    {
        if (buff is GameAttributeBuff)
        {
            isChangeAtt = true;
        }
    }

    public void OnRemoveBuff(Buff buff)
    {
        if (buff is GameAttributeBuff)
        {
            isChangeAtt = true;
        }
    }

    public IActiveSkill RefreshReadySkill()
    {
        if (target != null)
        {
            var skillCtrl = GetAgentCtrl<SkillCtrl>();
            
            for (int i = 0; i < skillCtrl.allSkill.Count; i++)
            {
                if (skillCtrl.allSkill[i] is IActiveSkill active && active.isWanted)
                {
                    return active;
                }
            }
        }

        return null;
    }

    public void OnReleasingSkill(ISkill skill)
    {
        OnSwitchStation();
    }

    public bool SwitchMoveStyle(MoveStyle style)
    {
        if (isAlive && this.moveStyle != style)
        {
            this.moveStyle = style;
            
            GetAgentCtrl<StateMachineCtrl>().SetFloat("moveStyle", (int) style, 0.2f);
            OnSwitchStation();
            LogFormat("切换移动样式{0}", style);
            return true;
        }

        return false;
    }

    public void BreakAction()
    {
        if (!ContainStation(PlayerStation.Roar) && !ContainStation(PlayerStation.Jump))
        {
            SkillCtrl skillCtrl = GetAgentCtrl<SkillCtrl>();
            if (skillCtrl.isBusy)
            {
                skillCtrl.BreakSkill(BreakSkillFlag.BreakAction | BreakSkillFlag.WithAnimation);
            }
        }
    }

    public void Move(Vector3 dir)
    {
        BreakAction();
        Vector3 newDir = GetTrueDir(dir);
        Vector3 moveTarget;
        moveTarget = transform.position + walkDistance * newDir;
        MoveToPoint(moveTarget);
        SwitchTarget(null);
        SwitchPosture(PlayerPosture.Normal);
    }

    public void MoveToPoint(Vector3 point)
    {
        NavMoveCtrl moveCtrl = GetAgentCtrl<NavMoveCtrl>();

        if (moveCtrl.MoveTo(point, null))
        {
            SwitchMoveStyle(MoveStyle.Run);
        }
    }

    public List<Monster> HurtMonsterRange(Predicate<Monster> predicate, Damage damage)
    {
        List<Monster> result = new List<Monster>();
        
        var ctrl = BattleController.GetCtrl<MonsterCtrl>();
        for (int i = 0; i < ctrl.exitMonster.Count; i++)
        {
            var dis = ctrl.exitMonster[i];
            if (dis.isAlive && predicate.Invoke(dis))
            {
                damage = Damage.RandomCritical(damage, finalAtt);
                HurtMonster(dis, damage);
                result.Add(dis);
            }
        }

        return result;
    }

    public void StopMove()
    {
        if (SwitchMoveStyle(MoveStyle.Idle))
        {
            GetAgentCtrl<NavMoveCtrl>().StopMove();
        }
    }

    public Monster[] GetMonster(Vector3 dir, int count)
    {
        if (count == 1) return new[] {GetMonster(dir, selectBound)};
        List<Monster> resujt = new List<Monster>();
        BattleController.GetCtrl<MonsterCtrl>().exitMonster.Sort((a, b) => a.toPlayerDistance.CompareTo(b.toPlayerDistance));
        var tempTarget = GetMonster(dir, selectBound);
        if (tempTarget != null) resujt.Add(tempTarget);
        return resujt.ToArray();
    }

    public Monster GetMonster(Vector3 direction)
    {
        return GetMonster(direction, selectBound);
    }

    public Monster GetMonster(Vector3 dir, Bounds bounds)
    {
        Vector3 trueDir = GetTrueDir(dir);
        if (fightState == FightState.Fight && Vector3.Angle(trueDir, transform.forward) > 45)
        {
            Debug.DrawRay(center, trueDir, Color.red);
            return null;
        }
        else
        {
            //MonsterCtrl monsterCtrl = BattleController.GetCtrl<MonsterCtrl>();
            //selectTransfor.forward = trueDir;
            //Vector3 selectPoint = trueDir * bounds.extents.z + transform.position;
            //Quaternion rotation = Quaternion.LookRotation(trueDir);
            //DrawTools.DrawCircle(selectPoint, rotation, 1, Color.magenta);
            Ray ray = new Ray(center, trueDir);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 20, 11))
            {
                var mon = hit.collider.GetComponent<Monster>();
                if (mon != null)
                {
                    Debug.DrawRay(center, trueDir, Color.green);
                    return mon;
                }
            }
            // Monster cur = null;
            // for (int i = 0; i < monsterCtrl.exitMonster.Count; i++)
            // {
            //     var temp = monsterCtrl.exitMonster[i];
            //     if (temp.isVisiable)
            //     {
            //         if (Tools.ContainRotateBounds(bounds, selectPoint, rotation, temp.transform.position))
            //         {
            //             if (cur == null || cur.toPlayerDistance > temp.toPlayerDistance)
            //             {
            //                 cur = temp;
            //             }
            //         }
            //     }
            // }
            //
            // return cur;
        }
        Debug.DrawRay(center, trueDir, Color.red);
        return null;

    }

    protected override void Update()
    {
        if (target != null)
        {
            toTargetDistance = transform.position.Distance(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z));
            Vector3 dir = target.transform.position - transform.position;
            toTargetAngle = Vector3.Angle(dir, new Vector3(transform.forward.x, dir.y, transform.forward.z));
        }
        else
        {
            toTargetDistance = float.MaxValue;
            toTargetAngle = 360;
        }
        base.Update();
    }

    public Vector3 GetTrueDir(Vector3 dir)
    {
        return new Vector3(dir.x, 0, dir.y);
    }
    
    
    private void SwitchNormalPosture()
    {
        SwitchPosture(PlayerPosture.Normal);
        animator.SetInteger("posture", 0);
    }
    
    public void PreAvoid()
    {
        if (toTargetAngle <= 20 && posture == PlayerPosture.Normal)
        {
            CancelInvoke("SwitchNormalPosture");
            SwitchPosture(PlayerPosture.CanAvoid);
            Invoke("SwitchNormalPosture", 0.6f);
        }
    }

    public void Avoid()
    {
        if (posture == PlayerPosture.CanAvoid)
        {
            GetAgentCtrl<SkillCtrl>().BreakSkill(BreakSkillFlag.WithAnimation);
            CancelInvoke("SwitchNormalPosture");
            SwitchPosture(PlayerPosture.Avoid);
            GetAgentCtrl<StateMachineCtrl>().Play<GeDang>();
            Invoke("SwitchNormalPosture", 0.6f);
        }
    }
    public void CounterAttack()
    {
        if (posture == PlayerPosture.Avoid)
        {
            CancelInvoke("SwitchNormalPosture");
            SwitchPosture(PlayerPosture.Normal);
            GetAgentCtrl<StateMachineCtrl>().Play<FanJi>();
            MonsterCtrl monster = BattleController.GetCtrl<MonsterCtrl>();

            Invoke("SwitchNormalPosture", 0.6f);
        }
    }

    public MonsterDamage OnHurt(MonsterDamage damage)
    {
        if (initStation == InitStation.Normal)
        {
            if (canHurt && damage.damage > 0)
            {
                GameDebug.LogError("玩家受伤");
                for (int i = 0; i < ctrlList.Count; i++)
                {
                    if (ctrlList[i] is IMonsterHurtObject hurtObj)
                    {
                        damage = hurtObj.OnHurt(damage);
                    }
                }

                if (damage.damage > 0)
                {
                    float currhp = hp;
                    currhp = Mathf.Clamp(currhp - damage.damage, 0, currhp);
                    _hp = new FloatField(currhp);
                    if (currhp <= 0)
                    {
                        PreDead();
                    }
                }
            }

            return damage;
        }
        else if (initStation == InitStation.PreDead)
        {
            OnDead();
            return MonsterDamage.maxValue;
        }

        return MonsterDamage.zero;
    }

    public void PreDead()
    {
        _initStation = InitStation.PreDead;
        
    }

    public void OnDead()
    {
        
    }

    public override T GetAgentCtrl<T>()
    {
        for (int i = 0; i < ctrlList.Count; i++)
        {
            if (ctrlList[i] is T res)
            {
                return res;
            }
        }

        return default;
    }

    // public override bool SwitchTarget(ITarget tar)
    // {
    //     if (base.SwitchTarget(tar))
    //     {
    //         if (levelIndex < 2)
    //         {
    //             GetAgentCtrl<SimpleBehaviorCtrl>().SwitchBehavior(null);
    //         }
    //         return true;
    //     }
    //
    //     return false;
    // }


    public void StartFight()
    {
        var monsteCtrl = BattleController.GetCtrl<MonsterCtrl>().exitMonster;
        var activeMonster = monsteCtrl.FindAll(fd => fd.isVisiable);
        var monster = activeMonster.Min((a, b) => a.toPlayerDistance.CompareTo(b.toPlayerDistance));
        SwitchTarget(monster);
        GetAgentCtrl<SimpleBehaviorCtrl>().SwitchBehavior(new PlayerMove());
        if (currLevel.agentType == AgentType.Human)
        {
            SwitchPosture(PlayerPosture.NoAvoid);
            CancelInvoke("SwitchNormalPosture");
            Invoke("SwitchNormalPosture", 0.5f);
        }
    }

}