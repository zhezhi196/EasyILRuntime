using System;
using Module;
using Project.Data;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

public class ProtectedHuman : Agent, IBasement, IStateMechineObject, IAgentAudioObject, INavMoveObject, IStationObject<BaseStation>, ISimpleBehaviorObject, ISkillObject, IPlayerHurt,IMonsterHurtObject
{
    public static bool isChangeAtt;
    [TabGroup("信息")] public int dbId = 20001;
    [SerializeField, TabGroup("状态")] private GameAttribute _finalAtt;
    [SerializeField, TabGroup("状态")] private BaseStation _station;
    [SerializeField, TabGroup("挂点")] private NavMeshAgent _navMesh;
    [SerializeField, TabGroup("挂点")] private Animator _animator;
    [SerializeField, TabGroup("挂点")] public AnimationCallback _animationCallback;
    public AnimationCallback animationCallback => _animationCallback;
    [TabGroup("状态")] public GameAttribute baseAttribute;
    [TabGroup("状态")] public GameAttribute growupAttribute;
    [TabGroup("状态"),SerializeField] private float _toPlayerDistance;
    public override bool isVisiable => true;

    public float toPlayerDistance
    {
        get { return _toPlayerDistance; }
    }

    public GameAttribute finalAtt
    {
        get
        {
            if (isChangeAtt)
            {
                isChangeAtt = false;
                _finalAtt = baseAttribute * (1 + growupAttribute);
            }

            return _finalAtt;
        }
    }

    public override string logName => "Wife";
    public NavMeshAgent navmesh => _navMesh;
    public Vector3 moveDirection => navmesh.steeringTarget - transform.position;

    public BaseStation station => _station;
    public Animator animator => _animator;
    public bool canReceiveEvent => isAlive;
    public float animatorSpeed => 1;
    public float rotateToMove => 90;
    public float stopMoveDistance => 0;
    public float rotateSpeed => finalAtt.rotateSpeed;
    public float moveSpeed => finalAtt.moveSpeed;
    public bool canReleaseSkill => true;
    
    public bool canBeHurt => true;

    public bool ContainStation(BaseStation station)
    {
        return (this.station & station) == station;
    }

    public bool AddStation(BaseStation station)
    {
        if (ContainStation(station)) return false;
        this._station = this.station | station;
        OnSwitchStation();
        LogFormat("添加状态{0}", station);
        return true;
    }

    public bool RemoveStation(BaseStation station)
    {
        if (!ContainStation(station)) return false;
        this._station = this.station & ~station;
        OnSwitchStation();
        LogFormat("移除状态{0}", station);
        return true;
    }

    public void Creat(PlayerCreator creator)
    {
        if (initStation == InitStation.UnActive)
        {
            _initStation = InitStation.OnlyCreat;
            transform.position = creator.transform.position;
            transform.rotation = creator.transform.rotation;
            var att=AttributeHelper.GetAttributeByType(DataInit.Instance.GetSqlService<PlayerData>().WhereID(dbId));
            baseAttribute = att[0];
            growupAttribute = att[1];
            AddCtrl();
        } 
    }

    protected override void AddCtrl()
    {
        ctrlList.Add(new SkillCtrl(this));
        ctrlList.Add(new SimpleBehaviorCtrl(this));
        ctrlList.Add(new NavMoveCtrl(this));
        ctrlList.Add(new StateMachineCtrl(this));
    }

    public void Born()
    {
        if (initStation >= InitStation.OnlyCreat)
        {
            isChangeAtt = true;
            _initStation = InitStation.Normal;
            ResetBornValue();
        }
    }

    private void ResetBornValue()
    {
        _hp = finalAtt.hp;
    }

    public bool SwitchStation(BaseStation station)
    {
        if (this._station != station)
        {
            this._station = station;
            OnSwitchStation();
            return true;
        }

        return false;
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


    public AgentAudio GetAudio(string name)
    {
        throw new NotImplementedException();
    }

    public float GetAudioVolume(AgentAudio tar)
    {
        throw new NotImplementedException();
    }

    public bool GetAudioActive(AgentAudio tar)
    {
        throw new NotImplementedException();
    }
    
    public bool SwitchMoveStyle(MoveStyle style)
    {
        if (isAlive && this.moveStyle != style)
        {
            this.moveStyle = style;
            GetAgentCtrl<AnimatorCtrl>().SetFloat("moveStyle", (int) style, 0.2f);
            OnSwitchStation();
            LogFormat("切换移动样式{0}", style);
            return true;
        }

        return false;
    }

    public void Move(Vector3 postion)
    {
        NavMoveCtrl moveCtrl = GetAgentCtrl<NavMoveCtrl>();

        if (moveCtrl.MoveTo(postion, (status, complete) =>
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

    protected override void Update()
    {
        base.Update();
        _toPlayerDistance = transform.position.HorizonDistance(Player.player.transform.position);
        Player.player.ChangeAngerDetatime(_toPlayerDistance < finalAtt.angerRadius);
    }

    public IActiveSkill RefreshReadySkill()
    {
        var skillCtrl = GetAgentCtrl<SkillCtrl>();
        for (int i = 0; i < skillCtrl.allSkill.Count; i++)
        {
            if (skillCtrl.allSkill[i] is IActiveSkill active && active.isWanted)
            {
                return active;
            }
        }

        return null;
    }

    public void OnReleasingSkill(ISkill skill)
    {
    }

    public Damage OnHurt(Damage damage)
    {
        return damage;
    }

    public MonsterDamage OnHurt(MonsterDamage damage)
    {
        return damage;
    }

    public bool isSenserable
    {
        get
        {
            return isAlive;
        }
    }

    public event Action<ISensorTarget> onDisable;
}