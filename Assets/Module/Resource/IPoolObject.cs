﻿using UnityEngine;

namespace Module
{
    public interface IPoolObject
    {
        GameObject gameObject { get; }
        ObjectPool pool { get; set; }
        void OnGetObjectFromPool();
        void OnReturnToPool();
    }
}