namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("执行实际中断。这将立即停止指定任务的运行，并根据中断成功的值返回成功或失败。")]
    [TaskIcon("{SkinColor}PerformInterruptionIcon.png")]
    public class PerformInterruption : Action
    {
        [Tooltip("The list of tasks to interrupt. Can be any number of tasks")]
        public Interrupt[] interruptTasks;
        [Tooltip("When we interrupt the task should we return a task status of success?")]
        public SharedBool interruptSuccess;

        public override TaskStatus OnUpdate()
        {
            // Loop through all of the tasks and fire an interruption. Once complete return success.
            for (int i = 0; i < interruptTasks.Length; ++i) {
                //interruptTasks[i].DoInterrupt(interruptSuccess.Value ? TaskStatus.Success : TaskStatus.Failure);
                interruptTasks[i].Disabled = true;
            }
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            // Reset the properties back to their original values.
            interruptTasks = null;
            interruptSuccess = false;
        }
    }
}