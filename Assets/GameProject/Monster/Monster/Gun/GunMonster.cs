using System;
using System.Collections.Generic;
using DG.Tweening;
using Module;
using Project.Data;
using Sirenix.OdinInspector;
using UnityEngine;

public class GunMonster : MonoBehaviour, IMonster, ISkillObject,ISimpleBehaviorObject
{
    private MonsterLevel _currentLevel;
    private List<ISensorTarget> _canSeeTarget = new List<ISensorTarget>();
    [SerializeField, TabGroup("其他")] private bool _isLog;
    [SerializeField, TabGroup("状态")] private MonsterStation _station;
    [SerializeField, TabGroup("状态")] private float _seePlayerTime;
    [TabGroup("状态"), SerializeField] public float missPlayerTime;
    [SerializeField, TabGroup("状态"),ReadOnly,LabelText("离玩家距离")] 
    public float toPlayerDistance = float.MaxValue;
    [SerializeField, TabGroup("状态"),ReadOnly,LabelText("与玩家的角度")] 
    public float toPlayerAnger;
    [SerializeField, TabGroup("状态")] public FightState _fightState;
    [SerializeField, TabGroup("状态")] public int seePlayerPointCount;

    private ITarget _target;

    public ITarget target
    {
        get
        {
            return _target;
        }
        set
        {
            if (_target != value)
            {
                _target = value;
                GetAgentCtrl<SkillCtrl>().BreakSkill(BreakSkillFlag.BreakAction | BreakSkillFlag.WithAnimation);
            }
        }
    }
    [SerializeField, TabGroup("挂点")] public MonsterCreator creator;
    [SerializeField, TabGroup("挂点")] public Transform horRotate;
    [SerializeField, TabGroup("挂点")] public Transform vecRotate;
    [SerializeField, TabGroup("挂点")] public Transform zero;
    [SerializeField, TabGroup("挂点")] public AgentSee eye;
    [TabGroup("信息")] //丢失掉玩家视野后多长时间会不跟踪玩家
    public float missPlayerDelayTime = 5;
    [LabelText("默认阶段")] [TabGroup("信息"), SerializeField]
    private List<MonsterLeveEditor> defaultLevel;

    public float seePlayerFightTime
    {
        get { return currentLevel.dbData.alertTime1; }
    }

    public FightState showUiState
    {
        get
        {
            if (target != null && target == Player.player)
            {
                return fightState;
            }
            return FightState.Normal;
        }
    }

    public List<MonsterLeveEditor> defaultLevels
    {
        get { return defaultLevel; }
        set { defaultLevel = value; }
    }

    private InitStation _initStation;

    public MonsterLevel currentLevel => _currentLevel;
    public bool isAlive => initStation == InitStation.Normal;
    public bool isProgressComplete => true;
    public bool canAss { get; }
    public FightState fightState => _fightState;

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

    public bool isVisiable => false;
    public bool isSenserable => false;
    public MonsterStation station => _station;
    public event Action onSwitchStation;
    public List<IAgentCtrl> ctrlList = new List<IAgentCtrl>();
    public LineRenderer line;
    public Transform look;
    public T GetAgentCtrl<T>() where T : IAgentCtrl
    {
        for (int i = 0; i < ctrlList.Count; i++)
        {
            if (ctrlList[i] is T t)
            {
                return t;
            }
        }

        return default;
    }

    public event Action<ISensorTarget> onDisable;

    public List<ISensorTarget> canSeeTarget
    {
        get
        {
            if (Player.player == null || Player.player.crouchSensorPoints == null || Player.player.standSensorPoints == null) return _canSeeTarget;
            MonsterCtrl ctrl = BattleController.GetCtrl<MonsterCtrl>();
            if (ctrl != null)
            {
                if (_canSeeTarget.Count != ctrl.exitMonster.Count + Player.player.crouchSensorPoints.Count+Player.player.standSensorPoints.Count)
                {
                    _canSeeTarget.Clear();
                    for (int i = 0; i < ctrl.exitMonster.Count; i++)
                    {
                        if (ctrl.exitMonster[i] is ISensorTarget sensor)
                        {
                            _canSeeTarget.Add(sensor);
                        }
                    }
                    _canSeeTarget.AddRange(Player.player.crouchSensorPoints);
                    _canSeeTarget.AddRange(Player.player.standSensorPoints);
                }

            }

            return _canSeeTarget;
        }
    }

