using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BehaviorDesigner.Runtime;
using DG.Tweening;
using Module;
using Project.Data;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public enum TimeLineType
{
    Ass,
    Exc,
    Story,
    KillPlayer,
    AssAbsorb,
    GetOut,
    Rivive
}

public enum DeadType
{
    Ass1,
    Ass2,
    Exc,
    Punch,
    Shot,
    FromMonster
}

public enum AfterAttackStyle
{
    None,
    Duizhi,
    Roar
}
public abstract class Monster : MonoBehaviour,IStationObject<MonsterStation>, IPatrolObject, ISimpleBehaviorObject, IAgentAudioObject, IBuffObject, ISkillObject, IAnimatorObject, IProgressObject, IHurtObject,ISee,IMonster
{
    [Flags]
    public enum ResetFlag
    {
        Hp = 1,
        Position = 2,
    }

    #region 字段
    private Coroutine tiredCortine;
    private MonsterLevel _currentLevel;
    protected List<IAgentCtrl> ctrlList = new List<IAgentCtrl>();
    private ITarget _target;
    public Drop drop;
    private SkillCtrl _skillCtrl;
    private SimpleBehaviorCtrl _simpleBtCtrl;
    private BuffCtrl _buffCtrl;
    private MonsterCtrl _monsterCtrl;
    public static Action<Monster, MonsterPart,DeadType,Damage> OnMonsterDead;
    public MonsterCtrl monsterCtrl
    {
        get
        {
            if(_monsterCtrl==null) _monsterCtrl = BattleController.GetCtrl<MonsterCtrl>();
            return _monsterCtrl;
        }
    }
    public SkillCtrl skillCtrl
    {
        get
        {
            if (_skillCtrl == null) _skillCtrl = GetAgentCtrl<SkillCtrl>();
            return _skillCtrl;
        }
    }

    public SimpleBehaviorCtrl simpleBtCtrl
    {
        get
        {
            if (_simpleBtCtrl == null) _simpleBtCtrl = GetAgentCtrl<SimpleBehaviorCtrl>();
            return _simpleBtCtrl;
        }
    }

    public BuffCtrl buffCtrl
    {
        get
        {
            if (_buffCtrl == null) _buffCtrl = GetAgentCtrl<BuffCtrl>();
            return _buffCtrl;
        }
    }

    public MonsterLevel currentLevel => _currentLevel;
    [TabGroup("挂点")] public MonsterCreator creator;

    [SerializeField, TabGroup("状态")] private FloatField _hp;

    [SerializeField, TabGroup("状态"), ReadOnly, LabelText("初始化状态")]
    protected InitStation _initStation;

    [TabGroup("状态"), SerializeField, LabelText("状态"), ReadOnly,]
    protected MonsterStation _station;

    [SerializeField, TabGroup("状态"), ReadOnly, LabelText("移动样式")]
    protected MoveStyle _moveStyle;


    [SerializeField, TabGroup("状态"), ReadOnly, LabelText("战斗状态")]
    private FightState _fightState;
    [ShowInInspector,TabGroup("状态")]
    private string behaviorName
    {
        get
        {
            if (simpleBtCtrl != null && simpleBtCtrl.currBehavior != null)
            {
                return simpleBtCtrl.currBehavior.ToString();
            }

            return null;
        }
    }
    [ShowInInspector,TabGroup("状态")]
    private string skillName
    {
        get
        {
            if (skillCtrl != null && skillCtrl.currActive != null)
            {
                return skillCtrl.currActive.name;
            }

            return null;
        }
    }
    [SerializeField, TabGroup("状态"), ReadOnly]
    public int seePlayerPointCount;
    [SerializeField, TabGroup("状态"),ReadOnly,LabelText("离玩家距离")] 
    public float toPlayerDistance = float.MaxValue;
    [SerializeField, TabGroup("状态"),ReadOnly,LabelText("与玩家的角度")] 
    public float toPlayerAnger;

    [HideInInspector] public Vector3 memeryPosition;

    [SerializeField, TabGroup("挂点")] protected Animator _animator;
    [SerializeField, TabGroup("挂点")] protected NavMeshAgent _navmesh;
    [SerializeField, TabGroup("挂点")] public AgentSee eye;
    [SerializeField, TabGroup("挂点")] public AgentHear ear;
    [SerializeField, TabGroup("挂点")] public Transform centerPoint;
    [SerializeField, TabGroup("挂点")] public AnimationCallback _animationCallback;
    [TabGroup("挂点")] protected Transform progressPoint;
    [TabGroup("挂点"), SerializeField] protected AgentAudio[] _agentAudio;
    [TabGroup("挂点")] public IdleStyle idleStyle;
    [TabGroup("挂点")] public Transform assPoint;
    [TabGroup("挂点")] public CapsuleCollider collider;

    [LabelText("怪物类型")][TabGroup("信息"), SerializeField]
    public MonsterType monsterType;
    [LabelText("模型")] [TabGroup("信息"), SerializeField]
    public MonsterModelName modelName;
    [LabelText("默认阶段")] [TabGroup("信息"), SerializeField]
    private List<MonsterLeveEditor> defaultLevel;
    [LabelText("通用动画")] [TabGroup("信息"), SerializeField]
    public List<AnimationKeyValue> animationKey;

    [TabGroup("信息")] public bool deadDestroy = true;
    [TabGroup("信息")]
    public float rongjieWaitSecond = 30;

    [TabGroup("信息"), LabelText("查看门是否能开的距离")]
    public float checkDistance = 2;
    [TabGroup("状态")]
    public bool isTired;


    public FightState showUiState
    {
        get { return fightState; }
    }

