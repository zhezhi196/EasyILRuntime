using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Module;
using DG.Tweening;
using System;
using Sirenix.OdinInspector;
using Project.Data;
using System.Text;
using UnityEngine.AI;
using UnityEngine.Rendering;
using GameGift;

/// <summary>
/// 游戏内角色
/// </summary>
public class Player : MonoBehaviour, InteractiveSource, ITarget, ILocalSave
{
    private static Player _player;
    public static Player player
    {
        get
        {
            if (_player == null)
            {

                if (BattleController.Instance.ctrlProcedure.mode == GameMode.Main)
                {
                    PlayerCtrl ctrl = BattleController.GetCtrl<PlayerCtrl>();
                    if (ctrl != null)
                    {
                        _player = BattleController.GetCtrl<PlayerCtrl>().player;
                    }
                }
                else {
                    BlackPlayerCtrl ctrl = BattleController.GetCtrl<BlackPlayerCtrl>();
                    if (ctrl != null)
                    {
                        _player = BattleController.GetCtrl<BlackPlayerCtrl>().player;
                    }
                }
               
            }
            return _player;
        }
    }
    
    #region StationEnum
    [Flags]
    public enum Station
    {
        Aim = 1,//瞄准.换武器可打断
        Crouch = 1 << 1,//蹲伏
        WaitingAttack = 1 << 2,//等待攻击,换弹,换武器可打断
        OpenBackpack = 1 << 3,//打开背包
        Reloading = 1 << 4,//换弹,换武器可打断
        Ass = 1 << 5,//暗杀
        Excute = 1 << 6,//处决
        Hiding = 1 << 7,//正在躲藏
        WeaponChanging = 1 << 8,//正在切换武器,不可打断
        Death = 1 << 9,//死亡状态
        Story = 1 << 10,//剧情动画
        Weak = 1 << 11,//虚弱
        Running = 1 << 12,//跑步
        Hurt = 1 << 13,//受击
        BowReloading = 1 << 14,//弩换弹
        MeleeAttack = 1 << 15,//近战攻击
        Stealth = 1 << 16,//隐身
        TimeCtrl = 1 << 17,//时间控制
        ShotGunReload = 1 << 18,//时间控制
        Dodge = 1 << 19,//闪避
        DragMove = 1<<20,//怪物技能抓钩
    }
    #endregion

    public Vector3 chasePoint
    {
        get
        {
            if (IsInCabinet)
            {
                return IsInCabinet.outPoint.position;
            }
            else
            {
                return transform.position;
            }
        }
    }

    /// <summary>
    /// 输入类型枚举 0=无,1=键鼠,2=ui
    /// </summary>
    public enum EInputType
    {
        None = 0,
        KeyBoard = 1,
        UI = 2
    }

    //--------------------角色设置-------------------------------
    #region 角色设置
    [Header("角色设置")]
    public GameCameraCtrl cameraCtrl;
    public GameWeaponManager weaponManager;
    public CharacterController characterController;
    public Rigidbody mRigidbody;
    public Transform cameraRoot;
    public PlayerArgConfig argConfig;
    //移动旋转相关
    [HideInInspector]
    public bool isMoving = false;
    public float moveSmoothTime = 0.1f;//移动数值阻尼变化
    private Vector2 moveValocity = Vector2.zero;
    private Vector2 currentMoveInput = Vector2.zero;
    [HideInInspector]
    public bool isRotate = false;
    public float rotateSmoothTime = 0.2f;//旋转阻尼
    private Vector2 rotateValocity = Vector2.zero;
    private Vector2 currentRotateInput = Vector2.zero;
    public Camera evCamera
    {
        get { return cameraCtrl.evCamera; }
    }
    [Header("Timeline配置")]
    public PlayerTimelineModel timelineModel;
    //public Animator timelineAnim;
    //public Transform timelineCamTrans;
    [Header("检测点")]
    public Transform standSensors;
    public Transform crouchSensors;
    public List<PlayerSensorPoint> standSensorPoints = new List<PlayerSensorPoint>();
    public List<PlayerSensorPoint> crouchSensorPoints = new List<PlayerSensorPoint>();
    public List<PlayerSensorPoint>CheckPoins
    {
        get {
            return isSquat ? crouchSensorPoints : standSensorPoints;
        }
    }
    public Transform eyePoint
    {
        get { return cameraRoot; }
    }
    /// <summary>
    /// 玩家当前中心点
    /// </summary>
    public Vector3 CenterPostion
    {
        get
        {
            return transform.position + characterController.center;
        }
    }
    /// <summary>
    /// Player检测点
    /// 包含player中心点,闪避检测点
    /// </summary>
    public List<PlayerPart> PlayerParts = new List<PlayerPart>();
    #endregion
    //---------------------角色状态---------------------------------
    #region 角色状态
    [Header("角色状态"), ReadOnly]
    public Weapon currentWeapon;//当前使用的武器
    [LabelText("移动速度"), ReadOnly] public float moveSpeed = 5f;//移动速度
    [ShowInInspector]
    public Station station { get; set; }
    [ReadOnly]
    public bool isSquat = false;//是否下蹲状态
    [ReadOnly]
    public bool isAim = false;//是否瞄准状态
    private GameAttribute playerBaseAtt;
    public GameAttribute BaseAtt
    {
        get { return playerBaseAtt; }
    }
    public GameAttribute playerAtt;
    [ReadOnly] public float hp = 0;
    private float maxHp = 0;
    public float MaxHp {

        get {
            return maxHp;
        }
    }
    [ReadOnly] public IntField maxHpUp;
    [ReadOnly] public float strength = 0;//体力
    private float maxStrength = 0;
    public float MaxStrength
    {
        get { return maxStrength; }
    }
    [ReadOnly] public IntField maxStrengthUp;
    [ReadOnly] public float energy = 0;//能量
    private float maxEnergy = 0;
    public float MaxEnergy
    {
        get { return maxEnergy; }
    }
    private Weapon lastWeapon;
    public bool IsAlive
    {
        get
        {
            return hp > 0;
        }
    }
    private HideProp playerHideProp;
    /// <summary>
    /// 玩家是否躲进了柜子
    /// </summary>
    public HideProp IsInCabinet
    {
        get
        {
            return playerHideProp;
        }
    }
    #endregion
    //----------------------逻辑参数-----------------------------------
    #region 逻辑参数
    private float mGravity = -9.81f;
    /// <summary>
    /// 当前输入状态,0=无,1=键鼠,2=ui
    /// </summary>
    private EInputType inputType = EInputType.None;
    private int audioIndex = 0;
    public bool NotChangeAim
    {   //没有持有武器,换枪,换弹中途不允许主动切换瞄准状态
        get { return (ContainStation(Station.WeaponChanging) || ContainStation(Station.Reloading) || ContainStation(Station.BowReloading)); }
    }
    [ReadOnly, ShowInInspector]
    private float tempSpeed = 1;
    private float moveMul = 1;//测试工具用
    public float MoveSpeed
    {
        get
        {
            tempSpeed = moveSpeed * (isSquat ? playerAtt.crouchMovek : 1f)
                * (IsRun ? playerAtt.runMoveK : 1f) * moveMul;// * ((isAim && !ContainStation(Station.Reloading) && !ContainStation(Station.BowReloading)) ? playerAtt.aimMoveK : 1f)
            return tempSpeed;
        }
    }
    /// <summary>
    /// 不可移动
    /// </summary>
    public bool NotMove
    {
        get { return impactMove || inTimeline || ContainStation(Station.Ass) || ContainStation(Station.Excute) || ContainStation(Station.Death) ||  ContainStation(Station.Story)
                || ContainStation(Station.Dodge) || ContainStation(Station.Hiding); }
    }
    /// <summary>
    /// 不可旋转
    /// </summary>
    public bool NotRotate
    {
        get { return ContainStation(Station.Ass) || ContainStation(Station.Excute) || ContainStation(Station.Death) || inTimeline; }
    }
    public bool canStand = true;
    public bool NotRun
    {
        get
        {
            return ContainStation(Station.Weak) || ContainStation(Station.Crouch)
              || !canStand || isSquat;//|| TeachingManager.teachingDic[TeachingName.RunTeaching].teachState == Teaching.TeachState.None
        }
    }
    /// <summary>
    /// 可以恢复体力
    /// </summary>
    public bool CanRecoverStrength
    {
        get
        {
            return strength < maxStrength && !isRun && !ContainStation(Station.Excute)
                && !ContainStation(Station.Dodge) && !ContainStation(Station.MeleeAttack);
        }
    }
    [ReadOnly]
    private bool isRun = false;
    public bool IsRun
    {
        set
        {
            isRun = value;
            if (isRun)
            {
                if (ContainStation(Station.Aim))
                    currentWeapon?.ChangeNoAim();
                AddStation(Station.Running);
            }
            else
            {
                RemoveStation(Station.Running);
            }
            currentWeapon?.SetRunStation(isRun);
        }
        get
        {
            return isRun;
        }
    }
    public Transform _cameraFllowTrans;
    public Transform cameraFllowTrans
    {
        get { return _cameraFllowTrans; }
    }
    private bool impactMove = false;
    private Vector3 impact = Vector3.zero;
    private bool inTimeline = false;
    public float forveMoveTime = 5f;//击飞差值时间
    private float t_hurt;//受击音效时间间隔
    private string dodgePartPath = "Agent/Player/PlayerDodgePart.prefab";
    #endregion
    //-----------------------事件---------------------------------------
    #region 事件
    public Action<float> onHpChange;
    public Action<float> onEnerggrChange;
    public Action<float> onStrengthChange;
    #endregion
    //---------------------游戏记录-------------------------------------
    #region 游戏记录
    public int hurtCount = 0;//受击次数
    public float hurtDamage = 0;//受到伤害
    public int skillGiftCount = 0;//使用技能次数,不包括拳击
    public float expendEnergy = 0;//消耗能量总量
    #endregion
    //------------------天赋影响数值--------------------------------------
    public FloatField runStepRang;//蹑足潜行天赋
    public FloatField walkStepTange;//蹑足潜行天赋
    public bool canAssMonster = false;//快速突袭天赋
    public bool maxRestore = false; //紧急治疗
    public FloatField dodgeTimeAdd;//灵巧闪避增加时间
    //-----------------------------------------------------------------

