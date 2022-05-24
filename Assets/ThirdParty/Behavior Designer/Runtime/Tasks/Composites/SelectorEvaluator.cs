namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("选择器求值器是一个选择器任务，它每勾选一次就重新计算其子项。它将运行优先级最低的子级，该子级返回任务运行状态。这是每一个滴答。如果高优先级子帧正在运行，而下一帧低优先级子帧要运行它，则会中断高优先级子帧。选择器计算器将在第一个子项返回成功时立即返回成功，否则它将继续尝试更高优先级的子项。此任务模拟条件中止功能，但子任务不一定总是条件任务。")]
    [TaskIcon("{SkinColor}SelectorEvaluatorIcon.png")]
    public class SelectorEvaluator : Composite
    {
        // The index of the child that is currently running or is about to run.
        private int currentChildIndex = 0;
        // The task status of the last child ran.
        private TaskStatus executionStatus = TaskStatus.Inactive;
        // The index of the child that was running before the tree started to be reevaluated.
        private int storedCurrentChildIndex = -1;
        // The task status of the last child ran before the tree started to be reevaluated.
        private TaskStatus storedExecutionStatus = TaskStatus.Inactive;

        public override int CurrentChildIndex()
        {
            return currentChildIndex;
        }

        public override void OnChildStarted(int childIndex)
        {
            // The children run sequentially so increment the index and set the status to running.
            currentChildIndex++;
            executionStatus = TaskStatus.Running;
        }

        public override bool CanExecute()
        {
            // We can continue to execuate as long as we have children that haven't been executed and no child has returned success.
            if (executionStatus == TaskStatus.Success || executionStatus == TaskStatus.Running) {
                return false;
            }

            // Used the storedCurrentChildIndex if reevaluating, otherwise the currentChildIndex
            if (storedCurrentChildIndex != -1) {
                return currentChildIndex < storedCurrentChildIndex - 1;
            }
            return currentChildIndex < children.Count;
        }

        public override void OnChildExecuted(int childIndex, TaskStatus childStatus)
        {
            // A disabled task is the equivalent of the task failing for a selector evaluator.
            if (childStatus == TaskStatus.Inactive && children[childIndex].Disabled) {
                executionStatus = TaskStatus.Failure;
            }
            // The child status will be inactive immediately following an abort from OnReevaluationEnded. The status will be running if the 
            // child task is interrupted. Ignore the status for both of these. 
            if (childStatus != TaskStatus.Inactive && childStatus != TaskStatus.Running) {
                executionStatus = childStatus;
            }
        }

        public override void OnConditionalAbort(int childIndex)
        {
            // Set the current child index to the index that caused the abort
            currentChildIndex = childIndex;
            executionStatus = TaskStatus.Inactive;
        }

        public override void OnEnd()
        {
            // All of the children have run. Reset the variables back to their starting values.
            executionStatus = TaskStatus.Inactive;
            currentChildIndex = 0;
        }

        public override TaskStatus OverrideStatus(TaskStatus status)
        {
            return executionStatus;
        }

        // The selector evaluator task is a parallel task to allow the previous child to continue to run while the higher priority task is active. If the
        // lower priority child can run then OnReevaluationEnded will interrupt the higher priority task.
        public override bool CanRunParallelChildren()
        {
            return true;
        }

        // Can reevaluate to allow the lower priority children the chance to rerun.
        public override bool CanReevaluate()
        {
            return true;
        }

        // The behavior tree wants to start reevaluating the tree.
        public override bool OnReevaluationStarted()
        {
            // Cannot reevaluate if the task hasn't even started yet
            if (executionStatus == TaskStatus.Inactive) {
                return false;
            }

            // Store the current index and execution status because it may need to be resumed.
            storedCurrentChildIndex = currentChildIndex;
            storedExecutionStatus = executionStatus;
            currentChildIndex = 0;
            executionStatus = TaskStatus.Inactive;
            return true;
        }

        // Reevaluation has ended. Determine if a task should be interrupted or resumed from the last index.
        public override void OnReevaluationEnded(TaskStatus status)
        {
            // Interrupt the currently running index if a lower priority child returns a status of running or success.
            if (executionStatus != TaskStatus.Failure && executionStatus != TaskStatus.Inactive) {
                BehaviorManager.instance.Interrupt(Owner, children[storedCurrentChildIndex - 1], this);
            } else {
                // The lower priority children returned the same status so resume with the current child
                currentChildIndex = storedCurrentChildIndex;
                executionStatus = storedExecutionStatus;
            }
            storedCurrentChildIndex = -1;
            storedExecutionStatus = TaskStatus.Inactive;
        }
    }
}