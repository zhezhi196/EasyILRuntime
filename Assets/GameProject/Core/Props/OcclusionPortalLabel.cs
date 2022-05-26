using System;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(OcclusionPortal))]
public class OcclusionPortalLabel: MonoBehaviour
{
    public static List<OcclusionPortalLabel> labels = new List<OcclusionPortalLabel>();
    
    public string key;
    public OcclusionPortal portal;

    private void Awake()
    {
        if (portal == null)
        {
            portal = GetComponent<OcclusionPortal>();
        }

        labels.Add(this);
    }

    private void OnEnable()
    {
        labels.Add(this);
    }

    private void OnDisable()
    {
        labels.Remove(this);
    }
}