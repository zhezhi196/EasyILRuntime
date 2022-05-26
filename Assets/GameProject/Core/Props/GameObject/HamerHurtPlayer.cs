using System;
using System.Collections;
using UnityEngine;

public class HamerHurtPlayer: MonoBehaviour
{
    public float damage;
    public bool addForce;
    public float force;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == MaskLayer.Playerlayer)
        {
            //Player.player.OnHurt(damage);
            if (addForce)
            {
                Vector3 dir = Player.player.chasePoint - transform.position;
                Player.player.AddImpact(new Vector3(dir.x, 0, dir.z), force);
            }
        }

    }
}