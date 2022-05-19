using System;
using System.Collections.Generic;

namespace Module
{
    public interface IEventSender : IEventCallback 
    {
        EventSender[] sender { get; }
        bool TryPredicate(string predicate, string[] sendArg, string[] predicateArgs);
    }
}