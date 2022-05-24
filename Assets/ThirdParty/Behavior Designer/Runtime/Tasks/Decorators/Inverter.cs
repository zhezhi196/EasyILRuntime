namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("逆变器任务在完成执行后将反转子任务的返回值。如果子任务返回成功，则逆变任务将返回失败。如果子任务返回失败，则逆变任务将返回成功。")]
    [TaskIcon("{SkinColor}InverterIcon.png")]
    public class Inverter : Decorator
    {
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

        public override TaskStatus Decorate(TaskStatus status)
        {
            // Invert the task status.
            if (status == TaskStatus.Success) {
                return TaskStatus.Failure;
            } else if (status == TaskStatus.Failure) {
                return TaskStatus.Success;
            }
            return status;
        }

        public override void OnEnd()
        {
            // Reset the execution status back to its starting values.
            executionStatus = TaskStatus.Inactive;
        }
    }
}