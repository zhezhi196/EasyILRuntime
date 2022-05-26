using Module;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponEmpty : Weapon
{
    public override bool CanReload
    {
        get { return false; }
    }

    public async override void TakeOut()
    {
        this.gameObject.OnActive(true);
        if (Player.player.ContainStation(Player.Station.Running))
        {
            SetRunStation(true);
        }
        _player.AddStation(Player.Station.WeaponChanging);
        await Async.WaitforSecondsRealTime(0.3f);
        _player.RemoveStation(Player.Station.WeaponChanging);
    }
}
