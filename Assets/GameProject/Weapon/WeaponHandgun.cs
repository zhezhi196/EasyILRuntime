using Module;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

/// <summary>
/// 手枪
/// 特殊处理没有子弹是手枪的状态
/// </summary>
public class WeaponHandgun : Weapon
{   
    [Header("手枪设置")]
    public AnimationClip emptyClip;
    private bool initPlayable = false;
    public string shellCasePath = "";
    public float force = 5f;
    [Header("单枪设置")]
    public Animator modelAnimtor;
    private PlayableGraph _playableGraph;
    //public string shellFallAudio = "";
    public Transform shellCasePos;
    [Header("双枪设置")]
    public bool isDouble = false;
    public GameObject[] d_weaponModels;
    public Transform[] d_firePoss;//射击位置
    public Transform[] d_shellCasePoss;//抛壳位
    public Animator[] d_modelAnimtors;
    public AnimationClip[] d_emptyClips;
    private PlayableGraph[] d_playableGraphs;
    private int attackCount = 0;
    public override bool CanReload
    {
        get
        {
            if (bulletCount <= (isDouble? maxBulletCount*2+1:maxBulletCount)
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
    public override void Init(Player p, bool readData)
    {
        base.Init(p, readData);
        ObjectPool.Cache(shellCasePath, 5);
    }

    private void Start()
    {
        if (!initPlayable)
        {
            CreatEmptyPlayable();
        }
    }

    public override void AttackBtnDown()
    {
        if (bulletCount <= 0 && CanReload)
        {
            //换弹
            ReloadBtnDown();
            return;
        }

        if (CanAttack)
        {
            _player.AddStation(Player.Station.WaitingAttack);
            if (!_player.ContainStation(Player.Station.Aim))
            {
                ChangeToAim();
            }
            _animator.SetInteger("AttackCount", attackCount % 2);
            _animator.SetTrigger(WeaponrAnimaKey.Fire);
            return;
        }
        if (cd <= 0 && bulletCount <= 0)
        {
            if (bullet.bagCount <= 0)
                //EventCenter.Dispatch(EventKey.ShowTextTip, GameUITips.TextTipType.Bullet);
            if (_player.ContainStation(Player.Station.Aim))
            {
                //武器空仓音效
                if (!System.String.IsNullOrEmpty(weaponArgs.emptyFireAuido))
                {
                    PlayAudio(weaponArgs.emptyFireAuido, transform.position);
                }
            }
        }
    }
   
    public override void Attack()
    {
        if (bulletCount <= 0)//子弹小于0不能射击
            return;
        if (isDouble)
        {
            EffectPlay.Play(fireEffect, d_firePoss[attackCount % 2]);//射击特效
        }
        else {
            EffectPlay.Play(fireEffect, firePoint);//射击特效
        }
        _player.RemoveStation(Player.Station.WaitingAttack);
        Vector2 random = Vector2.zero;
        if (allGrow > 0)
            random = Random.insideUnitSphere * allGrow;
        Physics.SyncTransforms();
        Ray ray = _player.evCamera.ScreenPointToRay(FireRayPos+ random);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 2000, MaskLayer.PlayerShot))
        {
            GameDebug.LogFormat("WeaponHit:" + hit.collider.name);
            IHurtObject hurtObject = hit.collider.GetComponent<IHurtObject>();
            if (hurtObject != null)
            {
                var damage = _player.CreateDamage(this, hurtObject);
                damage.hitPoint = hit.point;
                if (isDouble)
                {
                    damage.dir = hit.point - d_firePoss[attackCount % 2].position;
                }
                else
                {
                    damage.dir = hit.point - firePoint.position;
                }
                if (hurtObject is MonsterPart part)
                {
                    EventCenter.Dispatch(EventKey.HitMonster, (part.monster.hp - damage.damage) > 0, part);
                }
                hurtObject.OnHurt(_player, damage);
                HitSomething(hurtObject, hit,weaponType);//特效
            }
            GameDebug.DrawLine(_player.evCamera.ScreenToWorldPoint(FireRayPos + random), hit.point, Color.red,2f);
        }
        BulletChange(-1);
        cd = isDouble ? showInterval / 2f : showInterval;
        if (allGrow < weaponArgs.maxAccurate)
            allGrow += weaponAttribute.recoilForce;
        if (bulletCount <= 0)
        {
            OpenEmptyAnim();
            //尝试换弹
            ReloadBtnDown();
        }
        GameDebug.Log("Handgun Fire");
        AssetLoad.LoadGameObject<ShellCase>(shellCasePath, null, (shell, obj) =>
        {
            Vector3 v3 = new Vector3(-Random.Range(0, 30f), Random.Range(0, 30f), 0);
            if (isDouble)
            {
                shell.transform.position = d_shellCasePoss[attackCount % 2].position;
                shell.transform.rotation = d_shellCasePoss[attackCount % 2].rotation;
            }
            else {
                shell.transform.position = shellCasePos.position;
                shell.transform.rotation = shellCasePos.rotation;
            }
            shell.transform.eulerAngles += v3;
            shell.gameObject.OnActive(true);
            Vector3 v = shellCasePos.right + shellCasePos.up;
            shell.ThrowShellCase(v*force, shellCasePos.position);
        });
        attackCount += 1;
        base.Attack();
    }

    public override void Reload()
    {
        int getCount = 0;
        if (isDouble)//双枪，弹夹有一个子弹+1，超过一个+2，空弹夹不增加
        {
            getCount = bullet.Get(bulletCount <= 0 ? maxBulletCount*2 :
                (maxBulletCount * 2+2 - bulletCount ));
        }
        else {
            getCount = bullet.Get(bulletCount <= 0?maxBulletCount: (maxBulletCount+1 - bulletCount));
        }
        BulletChange(getCount);
        CloseEnmptyAnim();
        attackCount = 0;
        base.Reload();
    }

    public override void TakeBack()
    {
        for (int i = 0; i < d_weaponModels.Length; i++)
        {
            d_weaponModels[i].OnActive(false);
        }
        base.TakeBack();
    }
    public override void TakeOut()
    {
        d_weaponModels[isDouble ? 1 : 0].OnActive(true);
        _animator = d_weaponModels[isDouble ? 1 : 0].GetComponent<Animator>();
        base.TakeOut();
        if (bulletCount == 0)
        {
            CloseEnmptyAnim();
            OpenEmptyAnim();
        }
    }
    protected override void OnGamePasue(bool b)
    {
        base.OnGamePasue(b);
        //为了解决暂定是切换animator updatemode空仓动画复位的问题
        if (bulletCount == 0 && this!=null)
        {
            CloseEnmptyAnim();
            OpenEmptyAnim();
        }
    }
    [Sirenix.OdinInspector.Button("切换双枪")]
    public void ChangeToDouble()
    {
        isDouble = true;
        if (gameObject.activeInHierarchy)
        {
            TakeBack();
            TakeOut();
        }
    }

    #region 手枪空仓
    private void CreatEmptyPlayable()
    {
        initPlayable = true;
        _playableGraph = PlayableGraph.Create("PlayableAnimation");
        var playableOut = AnimationPlayableOutput.Create(_playableGraph, "Animation", modelAnimtor);
        var clipPlayable = AnimationClipPlayable.Create(_playableGraph, emptyClip);
        playableOut.SetSourcePlayable(clipPlayable);

        d_playableGraphs = new PlayableGraph[2] { PlayableGraph.Create("PlayableAnimation1"), PlayableGraph.Create("PlayableAnimation2") };
        for (int i = 0; i < d_playableGraphs.Length; i++)
        {
            d_playableGraphs[i] = PlayableGraph.Create("PlayableAnimation");
            var playableOut1 = AnimationPlayableOutput.Create(d_playableGraphs[i], "Animation", d_modelAnimtors[i]);
            var clipPlayable1 = AnimationClipPlayable.Create(d_playableGraphs[i], d_emptyClips[i]);
            playableOut1.SetSourcePlayable(clipPlayable1);
        }
    }
    private void OpenEmptyAnim()
    {
        if (!initPlayable)
        {
            CreatEmptyPlayable();
        }
        if (isDouble)
        {
            for (int i = 0; i < d_playableGraphs.Length; i++)
            {
                d_playableGraphs[i].Play();
            }
        }
        else{
            _playableGraph.Play();
        }
    }

    private void CloseEnmptyAnim()
    {
        if (!initPlayable)
        {
            CreatEmptyPlayable();
        }
        if (isDouble)
        {
            for (int i = 0; i < d_playableGraphs.Length; i++)
            {
                d_playableGraphs[i].Stop();
            }
        }
        else
        {
            _playableGraph.Stop();
        }
    } 
    #endregion
}
