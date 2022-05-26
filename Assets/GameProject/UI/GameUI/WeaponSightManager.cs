using Module;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSightManager : MonoBehaviour
{
    [System.Serializable]
    public class WeaponSightConfig
    {
        public WeaponType weaponType;
        public WeaponSight sight;
    }
    public WeaponSight normalSight;
    public GameObject hiteffect;
    public GameObject killffect;
    public GameObject headHitEffect;
    public GameObject headKillEffect;
    public List<WeaponSightConfig> weaponSightConfigs = new List<WeaponSightConfig>();
    private WeaponSight currentWeaponSight;
    private Clock clock;

    private void Awake()
    {
        ChangeWeapon(Player.player.currentWeapon);
        Player.player.onAddStation += OnPlayerAddStation;
        Player.player.onRemoveStation += OnPlayerRemoveStation;
    }

    private void Start()
    {
        clock = new Clock(0.3f);
        clock.onComplete += Clock_onComplete;
        EventCenter.Register<bool, MonsterPart>(EventKey.HitMonster, OnPlayerFire);
    }

    private void Clock_onComplete()
    {
        hiteffect.OnActive(false);
        killffect.OnActive(false);
        headHitEffect.OnActive(false);
        headKillEffect.OnActive(false);
    }

    private void OnDestroy()
    {
        EventCenter.UnRegister<bool, MonsterPart>(EventKey.HitMonster, OnPlayerFire);
    }
    private void OnPlayerAddStation(Player.Station station)
    {
        switch (station)
        {
            case Player.Station.Aim:
                ShowSight();
                break;
            case Player.Station.WeaponChanging:
                HideSight();
                break;
            default:
                break;
        }
    }

    private void OnPlayerRemoveStation(Player.Station station)
    {
        switch (station)
        {
            case Player.Station.Aim:
                HideSight();
                break;
            case Player.Station.Reloading:
                if (Player.player.ContainStation(Player.Station.Aim))
                {
                    ShowSight();
                }
                break;
            case Player.Station.WeaponChanging:
                ChangeWeapon(Player.player.currentWeapon);
                if (Player.player.ContainStation(Player.Station.Aim))
                {
                    ShowSight();
                }
                break;
            default:
                break;
        }
    }
    public void ChangeWeapon(Weapon weapon)
    {
        WeaponSight nextWeaponSight = null;
        if (weapon != null)
        {
            nextWeaponSight = weaponSightConfigs.Find(x => x.weaponType.HasFlag(weapon.weaponType)).sight;
            if (nextWeaponSight != null)
                nextWeaponSight.SetWeapon(weapon);
        }
        currentWeaponSight = nextWeaponSight;
        if (currentWeaponSight != null && !Player.player.isAim)
        {
            normalSight.ShowSight();
        }
        else
        {
            normalSight.HideSight();
        }
    }

    public void ShowSight()
    {
        currentWeaponSight?.ShowSight();
        if (currentWeaponSight != null)
        {
            normalSight.HideSight();
        }
    }

    public void HideSight()
    {
        currentWeaponSight?.HideSight();
        if (currentWeaponSight != null)
        {
            normalSight.ShowSight();
        }
    }

    public void OnPlayerFire(bool b, MonsterPart part)
    {
        if (b)
        {
            if (part.partType == MonsterPartType.Head || headHitEffect.activeSelf)
            {
                if (hiteffect.activeSelf)
                {
                    hiteffect.OnActive(false);
                }
                if (!headHitEffect.activeSelf)
                {
                    headHitEffect.OnActive(true);
                    clock.Restart();
                }
                return;
            }
            if (!hiteffect.activeSelf)
            {
                hiteffect.OnActive(true);
                clock.Restart();
            }
            return;
        }
        hiteffect.OnActive(false);
        headHitEffect.OnActive(false);

        if (part.partType == MonsterPartType.Head && !headKillEffect.activeSelf)
        {
            headKillEffect.OnActive(true);
            clock.Restart();
        }
        else if (!killffect.activeSelf)
        {
            killffect.OnActive(true);
            clock.Restart();
        }
    }
}

