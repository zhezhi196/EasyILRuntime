using Module;
using Sirenix.OdinInspector;
using UnityEngine;

public class FlyingObject : MonoBehaviour, IPoolObject
{
    public LayerMask obstacal;
    public float returnTime = 5;
    public ObjectPool pool { get; set; }
    [ReadOnly]
    public bool isFire;
    public float flyTime;
    [ReadOnly]
    public RemoteAttack skill;
    [ReadOnly]
    public Vector3 dir;

    public Bounds damageBounds;

    public void ReturnToPool()
    {
        isFire = false;
    }

    public void OnGetObjectFromPool()
    {
        isFire = true;
    }

    protected virtual void FixedUpdate()
    {
        if (isFire && Player.player != null)
        {
            flyTime += TimeHelper.fixedDeltaTime;
            transform.position += (dir.normalized * (skill.moveSpeed * TimeHelper.deltaTime));
            if (flyTime >= returnTime)
            {
                ObjectPool.ReturnToPool(this);
            }
            else
            {
                if (skill.monster.HurtPlayer(skill.GetDamage(), damageBounds, transform))
                {
                    ObjectPool.ReturnToPool(this);
                }
                else
                {
                    RaycastHit hit;
                    if (Physics.Raycast(new Ray(transform.position, dir), out hit,
                        TimeHelper.fixedDeltaTime * skill.moveSpeed, obstacal))
                    {
                        if (hit.collider.gameObject.layer != MaskLayer.Playerlayer)
                        {
                            ObjectPool.ReturnToPool(this);
                        }
                    }
                }
            }
        }
    }


    public void Fire(Vector3 dir, RemoteAttack skill)
    {
        this.skill = skill;
        this.dir = dir.normalized;
        flyTime = 0;
    }
}