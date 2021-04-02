using System;
using System.Collections.Generic;

namespace Module
{
    public static class QueueRunMethod
    {
        private static Queue<Action> queue = new Queue<Action>();

        public static void Update()
        {
            if (queue.Count > 0)
            {
                queue.Dequeue().Invoke();
            }
        }

        public static void EnqueueAction(Action action)
        {
            queue.Enqueue(action);
        }
    }
}