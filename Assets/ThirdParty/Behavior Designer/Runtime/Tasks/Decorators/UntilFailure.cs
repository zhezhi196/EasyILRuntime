namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("直到失败任务将继续执行其子任务，直到子任务返回失败为止。")]
    [TaskIcon("{SkinColor}UntilFailureIcon.png")]
    public class UntilFailure : Decorator
    {
        // The status of the child after it has finished running.
        private TaskStatus executionStatus = TaskStatus.Inactive;

        public override bool CanExecute()
        {
            // Keep running until the child task returns failure.
            return executionStatus == TaskStatus.Success || executionStatus == TaskStatus.Inactive;
        }

        public override void OnChildExecuted(TaskStatus childStatus)
        {
            // Update the execution status after a child has finished running.
            executionStatus = childStatus;
        }

        public override void OnEnd()
        {
            // Reset the execution status back to its starting values.
            executionStatus = TaskStatus.Inactive;
        }
    }
}