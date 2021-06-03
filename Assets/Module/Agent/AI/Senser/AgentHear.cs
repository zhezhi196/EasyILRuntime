using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Module
{
    public class AgentHear : AgentSensor
    {
        public static List<AgentHear> agentHears = new List<AgentHear>();

        /// <summary>
        /// 试图发送声音
        /// </summary>
        /// <param name="target">发出声音的人,被耳朵识别到的人</param>
        /// <param name="fromPoint">发出声音的位置</param>
        /// <param name="radius">声音的范围,和耳朵的听力范围的加和 和当前距离比大小</param>
        /// <returns></returns>
        public static List<AgentHear> TrySensor(ISensorTarget target, Vector3 fromPoint, float radius)
        {
            if (!target.isSenserable) return null;
            List<AgentHear> result = null;

            for (int i = 0; i < agentHears.Count; i++)
            {
                Vector3 tar = agentHears[i].transform.position;
                float rad = radius + agentHears[i].hearRange;
                if (Vector3.Distance(fromPoint, tar) <= rad)
                {
                    RaycastHit hit;
                    if (!Physics.Raycast(tar, fromPoint - tar, out hit, rad, agentHears[i].layerMask))
                    {
                        if (result == null) result = new List<AgentHear>();
                        result.Add(agentHears[i]);
                        agentHears[i].onHearTarget?.Invoke(target, fromPoint);
                    }
                }
            }

            return result;
        }
        [LabelText("听力范围")]
        public float hearRange;
        
        public event Action<ISensorTarget, Vector3> onHearTarget;
        public void AddListener(Action<ISensorTarget, Vector3> callback)
        {
            onHearTarget += callback;
        }
        
        private void OnEnable()
        {
            agentHears.Add(this);
        }

        private void OnDisable()
        {
            agentHears.Remove(this);
        }
    }
}