using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using DG.Tweening;
using Module;
using MonsterBehavior;
using Sirenix.OdinInspector;
using StationMachine;
using UnityEngine;
using UnityEngine.AI;

 public abstract class Monster : Agent, IStateMechineObject, IAgentAudioObject, IBuffObject, INavMoveObject, ISkillObject, IStationObject<MonsterStation>,ISimpleBehaviorObject,IPlayerHurt,ISee
 {
     protected ITarget _target;
     [TabGroup("其他")] public MonsterCreator creator;
     [TabGroup("状态")] public FloatField lag;
     [TabGroup("状态"), SerializeField, LabelText("状态"), ReadOnly,]
     protected MonsterStation _station;
     [SerializeField, TabGroup("状态"), ReadOnly, LabelText("移动样式")]
     protected MoveStyle _moveStyle;
     [SerializeField, TabGroup("其他")]
     public bool attributeIsChanged;
     [SerializeField, TabGroup("状态")]
     public MonsterAttribute _finnleAttribute;
     [SerializeField, TabGroup("状态"), ReadOnly, LabelText("战斗状态")]
     private FightState _fightState;
     [SerializeField, TabGroup("状态"),ReadOnly,LabelText("离玩家距离")] 
     public float toPlayerDistance = float.MaxValue;
     [SerializeField, TabGroup("状态"),ReadOnly,LabelText("与目标的距离")] 
     public float toTargetDistance;
     
     [SerializeField, TabGroup("状态"),ReadOnly,LabelText("与目标的角度")] 
     public float toTargetAngler;

     [SerializeField, TabGroup("状态"), ReadOnly, LabelText("与玩家的角度")]
     public float toPlayerAnger = 360;

     [SerializeField, TabGroup("状态"),  LabelText("怪阶段")]
     public int levelIndex = -1;
     [SerializeField, TabGroup("挂点")] protected Animator _animator;
     [SerializeField, TabGroup("挂点")] protected NavMeshAgent _navmesh;
     [SerializeField, TabGroup("挂点")] public AgentSee eye;
     [TabGroup("挂点"), SerializeField] protected AgentAudio[] _agentAudio;
     [LabelText("怪物类型")][TabGroup("信息"), SerializeField]
     public MonsterType monsterType;
     [TabGroup("信息"), SerializeField]
     public List<MonsterLevel> levelEditor;     

     public FightState fightState => _fightState;
     public Animator animator => _animator;
     public override string logName { get; }
     public virtual ITarget target => _target;
     [SerializeField, TabGroup("挂点")] public AnimationCallback _animationCallback;
     public AnimationCallback animationCallback => _animationCallback;

     public bool canReceiveEvent
     {
         get { return true; }
     }

     public NavMeshAgent navmesh => _navmesh;
     public Vector3 moveDirection => navmesh.steeringTarget - transform.position;
     public MoveStyle moveStyle => _moveStyle;
     public AgentAudio[] allAudio => _agentAudio;
     public InitStation initStation => _initStation;
     public float timeScale => TimeHelper.timeScale;
     private List<ISensorTarget> _canSeeTarget;

     public List<ISensorTarget> canSeeTarget
     {
         get
         {
             if (_canSeeTarget == null || _canSeeTarget.Count < 2)
             {
                 _canSeeTarget = new List<ISensorTarget>();
                 if (Player.player != null)
                 {
                     _canSeeTarget.Add(Player.player);
                 }

                 if (Player.baseMent != null)
                 {
                     _canSeeTarget.Add(Player.baseMent);
                 }
             }

             return _canSeeTarget;
         }
     }

     public MonsterLevel currentLevel
     {
         get { return levelEditor[levelIndex]; }
     }

     public MonsterAttribute finnleAttribute
     {
         get
         {
             if (attributeIsChanged)
             {
                 _finnleAttribute = currentLevel.levelAttribute * (1 + BattleController.Instance.procedure.mission.monsterAttribute);
             }

             return _finnleAttribute;
         }
     }

     public bool canHurt
     {
         get { return isAlive; }
     }

     protected override void AddCtrl()
     {
         ctrlList.Clear();
         ctrlList.Add(new MonsterSkillCtrl(this));
         ctrlList.Add(new MonsterBuffCtrl(this));
         ctrlList.Add(new StateMachineCtrl(this, new Dizzy(0, "Dizzy"), new Attack1(0, "Attack01"),
             new Attack2(0, "Attack02")));
         ctrlList.Add(new NavMoveCtrl(this));
         ctrlList.Add(new SimpleBehaviorCtrl(this));
     }

     public void OnTransform(int index)
     {
         if (this.levelIndex != index)
         {
             this.levelIndex = index;
             currentLevel.Init();
             GetAgentCtrl<SkillCtrl>().SetSkill(currentLevel.skill);
             attributeIsChanged = true;
         }
     }

     public float rotateToMove => 360;

     public bool canReleaseSkill => !isParalysis;

     public virtual float stopMoveDistance
     {
         get
         {
             return 0;
         }
     }

     public bool canBeHurt => isAlive;

     public bool canAddLag => isAlive && !isParalysis;

     public virtual IActiveSkill RefreshReadySkill()
     {
         if (target != null && fightState == FightState.Fight)
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
     }

     /// <summary>
     /// 移动速度
     /// </summary>
     public virtual float moveSpeed
     {
         get
         {
             if (ContainStation(MonsterStation.Attack)|| isParalysis) return 0;
             return finnleAttribute.moveSpeed;
         }
     }

     public void MoveToPoint(Vector3 point, Action<bool> callback)
     {
         var ctrl = GetAgentCtrl<NavMoveCtrl>();
         if (ctrl.MoveTo(point, (status, complte) =>
         {
             if (complte)
             {
                 SwitchMoveStyle(MoveStyle.Idle);
             }

             if (status == NavMeshPathStatus.PathComplete)
             {
                 callback?.Invoke(complte);
             }
         }))
         {
             SwitchMoveStyle(MoveStyle.Walk);
         }

     }

     /// <summary>
     /// 转向速度
     /// </summary>
     public float rotateSpeed
     {
         get { return finnleAttribute.rotateSpeed; }
     }

     public virtual bool canSee
     {
         get { return isAlive; }
     }

     public bool isAlive => initStation == InitStation.Normal;
     public MonsterStation station => _station;
     public float animatorSpeed => 1;
     /// <summary>
     /// 眩晕的
     /// </summary>
     public bool isParalysis
     {
         get { return ContainStation(MonsterStation.HitLag); }
     }
     
     public override bool isVisiable
     {
         get { return isAlive && base.isVisiable; }
     }
     
     public event Action<ITarget> onSwitchTarget;

     public virtual void OnCreat(MonsterCreator creator)
     {
         this.creator = creator;
         transform.SetParent(creator.transform);
         if (initStation == InitStation.UnActive)
         {
             _initStation = InitStation.OnlyCreat;
             AddCtrl();
             //eye.includeSights.Add(new DistanceEyeSight(eye.transform, 10, 30));
             eye.includeSights.Add(new ConicalEyeSight(eye.transform, 10, 30, 1));
             eye.onFousTarget += OnEyeFousTarget;
             eye.onLoseTarget += OnEyeLoseTarget;
             eye.onSenserTarget += OnEyeSenserTarget;
         }
     }

     public virtual void Born()
     {
         if (initStation == InitStation.OnlyCreat)
         {
             _initStation = InitStation.Normal;
             navmesh.Warp(creator.transform.position);
             transform.DORotateQuaternion(creator.transform.rotation, 0.5f);
             transform.localScale = Vector3.one;
             SwitchFightState(FightState.Normal);
             OnTransform(0);
         }
     }

     public void Chase()
     {
         if (SwitchFightState(FightState.Fight))  
         {
             SwitchTarget(Player.player);
             GetAgentCtrl<SimpleBehaviorCtrl>().SwitchBehavior(new AttackPlayer());
         }
     }

     public bool SwitchFightState(FightState state)
     {
         if (isAlive && this.fightState != state)
         {
             this._fightState = state;
             GetAgentCtrl<StateMachineCtrl>().SetFloat("fightState", (int) state, 0.2f);
             LogFormat("切换战斗状态{0}", state);
             OnSwitchStation();
             return true;
         }

         return false;
     }
     
     public bool SwitchMoveStyle(MoveStyle style)
     {
         if (isAlive && this._moveStyle != style)
         {
             this._moveStyle = style;
             GetAgentCtrl<StateMachineCtrl>().SetFloat("moveStyle", (int) _moveStyle, 0.2f);
             OnSwitchStation();
             LogFormat("切换移动样式{0}", _moveStyle);
             return true;
         }

         return false;
     }
     
     public virtual bool AddStation(MonsterStation station)
     {
         if (ContainStation(station)) return false;
         this._station = this.station | station;
         OnSwitchStation();
         LogFormat("添加状态{0}", station);
         return true;
     }
     public AgentAudio GetAudio(string name)
     {
         return allAudio.Find(fd => fd.name == name);
     }

     public float GetAudioVolume(AgentAudio tar)
     {
         return 1;
     }

     public bool GetAudioActive(AgentAudio tar)
     {
         return true;
     }

     public virtual float GetTranslateTime(string name)
     {
         return 0.1f;
     }

     public virtual float GetLayerFadeTime(int type, string name)
     {
         return 0.1f;
     }
    
     public virtual float getBreakTranslation(int layer, string name)
     {
         return 0;
     }

     public string GetLayerDefaultAnimation(int layer)
     {
         return "";
     }

     public void OnBreakAnamition(int layer, string name, bool sendEvent)
     {
     }


     public bool RemoveStation(MonsterStation station)
     {
         if (!ContainStation(station)) return false;
         this._station = this.station & ~station;
         OnSwitchStation();
         LogFormat("移除状态{0}", station);
         return true;
     }

     public bool ContainStation(MonsterStation station)
     {
         return (this._station & station) == station;
     }

     public bool SwitchStation(MonsterStation station)
     {
         if (this.station != station)
         {
             this._station = station;
             OnSwitchStation();
             LogFormat("切换状态{0}", station);
             return true;
         }

         return false;
     }
     
     
     private void OnEyeSenserTarget(ISensorTarget obj)
     {
     }

     private void OnEyeLoseTarget(ISensorTarget obj)
     {
     }

     private void OnEyeFousTarget(ISensorTarget obj)
     {
         if (obj is Agent agent)
         {
             if (agent.camp != camp)
             {
                 Chase();
             }
         }
     }

     public bool SwitchTarget(ITarget tar)
     {
         if (this.target != tar)
         {
             LogFormat("切换目标:{0}", tar);
             this._target = tar;
             onSwitchTarget?.Invoke(tar);
             OnSwitchStation();
             return true;
         }

         return false;
     }


     public void OnAddBuff(Buff buff)
     {
     }

     public void OnRemoveBuff(Buff buff)
     {
     }

     public void Push(Vector3 direction, float distance, Action callback)
     {
         navmesh.enabled = false;
         AddStation(MonsterStation.Push);

         direction = new Vector3(direction.x, 0, direction.z).normalized * distance;
         transform.position = transform.position + direction * 0.3f;
         transform.DOMove(transform.position + direction, 0.1f).OnComplete(() =>
         {
             navmesh.enabled = true;
             RemoveStation(MonsterStation.Push);
             callback?.Invoke();
         });
     }
     
     protected virtual void Update()
     {
         if (Player.player != null)
         {
             toPlayerDistance = Player.player.transform.position.HorizonDistance(transform.position);
             toPlayerAnger = Vector3.Angle(new Vector3(Player.player.transform.forward.x, 0, Player.player.transform.forward.z), new Vector3(transform.forward.x, 0, transform.forward.z));
         }

         if (target != null)
         {
             toTargetDistance = target.transform.position.HorizonDistance(transform.position);
             Vector3 v = target.transform.position - transform.position;
             toTargetAngler = Vector3.Angle(new Vector3(v.x, 0, v.z), new Vector3(transform.forward.x, 0, transform.forward.z));
         }
         else
         {
             toTargetDistance = float.MaxValue;
             toTargetAngler = 360;
         }
         
         if ((initStation == InitStation.Normal || initStation == InitStation.PreDead) && !ctrlList.IsNullOrEmpty())
         {
             for (int i = 0; i < ctrlList.Count; i++)
             {
                 ctrlList[i].OnUpdate();
             }
         }
     }

     public Damage OnHurt(Damage damage)
     {
         if (damage.damage > 0)
         {
             if (canHurt)
             {
                 for (int i = 0; i < ctrlList.Count; i++)
                 {
                     if (ctrlList[i] is IHurtObject hurtObj)
                     {
                         damage = hurtObj.OnHurt(damage);
                     }
                 }

                 if (damage.damage > 0)
                 {
                     if (damage.type == DamageType.Hp)
                     {
                         float currhp = hp;
                         currhp = Mathf.Clamp(currhp - damage.damage, 0, currhp);
                         _hp = new FloatField(currhp);
                         if (currhp <= 0)
                         {
                             OnDead();
                         }
                         else
                         {
                             Chase();
                         }
                     }
                     else if (damage.type == DamageType.Lag)
                     {
                         if (canAddLag)
                         {
                             lag = new FloatField(lag.value + damage.damage);
                             if (lag >= currentLevel.dbData.maxLag)
                             {
                                 OnFullLag();
                             }
                         }
                         Chase();
                     }
                 }
             }
         }
         
         return damage;
     }

     public void OnFullLag()
     {
         lag = new FloatField(0);
         if (AddStation(MonsterStation.HitLag))
         {
             GetAgentCtrl<StateMachineCtrl>().Play<Dizzy>().onEnd += com =>
             {
                 if (com)
                 {
                     RemoveStation(MonsterStation.HitLag);
                 }
             };
             // GetAgentCtrl<AnimatorCtrl>().Play("Dizzy", 1, 0).onStationChange += st =>
             // {
             //     if (st.isComplete)
             //     {
             //         RemoveStation(MonsterStation.HitLag);
             //     }
             // };
         }
     }

     public void OnDead()
     {
         _initStation = InitStation.PreDead;
     }

 }