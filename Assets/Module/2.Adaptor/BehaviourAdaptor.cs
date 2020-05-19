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
    public class BehaviourAdaptor : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get { return typeof(ViewBehaviour); }
        }

        public override Type AdaptorType
        {
            get { return typeof(Adaptor); }
        }

        public override object CreateCLRInstance(AppDomain appdomain, ILTypeInstance instance)
        {
            return new Adaptor(appdomain, instance);
        }

        public class Adaptor : ViewBehaviour, CrossBindingAdaptorType
        {
            private AppDomain appdomain;
            private ILTypeInstance instance;

            public ILTypeInstance ILInstance
            {
                get { return instance; }
            }

            private IMethod awake;
            private IMethod start;
            private IMethod onEnable;
            private IMethod onDisable;
            private IMethod onDestroy;
            private IMethod onBecameInvisible;
            private IMethod onBecameVisible;

            public Adaptor(AppDomain appdomain, ILTypeInstance instance)
            {
                this.appdomain = appdomain;
                this.instance = instance;
                awake = instance.Type.GetMethod("Awake", 0);
                start = instance.Type.GetMethod("Start", 0);
                onEnable = instance.Type.GetMethod("OnEnable", 0);
                onDisable = instance.Type.GetMethod("OnDisable", 0);
                onDestroy = instance.Type.GetMethod("OnDestroy", 0);
                onBecameVisible = instance.Type.GetMethod("OnBecameVisible", 0);
                onBecameInvisible = instance.Type.GetMethod("OnBecameInvisible", 0);
            }

            public override void Awake()
            {
                if (awake != null)
                {
                    appdomain.Invoke(awake, instance, null);
                }
            }

            public override void Start()
            {
                if (start != null)
                {
                    appdomain.Invoke(start, instance, null);
                }
            }

            public override void OnEnable()
            {
                if (onEnable != null)
                {
                    appdomain.Invoke(onEnable, instance, null);
                }
            }

            public override void OnDisable()
            {
                if (onDisable != null)
                {
                    appdomain.Invoke(onDisable, instance, null);
                }
            }

            public override void OnDestroy()
            {
                if (onDestroy != null)
                {
                    appdomain.Invoke(onDestroy, instance, null);
                }
            }

            public override void OnBecameInvisible()
            {
                if (onBecameInvisible != null)
                {
                    appdomain.Invoke(onBecameInvisible, instance, null);
                }
            }

            public override void OnBecameVisible()
            {
                if (onBecameVisible != null)
                {
                    appdomain.Invoke(onBecameVisible, instance, null);
                }
            }
        }
    }
}