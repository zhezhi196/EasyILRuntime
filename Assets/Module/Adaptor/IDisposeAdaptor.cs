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
    public class IDisposeAdaptor : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get { return typeof(IDisposable); }
        }

        public override Type AdaptorType
        {
            get { return typeof(Adaptor); }
        }

        public override object CreateCLRInstance(AppDomain appdomain, ILTypeInstance instance)
        {
            return new Adaptor(appdomain,instance);
        }
        
        private class Adaptor: IDisposable,CrossBindingAdaptorType
        {
            public ILTypeInstance ILInstance
            {
                get { return instance; }
            }

            private AppDomain appdomain;
            private ILTypeInstance instance;
            private IMethod dispose;

            public Adaptor(AppDomain appdomain, ILTypeInstance instance)
            {
                this.appdomain = appdomain;
                this.instance = instance;
                dispose = instance.Type.GetMethod("Dispose", 0);
            }

            public void Dispose()
            {
                if (dispose != null)
                {
                    appdomain.Invoke(dispose, instance, null);
                }
            }
        }
    }


}
