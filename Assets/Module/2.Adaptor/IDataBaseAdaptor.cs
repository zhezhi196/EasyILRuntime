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
    public class IDataBaseAdaptor: CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get { return typeof(IDataBase); }
        }

        public override Type AdaptorType
        {
            get { return typeof(Adaptor); }
        }

        public override object CreateCLRInstance(AppDomain appdomain, ILTypeInstance instance)
        {
            return new Adaptor(appdomain,instance);
        }


        public class Adaptor : IDataBase, CrossBindingAdaptorType
        {
            private AppDomain appdomain;
            private ILTypeInstance instance;
            private IMethod idMethod;

            public ILTypeInstance ILInstance
            {
                get { return instance; }
            }

            public Adaptor(AppDomain appdomain, ILTypeInstance instance)
            {
                this.appdomain = appdomain;
                this.instance = instance;
            }
        }
    }


}
