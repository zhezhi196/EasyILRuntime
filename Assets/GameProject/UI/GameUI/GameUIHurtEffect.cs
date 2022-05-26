using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Module;
using Sirenix.OdinInspector;
using DG.Tweening;
using UnityEngine.UI;

public class GameUIHurtEffect : MonoBehaviour
{
    [System.Serializable]
    public class EffectCongfig
    {
        public int type;
        public bool isContinue = false;
        [ShowIf("isContinue"),ReadOnly]
        public GameUIEffect effect;
        //public GameObject effect;
        //public float effectTime = 1;
        [FilePath(ParentFolder = "Assets/Bundles")]
        public string effectPath;
    }
    [Header("生命值低效果")]
    public GameObject hpLowEffect;
    [Header("潜行效果")]
    public Image crouchEffectImage;
    //public Color crouchEndColor = Color.black;
    public DOTweenAnimation[] crouchEffet;
    private Tweener crouchEffectTweener;
    [Header("受击效果")]
    public EffectCongfig[] effectCongfigs;
    private GameObject curEffect = null;

    void Start()
    {
        EventCenter.Register<int>(EventKey.OnPlayerHurt, OnPlayerHurt);
        for (int i = 0; i < effectCongfigs.Length; i++)
        {
            //if (!effectCongfigs[i].isContinue)
            //{
            //    ObjectPool.Cache(effectCongfigs[i].effectPath, 1);
            //}
            ObjectPool.Cache(effectCongfigs[i].effectPath, 1);
        }
        Player.player.onHpChange += OnPlayerHpChange;
        OnPlayerHpChange(0);
        Player.player.onAddStation += OnPlayerAddStation;
        Player.player.onRemoveStation += OnPlayerRemoveStation;
    }

    private void Update()
    {
        if (hasCrouch)
        {
            MonsterCtrl ctrl = BattleController.GetCtrl<MonsterCtrl>();
            if (ctrl != null)
            {
                bool noFightMonster = true;
                for (int i = 0; i < ctrl.exitMonster.Count; i++)
                {
                    if (ctrl.exitMonster[i].showUiState != FightState.Normal)
                    {
                        noFightMonster = false;
                    }
                }

                if (noFightMonster)
                {
                    SneakChange(true);
                }
                else
                {
                    SneakChange(false);
                }
            }
        }
    }

    private bool hasCrouch = false;
    private bool showSneakEffect = false;
 
    private void OnPlayerAddStation(Player.Station obj)
    {
        if (obj == Player.Station.Crouch)
        {
            hasCrouch = true;
        }
        else if (obj == Player.Station.Death)
        {
            for (int i = 0; i < effectCongfigs.Length; i++)
            {
                if (effectCongfigs[i].isContinue && effectCongfigs[i].effect != null)
                {
                    effectCongfigs[i].effect.gameObject.OnActive(false);
                }
            }
        }

    }
    private void OnPlayerRemoveStation(Player.Station obj)
    {
        if (obj == Player.Station.Crouch)
        {
            hasCrouch = false;
            SneakChange(false);
        }
    }

    private void SneakChange(bool b)
    {
        if (showSneakEffect == b)
        {
            return;
        }
        showSneakEffect = b;
        if (crouchEffectTweener != null)
        {
            crouchEffectTweener.Kill();
        }
        if (b)
        {
            crouchEffectTweener = crouchEffectImage.DOFade(1, 0.5f).SetUpdate(true).SetId(gameObject);
        }
        else {
            crouchEffectTweener = crouchEffectImage.DOFade(0, 0.5f).SetUpdate(true).SetId(gameObject);
        }
    }

    private void OnDestroy()
    {
        EventCenter.UnRegister<int>(EventKey.OnPlayerHurt, OnPlayerHurt);
        Async.StopAsync(gameObject);
        DOTween.Kill(gameObject);
    }

    public void OnPlayerHurt(int type)
    {
        if (curEffect==null)
        {
            EffectCongfig c = effectCongfigs.Find((a) => a.type == type);
            if (c != null)
            {
                if (c.isContinue)
                {
                    if (c.effect == null)
                    {
                        AssetLoad.LoadGameObject<GameUIEffect>(c.effectPath, transform, (e, o) =>
                        {
                            if (e != null)
                            {
                                e.transform.localScale = Vector3.one;
                                c.effect = e;
                                e.Show();
                            }
                        });
                    }
                    else {
                        c.effect.Show();
                    }
                }
                else {
                    AssetLoad.LoadGameObject<GameUIEffect>(c.effectPath, transform, (e, o) =>
                    {
                        if (e != null)
                        {
                            e.transform.localScale = Vector3.one;
                            Vector2 pos = UnityEngine.Random.insideUnitCircle * 400f;
                            (e.transform as RectTransform).anchoredPosition = pos;
                            e.Show();
                        }
                    });
                }
            }
        }
    }

    public async void PlayerEffect(float time)
    {
        curEffect.OnActive(true);
        await Async.WaitforSecondsRealTime(time, gameObject);
        curEffect.OnActive(false);
        curEffect = null;
    }

    private void OnPlayerHpChange(float change)
    {
        hpLowEffect.OnActive(Player.player.hp < (Player.player.MaxHp * 0.33f));
    }
}
