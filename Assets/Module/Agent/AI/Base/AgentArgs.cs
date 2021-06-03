using System;

namespace Module
{
    public struct AgentArgs<T>
    {
        public T value;
        public Func<bool> listener;

        private Action callback;

        public AgentArgs(T value, Func<bool> listener, Action callback)
        {
            this.value = value;
            this.listener = listener;
            this.callback = callback;
        }

        public void OnUpdate()
        {
            if (listener != null && listener.Invoke())
            {
                callback?.Invoke();
                listener = null;
                callback = null;
            }
        }
    }
}