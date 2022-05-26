using Module;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerPartType
{ 
    Player,
    DodgePoint,
}
/// <summary>
/// Player受击检测点
/// </summary>
public class PlayerPart : MonoBehaviour, ITarget
{
    public Player player;
    public PlayerPartType partType = PlayerPartType.Player;
    public Hurtmaterial hurtMaterial => Hurtmaterial.Meat;

    public bool isVisiable => true;

    public virtual PlayerDamage OnHurt(PlayerDamage playerDamage)
    {
        player.OnHurt(playerDamage);
        return playerDamage;
    }

    public virtual void Remove() { }

    public virtual Vector3 CheckPositon
    {
        get {
            return transform.position;
        }
    }

    public Vector3 targetPoint => transform.position;
}
