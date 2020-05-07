using System.Collections;
using System.Collections.Generic;
using ILRuntime.Runtime.Intepreter;
using UnityEngine;

namespace Module
{
    public abstract class MonoBehaviorInstance
    {
        public abstract object instance { get; }
    }

    public class AssemblyInstance : MonoBehaviorInstance
    {
        public override object instance { get; }
    }

    public class AppDomainInstance : MonoBehaviorInstance
    {
        public override object instance { get; }
    }
}
