using System;
using UnityEngine;

public class TwistCheck : MonoBehaviour
{
    public Transform target;
    public Monster monster;

    private void Awake()
    {
        monster = transform.GetComponentInParent<Monster>();
    }

    private void LateUpdate()
    {
        transform.localEulerAngles = target.localEulerAngles;
    }
}