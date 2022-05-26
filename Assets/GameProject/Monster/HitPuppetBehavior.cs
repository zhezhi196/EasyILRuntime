// using RootMotion.Dynamics;
// using UnityEngine;
//
// public class HitPuppetBehavior : BehaviourBase
// {
//     public override void OnReactivate()
//     {
//     }
//     
//     protected override void OnMuscleHitBehaviour(MuscleHit hit) {
//         // Should we activate the puppet?
//         if (masterProps.normalMode == BehaviourPuppet.NormalMode.Kinematic) puppetMaster.mode = PuppetMaster.Mode.Active;
//
//         // Unpin the muscle (and other muscles) and add force
//         UnPin(hit.muscleIndex, hit.unPin);
//
//         // Add force
//         puppetMaster.muscles[hit.muscleIndex].SetKinematic(false);
//         puppetMaster.muscles[hit.muscleIndex].rigidbody.AddForceAtPosition(hit.force, hit.position);
//     }
//
//     public void UnPin(int muscleIndex, float unpin)
//     {
//         if (muscleIndex >= puppetMaster.muscles.Length) return;
//
//         BehaviourPuppet.MuscleProps props = GetProps(puppetMaster.muscles[muscleIndex].props.group);
//
//         for (int i = 0; i < puppetMaster.muscles.Length; i++)
//         {
//             UnPinMuscle(i,
//                 unpin * GetFalloff(i, muscleIndex, props.unpinParents, props.unpinChildren, props.unpinGroup));
//         }
//
//         hasCollidedSinceGetUp = true;
//     }
//     
//     private void UnPinMuscle(int muscleIndex, float unpin) {
//         // All the conditions to ignore this
//         if (unpin <= 0f) return;
//         if (puppetMaster.muscles[muscleIndex].state.immunity >= 1f) return;
//
//         // Find the group properties
//         BehaviourPuppet.MuscleProps props = GetProps(puppetMaster.muscles[muscleIndex].props.group);
//
//         // Making the puppet more resistant to collisions while getting up
//         float stateF = 1f;
//         if (state == State.GetUp) stateF = Mathf.Lerp(getUpCollisionResistanceMlp, 1f, puppetMaster.muscles[muscleIndex].state.pinWeightMlp);
//
//         // Applying collision resistance
//         float cR = collisionResistance.mode == Weight.Mode.Float? collisionResistance.floatValue: collisionResistance.GetValue(puppetMaster.muscles[muscleIndex].targetVelocity.magnitude);
//         float damage = unpin / (props.collisionResistance * cR * stateF);
//         damage *= 1f - puppetMaster.muscles[muscleIndex].state.immunity;
//
//         // Finally apply the damage
//         //puppetMaster.muscles[muscleIndex].state.pinWeightMlp -= damage;
//         if (!puppetMaster.muscles[muscleIndex].state.isDisconnected)
//         {
//             puppetMaster.muscles[muscleIndex].state.pinWeightMlp = Mathf.Max(puppetMaster.muscles[muscleIndex].state.pinWeightMlp - damage, props.minPinWeight);
//         }
//     }
//     private BehaviourPuppet.MuscleProps GetProps(Muscle.Group group) {
//         foreach (BehaviourPuppet.MusclePropsGroup g in groupOverrides) {
//             foreach (Muscle.Group group2 in g.groups) {
//                 if (group2 == group) return g.props;
//             }
//         }
//         return defaults;
//     }
//
// }