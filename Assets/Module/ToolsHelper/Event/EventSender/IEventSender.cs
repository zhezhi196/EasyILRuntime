using System;
using System.Collections.Generic;

namespace Module
{
    public interface IEventSender : IEventCallback 
    {
        EventSender[] sender { get; }
        bool TryPredicate(SendEventCondition predicate, string[] sendArg, string[] predicateArgs);
    }
}