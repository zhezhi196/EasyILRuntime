using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using UnityEngine;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

namespace Module
{
    public class IAsyncStateMachineAdaptor : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get { return typeof(IAsyncStateMachine); }
        }

        public override Type AdaptorType
        {
            get { return typeof(Adaptor); }
        }

        public override object CreateCLRInstance(AppDomain appdomain, ILTypeInstance instance)
        {
            return new Adaptor(appdomain,instance);
        }

        public class Adaptor:IAsyncStateMachine,CrossBindingAdaptorType
        {
            private AppDomain appdomain;
            private ILTypeInstance instance;

            public ILTypeInstance ILInstance
            {
                get { return instance; }
            }
            
            private IMethod moveNext;
            private IMethod setStateMachine;

            public Adaptor(AppDomain appdomain, ILTypeInstance instance)
            {
                this.appdomain = appdomain;
                this.instance = instance;
                
                moveNext = instance.Type.GetMethod("MoveNext", 0);
                setStateMachine = instance.Type.GetMethod("SetStateMachine", 1);
            }
            
            public void MoveNext()
            {
                if (moveNext != null)
                {
                    appdomain.Invoke(moveNext, instance,null);
                }
            }

            public void SetStateMachine(IAsyncStateMachine stateMachine)
            {
                if (setStateMachine != null)
                {
                    appdomain.Invoke(setStateMachine, instance,stateMachine);
                }
            }
        }

    }


}
