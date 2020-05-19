using System;
using System.Collections.Generic;
using ILRuntime.Runtime.Intepreter;
using UnityEngine;

namespace Module
{
    public class ViewReference : MonoBehaviour
    {
        [HideInInspector] public string targetType;
        [NonSerialized] public ViewBehaviour target;

        #region param

        [HideInInspector] public List<InspectorStringData> stringList = new List<InspectorStringData>();
        [HideInInspector] public List<InspectorLongData> intList = new List<InspectorLongData>();
        [HideInInspector] public List<InspectorDoubleData> floatList= new List<InspectorDoubleData>();
        [HideInInspector] public List<InspectorBoolData> boolList= new List<InspectorBoolData>();
        [HideInInspector] public List<InspectorAnimationCurveData> animationList= new List<InspectorAnimationCurveData>();
        [HideInInspector] public List<InspectorVector3Data> vector3List= new List<InspectorVector3Data>();
        [HideInInspector] public List<InspectorColorData> colorList= new List<InspectorColorData>();
        [HideInInspector] public List<InspectorObjectData> objectList= new List<InspectorObjectData>();

        #endregion

        private void OnEnable()
        {
            if (target != null)
                target.OnEnable();
        }

        private void OnDisable()
        {
            if (target != null)
                target.OnDisable();
        }

        private void Start()
        {
            if (target != null)
                target.Start();
        }

        private void Awake()
        {
            if (target != null)
                target.Awake();
        }

        private void OnDestroy()
        {
            if (target != null)
                target.OnDestroy();
        }

        private void OnBecameInvisible()
        {
            if (target != null)
                target.OnBecameInvisible();
        }

        private void OnBecameVisible()
        {
            if (target != null)
                target.OnBecameVisible();
        }
    }
}
