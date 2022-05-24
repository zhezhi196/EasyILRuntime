using System.Collections.Generic;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("与选择器任务类似，优先级选择器任务将在子任务返回成功时立即返回成功。优先级选择器不会在树中从左到右依次运行任务，而是询问任务的优先级以确定顺序。优先级越高的任务越有可能首先运行。")]
    [TaskIcon("{SkinColor}PrioritySelectorIcon.png")]
    public class PrioritySelector : Composite
    {
        // The index of the child that is currently running or is about to run.
        private int currentChildIndex = 0;
        // The task status of every child task.
        private TaskStatus executionStatus = TaskStatus.Inactive;
        // The order to run its children in. 
        private List<int> childrenExecutionOrder = new List<int>();

        public override void OnStart()
        {
            // Make sure the list is empty before we add child indexes to it.
            childrenExecutionOrder.Clear();

            // Loop through each child task and determine its priority. The higher the priority the lower it goes within the list. The task with the highest
            // priority will be first in the list and will be executed first.
            for (int i = 0; i < children.Count; ++i) {
                float priority = children[i].GetPriority();
                int insertIndex = childrenExecutionOrder.Count;
                for (int j = 0; j < childrenExecutionOrder.Count; ++j) {
                    if (children[childrenExecutionOrder[j]].GetPriority() < priority) {
                        insertIndex = j;
                        break;
                    }
                }
                childrenExecutionOrder.Insert(insertIndex, i);
            }
        }

        public override int CurrentChildIndex()
        {
            // Use the execution order list in order to determine the current child index.
            return childrenExecutionOrder[currentChildIndex];
        }

        public override bool CanExecute()
        {
            // We can continue to execuate as long as we have children that haven't been executed and no child has returned success.
            return currentChildIndex < children.Count && executionStatus != TaskStatus.Success;
        }

        public override void OnChildExecuted(TaskStatus childStatus)
        {
            // Increase the child index and update the execution status after a child has finished running.
            currentChildIndex++;
            executionStatus = childStatus;
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
    }
}