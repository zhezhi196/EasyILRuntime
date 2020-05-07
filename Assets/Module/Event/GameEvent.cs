/*
 * 脚本名称：GameEvent
 * 项目名称：FrameWork
 * 脚本作者：黄哲智
 * 创建时间：2018-01-06 20:08:31
 * 脚本作用：
*/

using System;
using System.Collections.Generic;

namespace Module
{
    public abstract class GameEventBase
    {
        public string ID { protected set; get; }
        public abstract void Invoke();
    }

    #region GameEvent

    public class GameEvent: GameEventBase
    {
        private Action m_Action;

        public GameEvent(string ID)
        {
            this.ID = ID;
        }

        public void Register(Action callBack)
        {
            m_Action -= callBack;
            m_Action += callBack;
        }

        public void Unregister(Action callBack)
        {
            m_Action -= callBack;
        }

        public override void Invoke()
        {
            m_Action?.Invoke();
        }
    }

    #endregion

    #region GameEvent<T>

    public class GameEvent<T>: GameEventBase
    {
        private Action<T> m_Action;
        private List<T> arg1 = new List<T>();

        public GameEvent(string ID)
        {
            this.ID = ID;
        }

        public void Register(Action<T> callBack)
        {
            m_Action -= callBack;
            m_Action += callBack;
        }

        public void Unregister(Action<T> callBack)
        {
            m_Action -= callBack;
        }

        public GameEvent<T> SetArgs(T arg)
        {
            this.arg1.Add(arg);
            return this;
        }

        public override void Invoke()
        {
            for (int i = 0; i < this.arg1.Count; i++)
            {
                m_Action?.Invoke(this.arg1[i]);
            }

            this.arg1.Clear();
        }

        public void Invoke(T arg)
        {
            this.m_Action?.Invoke(arg);
        }
    }

    #endregion

    #region GameEvent<T, K>

    public class GameEvent<T, K>: GameEventBase
    {
        private Action<T, K> m_Action;
        private List<T> arg1 = new List<T>();
        private List<K> arg2 = new List<K>();
        
        public GameEvent(string ID)
        {
            this.ID = ID;
        }

        public void Register(Action<T, K> callBack)
        {
            m_Action -= callBack;
            m_Action += callBack;
        }

        public void Unregister(Action<T, K> callBack)
        {
            m_Action -= callBack;
        }

        public GameEvent<T, K> SetArgs(T arg1, K arg2)
        {
            this.arg1.Add(arg1);
            this.arg2.Add(arg2);
            return this;
        }

        public override void Invoke()
        {
            for (int i = 0; i < this.arg1.Count; i++)
            {
                m_Action?.Invoke(arg1[i], arg2[i]);
            }
            this.arg1.Clear();
            this.arg2.Clear();
        }

        public void Invoke(T arg1, K arg2)
        {
            this.m_Action?.Invoke(arg1, arg2);
        }
    }

    #endregion

    #region GameEvent<T, K, L>

    public class GameEvent<T, K, L>: GameEventBase
    {
        private Action<T, K, L> m_Action;
        private List<T> arg1 = new List<T>();
        private List<K> arg2 = new List<K>();
        private List<L> arg3 = new List<L>();
        
        public GameEvent(string ID)
        {
            this.ID = ID;
        }

        public void Register(Action<T, K, L> callBack)
        {
            m_Action -= callBack;
            m_Action += callBack;
        }

        public void Unregister(Action<T, K, L> callBack)
        {
            m_Action -= callBack;
        }

        public GameEvent<T, K, L> SetArgs(T arg1, K arg2, L arg3)
        {
            this.arg1.Add(arg1);
            this.arg2.Add(arg2);
            this.arg3.Add(arg3);
            return this;
        }

        public override void Invoke()
        {
            for (int i = 0; i < this.arg1.Count; i++)
            {
                m_Action?.Invoke(arg1[i], arg2[i], arg3[i]);
            }

            this.arg1.Clear();
            this.arg2.Clear();
            this.arg3.Clear();
        }

        public void Invoke(T arg1, K arg2, L arg3)
        {
            this.m_Action?.Invoke(arg1, arg2, arg3);
        }
    }

    #endregion

    #region GameEvent<T, K, L, M> 

    public class GameEvent<T, K, L, M>: GameEventBase
    {
        private Action<T, K, L, M> m_Action;
        private List<T> arg1 = new List<T>();
        private List<K> arg2 = new List<K>();
        private List<L> arg3 = new List<L>();
        private List<M> arg4 = new List<M>();
        
        public GameEvent(string ID)
        {
            this.ID = ID;
        }

