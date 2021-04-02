using System;
using System.Collections.Generic;

namespace Module
{
    public interface IEventSender<T> : IEventCallback where T : Enum
    {
        EventSender<T>[] sender { get; }
        bool TryPredicate(T predicate, string[] sendArg, string[] args);
    }
}