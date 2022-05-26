using System;
using UnityEngine;

public class PlayerDeadTrigger: MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == MaskLayer.Playerlayer)
        {
            if (Player.player.IsAlive)
            {
                Player.player.OnHurt(1);
            }
        }
    }
}