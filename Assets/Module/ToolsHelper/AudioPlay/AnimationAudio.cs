using UnityEngine;
using UnityEngine.Animations;

namespace Module
{
    public class AnimationAudio: StateMachineBehaviour
    {
        public string audioPart;
        public string audioKey;
        public bool isOneShot;
        public bool isLoop;
        public bool is3D;
        public AudioPlay play;
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
        {
            var obj = animator.GetComponentInParent<IAgentAudioObject>();
            if (isOneShot)
            {
                //play = obj.GetAgentCtrl<AudioCtrl>().PlayAudioOneShot(audioPart, audioKey);
            }
            else
            {
                //play = obj.GetAgentCtrl<AudioCtrl>().PlayAudio(audioPart, audioKey);
            }

            if (play != null)
            {
                if (isLoop)
                {
                    play.SetLoop(isLoop);
                }

                if (is3D)
                {
                    play.Set3D();
                }
                play.SetID(obj.transform.gameObject);
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
        {
            if (play != null)
            {
                play.Stop();
            }
        }
    }
}