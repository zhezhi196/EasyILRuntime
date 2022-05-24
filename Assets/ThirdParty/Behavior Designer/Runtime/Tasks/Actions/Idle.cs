

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("返回运行的任务状态。仅当中断或触发条件中止时才会停止。")]
    [TaskIcon("{SkinColor}IdleIcon.png")]
    public class Idle : Action
    {
        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Running;
        }
    }
}