using System;
using System.Collections;
using Module;
using UnityEngine;


public class DropBag : MonoBehaviour, InteractiveObject
{
    public Drop drop;
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
        if (drop.dbData.saveData == 1)
        {
            BattleController.GetCtrl<DropCtrl>().drops.Remove(this);
        }
    }

    public bool Interactive(bool fromMonster = false)
    {
        this.drop.GetReward();
        StartCoroutine(DestroySelf());
        return true;
    }

    private IEnumerator DestroySelf()
    {
        yield return new WaitForEndOfFrame();
        AssetLoad.Destroy(gameObject);
    }

    public void SetDrop(Drop drop)
    {
        this.drop = drop;
        if (drop.dbData.saveData == 1)
        {
            BattleController.GetCtrl<DropCtrl>().drops.Add(this);
        }

        lookpoint.Init(this);
    }
}