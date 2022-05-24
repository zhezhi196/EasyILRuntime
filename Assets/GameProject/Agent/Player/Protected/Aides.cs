using System;
using Module;
using PLAYERSKILL;
using Project.Data;
using SimBehavior;
using Sirenix.OdinInspector;
using StationMachine;
using UnityEngine;
using UnityEngine.AI;

public class Aides : Agent, IProtection, INavMoveObject, ISkillObject, ISimpleBehaviorObject, IStateMechineObject, IStationObject<SonStation>
{
    [TabGroup("其他")] public bool isChangedAtt;

    [TabGroup("信息")] public int dbId = 20001;
    [TabGroup("信息")] public float maxToPlayerDistance = 5;

    [SerializeField, TabGroup("挂点")] private NavMeshAgent _navMesh;
    [SerializeField, TabGroup("挂点")] private Animator _animator;
    [SerializeField, TabGroup("挂点")] public AnimationCallback _animationCallback;
    public AnimationCallback animationCallback => _animationCallback;
    [TabGroup("状态")] public GameAttribute _baseAttribute;
    [TabGroup("状态")] public GameAttribute _growAttribute;
    [TabGroup("状态")] public GameAttribute _finaleAttribute;
    [TabGroup("状态")] public float toPlayerDistance;
    [TabGroup("状态")] public float toTargetDistance;
    [TabGroup("状态")] public float toTargetAnger;
    [TabGroup("状态")] public float normalAttackDistance;
    [SerializeField, TabGroup("状态")] private SonStation _station;
    public PlayerData dbData;

    public GameAttribute finalAttribute
    {
        get
        {
            if (isChangedAtt)
            {
                _finaleAttribute = _baseAttribute + (1 + _growAttribute);
            }

            return _finaleAttribute;
        }
    }

    public IPlayerTarget target
    {
        get
        {
            if (Player.player.target != Player.baseMent)
            {
                return Player.player.target;
            }

            return null;
        }
    }
    
    public override bool isVisiable => true;
    
    public float rotateToMove => 90;

    public float stopMoveDistance
    {
        get
        {
            if (target == null || IsTooFar())
            {
                return 2;
            }
            else
            {
                return normalAttackDistance;
            }
        }
    }

    public float rotateSpeed
    {
        get
        {
            if (moveSpeed == 0)
            {
                return 0;
            }
            else
            {
                return finalAttribute.rotateSpeed;
            }
        }
    }

    public float moveSpeed
    {
        get
        {
            if (ContainStation(SonStation.Attack))
            {
                return 0;
            }

            return finalAttribute.moveSpeed;
        }
    }

    public NavMeshAgent navmesh => _navMesh;
    public Vector3 moveDirection => navmesh.steeringTarget - transform.position;


    public bool canReleaseSkill
    {
        get { return target != null && !ContainStation(SonStation.Attack); }
    }
    
    public Animator animator => _animator;
    public bool canReceiveEvent => isAlive;
    public float animatorSpeed => 1;
    public SonStation station => _station;

    public bool isSenserable
    {
        get { return false; }
    }

    public event Action<ISensorTarget> onDisable;

    public override string logName => "Son";

    public void Creat(PlayerCreator creator)
    {
        if (initStation == InitStation.UnActive)
        {
            _initStation = InitStation.OnlyCreat;
            transform.position = creator.transform.position;
            transform.rotation = creator.transform.rotation;
            AddCtrl();
            dbData = DataInit.Instance.GetSqlService<PlayerData>().WhereID(dbId);
            var att = AttributeHelper.GetAttributeByType(dbData);
            _baseAttribute = att[0];
            _growAttribute = att[1];
        }
    }

    public void Born()
    {
        if (initStation >= InitStation.OnlyCreat)
        {
            isChangedAtt = true;
            _initStation = InitStation.Normal;
            ResetValue();
            GetAgentCtrl<SimpleBehaviorCtrl>().SwitchBehavior(new FollowPlayer(this));

            PlayerAideSkill.CurrSkill(PlayerType.Aide).LoadSkillInstance(sd =>
            {
                GetAgentCtrl<SkillCtrl>().AddSkill(sd);
                normalAttackDistance = sd.maxHurtDistance;
            });
        }
    }

    public void ResetValue()
    {
        _hp = finalAttribute.hp;
    }

    protected override void AddCtrl()
    {
        ctrlList.Add(new SkillCtrl(this));
        ctrlList.Add(new StateMachineCtrl(this,new Attack1(0,"atk03")));
        ctrlList.Add(new NavMoveCtrl(this));
        ctrlList.Add(new SimpleBehaviorCtrl(this));
    }

    protected override void Update()
    {
        toPlayerDistance = transform.position.HorizonDistance(Player.player.transform.position);
        if (target != null)
        {
            toTargetDistance = transform.position.Distance(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z));
            Vector3 dir = target.transform.position - transform.position;
            toTargetAnger = Vector3.Angle(dir, new Vector3(transform.forward.x, dir.y, transform.forward.z));
        }
        else
        {
            toTargetDistance = float.MaxValue;
            toTargetAnger = 360;
        }
        base.Update();
    }


    public IActiveSkill RefreshReadySkill()
    {
        if (target == null) return null;
        Skill skill = GetAgentCtrl<SkillCtrl>().allSkill[0];

        if (skill is IActiveSkill act && act.isWanted)
        {
            return act;
        }

        return null;
    }

    public void OnReleasingSkill(ISkill skill)
    {
    }

    public float GetTranslateTime(string name)
    {
        return 0.1f;
    }

    public float GetLayerFadeTime(int type, string name)
    {
        return 0.1f;
    }

    public float getBreakTranslation(int layer, string name)
    {
        return 0;
    }

    public string GetLayerDefaultAnimation(int layer)
    {
        return "Normal";
    }

    public void OnBreakAnamition(int layer, string name, bool sendEvent)
    {
    }


    public bool SwitchStation(SonStation station)
    {
        if (this._station != station)
        {
            this._station = station;
            OnSwitchStation();
            return true;
        }

        return false;
    }

    public bool ContainStation(SonStation station)
    {
        return (this.station & station) == station;
    }

    public bool AddStation(SonStation station)
    {
        if (ContainStation(station)) return false;
        this._station = this.station | station;
        OnSwitchStation();
        LogFormat("添加状态{0}", station);
        return true;
    }

    public bool RemoveStation(SonStation station)
    {
        if (!ContainStation(station)) return false;
        this._station = this.station & ~station;
        OnSwitchStation();
        LogFormat("移除状态{0}", station);
        return true;
    }

    public void FollowPlayer()
    {
        MoveToPoint(Player.player.transform.position);
    }

    public void MoveToPoint(Vector3 moveTarget)
    {
        NavMoveCtrl moveCtrl = GetAgentCtrl<NavMoveCtrl>();
        if (moveCtrl.MoveTo(moveTarget, (status, complete) =>
        {
            if (complete)
            {
                StopMove();
            }
        }))
        {
            SwitchMoveStyle(MoveStyle.Run);
        }
    }
    
    public void StopMove()
    {
        if (SwitchMoveStyle(MoveStyle.Idle))
        {
            GetAgentCtrl<NavMoveCtrl>().StopMove();
        }
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

    public bool IsTooFar()
    {
        return toPlayerDistance >= maxToPlayerDistance;
    }

}