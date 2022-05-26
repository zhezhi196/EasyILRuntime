using System;
using Module;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

public abstract class EscapeMonster: Monster
{
   [LabelText("看到玩家多少秒后逃跑")]
   public float seePlayerEscape = 3;

   public float seePlayerTime;
   public bool isEscape;

   protected override void AddCtrl()
   {
      ctrlList.Add(new AnimatorCtrl(this));
      ctrlList.Add(new AudioCtrl(this));
      if (!creator.notNav)
      {
         ctrlList.Add(new NavMoveCtrl(this));
      }
      ctrlList.Add(new SimpleBehaviorCtrl(this));
   }

   protected override void OnHearTarget(Vector3 arg2, object[] arg3)
   {
      Escape();
   }

   protected override void Update()
   {
      base.Update();
      if (isSeePlayer)
      {
         seePlayerTime += TimeHelper.deltaTime;
         if (seePlayerTime >= seePlayerEscape)
         {
            Escape();
         }
      }
      else
      {
         seePlayerTime = 0;
      }
   }


   public void Escape()
   {
      //GetAgentCtrl<SimpleBehaviorCtrl>().SwitchBehavior(new ex);
      idleStyle.BreakFree();
      isEscape = true;
   }
}