        public void Register(Action<T, K, L, M> callBack)
        {
            m_Action -= callBack;
            m_Action += callBack;
        }

        public void Unregister(Action<T, K, L, M> callBack)
        {
            m_Action -= callBack;
        }

        public GameEvent<T, K, L, M> SetArgs(T arg1, K arg2, L arg3, M arg4)
        {
            this.arg1.Add(arg1);
            this.arg2.Add(arg2);
            this.arg3.Add(arg3);
            this.arg4.Add(arg4);
            return this;
        }

        public override void Invoke()
        {
            for (int i = 0; i < this.arg1.Count; i++)
            {
                m_Action?.Invoke(arg1[i], arg2[i], arg3[i], arg4[i]);
            }
            
            this.arg1.Clear();
            this.arg2.Clear();
            this.arg3.Clear();
            this.arg4.Clear();
        }

        public void Invoke(T arg1, K arg2, L arg3, M arg4)
        {
            this.m_Action.Invoke(arg1,arg2,arg3,arg4);
        }
    }

    #endregion

    #region GameEvent<T, K, L, M,N> 

    public class GameEvent<T, K, L, M, N>: GameEventBase
    {
        private Action<T, K, L, M, N> m_Action;
        private List<T> arg1 = new List<T>();
        private List<K> arg2 = new List<K>();
        private List<L> arg3 = new List<L>();
        private List<M> arg4 = new List<M>();
        private List<N> arg5 = new List<N>();
        
        public GameEvent(string ID)
        {
            this.ID = ID;
        }

        public void Register(Action<T, K, L, M, N> callBack)
        {
            m_Action -= callBack;
            m_Action += callBack;
        }

        public void Unregister(Action<T, K, L, M, N> callBack)
        {
            m_Action -= callBack;
        }

        public GameEvent<T, K, L, M, N> SetArgs(T arg1, K arg2, L arg3, M arg4, N arg5)
        {
            this.arg1.Add(arg1);
            this.arg2.Add(arg2);
            this.arg3.Add(arg3);
            this.arg4.Add(arg4);
            this.arg5.Add(arg5);
            return this;
        }

        public override void Invoke()
        {
            for (int i = 0; i < this.arg1.Count; i++)
            {
                m_Action?.Invoke(arg1[i], arg2[i], arg3[i], arg4[i], arg5[i]);
            }
            this.arg1.Clear();
            this.arg2.Clear();
            this.arg3.Clear();
            this.arg4.Clear();
            this.arg5.Clear();
        }

        public void Invoke(T arg1, K arg2, L arg3, M arg4, N arg5)
        {
            this.m_Action?.Invoke(arg1, arg2, arg3, arg4, arg5);
        }
    }

    #endregion

    #region GameEvent<T, K, L, M,N,B> 

    public class GameEvent<T, K, L, M, N, B>: GameEventBase
    {
        private Action<T, K, L, M, N, B> m_Action;
        private List<T> arg1 = new List<T>();
        private List<K> arg2 = new List<K>();
        private List<L> arg3 = new List<L>();
        private List<M> arg4 = new List<M>();
        private List<N> arg5 = new List<N>();
        private List<B> arg6 = new List<B>();

        public GameEvent(string ID)
        {
            this.ID = ID;
        }

        public void Register(Action<T, K, L, M, N, B> callBack)
        {
            m_Action -= callBack;
            m_Action += callBack;
        }

        public void Unregister(Action<T, K, L, M, N, B> callBack)
        {
            m_Action -= callBack;
        }

        public GameEvent<T, K, L, M, N, B> SetArgs(T arg1, K arg2, L arg3, M arg4, N arg5, B arg6)
        {
            this.arg1.Add(arg1);
            this.arg2.Add(arg2);
            this.arg3.Add(arg3);
            this.arg4.Add(arg4);
            this.arg5.Add(arg5);
            this.arg6.Add(arg6);
            return this;
        }

        public override void Invoke()
        {
            for (int i = 0; i < this.arg1.Count; i++)
            {
                m_Action?.Invoke(arg1[i], arg2[i], arg3[i], arg4[i], arg5[i], arg6[i]);
            }
            
            this.arg1.Clear();
            this.arg2.Clear();
            this.arg3.Clear();
            this.arg4.Clear();
            this.arg5.Clear();
            this.arg6.Clear();
        }

        public void Invoke(T arg1, K arg2, L arg3, M arg4, N arg5, B arg6)
        {
            this.m_Action?.Invoke(arg1,arg2,arg3,arg4,arg5,arg6);
        }
    }

    #endregion
}