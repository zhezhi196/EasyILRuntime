using System;
using System.Collections.Generic;
using DG.Tweening;

namespace Module
{
    public class BuffCtrl : IAgentCtrl
    {
        private bool _isPause;
        public bool isPause => _isPause;
        public List<Buff> buffList { get; }
        public IBuffObject owner { get; }

        public BuffCtrl(IBuffObject owner)
        {
            this.owner = owner;
            buffList = new List<Buff>();
        }

        public bool OnUpdate()
        {
            if (buffList.Count > 0)
            {
                float detaTime = owner.GetDelatime(false);
                for (int i = 0; i < buffList.Count; i++)
                {
                    if (!buffList[i].OnUpdate(detaTime))
                    {
                        var removeBuff = buffList[i];
                        removeBuff.OnRemove();
                        break;
                    }
                }

                for (int i = buffList.Count - 1; i >= 0; i--)
                {
                    if (!buffList[i].isActive)
                    {
                        var removeBuff = buffList[i];
                        buffList.RemoveAt(i);
                        owner.OnRemoveBuff(removeBuff);
                    }
                }
            }

            return true;
        }

        public void Pause()
        {
            _isPause = true;
            for (int i = 0; i < buffList.Count; i++)
            {
                buffList[i].Pause();
            }
        }

        public void Continue()
        {
            _isPause = false;
            for (int i = 0; i < buffList.Count; i++)
            {
                buffList[i].Continue();
            }
        }

        public void OnAgentDead()
        {
            for (int i = 0; i < buffList.Count; i++)
            {
                buffList[i].Stop();
            }
            buffList.Clear();
        }

        public void OnDestroy()
        {
            buffList.Clear();
        }

        #region Public

        /// <summary>
        /// 移除buff
        /// </summary>
        /// <param name="uuid"></param>
        /// <param name="agent"></param>
        public void RemoveBuff(string name)
        {
            for (int i = 0; i < buffList.Count; i++)
            {
                if (buffList[i].name == name)
                {
                    Buff buff = buffList[i];
                    buff.OnRemove();
                }
            }
        }
        /// <summary>
        /// 移除buff
        /// </summary>
        /// <param name="uuid"></param>
        /// <param name="agent"></param>
        public void RemoveBuff(Buff buff)
        {
            for (int i = 0; i < buffList.Count; i++)
            {
                if (buffList[i] == buff)
                {
                    buff.OnRemove();
                }
            }
        }

        public T AddBuff<T>(BuffType buffType, BuffOption option, params object[] args) where T : Buff, new()
        {
            T buff = null;
            switch (buffType)
            {
                case BuffType.IgnoreSecond:
                    buff = AddIgnoreSecondBuff<T>(buffType, args);
                    break;
                case BuffType.Restart:
                    buff = AddRestartBuff<T>(buffType, args);
                    break;
                case BuffType.Independent:
                    buff = AddIndependentBuff<T>(buffType, args);
                    break;
            }

            if (buff != null)
            {
                buff.Restart(option);
                buffList.Add(buff);
                owner.OnAddBuff(buff);
            }
            return buff;
        }

        #endregion

        #region Private

        private T AddIgnoreSecondBuff<T>(BuffType type, params object[] args) where T : Buff, new()
        {
            T buff = null;
            if (!buffList.IsNullOrEmpty())
            {
                for (int i = 0; i < buffList.Count; i++)
                {
                    if (buffList[i].GetType() == typeof(T))
                    {
                        buff = buffList[i] as T;
                        break;
                    }
                }
            }

            if (buff != null) return null;

            buff = new T();
            buff.OnInit(owner, type, args);
            buff.OnAdd();
            return buff;
        }

        private T AddRestartBuff<T>(BuffType type, params object[] args) where T : Buff, new()
        {
            T buff = null;
            if (!buffList.IsNullOrEmpty())
            {
                for (int i = 0; i < buffList.Count; i++)
                {
                    if (buffList[i].GetType() == typeof(T))
                    {
                        buff = buffList[i] as T;
                        buff.time = 0;
                        break;
                    }
                }
            }

            if (buff != null) return buff;
            buff = new T();

            buff.OnInit(owner, type, args);
            buff.OnAdd();
            return buff;
        }
        
        private T AddIndependentBuff<T>(BuffType type, params object[] args) where T : Buff, new()
        {
            T buff = new T();
            buff.OnInit(owner, type, args);
            buff.OnAdd();
            return buff;
        }

        #endregion

        public void ClearBuff()
        {
            for (int i = 0; i < buffList.Count; i++)
            {
                buffList[i].OnRemove();
            }
            buffList.Clear();
        }
        
        
        public T GetAgentCtrl<T>() where T : IAgentCtrl
        {
            return owner.GetAgentCtrl<T>();
        }

        public void EditorInit()
        {
        }

        public void OnDrawGizmos()
        {
            
        }
    }
}