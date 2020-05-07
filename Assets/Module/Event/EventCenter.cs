/*
 * 脚本名称：EventCenter
 * 项目名称：FrameWork
 * 脚本作者：黄哲智
 * 创建时间：2018-01-06 20:16:55
 * 脚本作用：
*/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Module
{
    public class EventCenter
    {
        private static Dictionary<string, GameEventBase> m_EventPool = new Dictionary<string, GameEventBase>();
        private static Dictionary<string, GameEventBase> eventCache = new Dictionary<string, GameEventBase>();

        #region 添加事件发布者

        private static GameEvent AddDispatcher(string ID)
        {
            GameEvent gameEvent = null;
            if (!m_EventPool.ContainsKey(ID))
            {
                gameEvent = new GameEvent(ID);
                m_EventPool.Add(ID, gameEvent);
                return gameEvent;
            }

            gameEvent = m_EventPool[ID] as GameEvent;
            if (gameEvent == null)
            {
                GameDebug.LogError("EventID:" + ID + "的类型是" + m_EventPool[ID].GetType().FullName);
            }

            return gameEvent;
        }

        private static GameEvent<T> AddDispatcher<T>(string ID)
        {
            GameEvent<T> gameEvent = null;
            if (!m_EventPool.ContainsKey(ID))
            {
                gameEvent = new GameEvent<T>(ID);
                m_EventPool.Add(ID, gameEvent);
                return gameEvent;
            }

            gameEvent = m_EventPool[ID] as GameEvent<T>;
            if (gameEvent == null)
            {
                GameDebug.LogError("EventID:" + ID + "的类型是" + m_EventPool[ID].GetType().FullName);
            }

            return gameEvent;
        }

        private static GameEvent<T, K> AddDispatcher<T, K>(string ID)
        {
            GameEvent<T, K> gameEvent = null;
            if (!m_EventPool.ContainsKey(ID))
            {
                gameEvent = new GameEvent<T, K>(ID);
                m_EventPool.Add(ID, gameEvent);
                return gameEvent;
            }

            gameEvent = m_EventPool[ID] as GameEvent<T, K>;
            if (gameEvent == null)
            {
                GameDebug.LogError("EventID:" + ID + "的类型是" + m_EventPool[ID].GetType().FullName);
            }

            return gameEvent;
        }

        private static GameEvent<T, K, L> AddDispatcher<T, K, L>(string ID)
        {
            GameEvent<T, K, L> gameEvent = null;
            if (!m_EventPool.ContainsKey(ID))
            {
                gameEvent = new GameEvent<T, K, L>(ID);
                m_EventPool.Add(ID, gameEvent);
                return gameEvent;
            }

            gameEvent = m_EventPool[ID] as GameEvent<T, K, L>;
            if (gameEvent == null)
            {
                GameDebug.LogError("EventID:" + ID + "的类型是" + m_EventPool[ID].GetType().FullName);
            }

            return gameEvent;
        }

        private static GameEvent<T, K, L, M> AddDispatcher<T, K, L, M>(string ID)
        {
            GameEvent<T, K, L, M> gameEvent = null;
            if (!m_EventPool.ContainsKey(ID))
            {
                gameEvent = new GameEvent<T, K, L, M>(ID);
                m_EventPool.Add(ID, gameEvent);
                return gameEvent;
            }

            gameEvent = m_EventPool[ID] as GameEvent<T, K, L, M>;
            if (gameEvent == null)
            {
                GameDebug.LogError("EventID:" + ID + "的类型是" + m_EventPool[ID].GetType().FullName);
            }

            return gameEvent;
        }

        private static GameEvent<T, K, L, M, N> AddDispatcher<T, K, L, M, N>(string ID)
        {
            GameEvent<T, K, L, M, N> gameEvent = null;
            if (!m_EventPool.ContainsKey(ID))
            {
                gameEvent = new GameEvent<T, K, L, M, N>(ID);
                m_EventPool.Add(ID, gameEvent);
                return gameEvent;
            }

            gameEvent = m_EventPool[ID] as GameEvent<T, K, L, M, N>;
            if (gameEvent == null)
            {
                GameDebug.LogError("EventID:" + ID + "的类型是" + m_EventPool[ID].GetType().FullName);
            }

            return gameEvent;
        }

        private static GameEvent<T, K, L, M, N, B> AddDispatcher<T, K, L, M, N, B>(string ID)
        {
            GameEvent<T, K, L, M, N, B> gameEvent = null;
            if (!m_EventPool.ContainsKey(ID))
            {
                gameEvent = new GameEvent<T, K, L, M, N, B>(ID);
                m_EventPool.Add(ID, gameEvent);
                return gameEvent;
            }

            gameEvent = m_EventPool[ID] as GameEvent<T, K, L, M, N, B>;
            if (gameEvent == null)
            {
                GameDebug.LogError("EventID:" + ID + "的类型是" + m_EventPool[ID].GetType().FullName);
            }

            return gameEvent;
        }

        #endregion

        #region 分发消息

        public static void Dispatch(string ID)
        {
            AddDispatcher(ID).Invoke();
        }

        public static void Dispatch<T>(string ID, T arg)
        {
            AddDispatcher<T>(ID).Invoke(arg);
        }

        public static void Dispatch<T, K>(string ID, T arg1, K arg2)
        {
            AddDispatcher<T, K>(ID).Invoke(arg1, arg2);
        }

        public static void Dispatch<T, K, L>(string ID, T arg1, K arg2, L arg3)
        {
            AddDispatcher<T, K, L>(ID).Invoke(arg1, arg2, arg3);
        }

        public static void Dispatch<T, K, L, M>(string ID, T arg1, K arg2, L arg3, M arg4)
        {
            AddDispatcher<T, K, L, M>(ID).Invoke(arg1, arg2, arg3, arg4);
        }

        public static void Dispatch<T, K, L, M, N>(string ID, T arg1, K arg2, L arg3, M arg4, N arg5)
        {
            AddDispatcher<T, K, L, M, N>(ID).Invoke(arg1, arg2, arg3, arg4, arg5);
        }

        public static void Dispatch<T, K, L, M, N, B>(string ID, T arg1, K arg2, L arg3, M arg4, N arg5, B arg6)
        {
            AddDispatcher<T, K, L, M, N, B>(ID).Invoke(arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static void WaitDispatch(string ID)
        {
            GameEvent gameEvent = AddDispatcher(ID);
            if (!eventCache.ContainsKey(ID))
            {
                eventCache.Add(ID, gameEvent);
            }
        }

        public static void WaitDispatch<T>(string ID, T arg)
        {
            GameEvent<T> gameEvent = AddDispatcher<T>(ID);
            if (!eventCache.ContainsKey(ID))
            {
                eventCache.Add(ID, gameEvent);
            }

            gameEvent.SetArgs(arg);
        }

        public static void WaitDispatch<T, K>(string ID, T arg1, K arg2)
        {
            GameEvent<T, K> gameEvent = AddDispatcher<T, K>(ID);
            if (!eventCache.ContainsKey(ID))
            {
                eventCache.Add(ID, gameEvent);
            }

            gameEvent.SetArgs(arg1, arg2);
        }

        public static void WaitDispatch<T, K, L>(string ID, T arg1, K arg2, L arg3)
        {
            GameEvent<T, K, L> gameEvent = AddDispatcher<T, K, L>(ID);
            if (!eventCache.ContainsKey(ID))
            {
                eventCache.Add(ID, gameEvent);
            }

            gameEvent.SetArgs(arg1, arg2, arg3);
        }

        public static void WaitDispatch<T, K, L, M>(string ID, T arg1, K arg2, L arg3, M arg4)
        {
            GameEvent<T, K, L, M> gameEvent = AddDispatcher<T, K, L, M>(ID);
            if (!eventCache.ContainsKey(ID))
            {
                eventCache.Add(ID, gameEvent);
            }

            gameEvent.SetArgs(arg1, arg2, arg3, arg4);
        }

        public static void WaitDispatch<T, K, L, M, N>(string ID, T arg1, K arg2, L arg3, M arg4, N arg5)
        {
            GameEvent<T, K, L, M, N> gameEvent = AddDispatcher<T, K, L, M, N>(ID);
            if (!eventCache.ContainsKey(ID))
            {
                eventCache.Add(ID, gameEvent);
            }

            gameEvent.SetArgs(arg1, arg2, arg3, arg4, arg5);
        }

        public static void WaitDispatch<T, K, L, M, N, B>(string ID, T arg1, K arg2, L arg3, M arg4, N arg5, B arg6)
        {
            GameEvent<T, K, L, M, N, B> gameEvent = AddDispatcher<T, K, L, M, N, B>(ID);
            if (!eventCache.ContainsKey(ID))
            {
                eventCache.Add(ID, gameEvent);
            }

            gameEvent.SetArgs(arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static void TriggerDispatch(string ID)
        {
            if (eventCache.ContainsKey(ID))
            {
                eventCache[ID].Invoke();
                eventCache.Remove(ID);
            }
        }

        #endregion

        #region Register

        public static void Register(string key, Action callBack)
        {
            AddDispatcher(key).Register(callBack);
        }

        public static void Register<T>(string key, Action<T> callback)
        {
            AddDispatcher<T>(key).Register(callback);
        }

        public static void Register<T, K>(string key, Action<T, K> callback)
        {
            AddDispatcher<T, K>(key).Register(callback);
        }

        public static void Register<T, K, L>(string key, Action<T, K, L> callback)
        {
            AddDispatcher<T, K, L>(key).Register(callback);
        }

        public static void Register<T, K, L, M>(string key, Action<T, K, L, M> callback)
        {
            AddDispatcher<T, K, L, M>(key).Register(callback);
        }

        public static void Register<T, K, L, M, N>(string key, Action<T, K, L, M, N> callback)
        {
            AddDispatcher<T, K, L, M, N>(key).Register(callback);
        }

        #endregion

        #region UnRegister

        public static void UnRegister(string key, Action callBack)
        {
            AddDispatcher(key).Unregister(callBack);
        }

        public static void UnRegister<T>(string key, Action<T> callback)
        {
            AddDispatcher<T>(key).Register(callback);
        }

        public static void UnRegister<T, K>(string key, Action<T, K> callback)
        {
            AddDispatcher<T, K>(key).Register(callback);
        }

        public static void UnRegister<T, K, L>(string key, Action<T, K, L> callback)
        {
            AddDispatcher<T, K, L>(key).Register(callback);
        }

        public static void UnRegister<T, K, L, M>(string key, Action<T, K, L, M> callback)
        {
            AddDispatcher<T, K, L, M>(key).Register(callback);
        }

        public static void UnRegister<T, K, L, M, N>(string key, Action<T, K, L, M, N> callback)
        {
            AddDispatcher<T, K, L, M, N>(key).Register(callback);
        }

        #endregion
    }
}