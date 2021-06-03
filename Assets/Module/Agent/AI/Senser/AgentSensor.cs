using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Module
{
    public class AgentSensor : MonoBehaviour
    {
        [LabelText("遮挡物")]
        public LayerMask layerMask;
    }
}