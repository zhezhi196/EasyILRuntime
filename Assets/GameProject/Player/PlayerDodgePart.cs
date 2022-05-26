using Module;
using UnityEngine;

/// <summary>
/// Player闪避检测点
/// </summary>
public class PlayerDodgePart:PlayerPart,IPoolObject
{
    private Clock clock;

    public ObjectPool pool { get; set; }

    private void Awake()
    {
        partType = PlayerPartType.DodgePoint;
    }

    public override PlayerDamage OnHurt(PlayerDamage playerDamage)
    {
        if (player != null)
        {
            //GameDebug.LogError("完美闪避");
            //增加一个完美闪避buff
            EventCenter.Dispatch(EventKey.PerfectDodge);
            Remove();
        }
        return playerDamage;
    }

    public void StartClock(Player p, float t)
    {
        if (p != null)
        {
            player = p;
            clock = new Clock(t);
            clock.onComplete += ClockComplete;
            p.PlayerParts.Add(this);
            clock.StartTick();
        }
    }

    public override void Remove()
    {
        if (clock != null && clock.isActive)
        {
            clock.Stop();
            ClockComplete();
        }
    }

    private void ClockComplete()
    {
        if (player != null)
        {
            player.PlayerParts.Remove(this);
            ObjectPool.ReturnToPool(this);
        }
    }

    public void ReturnToPool()
    {
        player = null;
        clock = null;
    }

    public void OnGetObjectFromPool()
    {
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, Vector3.one);
    }
#endif
}