    public List<MonsterLeveEditor> defaultLevels
    {
        get { return defaultLevel; }
        set { defaultLevel = value; }
    }
    public string logName => $"{creator.id} : {modelName}";
    [TabGroup("其他")]
    private bool _isLog;

    public bool isLog
    {
        get
        {
            return _isLog;
        }
        set
        {
            ILogObject[] log = transform.GetComponentsInChildren<ILogObject>(true);
            for (int i = 0; i < log.Length; i++)
            {
                if (log[i] != this)
                {
                    log[i].isLog = value;
                }
            }

            _isLog = value;
        }
    }


    public event Action<AnimationEvent,int> onAnimationCallback;

    #endregion

    #region 属性

    public FightState fightState => _fightState;
    public float hp => _hp.value;
    public Animator animator => _animator;
    public AnimationCallback animationCallback => _animationCallback;

    public bool canReceiveEvent => isAlive;
    public NavMeshAgent navmesh => _navmesh;
    public virtual Vector3 faceDirection
    {
        get
        {
            return navmesh.steeringTarget - transform.position;
        }
    }

    public Vector3 terrainNormal => transform.up;
    public MoveStyle moveStyle => _moveStyle;
    public AgentAudio[] allAudio => _agentAudio;

    public InitStation initStation
    {
        get
        {
            return _initStation;
        }
        set
        {
            _initStation = value;
            creator.initStation = value;
        }
    }
    public ITarget target => _target;
    public float timeScale => TimeHelper.timeScale;
    public Vector3 progressPos => progressPoint == null ? transform.position : progressPoint.position;
    public Hurtmaterial hurtMaterial => Hurtmaterial.Meat;

    public bool canReleaseSkill
    {
        get
        {
            return isAlive && fightState == FightState.Fight && !ContainStation(MonsterStation.Roar) &&
                   !ContainStation(MonsterStation.Attack) && !isParalysis &&
                   !ContainStation(MonsterStation.Alarm) && !ContainStation(MonsterStation.TimeLine);
        }
    }

    public virtual float rotateToMove => 90;

    public virtual float stopMoveDistance
    {
        get
        {
            if (ContainStation(MonsterStation.TimeLine) || ContainStation(MonsterStation.ResetToBorn)|| ContainStation(MonsterStation.IsSeePlayHide)) return 0;
            if (skillCtrl != null && skillCtrl.isBusy) return ((MonsterSkill) skillCtrl.currActive).stopMoveDistance;
            return navmesh.radius;
        }
    }
    
    /// <summary>
    /// 移动速度
    /// </summary>
    public virtual float moveSpeed
    {
        get
        {
            if (!isAlive || moveStyle == MoveStyle.None || initStation != InitStation.Normal || isParalysis ||
                ContainStation(MonsterStation.Roar) ||
                ContainStation(MonsterStation.Wait) || ContainStation(MonsterStation.Attack) ||
                ContainStation(MonsterStation.Alarm) || ContainStation(MonsterStation.TimeLine) ||
                ContainStation(MonsterStation.CheckLook) || ContainStation(MonsterStation.WakeUp))
            {
                return 0;
            }

            if (skillCtrl.currActive is DodgeSkill dodge)
            {
                return dodge.dodgeSpeed;
            }
            
            float buffk = buffCtrl.buffList.Count == 0 ? 0 : (buffCtrl.buffList[0] as WeekBuff).moveSpeedDown;
            if (moveStyle == MoveStyle.Run)
            {
                return currentLevel.attribute.moveSpeed * currentLevel.dbData.runMoveK * (1 - buffk);
            }

            return currentLevel.attribute.moveSpeed * (1 - buffk);
        }
    }

    /// <summary>
    /// 转向速度
    /// </summary>
    public virtual float rotateSpeed
    {
        get
        {
            if (moveSpeed == 0) return 0;
            if (fightState == FightState.Normal) return currentLevel.attribute.rotateSpeed*0.5f;
            return currentLevel.attribute.rotateSpeed;
        }
    }

    public virtual bool canHear
    {
        get
        {
            return isAlive && !isParalysis && !ContainStation(MonsterStation.TimeLine) &&
                   fightState != FightState.Fight && !ContainStation(MonsterStation.Sleep);
        }
    }

    public virtual bool canSee
    {
        get { return isAlive && !isParalysis && !ContainStation(MonsterStation.TimeLine); }
    }


    public bool isAlive => initStation == InitStation.Normal && !gameObject.IsNullOrDestroyed();
    
    public  MonsterStation station => _station;
    public float animatorSpeed => 1;
 
    public List<MonsterLeveEditor> levelEditor => creator != null && creator.levelEditor.Count != 0 ? creator.levelEditor : defaultLevel;
    
    public virtual bool isSeePlayer => isAlive&& Player.player != null  && !ContainStation(MonsterStation.Sleep)&&(seePlayerPointCount > Player.player.CheckPoins.Count * 0.5f || ContainStation(MonsterStation.IsSeePlayHide));

    public virtual bool canHurt
    {
        get { return isAlive && !ContainStation(MonsterStation.TimeLine) ; }
    }

    public virtual bool canExc
    {
        get { return canHurt && !ContainStation(MonsterStation.TimeLine); }
    }

    public virtual bool canAss
    {
        get { return !AddStation(MonsterStation.TimeLine); }
    }

    public virtual bool isParalysis => ContainStation(MonsterStation.Paralysis) || ContainStation(MonsterStation.HitParalysis);

    #endregion

    #region 事件
    public event Action onSwitchStation;

    public void SendSwitchStationEvent()
    {
        onSwitchStation?.Invoke();
    }
    #endregion

    #region 创建出生死亡受伤

