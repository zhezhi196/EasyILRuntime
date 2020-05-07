using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Module
{
    [Serializable]
    public class InspectorData
    {
        public string key;
    }
    [Serializable]
    public class InspectorStringData : InspectorData
    {
        public string value;
        public InspectorStringData(string key, string value)
        {
            this.key = key;
            this.value = value;
        }
    }
    [Serializable]
    public class InspectorLongData : InspectorData
    {
        public long value;
        public InspectorLongData(string key, long value)
        {
            this.key = key;
            this.value = value;
        }
    }
    [Serializable]
    public class InspectorDoubleData : InspectorData
    {
        public double value;
        public InspectorDoubleData(string key, double value)
        {
            this.key = key;
            this.value = value;
        }
    }
    [Serializable]
    public class InspectorBoolData : InspectorData
    {
        public bool value;
        public InspectorBoolData(string key, bool value)
        {
            this.key = key;
            this.value = value;
        }
    }
    [Serializable]
    public class InspectorAnimationCurveData :InspectorData
    {
        public AnimationCurve value;
        public InspectorAnimationCurveData(string key, AnimationCurve value)
        {
            this.key = key;
            this.value = value;
        }
    }

    [Serializable]
    public class InspectorVector3Data:InspectorData
    {
        public Vector3 value;
        public InspectorVector3Data(string key, Vector3 value)
        {
            this.key = key;
            this.value = value;
        }
    }
    [Serializable]
    public class InspectorColorData:InspectorData
    {
        public Color value;
        public InspectorColorData(string key, Color value)
        {
            this.key = key;
            this.value = value;
        }
    }
    [Serializable]
    public class InspectorObjectData:InspectorData
    {
        public GameObject value;
        public InspectorObjectData(string key, GameObject value)
        {
            this.key = key;
            this.value = value;
        }
    }

    [Serializable]
    public class InspectorObjectListData : InspectorData
    {
        public List<GameObject> value;

        public InspectorObjectListData(string key, List<GameObject> value)
        {
            this.key = key;
            this.value = value;
        }
    }
}
