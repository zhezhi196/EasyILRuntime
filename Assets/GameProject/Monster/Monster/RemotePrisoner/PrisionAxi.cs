using System;
using Module;
using Sirenix.OdinInspector;
using UnityEngine;

public class PrisionAxi : FlyingObject
{
    public Transform axi;
    public Transform rotateAxi;
    public float rotateSpeed = 5;
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        axi.RotateAround(axi.transform.position,rotateAxi.transform.right,TimeHelper.fixedDeltaTime * rotateSpeed);
    }
}