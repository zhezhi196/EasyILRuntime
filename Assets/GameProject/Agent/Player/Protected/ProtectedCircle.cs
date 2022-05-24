using System;
using System.Collections.Generic;
using Module;
using Unity.Collections;
using UnityEngine;

public class ProtectedCircle : MonoBehaviour
{
    public ProtectedHuman protectedHuman;

    public float radius
    {
        get { return protectedHuman.finalAtt.angerRadius; }
    }
    public LineRenderer line;
    public int spiteCount = 30;

    private Quaternion rotation;
    private List<Vector3> v = new List<Vector3>();

    private void Start()
    {
        rotation = Quaternion.LookRotation(Vector3.up, Vector3.up);
    }

    private void Update()
    {
        int step = 5;
        v.Clear();
        line.positionCount = -1+360 / 5;
        for (float i = 0; i <= 360; i += step)
        {
            Vector3 next = GetCirclePos(i, rotation, radius);
            v.Add(next + transform.position);
        }

        line.SetPositions(v.ToArray());
    }
    
    private static Vector3 GetCirclePos(float angle, Quaternion rotation, float radius)
    {
        angle = angle * Mathf.Deg2Rad;
        float x = radius * Mathf.Cos(angle);
        float y = radius * Mathf.Sin(angle);
        float z = 0;
        return (rotation * new Vector3(x, y, z));
    }
}