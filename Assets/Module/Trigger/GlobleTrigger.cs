using System.Collections.Generic;

namespace Module
{
    public class GlobleTrigger
    {
        public static Dictionary<object, object> trigger = new Dictionary<object, object>();

        public static void AddTrigger(object key, bool local)
        {
            AddTrigger(key, local, null);
        }

        public static void AddTrigger(object key, bool local, object value)
        {
            if (!trigger.ContainsKey(key))
            {
                trigger.Add(key, value);
                if (local)
                {
                    LocalFileMgr.Record(key.ToString());
                }
            }
        }

        public static bool Trigger(object key)
        {
            if (trigger.ContainsKey(key))
            {
                trigger.Remove(key);
                LocalFileMgr.RemoveKey(key.ToString());
                return true;
            }

            return false;
        }

        public static T Trigger<T>(object key)
        {
            object result = default;
            if (trigger.TryGetValue(key, out result))
            {
                trigger.Remove(key);
                LocalFileMgr.RemoveKey(key.ToString());
                return (T)result;
            }

            return default;
        }
    }
}