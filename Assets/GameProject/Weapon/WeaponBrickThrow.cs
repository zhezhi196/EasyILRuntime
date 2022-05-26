using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Module;
/// <summary>
/// 投掷板砖
/// </summary>
public class WeaponBrickThrow : MonoBehaviour, IPoolObject
{
    public ObjectPool pool { get; set; }

    //扔出,飞行模拟
    [Header("投掷飞行")]
    public float Power = 10;//这个代表发射时的速度/力度等，可以通过此来模拟不同的力大小
    public float Gravity = -10;//这个代表重力加速度
    [Tooltip("直线飞行时间")]
    public float ddd = 1f;//直线飞行距离时间
    private Vector3 MoveSpeed;//初速度向量
    private Vector3 GritySpeed = Vector3.zero;//重力的速度向量，t时为0
    private float dTime;//已经过去的时间
    //参数
    private bool isThrow = false;
    [Sirenix.OdinInspector.ReadOnly]
    private bool useGravity = false;
    private float _time = 0;
    private Vector3 lastPos = Vector3.zero;
    #region //反弹
    //[Header("反弹")]
    //public float bounceTime = 1f;
    //private float tempBounceTime = 1f;
    //public float bouncePower = 5f;
    //private bool hitDown = false;
    //private Vector3 bounceDir = Vector3.one;
    #endregion
    //伤害
    private Damage damage;
    private WeaponBrick _weapon;
    //生命周期
    [Header("生命周期")]
    public float maxLifeTime = 20f;
    private float liftTime = 0;

    private void Start()
    {
        EventCenter.Register(EventKey.OnPlayerDeath, OnPlayerDeath);
    }

    private void OnDestroy()
    {
        EventCenter.UnRegister(EventKey.OnPlayerDeath, OnPlayerDeath);
    }

    private void OnPlayerDeath()
    {
        if (gameObject.activeInHierarchy)
        {
            pool.ReturnObject(this);
        }
    }

    public void Throw(Transform fireTrans, Vector3 target, WeaponBrick weapon)
    {
        _weapon = weapon;
        liftTime = 0;
        transform.position = fireTrans.position;
        transform.rotation = fireTrans.rotation;
        lastPos = transform.position;
        MoveSpeed = (target - transform.position).normalized * Power;
        isThrow = true;
        damage = Player.player.CreateDamage(weapon, null);
    }

    void FixedUpdate()
    {
        //投掷
        if (isThrow)
        {
            _time += Time.fixedDeltaTime;
            if (_time > ddd / Power && !useGravity)
            {
                useGravity = true;
            }
            //位移模拟轨迹
            transform.Translate(MoveSpeed * Time.fixedDeltaTime, Space.World);
            if (useGravity)
            {
                //计算物体的重力速度
                GritySpeed.y = Gravity * (dTime += Time.fixedDeltaTime);
                transform.Translate(GritySpeed * Time.fixedDeltaTime, Space.World);
            }
            Ray ray = new Ray(transform.position, transform.position - lastPos);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Vector3.Distance(lastPos, transform.position) * 2, MaskLayer.PlayerShot))
            {
                //破碎
                isThrow = false;
                //特效
                //EffectPlay.Play("Hit_qiangti", hit.transform, (e) => {
                //    e.transform.position = hit.point;
                //    e.transform.rotation = Quaternion.LookRotation(hit.normal);
                //});
                //声音
                //AudioPlay.PlayOneShot("Break_the_bottle", transform.position).Set3D();
                if (BattleController.GetCtrl<MonsterCtrl>() != null)
                    BattleController.GetCtrl<MonsterCtrl>().TrySensorMonster(SensorMonsterType.Shot, transform.position, _weapon.weaponArgs.hitSoundRange);
                pool.ReturnObject(this);
            }
            lastPos = transform.position;
        }
        //反弹坠落
        #region
        //if (hitDown)
        //{
        //    if (tempBounceTime > 0)
        //    {
        //        transform.Translate(bounceDir * bouncePower * tempBounceTime * Time.fixedDeltaTime, Space.World);
        //        tempBounceTime -= Time.fixedDeltaTime;
        //    }
        //    GritySpeed.y = Gravity * (dTime += Time.fixedDeltaTime);
        //    transform.Translate(GritySpeed * Time.fixedDeltaTime, Space.World);
        //    Ray ray = new Ray(transform.position, transform.position - lastPos);
        //    RaycastHit hit;
        //    if (Physics.Raycast(ray, out hit, Vector3.Distance(lastPos, transform.position) * 2,MaskLayer.PlayerShot))
        //    {
        //        if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
        //        {
        //            hitDown = false;
        //            transform.position = hit.point;
        //        }
        //        else {
        //            bounceDir = Vector3.Reflect(transform.position - lastPos, hit.normal);
        //        }
        //    }
        //    lastPos = transform.position;
        //}
        #endregion
    }

    private void Update()
    {
        if (liftTime >= maxLifeTime)
        {
            pool.ReturnObject(this);
        }
        liftTime += TimeHelper.deltaTime;
    }
    public void ReturnToPool()
    {
        useGravity = false;
        isThrow = false;
        _time = 0;
        dTime = 0;
        liftTime = 0;
        //pool.ReturnObject(this);
    }
    public void OnGetObjectFromPool()
    {

    }
}
