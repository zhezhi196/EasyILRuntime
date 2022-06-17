using System;
using System.Collections;
using Module;
using UnityEngine;

public class DropBrick : MonoBehaviour, InteractiveObject
{
    public BulletEntity entity;
    public LookPoint lookpoint;

    public bool isActive
    {
        get { return true; }
    }

    public bool canInteractive
    {
        get { return true; }
    }

    public InterActiveStyle interactiveStyle
    {
        get { return InterActiveStyle.Handle; }
    }

    public bool isButtonActive
    {
        get { return true; }
    }

    public string tips { get; }
    public string interactiveTips { get; }


    private void OnDisable()
    {
    }

    public bool Interactive(bool fromMonster = false)
    {
        entity.GetReward(1,0);
        StartCoroutine(DestroySelf());
        return true;
    }

    private IEnumerator DestroySelf()
    {
        yield return new WaitForEndOfFrame();
        AssetLoad.Destroy(gameObject);
    }
    
}