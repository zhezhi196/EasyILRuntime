using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Module;
using System;
using Sirenix.OdinInspector;
[Flags]
public enum WeaponType
{
    /// <summary>
    /// 无武器
    /// </summary>
    Empty = 1,
    /// <summary>
    /// 手枪
    /// </summary>
    HandGun = 1 << 1,
    /// <summary>
    /// 霰弹枪
    /// </summary>
    ShotGun = 1 << 2,
    /// <summary>
    /// 突击步枪
    /// </summary>
    Rifle = 1 << 3,
    /// <summary>
    /// 近战武器
    /// </summary>
    MeleeWeapon = 1 << 4,
    /// <summary>
    /// 投掷武器
    /// </summary>
    Thrown = 1<<5,
    /// <summary>
    /// 弓箭
    /// </summary>
    Bow = 1 << 6,
    /// <summary>
    /// 火瓶
    /// </summary>
    FireBottle = 1 << 7,
    /// <summary>
    /// 刀
    /// </summary>
    Knife = 1 << 8,
    
    /// <summary>
    /// 油桶爆炸
    /// </summary>
    OilDrum = 1 << 9,

}
public static class WeaponrAnimaKey
{
    public const string Move = "Move";
    public const string Rotate = "Rotate";
    public const string AimBool = "isAim";
    public const string ReloadState = "ReloadState";
    public const string Reload = "Reload";
    public const string EmptyReload = "EmptyReload";
    public const string ReloadCount = "ReloadCount";
    public const string TackOut = "TackOut";
    public const string Fire = "Fire";
    public const string NoAim = "NoAim";
    public const string ToAim = "ToAim";
    public const string Runing = "Runing";
    public const string MoveSpeed = "MoveSpeed";
}

[System.Serializable]
public class WeaponUpFiled
{
    [ValueDropdown("@AttributeHelper.GetAttFiled"),OnValueChanged("ChangFiled")]
    public string filed;
    [ReadOnly]
    public int index;
    private void ChangFiled()
    {
        index = AttributeHelper.GetAttFiled.FindIndex(a => a == filed);
    }
}

/// <summary>
/// 武器
/// 2021.11.19--武器属性,武器升级属性全部使用基础属性相加
/// 创建武器需要创建配套的WeaponArgs,配置武器参数
/// 存在一个没有武器的控制器,id为0,所有需要处理找不到Entity
/// 有些武器不可切换皮肤,初始化时判定是否可以切换皮肤
/// </summary>
public class Weapon : MonoBehaviour,ILocalSave, IWeaponAnimEvent
{
    [Header("武器通用设置")]
    public int weaponID = 23001;
    public WeaponArgs weaponArgs;
    public WeaponType weaponType;
    public BulletType bulletType;
    private BulletEntity _bullet;
    public BulletEntity bullet
    {
        get {
            if (_bullet == null)
            {
                _bullet = BattleController.GetCtrl<BulletCtrl>().GetBullet(bulletType);
            }
            return _bullet;
        }
    }
    public WeaponEntity entity;
    public Animator _animator;
    public Transform cameraPoint;
    public Transform firePoint;//特效等
    public string fireEffect;
   