    #region 初始化,Update
    /// <summary>
    /// 角色初始化,武器初始化
    /// </summary>
    public void Init()
    {
        var playerSaveDataAdd = DataMgr.Instance.GetSqlService<PlayerSaveDataAdd>();
        maxHpUp = new IntField(playerSaveDataAdd.WhereID(1).arg1.ToInt());
        maxStrengthUp = new IntField(playerSaveDataAdd.WhereID(1).arg2.ToInt());
        runStepRang = new FloatField(0);
        walkStepTange = new FloatField(0);
        dodgeTimeAdd = new FloatField(0);
        ObjectPool.Cache(dodgePartPath, 2);
        PlayerPart part = this.gameObject.AddComponent<PlayerCenterPart>();
        PlayerParts.Add(part);
        cameraCtrl.Init(this);
        navMeshPath = new NavMeshPath();
        List<PlayerData> datas = DataMgr.Instance.GetSqlService<PlayerData>().tableList;
        playerBaseAtt = AttributeHelper.GetAttributeByType(datas[0])[0];
        BulletEntity.onBulletCountChanged += OnBulletChange;
        BattleController.GetCtrl<GiftCtrl>().OnStudyGift += OnStudyGift;
    }

    public void ShowPlayer(EnterNodeType enterType)
    {
        RefeshAttribute();
        //存档
        if (enterType == EnterNodeType.FromSave)
        {
            string data = LocalSave.Read(this);
            if (!string.IsNullOrEmpty(data))
            {
                //生命值，体力恢复到最大
                hp = playerAtt.hp + maxHpUp;
                strength = playerAtt.strength + maxStrengthUp;
                //精力,游戏记录,最大生命值增益
                string[] temp = data.Split(ConstKey.Spite1);
                //player.hp = temp[0].ToInt();
                if (temp.Length > 1)
                    energy = temp[1].ToInt();
                //if (temp.Length > 2)
                //    strength = temp[2].ToInt();
                if (temp.Length > 3)
                    hurtCount = temp[3].ToInt();
                if (temp.Length > 4)
                    hurtDamage = temp[4].ToFloat();
                if (temp.Length > 5)
                    expendEnergy = temp[5].ToFloat();
                if (temp.Length > 6)
                    skillGiftCount = temp[6].ToInt();
            }
        }
        else
        {
            hp = playerAtt.hp+maxHpUp;
            energy = playerAtt.energy;
            strength = playerAtt.strength+maxStrengthUp;
        }
        ChangeStrength(strength);//处理初始化虚弱bug
        if (weaponManager.ownWeapon.Count <= 0)
        {
            currentWeapon = weaponManager.defualtWeapon;
        }
        else
        {
            if (enterType == EnterNodeType.Restart)
            {   //新开游戏,手枪添加子弹
                Weapon w = weaponManager.FindWeapon(23002);
                if (w != null)
                {
                    if (w.weaponArgs.firstBulletCount > w.maxBulletCount)
                    {
                        w.bulletCount = w.maxBulletCount;
                        w.bullet.OwnBullet(BulletCreatType.Single, w.weaponArgs.firstBulletCount - w.maxBulletCount);
                    }
                    else
                    {
                        w.bulletCount = w.weaponArgs.firstBulletCount;
                    }
                }
            }
            for (int i = 0; i < weaponManager.weaponSolts.Count; i++)
            {
                if (weaponManager.weaponSolts[i].weapon != null)
                {
                    currentWeapon = weaponManager.weaponSolts[i].weapon;
                    break;
                }
            }
            if (currentWeapon == null)
            {
                currentWeapon = weaponManager.defualtWeapon;
            }
        }
        currentWeapon.TakeOut();
        _cameraFllowTrans = currentWeapon.cameraPoint;
        //ChangeWeapon(currentWeapon);
    }

    private void OnDestroy()
    {
        Async.StopAsync(gameObject);
        BulletEntity.onBulletCountChanged -= OnBulletChange;
        //重置渲染设置
        //if (GamePlay.Instance != null)
        //{
        //    GamePlay.Instance.renderData.rendererFeatures[0].SetActive(true);
        //    GamePlay.Instance.renderData.rendererFeatures[2].SetActive(false);
        //    GamePlay.Instance.renderData.rendererFeatures[1].SetActive(false);
        //}
    }

    private float stepRange = 1f;//
    public void Update()
    {
        var moncterCtrl = BattleController.GetCtrl<MonsterCtrl>();//脚步声传给怪物
        if (moncterCtrl != null)
        {
            if (isSquat)
                stepRange = argConfig.squatStepRange;
            else if (isRun)
                stepRange = argConfig.runStepRange*Mathf.Clamp(1 - runStepRang.value,0,1);
            else
                stepRange = argConfig.stepRange * Mathf.Clamp(1 - walkStepTange.value, 0, 1);
            moncterCtrl.TrySensorMonster(SensorMonsterType.Walk, transform.position, stepRange);
        }
        if (t_hurt > 0)//受击音效计时
        {
            t_hurt -= TimeHelper.unscaledDeltaTime;
        }
        RefehsMovement();
        RefeshRotate();
        if (CanRecoverStrength)
        {
            ChangeStrength(maxStrength* argConfig.strengthRecover * TimeHelper.unscaledDeltaTime);
        }
        AudioRoom.PlayerPos = transform.position;
        //if (energy < maxEnergy)
        //    ChangeEnergy(argConfig.energyRecover * TimeHelper.unscaledDeltaTime);
#if UNITY_EDITOR
        {
            //Move
            if (inputType != EInputType.UI)
            {
                Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
                if (moveInput.x != 0 || moveInput.y != 0 )
                {
                    if (!NotMove)
                    {
                        if (!this.moveInput && inputType == EInputType.None)
                        {
                            StartMove(EInputType.KeyBoard);
                        }
                        if (inputType == EInputType.KeyBoard)
                        {
                            Movement(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")), EInputType.KeyBoard);
                        }
                    }
                }
                else
                {
                    if (this.moveInput && inputType == EInputType.KeyBoard)
                    {
                        StopMove();
                    }
                }
            }
            //Cancel Run
            //if (Input.GetKeyUp(KeyCode.LeftShift))
            //{
            //    IsRun = false;
            //}
            if (inTimeline)
                return;
            //Crouch
            if (Input.GetKeyDown(KeyCode.C))
            {
                StandOrSquat();
            }
            //Aim
            //if (Input.GetMouseButtonDown(1))
            //{
            //    AimBtnDown();
            //}
            //Fire
            if (Input.GetKeyDown(KeyCode.Space))
            {
                AttackBtnDown();
            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
                AttackBtnUp();
            }
            //Reload
            if (Input.GetKeyDown(KeyCode.R))
            {
                ReloadBtnDown();
            }
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {

            }
            if (Input.GetKeyDown(KeyCode.V))
            {
                MeleeAttackBtnDown();
            }
            if (Input.GetKeyUp(KeyCode.V))
            {
                MeleeAttackBtnUp();
            }
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                Dodge();
            }
            if (Input.GetKeyDown(KeyCode.F1))
            {
                //开门,场景道具交互
                TryOpenDoor();
            }
            //切换武器
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                EditorChangeWeapon(0);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                EditorChangeWeapon(1);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                EditorChangeWeapon(2);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                EditorChangeWeapon(3);
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                EditorChangeWeapon(4);
            }

        }
#endif
    }

    private float radius = 0.3f;
    private void LateUpdate()
    {
        if (impactMove)
        {
            if (impact.magnitude > 0.2) characterController.Move(impact * TimeHelper.unscaledDeltaTime);
            else impactMove = false;
            impact = Vector3.Lerp(impact, Vector3.zero, forveMoveTime * TimeHelper.unscaledDeltaTime);
        }

        if (dragMoveing && dragTarget!=null)
        {
            transform.position = dragTarget.position - characterController.center;
            return;
        }

        if (characterController.enabled && !characterController.isGrounded)
            characterController.Move(new Vector3(0, mGravity, 0) * TimeHelper.unscaledDeltaTime);//*Time.deltaTime

        if (isSquat)//如果下蹲了,检查是否可以站起来
        {
            Ray ray;
            ray = new Ray(transform.position, transform.up);
            if (Physics.Raycast(ray, argConfig.standHeight + 0.2f, MaskLayer.PlayerShot))
            {
                canStand = false;
                return;
            }
            ray = new Ray(transform.position + transform.forward * radius, transform.up);
            if (Physics.Raycast(ray, argConfig.standHeight + 0.2f, MaskLayer.PlayerShot))
            {
                canStand = false;
                return;
            }
            ray = new Ray(transform.position - transform.forward * radius, transform.up);
            if (Physics.Raycast(ray, argConfig.standHeight + 0.2f, MaskLayer.PlayerShot))
            {
                canStand = false;
                return;
            }
            ray = new Ray(transform.position + transform.right * radius, transform.up);
            if (Physics.Raycast(ray, argConfig.standHeight + 0.2f, MaskLayer.PlayerShot))
            {
                canStand = false;
                return;
            }
            ray = new Ray(transform.position - transform.forward * radius, transform.up);
            if (Physics.Raycast(ray, argConfig.standHeight + 0.2f, MaskLayer.PlayerShot))
            {
                canStand = false;
                return;
            }
            canStand = true;
        }
        else
        {
            canStand = true;
        }
    }

#if UNITY_EDITOR
    private void EditorChangeWeapon(int index)
    {
        if (weaponManager.weaponSolts.Count > index && weaponManager.weaponSolts[index].weapon != null)
        {
            ChangeWeapon(weaponManager.weaponSolts[index].weapon);
        }
    }
