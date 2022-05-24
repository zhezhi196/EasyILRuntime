namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("与选择器任务类似，并行选择器任务将在子任务返回成功时立即返回成功。不同之处在于并行任务将同时运行其所有子任务，而不是一次运行一个任务。如果一个任务返回成功，并行选择器任务将结束所有子任务并返回成功。如果每个子任务返回失败，则并行选择器任务将返回失败。")]
    [TaskIcon("{SkinColor}ParallelSelectorIcon.png")]
    public class ParallelSelector : Composite
    {
        // The index of the child that is currently running or is about to run.
        private int currentChildIndex;
        // The task status of every child task.
        private TaskStatus[] executionStatus;

        public override void OnAwake()
        {
            // Create a new task status array that will hold the execution status of all of the children tasks.
            executionStatus = new TaskStatus[children.Count];
        }

        public override void OnChildStarted(int childIndex)
        {
            // One of the children has started to run. Increment the child index and set the current task status of that child to running.
            currentChildIndex++;
            executionStatus[childIndex] = TaskStatus.Running;
        }

        public override bool CanRunParallelChildren()
        {
            // This task can run parallel children.
            return true;
        }

        public override int CurrentChildIndex()
        {
            return currentChildIndex;
        }

        public override bool CanExecute()
        {
            // We can continue executing if we have more children that haven't been started yet.
            return currentChildIndex < children.Count;
        }

        public override void OnChildExecuted(int childIndex, TaskStatus childStatus)
        {
            // One of the children has finished running. Set the task status.
            executionStatus[childIndex] = childStatus;
        }

        public override void OnConditionalAbort(int childIndex)
        {
            // Start from the beginning on an abort
            currentChildIndex = 0;
            for (int i = 0; i < executionStatus.Length; ++i) {
                executionStatus[i] = TaskStatus.Inactive;
            }
        }

        public override TaskStatus OverrideStatus(TaskStatus status)
        {
            // Assume all of the children have finished executing. Loop through the execution status of every child and check to see if any tasks are currently running
            // or succeeded. If a task is still running then all of the children are not done executing and the parallel selector task should continue to return a task status of running.
            // If a task succeeded then return success. The Behavior Manager will stop all of the children tasks. If no child task is running or has succeeded then the parallel selector
            // task failed and it will return failure.
            bool childrenComplete = true;
            for (int i = 0; i < executionStatus.Length; ++i) {
                if (executionStatus[i] == TaskStatus.Running) {
                    childrenComplete = false;
                } else if (executionStatus[i] == TaskStatus.Success) {
                    return TaskStatus.Success;
                }
            }
            return (childrenComplete ? TaskStatus.Failure : TaskStatus.Running);
        }

        public override void OnEnd()
        {
            // Reset the execution status and the child index back to their starting values.
            for (int i = 0; i < executionStatus.Length; ++i) {
                executionStatus[i] = TaskStatus.Inactive;
            }
            currentChildIndex = 0;
        }
    }
}