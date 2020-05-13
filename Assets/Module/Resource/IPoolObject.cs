using UnityEngine;

namespace Module
{
    public interface IPoolObject
    {
        ObjectPool pool { get; set; }
        void OnGetObjectFromPool();
        void OnReturnToPool();
    }
}