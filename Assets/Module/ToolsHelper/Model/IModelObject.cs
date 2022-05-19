using System;
using UnityEngine;

namespace Module
{
    public interface IModelObject
    {
        string modelName { get; }
        void GetModel(Action<GameObject> callback);
    }
}