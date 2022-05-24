namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("如果中断任务被中断，它将停止所有子任务的运行。中断可以由执行中断任务触发。中断任务将继续运行其子任务，直到调用此中断。如果没有发生中断并且子任务完成了它的执行，则中断任务将返回子任务分配的值。")]
    [TaskIcon("{SkinColor}InterruptIcon.png")]
    public class Interrupt : Decorator
    {
        // When an interruption occurs return with this status.
        private TaskStatus interruptStatus = TaskStatus.Failure;
        // The status of the child after it has finished running.
        private TaskStatus executionStatus = TaskStatus.Inactive;

        public override bool CanExecute()
        {
            // Continue executing until the child task returns success or failure.
            return executionStatus == TaskStatus.Inactive || executionStatus == TaskStatus.Running;
        }

        public override void OnChildExecuted(TaskStatus childStatus)
        {
            // Update the execution status after a child has finished running.
            executionStatus = childStatus;
        }

        public void DoInterrupt(TaskStatus status)
        {
            // An interruption has occurred. Update the interrupt status and notify the Behavior Manager. The Behavior Manager will stop all
            // child tasks from running.
            interruptStatus = status;

            BehaviorManager.instance.Interrupt(Owner, this);
        }

        public override TaskStatus OverrideStatus()
        {
            // Return the interruption status as our status.
            return interruptStatus;
        }

        public override void OnEnd()
        {
            // Reset the variables back to their starting values.
            interruptStatus = TaskStatus.Failure;
            executionStatus = TaskStatus.Inactive;
        }
    }
}