    [LabelText("武器模型材质")]
    public Renderer[] weaponRenderers;
    [LabelText("武器可升级属性设置")]
    public List<WeaponUpFiled> weaponUpFileds = new List<WeaponUpFiled>();
    [ReadOnly, FoldoutGroup("武器状态属性")] public string weaponName = "101";
    [ReadOnly, FoldoutGroup("武器状态属性")] public float showInterval = 0.2f;
    [ReadOnly, FoldoutGroup("武器状态属性")] public int bulletCount = 0;
    [ReadOnly, FoldoutGroup("武器状态属性")] public int maxBulletCount = 7;
    [ReadOnly, FoldoutGroup("武器状态属性")] public int level = 0;
    [ReadOnly, FoldoutGroup("武器状态属性")] public bool isAim = false;//瞄准状态
    [ReadOnly, FoldoutGroup("武器状态属性")] public Monster aimMonster;//瞄准的怪物
    [Header("武器休闲展示")]
    public bool weaponShow = false;
    public float weaponShowTime = 3f;
    public float weaponShowCD = 60f;
    [ReadOnly, ShowInInspector] private float idleTime = 0;
    [ReadOnly, ShowInInspector] private bool isIdle = false;
    private float weaponShowCDTemp = 60f;
    [Header("首次拾取动画")]
    public bool firstGetAnim = false;
    //---------运行时参数------------------
    protected Player _player;
    protected float cd = 0f;
    protected float allGrow = 0;
    protected float t_cancleAim = 0;
    public GameAttribute baseAttribute;
    public GameAttribute weaponAttribute;
    protected Vector2 screenPos = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
    public Vector2 FireRayPos
    {
        get
        {
            screenPos.x = Screen.width * 0.5f;
            screenPos.y = Screen.height * 0.5f;
            return screenPos;
        }
    }
    public virtual bool CanAttack
    {
        get { return cd <=0 && bulletCount > 0; }
    }
    public virtual bool CanReload
    {
        get
        {
            if (bulletCount <= maxBulletCount
                && !_player.ContainStation(Player.Station.Reloading)
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
    public float GetAccurate
    {
        get
        {
            float acc = weaponAttribute.accurate;
            acc += allGrow;
            if (_player.isSquat && _player.isMoving)
            {
                acc *= weaponArgs.crouchMoveMul;
            }
            else if (_player.isMoving)
            {
                acc *= weaponArgs.moveMul;
            }
            else if (_player.isSquat)
            {
                acc *= weaponArgs.crouchMul;
            }
            return acc.Clamp(0, weaponArgs.maxAccurate); ;
        }
    }

    //-----------------------------------------
    public virtual void Init(Player p,bool readData)
    {
        weaponShowCDTemp = weaponShowCD;
        bulletCount = 0;
        baseAttribute = new GameAttribute(0);
        weaponAttribute = new GameAttribute(0);
        _player = p;
        if (weaponID != 0)
        {
            if (!WeaponManager.weaponAllEntitys.ContainsKey(weaponID))
            {
                GameDebug.LogErrorFormat("武器初始化失败:{0}",weaponID);
                return;
            }
            entity = WeaponManager.weaponAllEntitys[weaponID];
            weaponName = entity.weaponData.name;

            baseAttribute = AttributeHelper.GetAttributeByType(entity.weaponData)[0];
            if (BattleController.Instance.ctrlProcedure.mode == GameMode.Main)
                level = WeaponManager.weaponAllEntitys[weaponID].level;
        }
        RefeshAtt();
        if (readData)
        {
            ReadData();
        }
        //武器皮肤初始化
        if (weaponRenderers.Length>0 && entity != null &&entity.equipSkin!=null&&entity.equipSkin.skinMat!=null)
        {
            for (int i = 0; i < weaponRenderers.Length; i++)
            {
                weaponRenderers[i].material = entity.equipSkin.skinMat;
            }
        }
        EventCenter.Register<bool>(EventKey.GamePause, OnGamePasue);
        if (!string.IsNullOrEmpty(fireEffect))
            ObjectPool.Cache(EffectPlay.GetPath(fireEffect), 1);
    }

    private void OnDestroy()
    {
        EventCenter.UnRegister<bool>(EventKey.GamePause, OnGamePasue);
        if (gameObject.activeSelf)//武器销毁时,停止音效
        {
            StopAnimAudio();
        }
    }

    protected virtual void OnGamePasue(bool b)
    {
        if (_animator != null)
            _animator.updateMode = b ? AnimatorUpdateMode.Normal : AnimatorUpdateMode.UnscaledTime;
    }
    protected virtual void Update()
    {
        if (cd >= 0)
        {
            cd -= TimeHelper.unscaledDeltaTime;
        }
        if (allGrow > 0)
            allGrow -= TimeHelper.unscaledDeltaTime * weaponArgs.fallAccurate;
        if (t_cancleAim > 0)//取消瞄准计时
        {
            t_cancleAim -= TimeHelper.deltaTime;
            if (t_cancleAim<=0 && _player.ContainStation(Player.Station.Aim))
            {
                ChangeNoAim();
            }
        }
        AimCheck();
        WeaponShow();
    }

    private void WeaponShow()
    {
        if (!weaponShow)
            return;
        if (weaponShowCDTemp > 0)
        {
            weaponShowCDTemp -= TimeHelper.deltaTime;
            return;
        }
        if (isIdle)
        {
            idleTime += TimeHelper.deltaTime;
            if (idleTime >= weaponShowTime)
            {
                weaponShowCDTemp = weaponShowCD;
                idleTime = 0;
                _animator.CrossFade("ShowWeapon", 0.3f);
            }
        }
    }

    private void AimCheck()
    {
        //if (isAim)
        //{
            Ray ray = _player.evCamera.ScreenPointToRay(FireRayPos);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 2000, MaskLayer.PlayerShot))
            {
                IHurtObject hurtObject = hit.collider.GetComponent<IHurtObject>();
                if (hurtObject != null && hurtObject is MonsterPart part)
                {
                    aimMonster = part.monster;
                    return;
                }
            }
        //}
        aimMonster = null;
    }

    public void Movement(Vector2 moveDirection)
    {
        _animator.SetBool("Move", _player.isMoving);
        _animator.SetFloat("MoveV", moveDirection.y);
        _animator.SetFloat("MoveH", moveDirection.x);
    }

    public void Rotate(Vector2 rotDir)
    {
        _animator.SetBool("Rotate", _player.isRotate);
        _animator.SetFloat("RotateV", rotDir.y);
        _animator.SetFloat("RotateH", rotDir.x);
    }

    #region 武器升级,武器属性计算
    //武器升级
    public void Upgrade(int up = 1)
    {
        level += up;
        RefeshAtt();
    }
    //刷新武器当前等级属性
    public void RefeshAtt()
    {
        weaponAttribute = baseAttribute;
        if (WeaponManager.weaponAllPartDataDic.ContainsKey(weaponID))
        {
            for (int i = 0; i < level; i++)
            {
                weaponAttribute += WeaponManager.weaponAllPartDataDic[weaponID][i].attribute;
            }
            showInterval = 1f / weaponAttribute.shotInterval.value;
            maxBulletCount = weaponAttribute.bulletCount.value.ToInt();
        }
        else {
            GameDebug.LogFormat("没有找到武器升级属性:{0}", weaponID);
        }
    }
    //计算武器满级属性
    public GameAttribute MaxAttribute
    {
        get
        {
            GameAttribute att = baseAttribute;
            for (int i = 0; i < WeaponManager.weaponAllPartDataDic[weaponID].Count; i++)
            {
                att += WeaponManager.weaponAllPartDataDic[weaponID][i].attribute;
            }
            return att;
        }
    }
    #endregion

    /// <summary>
    /// 收起武器
    /// </summary>
    public virtual void TakeBack()
    {
        this.gameObject.OnActive(false);
        StopAnimAudio();
        isAim = false;
        aimMonster = null;
    }
    /// <summary>
    /// 拿出武器
    /// </summary>
    public async virtual void TakeOut()
    {
        this.gameObject.OnActive(true);
        //第一次拾取的展示动画
        if (firstGetAnim && !LocalFileMgr.ContainKey("FirstGetAnim"+weaponID))
        {
            _animator.Play("FirstGet");
            _player.AddStation(Player.Station.WeaponChanging);
            _player.AddStation(Player.Station.Story);
            EventCenter.Dispatch<AttackMonster, TimeLineType>(EventKey.MonsterTimeLine, null, TimeLineType.Story);
        }
        else {
            if (_player.ContainStation(Player.Station.Aim))
            {
                _animator.SetBool(WeaponrAnimaKey.AimBool, true);
                t_cancleAim = weaponArgs.cancelAimTime;
            }
            if (_player.ContainStation(Player.Station.Running))
            {
                SetRunStation(true);
            }
            _player.AddStation(Player.Station.WeaponChanging);
            await Async.WaitforSecondsRealTime(0.3f);
            _player.RemoveStation(Player.Station.WeaponChanging);
            //尝试换弹
            if (bulletCount <= 0 && CanReload)
                ReloadBtnDown();
        }
    }

    protected void BulletChange(int count)
    {
        bulletCount += count;
        EventCenter.Dispatch(EventKey.WeaponBulletChange, this, bulletCount);
        if (count > 0)
        {
            BattleController.Instance.Save(0);    
        }
    }

    public void HitSomething(IHurtObject hurtObject, RaycastHit hit,WeaponType type)
    {
        Vector3 direction;
        if (firePoint != null)
        {
            direction = firePoint.position- hit.point;
        }
        else {
            direction = hit.normal;
        }
        switch (hurtObject.hurtMaterial)
        {
            case Hurtmaterial.None:
                break;
            case Hurtmaterial.Close:
            case Hurtmaterial.Paper:
            case Hurtmaterial.Glass:
            case Hurtmaterial.Plastic:
            case Hurtmaterial.Stone:
            case Hurtmaterial.Wood:
                EffectPlay.Play("Hit_qiangti", hit.transform, (e) => {
                    e.transform.position = hit.point;
                    e.transform.rotation = Quaternion.LookRotation(direction);
                });
                GameDebug.DrawLine(hit.point, hit.point + direction, Color.blue, 1f);
                //todo  击中音效
                //if (type == WeaponType.MeleeWeapon)
                //{
                //    PlayAudio("qiang_peng", hit.point);
                //}
                //else
                //{
                //    PlayAudio("Hit_Concrete", hit.point);
                //}
                break;
            case Hurtmaterial.Mental:
                EffectPlay.Play("Hit_metal", hit.transform, (e) => {
                    e.transform.position = hit.point;
                    e.transform.rotation = Quaternion.LookRotation(direction);
                });
                //if (type == WeaponType.MeleeWeapon)
                //{
                //    PlayAudio("qiang_peng", hit.point);
                //}
                //else
                //{
                //    PlayAudio("Hit_Iron", hit.point);
                //}
                break;
            case Hurtmaterial.Meat:
                EffectPlay.Play("Hit_routi", hit.transform, (e) => {
                    e.transform.position = hit.point;
                    e.transform.rotation = Quaternion.LookRotation(direction);
                });
                //if (type == WeaponType.MeleeWeapon)
                //{
                //    PlayAudio("hit_peapole", hit.point);
                //}
                //else {
                //    PlayAudio("zidan_routi", hit.point);
                //}
                break;
        }
        BattleController.GetCtrl<MonsterCtrl>().TrySensorMonster(SensorMonsterType.Shot,hit.point+(transform.position-hit.point).normalized, weaponArgs.hitSoundRange);
    }

    #region UI按钮响应
    /// <summary>
    /// 按下武器瞄准按钮
    /// </summary>
    //public virtual void AimBtnDown()
    //{
    //    //检查是否可以切换瞄准姿态,没有持有武器,换枪,换弹中途不允许主动切换瞄准状态
    //    if (_player.ContainStation(Player.Station.Aim))
    //    {
    //        _animator.CrossFade(WeaponrAnimaKey.NoAim, 0);
    //        _animator.SetBool(WeaponrAnimaKey.AimBool, false);
    //    }
    //    else {
    //        _animator.CrossFade(WeaponrAnimaKey.ToAim, 0);
    //        _animator.SetBool(WeaponrAnimaKey.AimBool, true);
    //    }
    //}

    public void ChangeToAim()
    {
        _animator.CrossFade(WeaponrAnimaKey.ToAim, 0);
        _animator.SetBool(WeaponrAnimaKey.AimBool, true);
    }

    public void ChangeNoAim()
    {
        _animator.CrossFade(WeaponrAnimaKey.NoAim, 0);
        _animator.SetBool(WeaponrAnimaKey.AimBool, false);
    }

    public void SetRunStation(bool b)
    {
        _animator.SetBool(WeaponrAnimaKey.Runing, b);
        _animator.SetFloat(WeaponrAnimaKey.MoveSpeed,b?2f:1f);
    }
    /// <summary>
    /// 攻击按钮按下
    /// </summary>
    public virtual void AttackBtnDown()
    {
        if (!CanAttack)
        {
            return;
        }
    }
    /// <summary>
    /// 攻击按钮抬起
    /// </summary>
    public virtual void AttackBtnUp()
    {

    }
    /// <summary>
    /// 按下换弹按钮
    /// </summary>
    public virtual void ReloadBtnDown()
    {
        if (!CanReload)
            return;
        _player.AddStation(Player.Station.Reloading);
        if (bulletCount == 0)
        {
            _animator.CrossFade(WeaponrAnimaKey.EmptyReload, 0.1f);
        }
        else
        {
            _animator.CrossFade(WeaponrAnimaKey.Reload, 0.1f);
        }
    }
    //开始闪避
    public void StartDodge(Vector3 dir)
    {
        if (isAim)
        {
            isAim = false;
            _animator.SetBool(WeaponrAnimaKey.AimBool, isAim);
        }
        _animator.SetFloat("DodgeV", dir.z);
        _animator.SetFloat("DodgeH", dir.x);
        _animator.SetBool("Dodge", true);
        _animator.CrossFade("Dodge", 0.1f);
    }
    //闪避结束
    public void EndDodge()
    {
        _animator.SetBool("Dodge", false);
    }
    #endregion

    #region 动画事件响应
    /// <summary>
    /// 攻击
    /// </summary>
    public virtual void Attack()
    {
        GameDebug.Log("AnimEvent:Attack");
        EventCenter.Dispatch(EventKey.OnWeaponFire, this);
        BattleController.GetCtrl<MonsterCtrl>().TrySensorMonster(SensorMonsterType.Shot, transform.position, weaponArgs.attackSoundRange);
        if (!String.IsNullOrEmpty(weaponArgs.fireAudio))
        {
            PlayAudio(weaponArgs.fireAudio,transform.position);
        }
        _player.weaponManager.AllWeaponEmpty();
        t_cancleAim = weaponArgs.cancelAimTime;
    }
    /// <summary>
    /// 开始换弹,播放换弹音效
    /// </summary>
    public virtual void StartReload()
    {
        //t_cancleAim = weaponArgs.cancelAimTime;
        _animator.SetBool(WeaponrAnimaKey.AimBool, false);
        isAim = false;
        EventCenter.Dispatch(EventKey.WeaponReload);
        if (bulletCount == 0)
        {
            if (!String.IsNullOrEmpty(weaponArgs.emptyReloadAudio))
            {
                PlayAudio(weaponArgs.emptyReloadAudio, transform.position);
            }
            else if (!String.IsNullOrEmpty(weaponArgs.reloadAudio))
            {
                PlayAudio(weaponArgs.reloadAudio, transform.position);
            }
        }
        else
        {
            if (!String.IsNullOrEmpty(weaponArgs.reloadAudio))
            {
                PlayAudio(weaponArgs.reloadAudio, transform.position);
            }
        }
    }
    /// <summary>
    /// 换弹
    /// </summary>
    public virtual void Reload()
    {
        //bulletCount = maxBulletCount;
        _player.RemoveStation(Player.Station.Reloading);
        GameDebug.Log("AnimEvent:Reload");

    }

    public virtual void ToAim()
    {
        _player.AddStation(Player.Station.Aim);
        isAim = true;
    }

    public virtual void ToNoAim()
    {
        _player.RemoveStation(Player.Station.Aim);
        isAim = false;
    }

    public virtual void SkillEvent(string key)
    { 
    
    }
    public void PlayAudio(string audioName,Vector3 pos)
    {
        //AudioPlay.PlayOneShot(audioName, pos).SetID("WeaponAnimAudio");
    }

    public void StopAnimAudio()
    {
        AudioPlay.StopAudio(s => s.ID == "WeaponAnimAudio");
    }

    //public virtual void OnReloadExit()
    //{ 
    
    //}

    public virtual void OnAnimStateEnter(string stateName)
    {
        if (stateName == "Reload")
        { 
            
        }
        if (stateName == "NorIdle")
        {
            isIdle = true;
            idleTime = 0;
        }
        //if (stateName == "FirstGet")
        //{
        //    _player.RemoveStation(Player.Station.Story);
        //}
    }

    public virtual void OnAnimStateExit(string stateName)
    {
        if (stateName == "Reload")
        {

        }
        if (stateName == "NorIdle")
        {
            isIdle = false;
        }
        if (stateName == "FirstGet")
        {
            _player.RemoveStation(Player.Station.Story);
            _player.RemoveStation(Player.Station.WeaponChanging);
            EventCenter.Dispatch<AttackMonster, TimeLineType>(EventKey.MonsterEndTimeLine, null, TimeLineType.Story);
            LocalFileMgr.SetInt("FirstGetAnim" + weaponID, 1);
        }
    }
    #endregion

    #region 存档

    public string localFileName => LocalSave.savePath;
    public string localGroup => "Weapon";

    public string localUid
    {
        get { return "Weapon_" + weaponID; }
    }
    public virtual void ReadData()
    {
        string data = LocalSave.Read(this);
        if (data != null)
        {
            bulletCount = data.ToInt();
        }
    }

    public void Save()
    {
        LocalSave.Write(this);
    }

    public virtual string GetWriteDate()
    {
        return bulletCount.ToString();
    } 
    #endregion

    [Sirenix.OdinInspector.Button("播放动画")]
    private void PlayAnim()
    {
        _animator.Play("StandardM1911Takeback");
    }
    
#if LOG_ENABLE
    public void GMBulletChange(int count)
    {
        bulletCount = count;
        EventCenter.Dispatch(EventKey.WeaponBulletChange, this, bulletCount);
        BattleController.Instance.Save(0);
    }
#endif
}
