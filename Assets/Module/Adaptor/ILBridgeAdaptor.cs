using System;
using System.Collections;
using System.Collections.Generic;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using UnityEngine;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

namespace Module
{
    public class ILBridgeAdaptor : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get { return typeof(ILBridge); }
        }

        public override Type AdaptorType
        {
            get { return typeof(Adaptor); }
        }

        public override object CreateCLRInstance(AppDomain appdomain, ILTypeInstance instance)
        {
            return new Adaptor(appdomain, instance);
        }

        class Adaptor : ILBridge, CrossBindingAdaptorType
        {
            public ILTypeInstance ILInstance
            {
                get { return instance; }
            }

            private AppDomain appdomain;
            private ILTypeInstance instance;

            private IMethod update;
            private IMethod fixupdate;
            private IMethod lateupdate;
            private IMethod init;

            public Adaptor(AppDomain appdomain, ILTypeInstance instance)
            {
                this.appdomain = appdomain;
                this.instance = instance;
                init = instance.Type.GetMethod("Init", 1);
                update = instance.Type.GetMethod("Update", 0);
                fixupdate = instance.Type.GetMethod("FixedUpdate", 0);
                lateupdate = instance.Type.GetMethod("LateUpdate", 0);
            }

            public void Init(BridgeBase modulBridge)
            {
                if (init != null)
                {
                    appdomain.Invoke(init, instance, modulBridge);
                }
            }

            public void Update()
            {
                if (update != null)
                {
                    appdomain.Invoke(update, instance, null);
                }
            }

            public void FixedUpdate()
            {
                if (fixupdate != null)
                {
                    appdomain.Invoke(fixupdate, instance, null);
                }
            }

            public void LateUpdate()
            {
                if (lateupdate != null)
                {
                    appdomain.Invoke(lateupdate, instance, null);
                }
            }
        }
    }
}