    protected virtual void Start()
    {
        EventCenter.Register<AttackMonster, TimeLineType>(EventKey.MonsterTimeLine,OnMosterTimeLine);
        EventCenter.Register<AttackMonster, TimeLineType>(EventKey.MonsterEndTimeLine,OnMosterEndTimeLine);
        animationCallback.onAnimationCallback += OnAnimationCallback;
    }

    public virtual void OnCreat(MonsterCreator creator)
    {
        this.creator = creator;

        if (initStation == InitStation.UnActive)
        {
            initStation = InitStation.OnlyCreat;
            creator.ReceiveEvent();
            AddCtrl();
            //这里被注掉了
            // for (int i = 0; i < ctrlList.Count; i++)
            // {
            //     ctrlList[i].OnCreat();
            // }

            eye.onFousTarget += OnEyeFousTarget;
            eye.onLoseTarget += OnEyeLoseTarget;
            eye.onSenserTarget += OnEyeSenserTarget;
            if (ear != null)
            {
                ear.onHearTarget += OnHearTarget;
            }

            if (navmesh != null)
            {
                navmesh.avoidancePriority = BattleController.GetCtrl<MonsterCtrl>().GetMonsterProperty(monsterType);
            }

            if (collider != null)
            {
                var meeleAttack = skillCtrl.allSkill.Last();
                if (meeleAttack is MeleeAttack att)
                {
                    this.collider.radius = att.stopMoveDistance - 0.1f;
                }
            }
        }
        
    }

    protected virtual void AddCtrl()
    {
    }

    protected virtual void OnHearTarget(Vector3 arg2, object[] arg3)
    {
    }

    public virtual void Born()
    {
        if (initStation == InitStation.OnlyCreat)
        {
            initStation = InitStation.Normal;
            TransformBody(true);
            ResetValue(true, ResetFlag.Hp | ResetFlag.Position);
            if (currentLevel.dbData.visualDistance1 != 0)
            {
                eye.includeSights.Add(new DistanceEyeSight(eye.transform, currentLevel.dbData.visualDistance1, currentLevel.dbData.visualRange1));
            }

            if (currentLevel.dbData.visualDistance2 != 0)
            {
                eye.includeSights.Add(new DistanceEyeSight(eye.transform, currentLevel.dbData.visualDistance2, currentLevel.dbData.visualRange2));
            }
            var animatorCtrl = GetAgentCtrl<AnimatorCtrl>();
            if (animatorCtrl != null)
            {
                animatorCtrl.AddBlendTree("Hit",1);
            }

        }
    }

    public virtual void StopAttack()
    {
    }


    public bool CanDead(Damage damage)
    {
        return hp <= damage.damage;
    }
    public bool CanDead(int damage)
    {
        return hp <= damage;
    }

    public virtual Damage OnHurt(Damage damage)  
    {
        return damage;
    }

    public virtual Damage OnHurt(ITarget target, Damage damage)
    {
        if (canHurt)
        {
            MonsterPart part = (MonsterPart) target;
            float damageValue = damage.damage;
            float currHp = hp;
            GameDebug.LogError(damageValue);
            if (damageValue > 0)
            {
                if (damageValue >= currHp)
                {
                    _hp = new FloatField(0);
                    OnDead(target, DeadType.Shot, damage);
                }
                else
                {
                    _hp -= damageValue;
                }
            }

            return damage;
        }

        return default;
    }

    public void WakeUpFromSleep(Action callback)
    {
        if (RemoveStation(MonsterStation.Sleep))
        {
            AddStation(MonsterStation.WakeUp);
            GetAgentCtrl<AnimatorCtrl>().Play("zhuangsiqilai", 0, 0).onStationChange += st =>
            {
                if (st.station == AnimationPlayStation.Complete)
                {
                    RemoveStation(MonsterStation.WakeUp);
                    callback?.Invoke();
                }
            };
        }
        else if (!ContainStation(MonsterStation.WakeUp))
        {
            callback?.Invoke();
        }
    }


    public virtual bool OnDead(ITarget target,DeadType deadType,Damage damage)
    {
        return false;
    }

