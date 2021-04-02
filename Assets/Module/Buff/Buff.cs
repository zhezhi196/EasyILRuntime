using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Module
{
    public abstract class Buff
    {
        public int uuid
        {
            get { return dbData.ID; }
        }

        public IBuffData dbData { get; set; }
        public int layCount { get; set; }
        public bool isActive { get; set; }
        public IBuffAgent agent { get; set; }
        public Clock clock { get; set; }
        
        public BuffType type { get; set; }
        
        #region AddBuff RemoveBuff
        public static T AddBuff<T>(IBuffData data, BuffType buffType, IBuffAgent agent, int layCount = 1) where T : Buff, new()
        {
            T buff = null;
            switch (buffType)
            {
                case BuffType.IgnoreSecond:
                    buff = AddIgnoreSecondBuff<T>(data, agent);
                    break;
                case BuffType.Restart:
                    buff = AddRestartBuff<T>(data, agent);
                    break;
                case BuffType.Independent:
                    buff = AddIndependentBuff<T>(data, agent);
                    break;
            }

            buff.type = buffType;
            if (buff != null)
            {
                BuffOption option = new BuffOption(data, layCount);
                buff.Restart(option);
            }

            return buff;
        }

        private static T AddIgnoreSecondBuff<T>(IBuffData data, IBuffAgent agent) where T : Buff, new()
        {
            T buff = null;
            if (!agent.buffList.IsNullOrEmpty())
            {
                for (int i = 0; i < agent.buffList.Count; i++)
                {
                    if (agent.buffList[i].uuid == data.ID)
                    {
                        buff = agent.buffList[i] as T;
                        break;
                    }
                }
            }

            if (buff != null) return null;

            buff = new T();
            buff.OnAdd(agent,data);
            return buff;
        }

        private static T AddRestartBuff<T>(IBuffData data, IBuffAgent agent) where T : Buff, new()
        {
            T buff = null;
            if (!agent.buffList.IsNullOrEmpty())
            {
                for (int i = 0; i < agent.buffList.Count; i++)
                {
                    if (agent.buffList[i].uuid == data.ID)
                    {
                        buff = agent.buffList[i] as T;
                        break;
                    }
                }
            }

            if (buff != null) return buff;
            buff = new T();
            buff.OnAdd(agent,data);
            return buff;
        }
        
        private static T AddIndependentBuff<T>(IBuffData data, IBuffAgent agent) where T : Buff, new()
        {
            T buff = new T();
            buff.OnAdd(agent,data);
            return buff;
        }

        public static void RemoveBuff(int uuid, IBuffAgent agent)
        {
            for (int i = 0; i < agent.buffList.Count; i++)
            {
                if (agent.buffList[i].uuid == uuid)
                {
                    Buff buff = agent.buffList[i];
                    buff.OnRemove();
                }
            }
        }
        
        #endregion

        #region Callback

        public virtual void OnAdd(IBuffAgent agent, IBuffData data)
        {
            isActive = true;
            this.agent = agent;
            this.dbData = data;
            agent.buffList.Add(this);
        }

        public virtual void OnRemove()
        {
            layCount = 0;
            isActive = false;
            agent.buffList.Remove(this);
            agent = null;
        }

        #endregion

        #region Method

        public virtual void Restart(BuffOption option)
        {
            if (clock != null)
            {
                clock.Restart();
            }
            else
            {
                if (option.totalTime > 0)
                {
                    clock = Clock.GetClockByID(this, option.totalTime);
                    clock.StartTick();
                }
            }
        }

        public virtual void Pause()
        {
            if (clock != null)
            {
                clock.Pause();
            }
        }

        public virtual void Continue()
        {
            if (clock != null)
            {
                clock.StartTick();
            }
        }

        public virtual void Stop()
        {
            if (clock != null)
            {
                clock.Stop();
            }

            RemoveBuff(uuid, agent);
        }

        #endregion
    }
}