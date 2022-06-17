using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Module;

public class FirstMonsterTimeline : TimelineController
{
    public GameObject monster;
    public UIBtnBase dodgeBtn;
    public UIScrollBase attackBtn;
    private Player _player;
    private bool isTrigge = false;

    private void Start()
    {
        if (WeaponManager.weaponAllEntitys[23001].isGet)
        {
            gameObject.SetActive(false);
            return;
        }
        bool b = BattleController.GetCtrl<TimelineCtrl>().AddStoryTimeline(key, this);
        if (!b)
        {
            GameDebug.LogFormat("剧情动画初始化错误:{0}", key);
        }
        dodgeBtn.AddListener(OnDodgeBtn);
        attackBtn.AddDragPrepare(OnAttackBtn);
    }

    public override void Play(Player player, AttackMonster enemy, UnityAction callBack, params object[] args)
    {
        base.Play(player, enemy, callBack, args);
        onFinishEvent = callBack;
        playable.Play();
        if (bindingDict.ContainsKey("PlayerAnim") && player != null)
            playable.SetGenericBinding(bindingDict["PlayerAnim"].sourceObject, player.timelineModel._anim);
    }

    public override void OnComplete()
    {
        UIController.Instance.canPhysiceback = true;
        if (onFinishEvent != null)
        {
            onFinishEvent();
            onFinishEvent = null;
        }
        monster.GetComponent<Animator>().CrossFade("DeathIdle", 0);
        WeaponManager.weaponAllEntitys[23001].GetReward(1,RewardFlag.NoAudio|RewardFlag.NoRecord);
        //WeaponManager.AddWeapon(23001);
    }
    public void OnDestroy()
    {
        if (BattleController.GetCtrl<TimelineCtrl>() != null)
            BattleController.GetCtrl<TimelineCtrl>().RemoveStoryTimeline(key);
        Async.StopAsync(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!isTrigge && other.gameObject.layer == MaskLayer.Playerlayer)
        {
            isTrigge = true;
            Player.player.PlayStoryTimeline("FirstMonster", null);
        }
    }

    private void OnDodgeBtn()
    {
        Continue();
    }

    private void OnAttackBtn(Vector2 pos)
    {
        Continue();
    }
}
