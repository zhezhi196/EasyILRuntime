using System;
using System.Collections.Generic;

namespace Module
{
    public class GlobleTrigger
    {
        public static Dictionary<object, object[]> trigger = new Dictionary<object, object[]>();

        public static void AddTrigger(object key, params object[] value)
        {
            if (!trigger.ContainsKey(key))
            {
                trigger.Add(key, value);
            }
        }

        public static void Trigger(object key, Action<object[]> callback)
        {
            object[] arg = null;
            if (trigger.TryGetValue(key, out arg))
            {
                trigger.Remove(key);
                callback?.Invoke(arg);
            }
        }
    }
}