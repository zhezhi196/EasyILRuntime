using Module;
using UnityEngine;
using UnityEngine.AI;

public class Chase : SimpleBehavior
{
   public AttackMonster monster;
   public SkillCtrl skillCtrl;
   public override void OnStart(ISimpleBehaviorObject owner, object[] args)
   {
      base.OnStart(owner, args);
      monster = (AttackMonster) owner;
      skillCtrl = monster.GetAgentCtrl<SkillCtrl>();
   }

   public override TaskStatus OnUpdate()
   {
      if (skillCtrl.readyRelease != null)
      {
         if (!skillCtrl.TryRelease(null))
         {
            skillCtrl.UpdateCheck();
         }
      }

      return TaskStatus.Running;
   }
}