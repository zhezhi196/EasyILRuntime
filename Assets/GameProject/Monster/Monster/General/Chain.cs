using Module;
using UnityEngine;

public class Chain : MonoBehaviour,IPoolObject
{
    public Transform nextChainPos;
    public float distance;

    public void Join(Transform arc)
    {
        transform.position = arc.position;
        transform.rotation = arc.rotation;
    }

    public ObjectPool pool { get; set; }
    public void ReturnToPool()
    {
    }

    public void OnGetObjectFromPool()
    {
    }
}