    public virtual bool isSeePlayer => isAlive&& Player.player != null  && (seePlayerPointCount > Player.player.CheckPoins.Count * 0.5f || ContainStation(MonsterStation.IsSeePlayHide));

    [Button]
    public void SetLog()
    {
        isLog = !isLog;
    }
    private void Start()
    {
        EventCenter.Register<AttackMonster, TimeLineType>(EventKey.MonsterTimeLine,OnMosterTimeLine);
        EventCenter.Register<AttackMonster, TimeLineType>(EventKey.MonsterEndTimeLine,OnMosterEndTimeLine);
    }

    private void OnDestroy()
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
        AudioPlay.MuteAudio(sd => sd.ID == gameObject, true);
    }

    private void OnMosterEndTimeLine(AttackMonster arg1, TimeLineType arg2)
    {
        RemoveStation(MonsterStation.TimeLine);
    }

    private void OnMosterTimeLine(AttackMonster arg1, TimeLineType arg2)
    {
        AddStation(MonsterStation.TimeLine);
    }

    public void ResetToBorn()
    {
        _seePlayerTime = 0;
        _fightState = FightState.Normal;
        GetAgentCtrl<SimpleBehaviorCtrl>().SwitchBehavior(new Idle());
        target = null;
    }

    public void OnCreat(MonsterCreator creator)
    {
        this.creator = creator;
        if (initStation == InitStation.UnActive)
        {
            initStation = InitStation.OnlyCreat;
            creator.ReceiveEvent();
            ctrlList.Add(new SkillCtrl(this));
            ctrlList.Add(new SimpleBehaviorCtrl(this));
            eye.onFousTarget += OnEyeFousTarget;
            eye.onLoseTarget += OnEyeLoseTarget;
            eye.onSenserTarget += OnEyeSenserTarget;
            _currentLevel = new MonsterLevel(0, defaultLevels[0], this);
            if (currentLevel.dbData.visualDistance1 != 0)
            {
                eye.includeSights.Add(new DistanceEyeSight(eye.transform, currentLevel.dbData.visualDistance1, currentLevel.dbData.visualRange1));
            }
            
            if (currentLevel.dbData.visualDistance2 != 0)
            {
                eye.includeSights.Add(new DistanceEyeSight(eye.transform, currentLevel.dbData.visualDistance2, currentLevel.dbData.visualRange2));
            }
        }
    }
    
    public void Born()
    {
        if (initStation == InitStation.OnlyCreat)
        {
            initStation = InitStation.Normal;
            List<Skill> skillTemp = new List<Skill>();
            if (!currentLevel.editorData.skills.IsNullOrEmpty())
            {
                for (int i = 0; i < currentLevel.editorData.skills.Count; i++)
                {
                    skillTemp.Add(GameObject.Instantiate(currentLevel.editorData.skills[i]));
                }
            }
            else
            {
                skillTemp.AddRange(defaultLevel[0].skills);
            }

            GetAgentCtrl<SkillCtrl>().SetSkill(skillTemp.ToArray());
            //GetAgentCtrl<SkillCtrl>().SetSkill(currentLevel.editorData.skills.ToArray());
        }
    }

    public void StopAttack()
    {
        if (initStation == InitStation.Normal)
        {
            initStation = InitStation.Dead;
            GetAgentCtrl<SkillCtrl>().BreakSkill(0);
            for (int i = 0; i < ctrlList.Count; i++)
            {
                ctrlList[i].OnAgentDead();
            }
        }
    }

    private void OnEyeSenserTarget(ISensorTarget obj)
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

    private Vector3 tempRotate;

    public void LookAtPoint(Vector3 point)
    {
        look.transform.LookAt(point);
        Vector3 tar = new Vector3(horRotate.transform.eulerAngles.x, look.transform.eulerAngles.y, horRotate.transform.eulerAngles.z);
        //Vector3 vecTar = new Vector3(look.transform.eulerAngles.x, 0, 0);
        horRotate.transform.eulerAngles = Vector3.SmoothDamp(horRotate.transform.eulerAngles, tar, ref tempRotate, 0.5f);
        //vecRotate.transform.localEulerAngles = Vector3.SmoothDamp(vecRotate.transform.eulerAngles, vecTar, ref tempVecRotate, 0.5f);
        vecRotate.transform.localEulerAngles = new Vector3(look.transform.localEulerAngles.x, 0, 0);
        isAim = Mathf.Abs((horRotate.transform.eulerAngles.y - look.transform.eulerAngles.y)) <= 10;
        
        //vecRotate.transform.forward = new Vector3(0, dir.y, 0);
    }

    public bool isAim;

    private void Update()
    {
        if (!isAlive) return;
        if (Player.player != null)
        {
            toPlayerDistance = Vector3.Distance(Player.player.chasePoint, transform.position);
            toPlayerAnger = Vector3.Angle(new Vector3(Player.player.transform.forward.x, 0, Player.player.transform.forward.z), new Vector3(transform.forward.x, 0, transform.forward.z));
        }

        RefreshTarget();

        if (target != null)
        {
            if (fightState == FightState.Normal)
            {
                _fightState = FightState.Alert;
                GetAgentCtrl<SimpleBehaviorCtrl>().SwitchBehavior(new ShotGunChase());
            }
            else
            {
                _seePlayerTime += GetDelatime(false);
                if (_seePlayerTime >= seePlayerFightTime)
                {
                    _seePlayerTime = seePlayerFightTime;
                    _fightState = FightState.Fight;
                }
            }
        }
        else
        {
            ResetToBorn();
        }

        if (initStation == InitStation.Normal)
        {
            for (int i = 0; i < ctrlList.Count; i++)
            {
                ctrlList[i].OnUpdate();
            }
        }
    }

    public void RefreshTarget()
    {
        if (isSeePlayer)
        {
            target = Player.player;
        }
        else
        {
            if (target == null)
            {
                target = eye.onViewTarget.First(fd => fd is AttackMonster);
            }
            else
            {
                if ((target is AttackMonster monster && !monster.isAlive) || target is Player player)
                {
                    target = null;
                }
            }
        }
    }
    

    // private void OnDisable()
    // {
    //     onDisable?.Invoke(this);
    // }

    public virtual bool AddStation(MonsterStation station)
    {
        if (ContainStation(station)) return false;
        this._station = this.station | station;
        onSwitchStation?.Invoke();
        return true;
    }

    public bool RemoveStation(MonsterStation station)
    {
        if (!ContainStation(station)) return false;
        this._station = this.station & ~station;
        onSwitchStation?.Invoke();
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
            return true;
        }

        return false;
    }

    public string logName
    {
        get
        {
            return creator.id + "GunShoter";
        }
    }

    public bool isLog
    {
        get
        {
            return _isLog;
        }
        set
        {
            _isLog = value;
            eye.isLog = value;
        }
    }

    public void LogFormat(string obj, params object[] args)
    {
        if (isLog)
        {
            GameDebug.LogFormat(obj, args);
        }
    }

    public float timeScale
    {
        get { return TimeHelper.timeScale; }
    }

    public float GetUnscaleDelatime(bool ignorePause)
    {
        if (ignorePause)
        {
            return TimeHelper.unscaledDeltaTimeIgnorePause;
        }
        else
        {
            return TimeHelper.unscaledDeltaTime;
        }
    }

    public float GetDelatime(bool ignorePause)
    {
        if (ignorePause)
        {
            return TimeHelper.deltaTimeIgnorePause;
        }
        else
        {
            return TimeHelper.deltaTime;
        }
    }

    public void SetTimescale(float timeScale)
    {
        TimeHelper.timeScale = timeScale;
    }

    public Tweener SetTimescale(float timeScale, float time)
    {
        return TimeHelper.SetTimeScale(timeScale, time);
    }

    public bool canReleaseSkill
    {
        get { return isAlive && !ContainStation(MonsterStation.TimeLine); }
    }

    public IActiveSkill RefreshReadySkill()
    {
        SkillCtrl skilCtrl = GetAgentCtrl<SkillCtrl>();
        for (int i = 0; i < skilCtrl.allSkill.Count; i++)
        {
            if (skilCtrl.allSkill[i] is IActiveSkill active)
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


    public Vector3 targetPoint => transform.position;
}