using System;

namespace Module
{
    public static class EventTools
    {
        public static void SendEvent(int id, string arg, IEventCallback sender)
        {
            EventCenter.Dispatch<string, IEventCallback>(GetKey(id), arg, sender);
            GameDebug.LogFormat("发送事件ID: {0}" , id);
        }
        
        public static void ReceiveEvent(int id, Action<string, IEventCallback> callback)
        {
            EventCenter.Register<string, IEventCallback>(GetKey(id), callback);
        }
        
        public static void UnReceiveEvent(int id, Action<string, IEventCallback> callback)
        {
            EventCenter.UnRegister<string, IEventCallback>(GetKey(id), callback);
        }

        private static string GetKey(int id)
        {
            return "EventKey_" + id;
        }
    }
}