    public void SetLayer(bool Dead)
    {
        Transform[] allTransform = transform.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < allTransform.Length; i++)
        {
            if (Dead)
            {
                allTransform[i].gameObject.layer = MaskLayer.deadMonster;
            }
            else
            {
                if (allTransform[i].gameObject.layer == MaskLayer.deadMonster)
                {
                    if (allTransform[i].gameObject.name == "PlayerCollider")
                    {
                        allTransform[i].gameObject.layer = MaskLayer.playerBlock;
                    }
                    else
                    {
                        allTransform[i].gameObject.layer = MaskLayer.monster;
                    }
                }
            }
        }
    }

    protected virtual void OnDestroy()
    {
        creator.monster = null;
        StopAllCoroutines();
        MonsterCtrl ctrl = BattleController.GetCtrl<MonsterCtrl>();
        if (ctrl != null)
        {
            ctrl.TryRemoveMonster(this);
        }

        EventCenter.UnRegister<AttackMonster, TimeLineType>(EventKey.MonsterTimeLine, OnMosterTimeLine);
        EventCenter.UnRegister<AttackMonster, TimeLineType>(EventKey.MonsterEndTimeLine, OnMosterEndTimeLine);
        animationCallback.onAnimationCallback -= OnAnimationCallback;
        AudioPlay.MuteAudio(sd => sd.ID == gameObject, true);

    }

    protected virtual void OnMosterEndTimeLine(AttackMonster obj, TimeLineType type)
    {
        if (isAlive)
        {
            RemoveStation(MonsterStation.TimeLine);
            animator.SetLayerWeight(2, 0);
            AudioPlay.MuteAudio(sd => sd.ID == gameObject, false);
        }
    }

    protected virtual void OnMosterTimeLine(AttackMonster obj, TimeLineType type)
    {
        if (isAlive)
        {
            if (lookTweener != null && lookTweener.IsActive())
            {
                lookTweener.Kill();
            }

            if (obj == this && type != TimeLineType.GetOut)
            {
                BreakAction();
            }

            animator.SetLayerWeight(2, 1);
            AddStation(MonsterStation.TimeLine);
            StopAllAudio();
            AudioPlay.MuteAudio(sd => sd.ID == gameObject, true);
        }
    }


    public void ResetValue(bool isBorn,ResetFlag flag)
    {
        if (initStation == InitStation.Normal)
        {
            if ((flag & ResetFlag.Position) != 0)
            {
                FlashToPoint(creator.transform.position);
                transform.DORotateQuaternion(creator.transform.rotation, 0.5f);
                transform.localScale = Vector3.one;
            }

            SwitchFightState(FightState.Normal);
            if ((flag & ResetFlag.Hp) != 0)
            {
                _hp = new FloatField(currentLevel.attribute.GetMissionHp());
                GetAgentCtrl<ParalysisCtrl>()?.ClearPara();
            }

            //ctrlList.ForEach(st => st.OnBorn());
            if (!isBorn)
            {
                SwitchStation(currentLevel.editorData.resetStation.bornStation);
            }
            else
            {
                SwitchStation(currentLevel.editorData.bornStation.bornStation);
            }
            var buffctr = GetAgentCtrl<BuffCtrl>();
            if (buffctr != null)
                buffctr.ClearBuff();
        }
    }

    #endregion

    #region 回调

    public IEnumerator WaitRemoveHit()
    {
        yield return new WaitForSeconds(5);
        RemoveStation(MonsterStation.HitParalysis);
    }
    /// <summary>
    /// 硬直满的逻辑
    /// </summary>
    /// <param name="type"></param>
    public virtual  void OnPartFull(MonsterPartType type,Damage damage)
    {
        if (AddStation(MonsterStation.HitParalysis))
        {
            StartCoroutine("WaitRemoveHit");
        }
        string animation = "hit_body";
        if (damage.weapon == WeaponType.MeleeWeapon)
        {
            animation = "hit_chuizi";
            animator.SetFloat("HitFoward", damage.dir.z);
            animator.SetFloat("HitLeft", damage.dir.x);
        }
        else
        {
            if (type == MonsterPartType.Head)
            {
                animation = "hit_head";
            }
            else if (type == MonsterPartType.Leg)
            {
                animation = "hit_leg";
            }
        }

        BreakAction();
        var play=PlayAnimation(animation, 1);
        if (play != null)
        {
            play.onStationChange += st =>
            {
                if (st.isComplete)
                {
                    RemoveStation(MonsterStation.HitParalysis);
                }
            };
        }
        else
        {
            GameDebug.LogError("无法播放动画" + animation);
        }

    }

    public virtual IActiveSkill RefreshReadySkill()
    {
        var ctrl = GetAgentCtrl<SkillCtrl>();
        for (int i = 0; i < ctrl.allSkill.Count; i++)
        {
            if (ctrl.allSkill[i] is IActiveSkill active)
            {
                if (active.isWanted)
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

    public MonsterSkill BreakAction()
    {
        if (skillCtrl != null)
        {
            MonsterSkill resu = skillCtrl.currActive as MonsterSkill;
            if (skillCtrl.isBusy)
            {
                skillCtrl.BreakSkill(BreakSkillFlag.BreakAction | BreakSkillFlag.WithAnimation);
                return resu;
            }
            else
            {
                GetAgentCtrl<AnimatorCtrl>().BreakAnimation(0, true);
            }

            if (waitCoroutine != null)
            {
                StopCoroutine(waitCoroutine);
            }

            if (checkCoroutine != null)
            {
                StopCoroutine(checkCoroutine);
            }
            
            RemoveStation(MonsterStation.Alarm);
            RemoveStation(MonsterStation.Wait);
            return resu;
        }

        return null;
    }

//20220118 火持续烧10秒,出圈继续烧5秒
    public virtual void OnAddBuff(Buff buff)
    {
        onSwitchStation?.Invoke();
    }

    public virtual void OnRemoveBuff(Buff buff)
    {
        onSwitchStation?.Invoke();
    }

    public abstract string GetLayerDefaultAnimation(int layer);


    public virtual void OnBreakAnamition(int layer, string name, bool sendEvent)
    {
    }

    public virtual void OnAnimationCallback(AnimationEvent @event,int index)
    {
        onAnimationCallback?.Invoke(@event, index);
    }

    #endregion

    #region 获取状态

    public virtual float GetAudioVolume(AgentAudio tar)
    {
        if (GetAudioActive(tar))
        {
            return AudioPlay.AudioVolume;
        }
        else
        {
            return 0;
        }

    }

    public virtual bool GetAudioActive(AgentAudio tar)
    {
        if (tar == null) return false;
        return isAlive && toPlayerDistance <= tar.audioSource.maxDistance && !ContainStation(MonsterStation.TimeLine);
    }

    public AudioPlay PlayAudio(string key, string audioClip)
    {
        // var play = GetAgentCtrl<AudioCtrl>().PlayAudioOneShot(key, audioClip);
        // if (play != null)
        // {
        //     play.SetID(gameObject);
        // }
        //
        // return play;
        return null;
    }

    public void StopAllAudio()
    {
        AudioPlay.StopAudio(sd => sd.ID == gameObject);
    }


    #endregion

    #region 基本不会变的方法

    protected virtual void Update()
    {
        if ((initStation == InitStation.Normal || initStation == InitStation.PreDead) && !ctrlList.IsNullOrEmpty())
        {
            for (int i = 0; i < ctrlList.Count; i++)
            {
                ctrlList[i].OnUpdate();
            }
        }
        if (Player.player != null)
        {
            toPlayerDistance = Vector3.Distance(Player.player.chasePoint, transform.position);
            toPlayerAnger = Vector3.Angle(new Vector3(Player.player.transform.forward.x, 0, Player.player.transform.forward.z), new Vector3(transform.forward.x, 0, transform.forward.z));
        }

        CheckOpenDoor(checkDistance);
    }


    public void CheckOpenDoor(float dis)
    {
        if (fightState == FightState.Fight && dis > 0)
        {
            RaycastHit hit;
            Ray ray = new Ray(eye.transform.position, eye.transform.forward);
            Debug.DrawLine(ray.origin, ray.GetPoint(dis), Color.cyan);
            if (Physics.Raycast(ray, out hit, dis, MaskLayer.door))
            {
                if (hit.collider.gameObject.CompareTag("Door"))
                {
                    var faceDoor = hit.collider.gameObject.GetComponentInParent<Door>();
                    if (faceDoor != null && faceDoor.ContainStation(PropsStation.Off) && faceDoor.Interactive(true))
                    {
                        BreakAction();
                        WaitWhile(faceDoor.openDoorTime + 0.5f, false, null);
                    }
                }
            }
        }
    }


    public bool SwitchTarget(ITarget tar)
    {
        if (this.target != tar)
        {
            LogFormat("切换目标:{0}", tar);
            this._target = tar;
            onSwitchStation?.Invoke();
            return true;
        }

        return false;
    }

    public virtual bool SwitchFightState(FightState state)
    {
        if (this.fightState != state)
        {
            this._fightState = state;
            if (state == FightState.Fight)
            {
                if (SwitchTarget(Player.player))
                {
                    Alarm();
                }
            }
            else
            {
                SwitchTarget(null);
            }

            if (state == FightState.Normal)
            {
                GetAgentCtrl<AnimatorCtrl>().SetFloat("fightState", 0, 0.2f);
            }
            else if (state == FightState.Fight)
            {
                GetAgentCtrl<AnimatorCtrl>().SetFloat("fightState", 1, 0.2f);
            }

            LogFormat("切换战斗状态{0}", state);
            onSwitchStation?.Invoke();
            return true;
        }

        return false;
    }

    public bool SwitchMoveStyle(MoveStyle style)
    {
        if (isAlive && this.moveStyle != style)
        {
            this._moveStyle = style;
            GetAgentCtrl<AnimatorCtrl>().SetFloat("moveStyle", (int) moveStyle, 0.2f);
            if (idleStyle != null && style != MoveStyle.None)
            {
                idleStyle.BreakFree();
            }
            onSwitchStation?.Invoke();
            LogFormat("切换移动样式{0}", moveStyle);
            return true;
        }

        return false;
    }

    public T GetAgentCtrl<T>() where T : IAgentCtrl
    {
        for (int i = 0; i < ctrlList.Count; i++)
        {
            if (ctrlList[i].GetType() == typeof(T))
            {
                return (T) ctrlList[i];
            }
        }

        return default;
    }

    public virtual bool AddStation(MonsterStation station)
    {
        if (ContainStation(station)) return false;
        this._station = this.station | station;
        onSwitchStation?.Invoke();
        LogFormat("添加状态{0}", station);
        return true;
    }

    public bool RemoveStation(MonsterStation station)
    {
        if (!ContainStation(station)) return false;
        this._station = this.station & ~station;
        onSwitchStation?.Invoke();
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
            onSwitchStation?.Invoke();
            LogFormat("切换状态{0}", station);
            return true;
        }

        return false;
    }

    private bool patrolforward = true;

    public Vector3 GetPatrolPoint(ref int index)
    {
        if (currentLevel.editorData.patrolPoint.IsNullOrEmpty())
        {
            GameDebug.LogError("巡逻点出错");
            return transform.position;
        }

        var temp = currentLevel.editorData.patrolPoint[index % currentLevel.editorData.patrolPoint.Count].position;

        if (currentLevel.editorData.patrolType == PatrolType.Loop)
        {
            index++;
        }
        else if (currentLevel.editorData.patrolType == PatrolType.Pingpong)
        {
            if (patrolforward)
            {
                if (index == currentLevel.editorData.patrolPoint.Count - 1)
                {
                    patrolforward = false;
                    index--;
                }
                else
                {
                    index++;
                }
            }
            else
            {
                if (index == 0)
                {
                    patrolforward = true;
                    index++;
                }
                else
                {
                    index--;
                }
            }
        }
        else if (currentLevel.editorData.patrolType == PatrolType.Random)
        {
            int randomIndex = index;
            while (randomIndex == index)
            {
                randomIndex = Random.Range(0, currentLevel.editorData.patrolPoint.Count);
            }

            index = randomIndex;
        }

        return temp;
    }


    public AgentAudio GetAudio(string name)
    {
        return allAudio.Find(fd => fd.key == name);
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

    public float GetUnscaleDelatime(bool ignorePause)
    {
        return ignorePause ? TimeHelper.unscaledDeltaTimeIgnorePause : TimeHelper.unscaledDeltaTime;
    }

    public float GetDelatime(bool ignorePause)
    {
        return ignorePause ? TimeHelper.deltaTimeIgnorePause : TimeHelper.deltaTime;
    }

    public void SetTimescale(float timeScale)
    {
        TimeHelper.timeScale = timeScale;
    }

    public Tweener SetTimescale(float timeScale, float time)
    {
        return TimeHelper.SetTimeScale(time, time);
    }

    #endregion

    #region 动作

    /// <summary>
    /// 变身
    /// </summary>
    public void TransformBody(bool ignoreUnactive)
    {
        if (CanTransform(ignoreUnactive))
        {
            OnTransform(currentLevel?.index + 1 ?? 0);
        }
    }

    public virtual bool CanTransform(bool ignoreUnactive)
    {
        if (isAlive || ignoreUnactive)
        {
            int maxLevel = levelEditor.Count - 1;
            if (currentLevel == null || currentLevel.index < maxLevel)
            {
                return true;
            }
        }

        return false;
    }


    public virtual void OnTransform(int nextLevel)
    {
        if (currentLevel == null)
        {
            _currentLevel = new MonsterLevel(0, levelEditor[0], this);
        }
        else
        {
            if (nextLevel != currentLevel.index)
            {
                _currentLevel = new MonsterLevel(nextLevel, levelEditor[nextLevel], this);
            }
        }

        List<Skill> skillTemp = new List<Skill>();
        for (int i = 0; i < currentLevel.editorData.skills.Count; i++)
        {
            skillTemp.Add(GameObject.Instantiate(currentLevel.editorData.skills[i]));
        }
        GetAgentCtrl<SkillCtrl>().SetSkill(skillTemp.ToArray());
        //GetAgentCtrl<SkillCtrl>().SetSkill(currentLevel.editorData.skills.ToArray());
        SwitchBehaviorTree(currentLevel.editorData.bornStation.behavior.ToString(), currentLevel.editorData.bornStation.Args);

        // eye.viewAngle = currentLevel.dbData.visualRange;
        // eye.viewDistance = currentLevel.dbData.visualDistance;
        // ctrlList.ForEach(ct => ct.OnTransform(currentLevel.index));
    }

    //跟潘宇一下情况会播放警觉动画 1: 由非追捕逻辑进入追捕逻辑的时候 
    public virtual bool Alarm()
    {
        if (!isParalysis)
        {
            if (AddStation(MonsterStation.Alarm))
            {
                LookAtPoint(Player.player.chasePoint);
                var play = PlayAnimation("alarm", 0);
                if (play != null)
                {
                    play.onStationChange += st =>
                    {
                        if (st.isComplete)
                        {
                            RemoveStation(MonsterStation.Alarm);
                        }
                    };
                    return true;
                }
                else
                {
                    RemoveStation(MonsterStation.Alarm);
                }
            }
        }

        return false;
    }

    public virtual bool Roar(Action callback)
    {
        if (!isParalysis && AddStation(MonsterStation.Roar))
        {
            RemoveStation(MonsterStation.Paraller);
            var play = PlayAnimation("roar", 0);
            if (play != null)
            {
                if (target != null)
                {
                    LookAtPoint(Player.player.chasePoint);
                }

                play.onStationChange += st =>
                {
                    if (st.isComplete)
                    {
                        callback?.Invoke();
                        RemoveStation(MonsterStation.Roar);
                    }
                };
                return true;
            }
            else
            {
                callback?.Invoke();
                RemoveStation(MonsterStation.Roar);
            }
        }

        return false;
    }


    public void Check(Action callback)
    {
        if (AddStation(MonsterStation.CheckLook))
        {
            checkPlay = PlayAnimation("look", 0);
            if (checkPlay != null)
            {
                checkPlay.SetLoop(-1);
                checkCoroutine = StartCoroutine(CheckSecond(callback));
            }
        }
    }

    private IEnumerator CheckSecond(Action callback)
    {
        yield return new WaitForSeconds(5);
        if (isAlive && fightState != FightState.Fight)
        {
            checkCoroutine = null;
            StopCheck();
            GetAgentCtrl<AnimatorCtrl>().BreakAnimation(0, true);
            SwitchFightState(FightState.Normal);
            ResetToBorn(false, callback);
        }
    }

    public void StopCheck()
    {
        if (RemoveStation(MonsterStation.CheckLook))
        {
            if (checkCoroutine != null)
            {
                StopCoroutine(checkCoroutine);
            }
        }
    }

    protected Coroutine waitCoroutine;
    protected Coroutine checkCoroutine;
    private AnimationPlay checkPlay;

    public virtual bool canAddParalysis
    {
        get
        {
            return canHurt && !isParalysis && !ContainStation(MonsterStation.WakeUp);
        }
    }

    public virtual void WaitWhile(float time, bool startIdleStyle, Action callback)
    {
        if (AddStation(MonsterStation.Wait))
        {
            if (idleStyle != null && startIdleStyle)
            {
                idleStyle.Style();
            }
        
            Idle();
            if (waitCoroutine != null)
            {
                StopCoroutine(waitCoroutine);
            }
            waitCoroutine = StartCoroutine(WaitNext(time, callback));
        }
    }

    private IEnumerator WaitNext(float time, Action callback)
    {
        yield return new WaitForSeconds(time);
        RemoveStation(MonsterStation.Wait);
        callback?.Invoke();
    }
    
    public virtual void Idle(int type = 0)
    {
        RemoveStation(MonsterStation.Paraller);
        SwitchMoveStyle(MoveStyle.None);
    }

    /// <summary>
    /// 移动
    /// </summary>
    /// <param name="type"></param>
    /// <param name="point"></param>
    /// <param name="callback"></param>
    public virtual bool MoveTo(MoveStyle type, Vector3 point, Action<NavMeshPathStatus, bool> callback)
    {
        if (isAlive && !ContainStation(MonsterStation.Wait))
        {
            NavMoveCtrl moveCtrl = GetAgentCtrl<NavMoveCtrl>();
            if (moveCtrl != null)
            {
                if (GetAgentCtrl<NavMoveCtrl>().MoveTo(point, (status, complete) =>
                    {
                        if (complete)
                        {
                            if (status == NavMeshPathStatus.PathComplete)
                            {
                                Idle();
                            }
                            else if (status == NavMeshPathStatus.PathInvalid ||
                                     (status == NavMeshPathStatus.PathPartial &&
                                      new Vector3(point.x, transform.position.y, point.z)
                                          .Distance(transform.position) >= stopMoveDistance))
                            {
                                if (!moveCtrl.CanMoveTo(point))
                                {
                                    RoarAndReset();
                                }
                            }
                        }
                        else
                        {
                            if (status == NavMeshPathStatus.PathPartial || status == NavMeshPathStatus.PathInvalid)
                            {
                                if (skillCtrl != null)
                                {
                                    if (skillCtrl.isBusy)
                                    {
                                        skillCtrl.BreakSkill(BreakSkillFlag.BreakAction | BreakSkillFlag.WithAnimation);
                                    }
                                }

                                //todo 在这里有个问题,如果是寻初始化行为树是巡逻,那么走到出生点的时候巡逻会一直判断那个点是不可到的状态,这样怪物会吼一下,但是因为reset状态了,导致会漂移
                                //这个问题现在还不知道怎么解决
                                if (currentLevel.editorData.resetStation.behavior == BehaviorType.Idle ||
                                    currentLevel.editorData.resetStation.behavior == BehaviorType.None)
                                {
                                    RoarAndReset();
                                }
                            }
                        }

                        callback?.Invoke(status, complete);
                    }))
                {
                    //RoarAndReset();
                    SwitchMoveStyle(type);
                    return true;
                }
            }
        }

        return false;
    }

    public void RoarAndReset()
    {
        Roar(() =>
        {
            ResetToBorn(true, null);
        });
    }

    
    public virtual void ResetToBorn()
    {
        if (initStation >= InitStation.Normal)
        {
            initStation = InitStation.Normal;
            SetLayer(false);
            ResetValue(false, ResetFlag.Hp | ResetFlag.Position);
            SwitchBehaviorTree(currentLevel.editorData.resetStation.behavior.ToString(),currentLevel.editorData.resetStation.Args);
            BreakAction();
            if (creator.loadHide) gameObject.OnActive(false);
            if (!creator.notNav && navmesh != null)
            {
                navmesh.enabled = true;
            }
        }
    }
    
    public void ResetToBorn(bool addTired, Action callback)
    {
        if (AddStation(MonsterStation.ResetToBorn))
        {
            BreakAction();
            if (addTired)
            {
                if (!isTired && tiredCortine != null)
                {
                    StopCoroutine(tiredCortine);
                }

                isTired = true;
                StartCoroutine(RemoveTired());
            }

            SwitchFightState(FightState.Normal);
            MoveTo(MoveStyle.Walk,creator.transform.position, (statas, complete) =>
            {
                if (complete)
                {
                    LookAtPoint(creator.transform.forward + creator.transform.position);
                    SwitchBehaviorTree(currentLevel.editorData.resetStation.behavior.ToString(),currentLevel.editorData.resetStation.Args);
                    BreakAction();
                    SwitchStation(currentLevel.editorData.resetStation.bornStation);
                    callback?.Invoke();
                }
            });
        }

    }

    public void SwitchBehaviorTree(string behavior, params object[] args)
    {
        if (behavior == "None")
        {
            GetAgentCtrl<SimpleBehaviorCtrl>().SwitchBehavior(new Idle());
            return;
        }

        Type type = Type.GetType(behavior);
        ISimpleBehavior sb = (ISimpleBehavior) Activator.CreateInstance(type, null);
        GetAgentCtrl<SimpleBehaviorCtrl>().SwitchBehavior(sb, args);
    }
    
    
    private IEnumerator RemoveTired()
    {
        yield return new WaitForSeconds(1);
        isTired = false;
    }



    public void FlashToPoint(Vector3 point)
    {
        if (navmesh != null)
        {
            navmesh.Warp(creator.transform.position);
        }
        else
        {
            transform.position = point;
        }
    }


    /// <summary>
    /// 移动
    /// </summary>
    /// <param name="type"></param>
    /// <param name="point"></param>
    /// <param name="callback"></param>
    public virtual void MoveTo(Vector3 point, Action<NavMeshPathStatus, bool> callback)
    {
        MoveTo(moveStyle, point, callback);
    }

    private Tweener lookTweener;

    public virtual bool isProgressComplete
    {
        get { return !isAlive; }
    }


    public virtual void LookAtPoint(Vector3 point)
    {
        point = new Vector3(point.x, transform.position.y, point.z);
        if (lookTweener != null && lookTweener.IsActive())
        {
            //lookTweener.SetTarget(point);
            lookTweener.Kill();
        }
        lookTweener = transform.DOLookAt(point, 100 / currentLevel.attribute.rotateSpeed);


    }

    #endregion

    #region 看见玩家的回调

    protected virtual void OnEyeSenserTarget(ISensorTarget obj)
    {
    }

    protected virtual void OnEyeLoseTarget(ISensorTarget obj)
    {
        if (obj is PlayerSensorPoint)
        {
            seePlayerPointCount--;
        }
    }

    protected virtual void OnEyeFousTarget(ISensorTarget obj)
    {
        if (obj is PlayerSensorPoint)
        {
            seePlayerPointCount++;
        }
    }

    public AnimationPlay PlayAnimation(string key, int layer, AnimationFlag flag = 0)
    {
        for (int i = 0; i < animationKey.Count; i++)
        {
            if (animationKey[i].key == key)
            {
                return GetAgentCtrl<AnimatorCtrl>().Play(animationKey[i].animation, layer, flag);
            }
        }


        return null;
    }

    #endregion

    #region Editor

    public void LogFormat(string obj, params object[] args)
    {
        if (isLog)
        {
            GameDebug.LogFormat($"{this}{obj}", args);
        }
    }

    public void OnDrawGizmos()
    {
        if (isLog && Application.isPlaying && GamePlay.Instance.GMUI)
        {
            if (isAlive)
            {
                DrawTools.DrawText(transform.position, hp.ToString(), 30);
                for (int i = 0; i < ctrlList.Count; i++)
                {
                    ctrlList[i].OnDrawGizmos();
                }
            }
        }
    }
#if UNITY_EDITOR

    [Button]
    protected virtual void EditorInit()
    {
        _animator = gameObject.GetComponentInChildren<Animator>();
        var call = _animator.gameObject.AddOrGetComponent<AnimationCallback>();
        call.SetParmator();
        _navmesh = gameObject.AddOrGetComponent<NavMeshAgent>();
        _navmesh.enabled = false;

        //加感受器
        eye = transform.FindOrNew("Eye").gameObject.AddOrGetComponent<AgentSee>();
        eye.transform.localEulerAngles = new Vector3(0, 0, 0);
        eye.transform.localPosition = new Vector3(0, 1.738366f, -0.01125622f);
        //eye.seeType = EyeSeeType.Distance;
        eye.layerMask = MaskLayer.obstacal;
        ear = transform.FindOrNew("Ear").gameObject.AddOrGetComponent<AgentHear>();
        ear.transform.localEulerAngles = new Vector3(0, 0, 0);
        ear.transform.localPosition = new Vector3(0, 1.738366f, -0.01125622f);
        ear.layerMask = MaskLayer.obstacal;

        transform.SetLayer("Monster", true);
        transform.SetTag("Monster", true);
        if(animationKey==null) animationKey = new List<AnimationKeyValue>();
        if (!animationKey.Contains(st => st.key == "alarm"))
            animationKey.Add(new AnimationKeyValue() {key = "alarm"});
        if (!animationKey.Contains(st => st.key == "look"))
            animationKey.Add(new AnimationKeyValue() {key = "look"});
        if (!animationKey.Contains(st => st.key == "roar"))
            animationKey.Add(new AnimationKeyValue() {key = "roar"});
        if (!animationKey.Contains(st => st.key == "hit_body"))
            animationKey.Add(new AnimationKeyValue() {key = "hit_body"});
        if (!animationKey.Contains(st => st.key == "hit_leg"))
            animationKey.Add(new AnimationKeyValue() {key = "hit_leg"});
        if (!animationKey.Contains(st => st.key == "hit_head"))
            animationKey.Add(new AnimationKeyValue() {key = "hit_head"});
        if (!animationKey.Contains(st => st.key == "die_head"))
            animationKey.Add(new AnimationKeyValue() {key = "die_head"});
        if (!animationKey.Contains(st => st.key == "die_body"))
            animationKey.Add(new AnimationKeyValue() {key = "die_body"});

        
        GameObject collider = transform.FindOrNew("PlayerCollider").gameObject;
        CapsuleCollider playerCollider = collider.GetComponent<CapsuleCollider>();
        if (playerCollider == null)
        {
            playerCollider = collider.AddComponent<CapsuleCollider>();
            playerCollider.direction = 1;
        }

        collider.layer = LayerMask.NameToLayer("PlayerBlock");
    }
    [Button]
    private void SetLog()
    {
        this.isLog = !isLog;
    }
    [Button]
    private void SelectAudio()
    {
        for (int i = 0; i < allAudio.Length; i++)
        {
            if (UnityEditor.Selection.activeObject == allAudio[i].gameObject)
            {
                UnityEditor.Selection.activeObject = allAudio[(i + 1) % allAudio.Length].gameObject;
            }
        }
    }
#endif

    #endregion

    [Button]
    public void SetAudio()
    {
        GameObject goo = new GameObject("HeadAudio");
        goo.transform.SetParent(transform);
        goo.transform.localPosition = Vector3.zero;
        AgentAudio agg = goo.AddOrGetComponent<AgentAudio>();
        agg.key = "Head";
        AudioSource sss = goo.AddOrGetComponent<AudioSource>();
        sss.spatialBlend = 1;
        sss.maxDistance = 15;
        sss.rolloffMode = AudioRolloffMode.Linear;
        for (int i = 0; i < allAudio.Length; i++)
        {
            if (allAudio[i].key == "Head")
            {
                var ttt = allAudio[i].gameObject;
                GameObject.DestroyImmediate(allAudio[i]);
                GameObject.DestroyImmediate(ttt.GetComponent<AudioSource>());
                allAudio[i] = agg;
            }
        }
    }

    public bool isVisiable
    {
        get { return true; }
    }

    private List<ISensorTarget> _canSeeTarget = new List<ISensorTarget>();
    public List<ISensorTarget> canSeeTarget
    {
        get
        {
            if (_canSeeTarget.IsNullOrEmpty())
            {
                _canSeeTarget.AddRange(Player.player.crouchSensorPoints);
                _canSeeTarget.AddRange(Player.player.standSensorPoints);
            }

            return _canSeeTarget;
            // for (int i = 0; i < Player.player.CheckPoins.Count; i++)
            // {
            //     var point = Player.player.CheckPoins[i];
            //     if (point.gameObject.activeInHierarchy)
            //     {
            //         _canSeeTarget.Add(Player.player.CheckPoins[i]);
            //     }
            // }
            //
            // return _canSeeTarget;
        }
    }

    public bool isSenserable
    {
        get { return isAlive && !gameObject.IsNullOrDestroyed(); }
    }

    // private void OnDisable()
    // {
    //     onDisable?.Invoke(this);
    // }

    public event Action<ISensorTarget> onDisable;

    public Vector3 targetPoint
    {
        get
        {
            return centerPoint.transform.position;
        }
    }
}