#endif
    #endregion

    #region 属性改变,属性计算,伤害计算,使用注射器
    [Button("刷新属性")]
    public void RefeshAttribute()
    {
        playerAtt = GetPlayerAttribute;
        moveSpeed = playerAtt.moveSpeed;
        if (maxHp < playerAtt.hp + maxHpUp)
        {
            float addHp = playerAtt.hp + maxHpUp - maxHp;
            maxHp = playerAtt.hp + maxHpUp;
            ChangeHp(addHp);
        }
        if (maxEnergy < playerAtt.energy)
        {
            ChangeEnergy(playerAtt.energy - maxEnergy);
            maxEnergy = playerAtt.energy;
        }
        if (maxStrength < playerAtt.strength+maxStrengthUp)
        {
            float addS = playerAtt.strength + maxStrengthUp - maxStrength;
            maxStrength = playerAtt.strength+ maxStrengthUp;
            ChangeStrength(addS);
        }
    }

    public GameAttribute GetPlayerAttribute
    {
        get
        {
            GameAttribute att = playerBaseAtt;
            //todo--区分模式获取属性
            //属性天赋直接相加
            List<Gift> attGift = BattleController.GetCtrl<GiftCtrl>().attGift;
            //GameAttribute growatt = default;
            for (int i = 0; i < attGift.Count; i++)
            {
                //growatt += attGift[i].growUpAttribute;
                att += attGift[i].baseAttribute;
            }
            //att *= (1 + growatt);
            return att;
        }
    }

    public Hurtmaterial hurtMaterial => Hurtmaterial.Meat;
    /// <summary>
    /// 根据武器返回伤害
    /// </summary>
    /// <param name="weapon">武器</param>
    /// <param name="hurtObject">要伤害的对象</param>
    /// <param name="isAss">是否是暗杀</param>
    /// <returns>返回伤害</returns>
    public Damage CreateDamage(Weapon weapon, IHurtObject hurtObject, bool isAss = false)
    {
        GameAttribute att = playerAtt + weapon.weaponAttribute;//角色属性+天赋属性+武器属性升级后属性
                                                               //todo--区分模式获取属性
        List<Gift> attGift = BattleController.GetCtrl<GiftCtrl>().buffGift;
        GameAttribute growatt = new GameAttribute(0);
        for (int i = 0; i < attGift.Count; i++)
        {
            growatt += attGift[i].GetGrowAtt(weapon, (hurtObject is MonsterPart)? (hurtObject as MonsterPart):null);
        }
        att *= (1 + growatt);//计算天赋属性增益
        Damage weaponDamage = new Damage();
        weaponDamage.weapon = weapon.weaponType;
        //伤害计算
        //DamageDetail damageDetail = new DamageDetail(0, att.gunAtt);
        if (weapon.weaponType == WeaponType.MeleeWeapon)
        {
            if (isAss)
            {
                weaponDamage.damage = att.assDamage;
            }
            else {
                weaponDamage.damage = att.meleeAtt;
            }
        }
        else
        {
            if (hurtObject is HeadPart)
            {
                weaponDamage.damage = att.gunAtt * att.gunHeadAttK;
            }
            else
            {
                weaponDamage.damage = att.gunAtt;
            }
        }
        //计算暴击伤害
        bool b = RandomHelper.RandomValue(att.violentAttP);
        if (b)
        {
            weaponDamage.damage = weaponDamage.damage * att.violentAttK;
        }
        //硬直伤害
        weaponDamage.lagDamage = att.paralysis;
        weaponDamage.force = weapon.weaponArgs.attackForce;
        return weaponDamage;
    }

    /// <summary>
    /// 玩家受击
    /// </summary>
    /// <param name="target"></param>
    /// <param name="damage"></param>
    /// <param name="type">击中类型,对应UI受击特效</param>
    /// <param name="zhengtuo">是否可以挣脱,1为可挣脱</param>
    /// <param name="canExc">是否可以处决玩家,1为不可处决</param>
    public void OnHurt(PlayerDamage damage)
    {
        GameDebug.LogError("玩家受到伤害------>" + damage.damage);
        if (damage.monster != null && !damage.monster.isAlive) return;

        if (inTimeline || hp <= 0)
            return;
        EventCenter.Dispatch(EventKey.OnPlayerHurt, damage.attackType);
        RefeshCheckPart();
#if LOG_ENABLE
        if (godMod)
            return;
#endif
        hurtCount += 1;
        hurtDamage += damage.damage;
        ChangeHp(-damage.damage);
        if (IsInCabinet)//柜子受击
        {
            if (hp <= 0)
            {
                //柜子处决
                ExitHideProp();
                OnPlayerDeath(damage.monster, damage.excuteAnim);//死亡动画
            }
            else {
                //柜子受击
                ExitHideProp();
                GetoutMonster(damage.monster as AttackMonster);
            }
            return;
        }
        //普通受击
        if (hp <= 0)
        {
            OnPlayerDeath(damage.monster, damage.excuteAnim);//死亡动画
        }
        else if (damage.outAnim)//挣脱
        {
            GetoutMonster(damage.monster as AttackMonster);
        }
        else //普通受击
        {
            if (t_hurt <= 0)
            {
                t_hurt = argConfig.hurtAudioTime;
                PlayAudio(argConfig.hurtAudio[UnityEngine.Random.Range(0, argConfig.hurtAudio.Length)]);
            }
        }
    }
    //百分比扣除生命值
    public void OnHurt(float f,int type =0)
    {
        if (inTimeline || hp<=0)
            return;
        RefeshCheckPart();
#if LOG_ENABLE
        if (godMod)
            return;
#endif
        hurtCount += 1;
        hurtDamage += MaxHp * f;
        ChangeHp(-(hurtDamage));
        if (t_hurt <= 0)
        {
            t_hurt = argConfig.hurtAudioTime;
            PlayAudio(argConfig.hurtAudio[UnityEngine.Random.Range(0, argConfig.hurtAudio.Length)]);
        }
        if (hp <= 0)
        {
            OnPlayerDeath(null,false);
        }
        EventCenter.Dispatch(EventKey.OnPlayerHurt, type);
    }
    //使用吗啡注射器
    public async void UseMorphine(float f)
    {
        if (maxRestore)//紧急治疗天赋恢复满生命
        {
            ChangeHp(MaxHp);
        }
        else
        {
            ChangeHp(f);
        }
        inTimeline = true;
        UIController.Instance.canPhysiceback = false;
        AddStation(Station.Story);
        currentWeapon.TakeBack();
        timelineModel.Show(PlayerTimelineModel.ShowModel.Morphine);
        _cameraFllowTrans = timelineModel.camerTrans;
        timelineModel._anim.CrossFade("Morphine", 0.1f);
        await Async.WaitforSecondsRealTime(1.15f, gameObject);
        _cameraFllowTrans = currentWeapon.cameraPoint;
        UIController.Instance.canPhysiceback = true;
        timelineModel.Hide();
        currentWeapon.TakeOut();
        _cameraFllowTrans = currentWeapon.cameraPoint;
        RemoveStation(Station.Story);
        inTimeline = false;
    }

    private void RefeshCheckPart()
    {
        for (int i = PlayerParts.Count-1; i >0; i--)
        {
            if (PlayerParts[i].partType == PlayerPartType.DodgePoint)
            {
                PlayerParts[i].Remove();
            }
        }
    }
    /// <summary>
    /// 击飞
    /// </summary>
    /// <param name="dir">击飞方向</param>
    /// <param name="force">击飞的力</param>
    [Button("击飞测试")]
    public void AddImpact(Vector3 dir, float force)
    {
        dir.Normalize();
        if (dir.y < 0) dir.y = -dir.y;
        impact += dir.normalized * force;
        impactMove = true;
    }
    /// <summary>
    /// 体力值改变
    /// </summary>
    /// <param name="f"></param>
    public void ChangeStrength(float f)
    {
        strength = Mathf.Clamp(strength + f, 0, maxStrength);
        onStrengthChange?.Invoke(f);
        if (strength <= 0)
        {
            AddStation(Station.Weak);
        }
        if (strength >= maxStrength)
        {
            RemoveStation(Station.Weak);
        }
    }
    /// <summary>
    /// 能量值改变
    /// </summary>
    /// <param name="f"></param>
    public void ChangeEnergy(float f)
    {
        energy = Mathf.Clamp(energy + f, 0, playerAtt.energy);
        onEnerggrChange?.Invoke(f);
        if (f < 0)
        {
            expendEnergy += f;
        }
    }
    public float ChangeHp(float change)
    {
        hp = Mathf.Clamp(hp + change, 0, MaxHp);
        onHpChange?.Invoke(change);
        return hp;
    }
    [Button("增加生命上限")]
    //增加生命上限
    public void AddMaxHp(int f)
    {
        maxHpUp += f;
        var psave = DataMgr.Instance.GetSqlService<PlayerSaveDataAdd>();
        psave.Update(d => d.ID == 1, "arg1", maxHpUp.value.ToString());
        RefeshAttribute();
        (UIController.Instance.Get("GameUI").viewBase as ProjectUI.GameUI).playerStateUI.Refesh();
        ChangeHp(f);
    }
    //增加体力上限
    public void AddMaxStrength(int f)
    {
        maxStrengthUp += f;
        var psave = DataMgr.Instance.GetSqlService<PlayerSaveDataAdd>();
        psave.Update(d => d.ID == 1, "arg2", maxStrengthUp.value.ToString());
        RefeshAttribute();
        (UIController.Instance.Get("GameUI").viewBase as ProjectUI.GameUI).playerStateUI.Refesh();
        ChangeStrength(f);
    }
    #endregion
    #region Station
    public event Action<Station> onAddStation;
    public event Action<Station> onRemoveStation;

    public bool ContainStation(Station station)
    {
        return (this.station & station) == station;
    }
    public bool AddStation(Station station)
    {
        if (ContainStation(station) || ContainStation(Station.Death)) return false;
        GameDebug.LogFormat("{0}添加状态{1}", gameObject.name, station);
        //移除状态
        PlayerStationConfig stationConfig = argConfig.GetRejectStation(station);
        if (stationConfig!=null)
        {
            for (int i = 0; i < stationConfig.outStations.Length; i++)
            {
                RemoveStation(stationConfig.outStations[i]);
            }
        }
        if (station == Station.Weak)
        {
            AudioPlay.Play("tired", transform).SetID("PlayerTired").SetLoop(true);
        }
        if (station == Station.Aim)
        {
            isAim = true;
            //DoEvCamFov(30f, 0.2f);
            PlayAudio("xiadun3");
        }
        if (station == Station.DragMove)
            dragMoveing = true;
        if (station == Station.Stealth)
            AddStealth();
        if(station == Station.Running)
            DoEvCamFov(70, 0.2f);
        this.station = this.station | station;
        onAddStation?.Invoke(station);
        return true;
    }

    public bool RemoveStation(Station station)
    {
        //移除状态
        PlayerStationConfig stationConfig = argConfig.GetBaseStation(station);
        if (stationConfig!=null)
        {
            for (int i = 0; i < stationConfig.outStations.Length; i++)
            {
                RemoveStation(stationConfig.outStations[i]);
            }
        }
        if (!ContainStation(station)) return false;
        GameDebug.LogFormat("{0}移除状态{1}", gameObject.name, station);
        if (station == Station.Weak)
        {
            AudioPlay.StopAudio(st =>
            {
                if (st.ID != null && st.ID.ToString() == "PlayerTired")
                {
                    return true;
                }
                return false;
            });
        }
        if (station == Station.Aim)
        {
            isAim = false;
            //DoEvCamFov(45f, 0.2f);
            PlayAudio("xiadun4");
        }
        if (station == Station.DragMove)
            dragMoveing = false;
        if (station == Station.Stealth)
            RemoveStealth();
        if (station == Station.Running)
            DoEvCamFov(45f, 0.2f);
        this.station = this.station & ~station;
        onRemoveStation?.Invoke(station);
        return true;
    }
    #endregion

    #region 角色下蹲,移动,旋转
    public void StandOrSquat()
    {
        cameraRoot.DOLocalMove(new Vector3(0, isSquat ?argConfig.standHeight : argConfig.squatHeight, 0), 0.3f).SetId(gameObject).SetUpdate(true);
        isSquat = !isSquat;
        standSensors.OnActive(!isSquat);
        crouchSensors.OnActive(isSquat);
        if (isSquat)
        {
            characterController.center = new Vector3(0, argConfig.squatHeight * 0.5f, 0);
            characterController.height = argConfig.squatHeight;
            AddStation(Station.Crouch);
            //PlayAudio("xiadun");
            PlayAudio("xiadun2");
        }
        else
        {
            characterController.center = new Vector3(0, argConfig.standHeight * 0.5f, 0);
            characterController.height = argConfig.standHeight;
            RemoveStation(Station.Crouch);
        }
    }
    #region 移动
    Vector3 moveDir = Vector3.zero;
    private float t_move;//移动时间，计算脚步声
    float ss = 1f;//以站立不瞄准为基准的播放间隔系数
    bool moveInput = false;//移动输入
    //UI输入开始移动
    public void StartMove(EInputType type = EInputType.UI)
    {
#if UNITY_EDITOR
        if (inputType != EInputType.None && inputType != type)
            return;
        inputType = type;
#endif
        PlayWalkAudio();
        t_move = 0;
        moveInput = true;
    }
    //UI输入持续移动
    public void Movement(Vector2 moveDirection, EInputType type = EInputType.UI)
    {
#if UNITY_EDITOR
        if (inputType != EInputType.None && inputType != type)
            return;
#endif
        if (NotMove)
            moveDirection = Vector2.zero;
        if (isMoving != (currentMoveInput.sqrMagnitude > 0.001f))
        {
            isMoving = !isMoving;
            //Animator播放移动动画
        }
        currentMoveInput = Vector2.SmoothDamp(currentMoveInput, moveDirection, ref moveValocity, moveSmoothTime, Mathf.Infinity, TimeHelper.unscaledDeltaTime);
        //角色前后左右移动动画
        currentWeapon?.Movement(currentMoveInput);
        if (characterController.isGrounded)
        {
            moveDir = new Vector3(currentMoveInput.x, 0, currentMoveInput.y).normalized;//normalized标准化,避免斜向走加速问题
            moveDir = transform.TransformDirection(moveDir);
            moveDir *= (Time.timeScale > 0.01f) ? (MoveSpeed / Time.timeScale) : MoveSpeed;
        }
        else
        {
            moveDir = Vector3.zero;
        }
        //GameDebug.Log("MoveDir:" + moveDir);
        if (characterController.enabled && characterController.isGrounded)
            characterController.SimpleMove(moveDir);//*Time.deltaTime

        //脚步声间隔根据状态不同改变
       
        if (IsRun)
            ss = argConfig.runAudioTime;
        else if (isSquat)
            ss = argConfig.squatWalkAudioTime;
        else
            ss = argConfig.walkAudioTime;

        if (isMoving)
        {
            t_move += TimeHelper.unscaledDeltaTime;
            if (t_move >= ss)
            {
                PlayWalkAudio();
                t_move = 0;
            }
        }
        //体力消耗
        if (strength > 0 && isRun)
        {
            ChangeStrength(-argConfig.runStrengthExpend * TimeHelper.unscaledDeltaTime);
        }
    }
    //UI输入停止移动
    public void StopMove()
    {
#if UNITY_EDITOR
        if (inputType == EInputType.None)
            return;
        inputType = EInputType.None;
#endif
        t_move = 0;
        isMoving = false;
        moveInput = false;
    }
    //停止移动输入后，动画的平滑停止
    public void RefehsMovement()
    {
        if (!moveInput && currentMoveInput != Vector2.zero &&!inTimeline)
        {
            currentMoveInput = Vector2.SmoothDamp(currentMoveInput, Vector2.zero, ref moveValocity, moveSmoothTime / 2f, Mathf.Infinity,TimeHelper.unscaledDeltaTime);
            currentWeapon?.Movement(currentMoveInput);
        }
    }
    // 走路脚步声
    public void PlayWalkAudio()
    {
        //播放脚步声
        audioIndex += 1;
        if (argConfig.walkAudio.Length > 0)
        {
            if (audioIndex >= argConfig.walkAudio.Length)
                audioIndex = 0;
            PlayAudio(isSquat? argConfig.squatWalkAudio[audioIndex]: argConfig.walkAudio[audioIndex]);
        }
    }
    #endregion
    #region 旋转
    private bool rotateInput = false;
    public void StartRotate()
    {
        rotateInput = true;
    }

    public void Rotate(Vector2 v2)
    {
        if (NotRotate)
            return;
        if (isRotate != (currentRotateInput.sqrMagnitude > 0.001f))
        {
            isRotate = !isRotate;
            //设置角色旋转动画
            //SetAnimatorBool(PlayerAnimatorKey.Rotate, isRotate);
        }
        currentRotateInput = Vector2.SmoothDamp(currentRotateInput, v2, ref rotateValocity, rotateSmoothTime,Mathf.Infinity,TimeHelper.unscaledDeltaTime);
        //旋转动画融合
        //playerAnima.SetFloat("RotateV", currentRotateInput.y / 100f);
        //playerAnima.SetFloat("RotateH", currentRotateInput.x / 100f);
        Vector3 rotateDir = new Vector3(-v2.y, v2.x, 0);
        currentWeapon?.Rotate(currentRotateInput/100f);
        //float x = cameraRoot.localEulerAngles.x + (isAim ? argConfig.aimRotateSpeed*(1f+Setting.controllerSetting.aimSensitivity.value) :
        //    argConfig.rotateSpeed * Setting.controllerSetting.sensitivity.value) * rotateDir.x;//*灵敏度设置
        //float y = transform.localEulerAngles.y + (isAim ? argConfig.aimRotateSpeed * (1f + Setting.controllerSetting.aimSensitivity.value) :
        //    argConfig.rotateSpeed * Setting.controllerSetting.sensitivity.value) * rotateDir.y;//*灵敏度设置
        float x = cameraRoot.localEulerAngles.x + argConfig.rotateSpeed * Setting.controllerSetting.sensitivity.value * rotateDir.x;//*灵敏度设置
        float y = transform.localEulerAngles.y + argConfig.rotateSpeed * Setting.controllerSetting.sensitivity.value * rotateDir.y;//*灵敏度设置
        if (x > 180)
        {
            x = x - 360;
        }
        cameraRoot.localEulerAngles = new Vector3(Mathf.Clamp(x, Mathf.Min(-60f, 60f), Mathf.Max(-60f, 60f)), 0);
        transform.localEulerAngles = new Vector3(0, y);
    }
    public void StopRotate()
    {
        rotateInput = false;
    }

    public void RefeshRotate()
    {
        if (!rotateInput && currentRotateInput != Vector2.zero &&!inTimeline)
        {
            currentRotateInput = Vector2.SmoothDamp(currentRotateInput, Vector2.zero, ref rotateValocity, rotateSmoothTime / 2f, Mathf.Infinity, TimeHelper.unscaledDeltaTime);
            //旋转动画融合
            currentWeapon?.Rotate(currentRotateInput/100f);
            //playerAnima.SetFloat("RotateV", currentRotateInput.y / 100f);
            //playerAnima.SetFloat("RotateH", currentRotateInput.x / 100f);
            if (currentRotateInput.sqrMagnitude <= 10f)
            {
                isRotate = false;
                //设置角色旋转动画
                //SetAnimatorBool(PlayerAnimatorKey.Rotate, false);
            }
        }
    }
    #endregion

    #endregion

    #region 闪避
    private NavMeshPath navMeshPath;
    public float dodgeTime = 0.2f;
    public float dodgeSpeed = 50f;
    public void Dodge()
    {
        if (isSquat)//下蹲状态
        {
            if (!canStand)
                return;
            StandOrSquat();
        }
        if (ContainStation(Station.Dodge) || ContainStation(Station.Weak))
            return;
        ChangeStrength(-argConfig.dodgeExpend);
        Vector3 dodgeDir = transform.forward * -1f;
        if (Vector3.SqrMagnitude(currentMoveInput) > 0.001f)
        {
            //dodgeDir =new Vector3(currentMoveInput.x, 0, currentMoveInput.y).normalized;
            dodgeDir = (Quaternion.AngleAxis(transform.localEulerAngles.y, Vector3.up) * new Vector3(currentMoveInput.x, 0, currentMoveInput.y)).normalized;
        }
        //NavMeshRayCast(dodgeDir);
        //return;
        PhysicsDodge(dodgeDir);
        return;
        Vector3 basePos = transform.position + new Vector3(0, 0.2f, 0);
        Vector3 targetPos = basePos + dodgeDir * 5f;
        Ray ray;
        ray = new Ray(basePos, targetPos - basePos);
        RaycastHit hit;
        GameDebug.DrawRay(basePos, targetPos - basePos, Color.red, 1f);
        //判断是否有障碍物
        if (Physics.Raycast(ray, out hit, 5f))
        {
            GameDebug.DrawRay(hit.point + (basePos - targetPos) * 0.1f, Vector3.down, Color.yellow, 1f);
            //如果有障碍物,检测是否可到达障碍物位置
            ray = new Ray(hit.point + (basePos - targetPos) * 0.2f, Vector3.down);//对检测到的点进行偏移
            if (Physics.Raycast(ray, out hit, 2f))//对新点做射线检测
            {
                DodgeToTarger(hit.point);
            }
        }
        else
        {
            GameDebug.DrawRay(targetPos, Vector3.down, Color.green, 1f);
            ray = new Ray(targetPos, Vector3.down);//无障碍物,使用目标点
            if (Physics.Raycast(ray, out hit, 2f))//对目标点向下射线
            {

                DodgeToTarger(hit.point);
            }
        }
    }

    private void DodgeToTarger(Vector3 target)
    {
        if (NavMesh.CalculatePath(transform.position, target, NavMesh.AllAreas, navMeshPath))
        {
            //可以找到到目标点的路径,并且路径距离小于检测点距离1.1倍
            float navPathDis = 0;
            for (int i = 0; i < navMeshPath.corners.Length - 1; i++)
            {
                navPathDis += Vector3.Distance(navMeshPath.corners[i], navMeshPath.corners[i + 1]);
                GameDebug.DrawLine(navMeshPath.corners[i], navMeshPath.corners[i + 1], Color.blue, 2f);
            }
            if (navMeshPath.status == NavMeshPathStatus.PathComplete && navPathDis < Vector3.Distance(transform.position, target) * 1.1f
                && Vector3.Distance(navMeshPath.corners[navMeshPath.corners.Length - 1], target) < 0.2f)
            {
                GameObject debugObj = new GameObject("DebugObj01");
                debugObj.transform.position = navMeshPath.corners[navMeshPath.corners.Length - 1];
                Destroy(debugObj, 2f);
                characterController.enabled = false;
                transform.position = navMeshPath.corners[navMeshPath.corners.Length - 1];
                characterController.enabled = true;
                return;
            }
        }

        NavMeshHit navHit;
        if (NavMesh.FindClosestEdge(target, out navHit, NavMesh.AllAreas))
        {
            GameObject debugObj = new GameObject("DebugObj02");
            debugObj.transform.position = navHit.position;
            Destroy(debugObj, 2f);
            characterController.enabled = false;
            transform.position = navHit.position;
            characterController.enabled = true;
        }
    }

    private void NavMeshRayCast(Vector3 dir)
    {
        Vector3 target = transform.position + dir * 5f;
        GameDebug.DrawLine(transform.position, target, Color.red, 1f);
        NavMeshHit hit;
        bool blocked = false;
        blocked = NavMesh.Raycast(transform.position, target, out hit, NavMesh.AllAreas);
        if (blocked)
        {
            GameObject debugObj = new GameObject("DebugObj03");
            debugObj.transform.position = hit.position;
            Destroy(debugObj, 2f);
            characterController.enabled = false;
            transform.position = hit.position;
            characterController.enabled = true;
        }
        else
        {
            GameObject debugObj = new GameObject("DebugObj04");
            debugObj.transform.position = target;
            Destroy(debugObj, 2f);
            characterController.enabled = false;
            transform.position = target;
            characterController.enabled = true;
        }
    }

    private async void PhysicsDodge(Vector3 dir)
    {
        Vector3 dodgeDir = Vector3.zero;
        float tt = 0f;
        Vector3 target = transform.position + dir * 5f;
        AddStation(Station.Dodge);
        currentWeapon.StartDodge(dir);
        float dTime = dodgeTime + dodgeTimeAdd;
        AssetLoad.LoadGameObject<PlayerDodgePart>(dodgePartPath, null, (part, obj) =>
        {
            part.transform.position = CenterPostion;
            part.StartClock(this, dTime);
        });
        while (tt <= dTime)
        {
            dodgeDir = dir * dodgeSpeed / Time.timeScale;
            characterController.SimpleMove(dodgeDir);
            tt += Time.deltaTime;
            await Async.WaitForEndOfFrame();
        }
        RemoveStation(Station.Dodge);
        currentWeapon.EndDodge();
    }
    #endregion

    #region 被怪物拽走
    private bool dragMoveing = false;
    private Transform dragTarget;
    public void StartDragMove(Transform target,AttackMonster monster)
    {
        dragTarget = target;
        characterController.enabled = false;
        cameraRoot.DOLocalRotate(Vector3.zero, 0.2f).SetId(gameObject);
        Vector3 lookPos = new Vector3(monster.transform.position.x, transform.position.y, monster.transform.position.z);
        transform.rotation = Quaternion.LookRotation(lookPos-transform.position);
        if (isSquat)//下蹲状态
        {
            StandOrSquat();
        }
        AddStation(Station.DragMove);
    }

    public void EndDragMove()
    {
        RemoveStation(Station.DragMove);
        characterController.enabled = true;
        characterController.Move(Vector3.zero);
    }
    #endregion

    #region UI按钮响应,射击,瞄准,换弹
    public void AttackBtnDown()
    {
        currentWeapon?.AttackBtnDown();
    }

    public void AttackBtnUp()
    {
        currentWeapon?.AttackBtnUp();
    }

    public void AimBtnDown()
    {
        if (NotChangeAim)
            return;

        if (ContainStation(Player.Station.Aim))
        {
            currentWeapon?.ChangeNoAim();
        }
        else
        {
            currentWeapon?.ChangeToAim();
        }
    }

    public void ReloadBtnDown()
    {
        currentWeapon?.ReloadBtnDown();
    }

    public void MeleeAttackBtnDown()
    {
        //weaponManager.meleeWeapon?.AttackBtnDown();
    }

    public void MeleeAttackBtnUp()
    {
        //weaponManager.meleeWeapon?.AttackBtnUp();
    }
    #endregion

    #region 武器获得,武器升级,切换武器,近战攻击,隐身
    public void GetWeapon(int id)
    {
        Weapon w = weaponManager.OnGetWeapon(id);
        if (w != null)
        {
            ChangeWeapon(w);
        }
    }

    public void WeaponUpgrade(int id, int up)
    {
        Weapon w = weaponManager.FindWeapon(id);
        if (w != null)
        {
            w.Upgrade(up);
        }
    }

    public void ChangeWeapon(Weapon w)
    {
        if (w == currentWeapon)
            return;
        currentWeapon?.TakeBack();
        currentWeapon = w;
        //if (currentWeapon.weaponType != WeaponType.Empty)
        //    currentWeapon.gameObject.OnActive(true);
        //else
        //    currentWeapon.TakeOut();
        currentWeapon.TakeOut();
        _cameraFllowTrans = currentWeapon.cameraPoint;
    }

    public void RemvoveSlotWeapon(Weapon w)
    {
        Weapon nWeapon = weaponManager.RemoveSoltWeapon(w);
        if (nWeapon != null)
        {
            ChangeWeapon(nWeapon);
        }
        else {
            ChangeWeapon(weaponManager.defualtWeapon);
        }
    }

    public void TakeLastWeapon()
    { 
    
    }

    public void TakeCurrentWeapon()
    { 
    
    }

    public void StartMeleeGift()
    {

    }

    public void EndMeleeGift()
    {
        RemoveStation(Station.MeleeAttack);
        //weaponManager.meleeWeapon.TakeBack();
        currentWeapon.gameObject.OnActive(true);
        _cameraFllowTrans = currentWeapon.cameraPoint;
    }

    public async void AddStealth()
    {
        GamePlay.Instance.renderData.rendererFeatures[1].SetActive(true);
        GamePlay.Instance.renderData.rendererFeatures[2].SetActive(true);
        GamePlay.Instance.renderData.rendererFeatures[0].SetActive(false);
        await Async.WaitforSecondsRealTime(0.2f,gameObject);
        GamePlay.Instance.renderData.rendererFeatures[2].SetActive(false);
        GamePlay.Instance.renderData.rendererFeatures[0].SetActive(true);
        await Async.WaitforSecondsRealTime(0.1f, gameObject);
        GamePlay.Instance.renderData.rendererFeatures[2].SetActive(true);
        GamePlay.Instance.renderData.rendererFeatures[0].SetActive(false);
        await Async.WaitforSecondsRealTime(0.1f, gameObject);
        GamePlay.Instance.renderData.rendererFeatures[2].SetActive(false);
        GamePlay.Instance.renderData.rendererFeatures[0].SetActive(true);
        await Async.WaitforSecondsRealTime(0.1f, gameObject);
        GamePlay.Instance.renderData.rendererFeatures[2].SetActive(true);
        GamePlay.Instance.renderData.rendererFeatures[0].SetActive(false);
    }

    public async void RemoveStealth()
    {
        GamePlay.Instance.renderData.rendererFeatures[2].SetActive(false);
        GamePlay.Instance.renderData.rendererFeatures[0].SetActive(true);
        await Async.WaitforSecondsRealTime(0.2f, gameObject);
        GamePlay.Instance.renderData.rendererFeatures[2].SetActive(true);
        GamePlay.Instance.renderData.rendererFeatures[0].SetActive(false);
        await Async.WaitforSecondsRealTime(0.1f, gameObject);
        GamePlay.Instance.renderData.rendererFeatures[2].SetActive(false);
        GamePlay.Instance.renderData.rendererFeatures[0].SetActive(true);
        await Async.WaitforSecondsRealTime(0.1f, gameObject);
        GamePlay.Instance.renderData.rendererFeatures[2].SetActive(true);
        GamePlay.Instance.renderData.rendererFeatures[0].SetActive(false);
        await Async.WaitforSecondsRealTime(0.1f, gameObject);
        GamePlay.Instance.renderData.rendererFeatures[2].SetActive(false);
        GamePlay.Instance.renderData.rendererFeatures[0].SetActive(true);
        GamePlay.Instance.renderData.rendererFeatures[1].SetActive(false);
    }
    #endregion

    #region 躲藏
    /// <summary>
    /// 进入躲藏点
    /// </summary>
    /// <param name="hideProp">躲藏点</param>
    public void EnterHideProp(HideProp hideProp)
    {
        EventCenter.Dispatch<bool>(EventKey.PlayerHide, true);
        playerHideProp = hideProp;
        characterController.enabled = false;
        transform.position = playerHideProp.hidePoint.position;
        transform.rotation = playerHideProp.hidePoint.rotation;
        if (currentWeapon.weaponType != WeaponType.Empty)
        {
            currentWeapon.TakeBack();
            weaponManager.defualtWeapon.TakeOut();
            _cameraFllowTrans = weaponManager.defualtWeapon.cameraPoint;
        }
        AddStation(Station.Hiding);
    }
    /// <summary>
    /// 退出躲藏点
    /// </summary>
    public void ExitHideProp()
    {
        if (playerHideProp != null)
        {
            EventCenter.Dispatch<bool>(EventKey.PlayerHide, false);
            playerHideProp.Exit();
            transform.position = playerHideProp.outPoint.position;
            transform.rotation = playerHideProp.outPoint.rotation;
            if (currentWeapon.weaponType != WeaponType.Empty)
            {
                currentWeapon.TakeOut();
                weaponManager.defualtWeapon.TakeBack();
                _cameraFllowTrans = currentWeapon.cameraPoint;
            }
            playerHideProp = null;
            player.characterController.enabled = true;
            characterController.Move(Vector3.zero);
            RemoveStation(Station.Hiding);
        }
    }

    public void HideHurt()
    {
        ExitHideProp();
    }
    #endregion

    #region Timeline:暗杀怪物,处决怪物,被怪物处决,剧情,挣脱动画
    //暗杀怪物
    public void AssMonster(AttackMonster monster)
    {
        if (inTimeline)
            return;
        Weapon w =  weaponManager.FindWeapon(WeaponType.MeleeWeapon);
        if (w != null)
        {
            inTimeline = true;
            var assDamage = CreateDamage(w, monster.monsterParts[0], true);
            bool b = monster.Ass((int)assDamage.damage);
            BattleController.GetCtrl<TimelineCtrl>().GetAssTimeline(monster, b, (timeline) =>
            {
                if (timeline != null)
                {
                    characterController.enabled = false;
                    AddStation(Station.Ass);
                    cameraRoot.DOLocalRotate(Vector3.zero, 0.2f).SetId(gameObject);
                    Vector3 lookPos = new Vector3(transform.position.x, monster.transform.position.y, transform.position.z);
                    monster.transform.rotation = Quaternion.LookRotation(monster.transform.position - lookPos);
                    timeline.transform.position = monster.transform.position;
                    timeline.transform.rotation = monster.transform.rotation;
                    transform.position = timeline.playerPoint.position;
                    transform.rotation = timeline.playerPoint.rotation;
                    currentWeapon.TakeBack();
                    timelineModel.Show(PlayerTimelineModel.ShowModel.Weapon);
                    _cameraFllowTrans = timelineModel.camerTrans;
                    //timelineAnim.gameObject.OnActive(true);
                    //_cameraFllowTrans = timelineCamTrans;
                    bool lastSquat = false;
                    if (isSquat)//站起来
                    {
                        lastSquat = true;
                        StandOrSquat();
                    }
                    EventCenter.Dispatch<AttackMonster, TimeLineType>(EventKey.MonsterTimeLine, monster, TimeLineType.Ass);
                    timeline.Play(this, monster, () =>
                    {
                        characterController.enabled = true;
                        characterController.Move(Vector3.zero);
                        EventCenter.Dispatch<AttackMonster, TimeLineType>(EventKey.MonsterEndTimeLine, monster, TimeLineType.Ass);
                        timelineModel.Hide();
                        //timelineAnim.gameObject.OnActive(false);
                        currentWeapon.TakeOut();
                        _cameraFllowTrans = currentWeapon.cameraPoint;
                        monster.AssEnd((int)assDamage.damage, timeline.type == TimeLineType.Ass ? 0 : 1);
                        RemoveStation(Station.Ass);
                        inTimeline = false;
                        if (lastSquat)//恢复下蹲状态
                        {
                            StandOrSquat();
                        }
                    });
                }
                else
                {
                    inTimeline = false;
                    GameDebug.Log("未找到怪物暗杀动画");
                }
            });
        }
    }
    //处决怪物
    public void ExcuteMonster(AttackMonster monster,Damage d)
    {
        if (inTimeline)
            return;
        inTimeline = true;
        monster.Exc();
        BattleController.GetCtrl<TimelineCtrl>().GetExcuteTimeline(monster, (timeline) =>
        {
            if (timeline != null)
            {
                characterController.enabled = false;
                AddStation(Station.Excute);
                cameraRoot.DOLocalRotate(Vector3.zero, 0.2f).SetId(gameObject);
                Vector3 lookPos = new Vector3(transform.position.x, monster.transform.position.y, transform.position.z);
                monster.transform.rotation = Quaternion.LookRotation(lookPos - monster.transform.position);
                timeline.transform.position = monster.transform.position;
                timeline.transform.rotation = monster.transform.rotation;
                transform.position = timeline.playerPoint.position;
                transform.rotation = timeline.playerPoint.rotation;
                currentWeapon.TakeBack();
                timelineModel.Show(PlayerTimelineModel.ShowModel.Weapon);
                _cameraFllowTrans = timelineModel.camerTrans;
                //timelineAnim.gameObject.OnActive(true);
                //_cameraFllowTrans = timelineCamTrans;
                bool lastSquat = false;
                if (isSquat)//站起来
                {
                    lastSquat = true;
                    StandOrSquat();
                }
                EventCenter.Dispatch<AttackMonster, TimeLineType>(EventKey.MonsterTimeLine, monster, TimeLineType.Exc);
                timeline.Play(this, monster, () =>
                {
                    characterController.enabled = true;
                    characterController.Move(Vector3.zero);
                    EventCenter.Dispatch<AttackMonster, TimeLineType>(EventKey.MonsterEndTimeLine, monster, TimeLineType.Exc);
                    timelineModel.Hide();
                    currentWeapon.TakeOut();
                    _cameraFllowTrans = currentWeapon.cameraPoint;
                    monster.ExcEnd((int)d.damage);
                    RemoveStation(Station.Excute);
                    inTimeline = false;
                    if (lastSquat)//恢复下蹲状态
                    {
                        StandOrSquat();
                    }
                });
            }
            else
            {
                inTimeline = false;
                GameDebug.Log("未找到怪物处决动画");
            }
        });
    }
    //挣脱怪物
    public void GetoutMonster(AttackMonster monster)
    {
        if (inTimeline)
            return;
        inTimeline = true;
        BattleController.GetCtrl<TimelineCtrl>().GetGetoutTimeline(monster, (timeline) =>
        {
            if (timeline != null)
            {
                characterController.enabled = false;
                AddStation(Station.Excute);
                cameraRoot.DOLocalRotate(Vector3.zero, 0.2f).SetId(gameObject);
                Vector3 lookPos = new Vector3(transform.position.x, monster.transform.position.y, transform.position.z);
                monster.transform.rotation = Quaternion.LookRotation(lookPos - monster.transform.position);
                timeline.transform.position = monster.transform.position;
                timeline.transform.rotation = monster.transform.rotation;
                transform.position = timeline.playerPoint.position;
                transform.rotation = timeline.playerPoint.rotation;
                currentWeapon.TakeBack();
                timelineModel.Show(PlayerTimelineModel.ShowModel.None);
                _cameraFllowTrans = timelineModel.camerTrans;
                EventCenter.Dispatch<AttackMonster, TimeLineType>(EventKey.MonsterTimeLine, monster, TimeLineType.GetOut);
                timeline.Play(this, monster, () =>
                {
                    characterController.enabled = true;
                    characterController.Move(Vector3.zero);
                    EventCenter.Dispatch<AttackMonster, TimeLineType>(EventKey.MonsterEndTimeLine, monster, TimeLineType.GetOut);
                    timelineModel.Hide();
                    currentWeapon.TakeOut();
                    _cameraFllowTrans = currentWeapon.cameraPoint;
                    RemoveStation(Station.Excute);
                    inTimeline = false;
                });
            }
            else
            {
                inTimeline = false;
                GameDebug.Log("未找到怪物挣脱动画");
            }
        });
    }
    public void PlayStoryTimeline(string key,Action callback)
    {
        inTimeline = true;
        TimelineController timeline = BattleController.GetCtrl<TimelineCtrl>().GetStoryTimeline(key);
        if (timeline != null && timeline.canPlay)
        {
            characterController.enabled = false;
            AddStation(Station.Story);
            cameraRoot.DOLocalRotate(Vector3.zero, 0.2f).SetId(gameObject).SetUpdate(true);
            transform.position = timeline.playerPoint.position;
            transform.rotation = timeline.playerPoint.rotation;
            currentWeapon.TakeBack();
            timelineModel.Show(PlayerTimelineModel.ShowModel.None);
            _cameraFllowTrans = timelineModel.camerTrans;
            if (isSquat)//站起来
            {
                StandOrSquat();
            }
            EventCenter.Dispatch<AttackMonster, TimeLineType>(EventKey.MonsterTimeLine, null, TimeLineType.Story);
            timeline.Play(this, null, () =>
            {
                characterController.enabled = true;
                EventCenter.Dispatch<AttackMonster, TimeLineType>(EventKey.MonsterEndTimeLine, null, TimeLineType.Story);
                timelineModel.Hide();
                currentWeapon.TakeOut();
                _cameraFllowTrans = currentWeapon.cameraPoint;
                inTimeline = false;
                callback?.Invoke();
                RemoveStation(Station.Story);
            });
        }
        else
        {
            callback?.Invoke();
            inTimeline = false;
            GameDebug.LogFormat("未找到指定剧情动画,或不可播放:{0}", key);
        }
    }

    #endregion

    #region 死亡复活流程
    //玩家死亡,判断是否死亡timeline
    public void OnPlayerDeath(ITarget target, bool canExc)
    {
        ChangeStrength(playerAtt.strength);//2022.3.10,王浩需求死亡恢复体力
        AudioPlay.StopPlayBGM();
        //AudioPlay.StopAudio();
        AnalyticsEvent.SendEvent(AnalyticsType.PlayerDead, null);
        EventCenter.Dispatch(EventKey.OnPlayerDeath);
        BattleController.Instance.Save(0);
        //inTimeline = true;
        if (target == null || !canExc)
        {
            //普通死亡
            PlayerDeathAnim();
            return;
        }
        if (target is AttackMonster monster)
        {
            PlayerDeathTimeline(monster);
        }
        else
        {
            PlayerDeathAnim();
        }
    }
    //玩家死亡timeline动画判定
    public void PlayerDeathTimeline(AttackMonster monster)
    {
        BattleController.GetCtrl<TimelineCtrl>().GetDeathTimeline(monster, (timeline) =>
        {
            if (timeline != null)
            {
                inTimeline = true;
                characterController.enabled = false;
                AddStation(Station.Death);
                cameraRoot.DOLocalRotate(Vector3.zero, 0.2f).SetId(gameObject);
                Vector3 lookPos = new Vector3(transform.position.x, monster.transform.position.y, transform.position.z);
                monster.transform.rotation = Quaternion.LookRotation(lookPos - monster.transform.position);
                timeline.transform.position = monster.transform.position;
                timeline.transform.rotation = monster.transform.rotation;
                transform.position = timeline.playerPoint.position;
                transform.rotation = timeline.playerPoint.rotation;
                currentWeapon.TakeBack();
                timelineModel.Show(PlayerTimelineModel.ShowModel.None);
                _cameraFllowTrans = timelineModel.camerTrans;
                if (isSquat)//站起来
                {
                    StandOrSquat();
                }
                EventCenter.Dispatch<AttackMonster, TimeLineType>(EventKey.MonsterTimeLine, monster, TimeLineType.KillPlayer);
                timeline.Play(this, monster, () =>
                {
                    characterController.enabled = true;
                    EventCenter.Dispatch<AttackMonster, TimeLineType>(EventKey.MonsterEndTimeLine, monster, TimeLineType.KillPlayer);
                    timelineModel.Hide();
                    currentWeapon.TakeOut();
                    _cameraFllowTrans = currentWeapon.cameraPoint;
                    WaitAlive();
                    inTimeline = false;
                });
            }
            else
            {
                //普通死亡
                PlayerDeathAnim();
                GameDebug.Log("未找到怪物处决动画");
            }
        });
    }
    //玩家死亡倒地动画
    public async void PlayerDeathAnim()
    {
        inTimeline = true;
        UIController.Instance.canPhysiceback = false;
        AddStation(Station.Death);
        cameraRoot.DOLocalRotate(Vector3.zero, 0.2f).SetId(gameObject);
        if (isSquat)//站起来
        {
            StandOrSquat();
        }
        currentWeapon.TakeBack();
        timelineModel.Show(PlayerTimelineModel.ShowModel.None);
        _cameraFllowTrans = timelineModel.camerTrans;
        timelineModel._anim.CrossFade("Death", 0.1f);
        await Async.WaitforSecondsRealTime(2.4f, gameObject);
        _cameraFllowTrans = currentWeapon.cameraPoint;
        UIController.Instance.canPhysiceback = true;
        inTimeline = false;
        WaitAlive();
    }
    private bool waitAD = true;
    public async void WaitAlive()
    {
        BattleController.Instance.ctrlProcedure.currentNode.nodeParent.playerCreator.TrySendEvent(SendEventCondition.Dead);
        UIController.Instance.canPhysiceback = false;
        if (BattleController.GetCtrl<TimelineCtrl>().deathStory != null)
        {
            BlackMaskChange.Instance.White();//白屏
        }
        else {
            BlackMaskChange.Instance.Black();//黑屏
        }
        
        BattleController.Instance.ctrlProcedure.OnPlayerDead();
        //移动到出生点
        characterController.enabled = false;
        player.transform.position = BattleController.Instance.ctrlProcedure.currentNode.nodeParent.playerCreator.transform.position;
        player.transform.eulerAngles = BattleController.Instance.ctrlProcedure.currentNode.nodeParent.playerCreator.transform.eulerAngles;
        characterController.enabled = true;
        characterController.Move(Vector3.zero);
        BattleController.Instance.Pause("PlayerDeath");
        ChangeHp(MaxHp);
        ChangeStrength(maxStrength);
        await Async.WaitforSecondsRealTime(0.3f, gameObject);
        PlayDeathStory();
    }

    //死亡复活广告弹窗
    //public void OpenRelifePop()
    //{
    //    SpriteLoader.LoadIcon("kanguanggao", sp =>
    //    {
    //        CommonPopup.PopupNoClose(Language.GetContent("701"), Language.GetContent("705"), null, new PupupOption(() =>
    //        {
    //            CommonPopup.Close();
    //            OpenSecPop();
    //        }, Language.GetContent("707")), new PupupOption(() =>
    //        {
    //            RewardBag reward= (RewardBag)Commercialize.GetRewardBag(DataMgr.CommonData(33006).ToInt());
    //            reward.GetReward((r) =>
    //            {
    //                if (r.result == IapResultMessage.Success)
    //                {
    //                    ChangeHp(MaxHp);
    //                    waitAD = false;
    //                    CommonPopup.Close();
    //                }
    //                //else {
    //                //    waitAD = false;
    //                //}
    //            });
    //        }, Language.GetContent("706"),sp));
    //    });

    //}

    //private void OpenSecPop()
    //{
    //    CommonPopup.Popup(Language.GetContent("701"), Language.GetContent("718"), null, new PupupOption(() =>
    //    {
    //        CommonPopup.Close();
    //        OpenRelifePop();
    //    }, Language.GetContent("703")), new PupupOption(() =>
    //    {
    //        CommonPopup.Close();
    //        waitAD = false;
    //    }, Language.GetContent("702")));
    //}
    /// <summary>
    /// 死亡剧情动画播放
    /// </summary>
    private async void PlayDeathStory()
    {
        //BattleController.GetCtrl<TimelineCtrl>().GetDeathStory((timeline) =>
        //{
        //    if (timeline != null)
        //    {
        //        timeline.Play(this, null, () =>
        //        {
        //            Resurgence();
        //        });
        //    }
        //    else {
        //        Resurgence();
        //    }
        //});
        if (BattleController.GetCtrl<TimelineCtrl>().deathStory != null)
        {
            await Async.WaitforSecondsRealTime(1f);
            BattleController.GetCtrl<TimelineCtrl>().deathStory.Play(this, null, () =>
            {
                BattleController.GetCtrl<TimelineCtrl>().NextDeathStory();
                Resurgence();
            });
        }
        else {
            Resurgence();
        }
    }

    public async void Resurgence()
    {
        await Async.WaitForEndOfFrame();
        BattleController.Instance.Continue("PlayerDeath");
        BattleController.Instance.Save(0);
        if (hp > 0)
        {
            BlackMaskChange.Instance.ShowText();//死亡文本
            await Async.WaitforSecondsRealTime(4f, gameObject);
        }
        UIController.Instance.canPhysiceback = true;
        //播放复活动画
        PlayWeakUp(() => { RemoveStation(Station.Death); });
        //关闭黑屏
        BlackMaskChange.Instance.Close();
        BattleController.Instance.ctrlProcedure.currentNode.nodeParent.playerCreator.TrySendEvent(SendEventCondition.PlayerBorn);
    }
    /// <summary>
    /// 播放电刑椅站起动画
    /// </summary>
    public void PlayWeakUp(Action callback = null)
    {
        TimelineController timeline = BattleController.GetCtrl<TimelineCtrl>().GetReviveAnimtion(transform.position);
        if (timeline != null)
        {
            AddStation(Station.Story);
            currentWeapon.TakeBack();
            timelineModel.Show(PlayerTimelineModel.ShowModel.None);
            inTimeline = true;
            characterController.enabled = false;
            cameraRoot.DOLocalRotate(Vector3.zero, 0.2f).SetId(gameObject);
            transform.position = timeline.playerPoint.position;
            transform.rotation = timeline.playerPoint.rotation;
            _cameraFllowTrans = timelineModel.camerTrans;
            timeline.Play(this, null, () =>
            {
                characterController.enabled = true;
                characterController.Move(Vector3.zero);
                timelineModel.Hide();
                currentWeapon.TakeOut();
                _cameraFllowTrans = currentWeapon.cameraPoint;
                inTimeline = false;
                RemoveStation(Station.Story);
                callback?.Invoke();
            });
        }
        else
        {
            //模型切换
            currentWeapon.TakeOut();
            timelineModel.Hide();
            _cameraFllowTrans = currentWeapon.cameraPoint;
            callback?.Invoke();
        }
    }
    #endregion
    public void PlayAudio(string name)
    {
        //AudioPlay.PlayOneShot(name, transform);
    }

    #region 存档

    public string localFileName => LocalSave.savePath;
    public string localGroup => "Player";


    public string localUid
        {
            get { return "Player"; }
        }
    public void Save()
    {
        LocalSave.Write(this);
    }

    public string GetWriteDate()
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(hp);//玩家生命
        builder.Append(ConstKey.Spite1);
        builder.Append(energy);
        builder.Append(ConstKey.Spite1);
        builder.Append(strength);
        builder.Append(ConstKey.Spite1);
        builder.Append(hurtCount);
        builder.Append(ConstKey.Spite1);
        builder.Append(hurtDamage);
        builder.Append(ConstKey.Spite1);
        builder.Append(expendEnergy);
        builder.Append(ConstKey.Spite1);
        builder.Append(skillGiftCount);
        //builder.Append(ConstKey.Spite1);
        //builder.Append(maxHpUp);
        return builder.ToString();
    } 
    #endregion
    #region 事件处理
    public void RunLogical(RunLogicalName logical, IEventCallback sender, RunLogicalFlag flag, params string[] args)
    {
        if (logical == RunLogicalName.TimeLine)
        {
            if (args.Length > 0)
            {
                string id = args[0];
                PlayStoryTimeline(id, () =>
                {

                });
            }
        }

        if (logical == RunLogicalName.Flash)
        {
            if (args.Length > 0)
            {
                Transform flashTrans = BattleController.Instance.ctrlProcedure.currentNode.GetPlayerFlashPoint(args[0].ToInt());
                characterController.enabled = false;
                transform.position = flashTrans.position;
                transform.rotation = flashTrans.rotation;
                characterController.enabled = true;
                characterController.Move(Vector3.zero);
            }
        }
        if (logical == RunLogicalName.HoldGun)
        {
            currentWeapon.TakeBack();
            currentWeapon = weaponManager.defualtWeapon;
            currentWeapon.TakeOut();
            _cameraFllowTrans = currentWeapon.cameraPoint;
        }
    }

    private void OnBulletChange(BulletEntity bullet,int count)
    {
        if (count > 0)
        {
            if (currentWeapon!=null && currentWeapon.weaponType != WeaponType.Empty && currentWeapon.weaponType != WeaponType.Thrown
                && currentWeapon.bulletCount <= 0 && currentWeapon.bulletType == bullet.bulletType)
            {
                ReloadBtnDown();
            }
            //获取砖头时,可以添加砖头
            if (bullet.bulletType == BulletType.Birck)
            {
                if (bullet.weapon.bulletCount == 0)
                {
                    bullet.Get(1);
                    Weapon w = weaponManager.OnGetWeapon(bullet.weapon.weaponID);
                    if (w != null && w.entity.collectionStation == CollectionStation.NewGet)
                    {
                        ChangeWeapon(w);
                    }
                }
            }
        }
    }

    private void OnStudyGift(Gift gift)
    {
        RefeshAttribute();
    }

    Tweener fovTween;
    private void DoEvCamFov(float target, float time)
    {
        GameDebug.LogFormat("DoCamFov:{0}", target);
        if (fovTween != null && fovTween.IsActive())
            fovTween.Kill();
        fovTween = evCamera.DOFieldOfView(target, time).SetId(gameObject).SetUpdate(true);
    }
    #endregion
    #region 测试工具
    bool openTools = false;
    float sizeK = 1f;
    bool godMod = false;
    Vector2 toolWidSize = new Vector2(800, 600);
    bool propTools = false;
    string toolsPropid="0";
    string toolsPropCount = "0";
    private void OnGUI()
    {
        if (!GamePlay.Instance.GMUI)
            return;
        if (GUI.Button(new Rect(Screen.width / 2 - 50, 60, 100, 50), "F1/点击开门"))
        {
            TryOpenDoor();
        }
        if (!openTools)
        {
            if (GUI.Button(new Rect(Screen.width / 2 - 50, 0, 100f * sizeK, 50 * sizeK), ""))
            {
                sizeK = Tools.GetScreenScale().x;
                openTools = true;
                propTools = false;
            }
        }
        else
        {
            GUI.BeginGroup(new Rect(Screen.width / 2 - toolWidSize.x * 0.5f * sizeK, Screen.height / 2 - toolWidSize.y * 0.5f * sizeK, toolWidSize.x * sizeK, toolWidSize.y * sizeK));
            GUI.Box(new Rect(0, 0, toolWidSize.x * sizeK, toolWidSize.y * sizeK), "(((((((((((っ•ω•)っ Σ(σ｀•ω•´)σ 起飞！");
            if (GUI.Button(new Rect(0, 0, 50 * sizeK, 30 * sizeK), "关闭"))
            {
                openTools = false;
            }
            if (GUI.Button(GetBtnRect(0, 0), "获取手枪"))
            {
                WeaponManager.AddWeapon(23002);
            }
            if (GUI.Button(GetBtnRect(0, 1), "获取HK416"))
            {
                WeaponManager.AddWeapon(23005);
            }
            if (GUI.Button(GetBtnRect(0, 2), "获取霰弹枪"))
            {
                WeaponManager.AddWeapon(23003);
            }
            if (GUI.Button(GetBtnRect(0, 3), "获取砖头"))
            {
                WeaponManager.AddWeapon(23006);
            }
            if (GUI.Button(GetBtnRect(0, 4), "获取弩"))
            {
                WeaponManager.AddWeapon(23004);
            }
            if (GUI.Button(GetBtnRect(1, 0), "手枪子弹"))
            {
                BattleController.GetCtrl<BulletCtrl>().bulletList.Find(b => b.bulletType == BulletType.Pistol).OwnBullet(BulletCreatType.Single, 7);
            }
            if (GUI.Button(GetBtnRect(1, 1), "MP5子弹"))
            {
                BattleController.GetCtrl<BulletCtrl>().bulletList.Find(b => b.bulletType == BulletType.Mp5).OwnBullet(BulletCreatType.Single, 20);
            }
            if (GUI.Button(GetBtnRect(1, 2), "获取霰弹枪子弹"))
            {
                BattleController.GetCtrl<BulletCtrl>().bulletList.Find(b => b.bulletType == BulletType.ShotGun).OwnBullet(BulletCreatType.Single, 1);
            }
            if (GUI.Button(GetBtnRect(1, 3), "获取箭"))
            {
                BattleController.GetCtrl<BulletCtrl>().bulletList.Find(b => b.bulletType == BulletType.Arrow).OwnBullet(BulletCreatType.Single, 5);
            }
            if (GUI.Button(GetBtnRect(1, 4), "获取锤子"))
            {
                WeaponManager.AddWeapon(23001);
            }
            if (GUI.Button(GetBtnRect(2, 0), "加血100"))
            {
                ChangeHp(100);
            }
            if (GUI.Button(GetBtnRect(2, 1), "加能量100"))
            {
                ChangeEnergy(100);
            }
            if (GUI.Button(GetBtnRect(2, 2), "加体力100"))
            {
                ChangeStrength(100);
            }
            if (GUI.Button(GetBtnRect(2, 3), "加钱"))
            {
                MoneyInfo.OwnMoney(MoneyType.Alloy, 10000);
                MoneyInfo.OwnMoney(MoneyType.Memory, 10000);
                MoneyInfo.OwnMoney(MoneyType.Parts, 10000);
            }
            if (GUI.Button(GetBtnRect(2, 4), "无敌:"+godMod))
            {
                godMod = !godMod;
            }
            if (GUI.Button(GetBtnRect(3, 0), "跳节点"))
            {
                BattleController.Instance.SkipCurrNode();
                characterController.enabled = false;
                transform.position = BattleController.Instance.ctrlProcedure.currentNode.nodeParent.playerCreator.transform.position;
                transform.rotation = BattleController.Instance.ctrlProcedure.currentNode.nodeParent.playerCreator.transform.rotation;
                characterController.enabled = true;
                characterController.Move(Vector3.zero);

            }
            if (GUI.Button(GetBtnRect(3, 1), "速度加一倍"))
            {
                moveMul += 1;
            }
            if (GUI.Button(GetBtnRect(3, 2), "调节减一倍"))
            {
                moveMul = Mathf.Clamp(moveMul-1,1,Mathf.Infinity);
            }

            if (GUI.Button(GetBtnRect(3, 4), "获取物品"))
            {
                propTools = true;
                openTools = false;
            }
            else if (GUI.Button(GetBtnRect(3, 3), "关闭怪"))
            {
                MonsterCreator[] creator = GameObject.FindObjectsOfType<MonsterCreator>();
                for (int i = 0; i < creator.Length; i++)
                {
                    creator[i].gameObject.OnActive(false);
                }
            }
            else if (GUI.Button(GetBtnRect(4, 0), "所有武器皮肤"))
            {
                foreach (var skinEntity in WeaponManager.allSkins)
                {
                    skinEntity.Value.Acquire();
                }
            }
            GUI.EndGroup();
        }
        if (propTools)
        {
            GUI.BeginGroup(new Rect(Screen.width / 2 - toolWidSize.x * 0.5f * sizeK, Screen.height / 2 - toolWidSize.y * 0.5f * sizeK, toolWidSize.x * sizeK, toolWidSize.y * sizeK));
            GUI.Box(new Rect(0, 0, toolWidSize.x * sizeK, toolWidSize.y * sizeK), "获取物品");
            if (GUI.Button(new Rect(0, 0, 50 * sizeK, 30 * sizeK), "关闭"))
            {
                propTools = false;
            }
            GUI.Label(new Rect(50 * sizeK + 20, 0, toolWidSize.x * sizeK- 50 * sizeK, 30 * sizeK), "20017--20020:能量石");
            toolsPropid = GUI.TextField(GetBtnRect(0, 0), toolsPropid);
            toolsPropCount = GUI.TextField(GetBtnRect(0, 1), toolsPropCount);
            if (GUI.Button(GetBtnRect(0, 3), "获取物品"))
            {
                PropEntity entity = PropEntity.GetEntity(toolsPropid.ToInt());
                if (entity == null)
                {
                    GameDebug.LogErrorFormat("测试工具:没有找到指定物品{0}", toolsPropid.ToInt());
                }
                else {
                    entity.GetReward(toolsPropCount.ToInt(), RewardFlag.NoAudio);
                }
            }
            GUI.EndGroup();
        }
    }
    private void TryOpenDoor()
    {
        Ray ray = evCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 20, MaskLayer.door))
        {
            if (hit.collider.GetComponentInParent<Door>())
            {
                hit.collider.GetComponentInParent<Door>().RunLogicalOnSelf(RunLogicalName.RemoveLock);
                hit.collider.GetComponentInParent<Door>().RunLogicalOnSelf(RunLogicalName.On);
            }
        }
    }
    private float btnWidth = 160f;
    private float btnHeight = 90f;
    private float vSpacing = 10f;
    /// <summary>
    /// 获取一个按钮的rect
    /// </summary>
    /// <param name="v">行</param>
    /// <param name="h">列</param>
    /// <returns></returns>
    private Rect GetBtnRect(int v, int h)
    {
        return new Rect(btnWidth * h * sizeK, 30f + (btnHeight + vSpacing) * v * sizeK, btnWidth * sizeK, btnHeight * sizeK);
    } 
    #endregion

    public bool isVisiable
    {
        get { return true; }
    }

    [Button("屏幕错误效果测试")]
    public async void AnimTest()
    {
        Volume volume = GameObject.Find("Volume").GetComponent<Volume>();
        GlitchEffect glitchEffect;
        volume.profile.TryGet<GlitchEffect>(out glitchEffect);
        if (glitchEffect == null) { return; }
        glitchEffect.enableEffect.value = true;
        DOTween.To(v => glitchEffect.scanLineJitter.value = v, 0f, 0.2f, 0.3f);
        DOTween.To(v => glitchEffect.colorDrift.value = v, 0f, 0.6f, 0.5f);
        await Async.WaitforSeconds(2f);
        glitchEffect.enableEffect.value = false;
        glitchEffect.scanLineJitter.value = 0f;
        glitchEffect.colorDrift.value = 0f;
    }

    public Vector3 targetPoint => CenterPostion;
}