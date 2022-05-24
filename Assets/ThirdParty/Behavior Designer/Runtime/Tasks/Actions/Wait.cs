using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("等待指定的时间。任务将返回运行状态，直到任务完成等待。等待时间过后，它将返回成功。")]
    [TaskIcon("{SkinColor}WaitIcon.png")]
    public class Wait : Action
    {
        [Tooltip("The amount of time to wait")]
        public SharedFloat waitTime = 1;
        [Tooltip("Should the wait be randomized?")]
        public SharedBool randomWait = false;
        [Tooltip("The minimum wait time if random wait is enabled")]
        public SharedFloat randomWaitMin = 1;
        [Tooltip("The maximum wait time if random wait is enabled")]
        public SharedFloat randomWaitMax = 1;

        public bool ignoreTimescale;

        // The time to wait
        protected float waitDuration;

        protected float pastTime;

        protected bool isPause;


        public override void OnStart()
        {
            if (randomWait.Value) {
                waitDuration = Random.Range(randomWaitMin.Value, randomWaitMax.Value);
            } else {
                waitDuration = waitTime.Value;
            }
        }

        public override void OnBehaviorComplete()
        {
            Debug.Log("OnBehaviorComplete");
        }

        public override void OnEnd()
        {
            Debug.Log("OnEnd");

        }

        public override TaskStatus OnUpdate()
        {
            if (!isPause)
            {
                float delatime = ignoreTimescale ? Time.unscaledDeltaTime : Time.deltaTime;
                pastTime += delatime;
                if (pastTime >= waitDuration)
                {
                    pastTime = 0;
                    return TaskStatus.Success;
                }
            }
            
            return TaskStatus.Running;
        }

        public override void OnPause(bool paused)
        {
            Debug.Log("onPause"+paused);
            isPause = paused;
        }

        public override void OnReset()
        {
            Debug.Log("OnReset");
            waitTime = 1;
            randomWait = false;
            randomWaitMin = 1;
            randomWaitMax = 1;
            isPause = false;
        }
    }
}