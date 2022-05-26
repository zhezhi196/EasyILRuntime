using BehaviorDesigner.Runtime.Tasks.Unity.SharedVariables;
using Module;
using UnityEngine;

public class WeaponBowBullet : MonoBehaviour, IPoolObject
{
    public ObjectPool pool { get; set; }
    public float lifeTime = 5f;

    public void OnGetObjectFromPool()
    {
    }

    public void ReturnToPool()
    {
        Async.StopAsync(gameObject);
        transform.localEulerAngles = Vector3.zero;
        
    }

    public void Hit(IHurtObject hurtObject)
    {
        if(hurtObject.transform.localScale==Vector3.one)
            transform.SetParent(hurtObject.transform);
        //if (hurtObject is MonsterPart part)
            //part.AddHideObject(this.gameObject);
        WaitRetun();
    }

    private async void WaitRetun()
    {
        await Async.WaitforSecondsRealTime(lifeTime,gameObject);
        ObjectPool.ReturnToPool(this);
    }

    private void OnDestroy()
    {
        Async.StopAsync(gameObject);
    }
}
