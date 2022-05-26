using System;
using Module;
using UnityEngine;

public abstract class AgentSensorPoint : MonoBehaviour, ISensorTarget
{
    public abstract bool isSenserable { get; }

    // private void OnEnable()
    // {
    //     AgentSee.target.Add(this);
    // }
    //
    // private void OnDisable()
    // {
    //     onDisable?.Invoke(this);
    // }
    public bool isVisiable { get; }
    public Vector3 targetPoint => transform.position;
}