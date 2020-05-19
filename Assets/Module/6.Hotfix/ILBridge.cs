using System;
using System.Collections;
using System.Collections.Generic;
using ILRuntime.Runtime.Intepreter;
using UnityEngine;

namespace Module
{

    public interface ILBridge
    {
        void Init(BridgeBase modulBridge);
    }
}
