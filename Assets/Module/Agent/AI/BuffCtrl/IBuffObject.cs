using System;
using System.Collections.Generic;

namespace Module
{
    public interface IBuffObject : IAgentObject, ITarget
    {
        void OnAddBuff(Buff buff);
        void OnRemoveBuff(Buff buff);
    }
}