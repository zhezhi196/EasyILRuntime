namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("任务保护任务类似于多线程编程中的信号量。任务保护任务是为了确保有限的资源不会被过度使用。\n\n例如，可以在播放动画的任务上方放置任务保护。在行为树中的其他位置，您可能还有另一个任务，该任务播放不同的动画，但对该动画使用相同的骨骼。因此，您不希望动画同时播放两次。放置任务保护将允许您指定可以同时访问特定任务的次数。\n\n在上一个动画任务示例中，您将指定访问计数为1。使用此设置，动画任务一次只能由一个任务控制。如果第一个任务正在播放动画，而第二个任务也希望控制动画，则必须等待或完全跳过该任务。")]
    [TaskIcon("{SkinColor}TaskGuardIcon.png")]
    public class TaskGuard : Decorator
    {
        [Tooltip("The number of times the child tasks can be accessed by parallel tasks at once")]
        public SharedInt maxTaskAccessCount;
        [Tooltip("The linked tasks that also guard a task. If the task guard is not linked against any other tasks it doesn't have much purpose. Marked as LinkedTask to " +
                 "ensure all tasks linked are linked to the same set of tasks")]
        [LinkedTask]
        public TaskGuard[] linkedTaskGuards = null;
        [Tooltip("If true the task will wait until the child task is available. If false then any unavailable child tasks will be skipped over")]
        public SharedBool waitUntilTaskAvailable;

        // The number of tasks that are currently using a particular task.
        private int executingTasks = 0;
        // True if the current task is executing.
        private bool executing = false;

        public override bool CanExecute()
        {
            // The child task can execute if the number of executing tasks is less than the maximum number of tasks allowed.
            return executingTasks < maxTaskAccessCount.Value && !executing;
        }

        public override void OnChildStarted()
        {
            // The child task has started to run. Increase the executing tasks counter and notify all of the other linked tasks.
            executingTasks++;
            executing = true;
            for (int i = 0; i < linkedTaskGuards.Length; ++i) {
                linkedTaskGuards[i].taskExecuting(true);
            }
        }

        public override TaskStatus OverrideStatus(TaskStatus status)
        {
            // return a running status if the children are currently waiting for a task to become available
            return (!executing && waitUntilTaskAvailable.Value) ? TaskStatus.Running : status;
        }

        public void taskExecuting(bool increase)
        {
            // A linked task is currently executing a task that is being guarded. If the task just started executing then increase will be true and if it is ending then
            // true will be false.
            executingTasks += (increase ? 1 : -1);
        }

        public override void OnEnd()
        {
            // The child task has been executed or skipped over. Only decrement the executing tasks count if the child task was being executed. Following that
            // notify all of the linked tasks that we are done executing.
            if (executing) {
                executingTasks--;
                for (int i = 0; i < linkedTaskGuards.Length; ++i) {
                    linkedTaskGuards[i].taskExecuting(false);
                }
                executing = false;
            }
        }

        public override void OnReset()
        {
            // Reset the public properties back to their original values
            maxTaskAccessCount = null;
            linkedTaskGuards = null;
            waitUntilTaskAvailable = true;
        }
    }
}