using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module
{
    public abstract class ViewBehaviour
    {
        public ViewBehaviour()
        {
        }

        protected ViewReference m_target;

        public abstract void Awake();
        public abstract void OnEnable();
        public abstract void Start();

        public abstract void OnDisable();


        public abstract void OnDestroy();


        public abstract void OnBecameInvisible();

        public abstract void OnBecameVisible();
    }
}