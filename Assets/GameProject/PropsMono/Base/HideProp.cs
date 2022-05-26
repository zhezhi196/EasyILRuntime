using System.Collections;
using System.Collections.Generic;
using Module;
using UnityEngine;
using UnityEngine.AI;

public class HideProp : OnlyInteractive
{
    public Transform hidePoint;
    public Transform outPoint;
    public NavMeshObstacle obstacle;

    public override InterActiveStyle interactiveStyle => InterActiveStyle.HideDoor;

    
    public override bool canInteractive
    {
        get { return true; } //PlayerCtrl.Instance.player.GetAtts().canHide.value; }
    }

    protected override bool OnInteractive(bool fromMonster = false)
    {
        Player.player.EnterHideProp(this);
        obstacle.gameObject.OnActive(true);
        return true;
    }

    public void Exit()
    {
        obstacle.gameObject.OnActive(false);
    }
}