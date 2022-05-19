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
        public static List<AgentHear> TrySensor(Vector3 fromPoint, float radius, Predicate<AgentHear> predicate = null)
        {
            List<AgentHear> result = null;

            for (int i = 0; i < agentHears.Count; i++)
            {
                if (predicate == null || predicate.Invoke(agentHears[i]))
                {
                    Vector3 tar = agentHears[i].transform.position;
                    float rad = radius + agentHears[i].hearRange;
                    float currDistance = fromPoint.Distance(tar);
                    if (currDistance <= rad)
                    {
                        RaycastHit hit;
                        if (!Physics.Raycast(fromPoint, tar-fromPoint, out hit, currDistance, agentHears[i].layerMask))
                        {
                            if (result == null) result = new List<AgentHear>();
                            result.Add(agentHears[i]);
                            GameDebug.DrawLine(fromPoint, tar, Color.green, 3);
                        }
                        else
                        {
                            GameDebug.DrawLine(fromPoint, hit.point, Color.red, 3);
                        }
                    }
                }
            }

            return result;
        }
        
        /// <summary>
        /// 试图发送声音
        /// </summary>
        /// <param name="target">发出声音的人,被耳朵识别到的人</param>
        /// <param name="fromPoint">发出声音的位置</param>
        /// <param name="radius">声音的范围,和耳朵的听力范围的加和 和当前距离比大小</param>
        /// <returns></returns>
        public static List<AgentHear> TrySensor(Vector3 fromPoint, float radius, int obstaleMask, Predicate<AgentHear> predicate = null)
        {
            List<AgentHear> result = null;

            for (int i = 0; i < agentHears.Count; i++)
            {
                if (predicate == null || predicate.Invoke(agentHears[i]))
                {
                    Vector3 tar = agentHears[i].transform.position;
                    float rad = radius + agentHears[i].hearRange;
                    float currDistance = fromPoint.Distance(tar);
                    if (currDistance <= rad)
                    {
                        RaycastHit hit;
                        if (!Physics.Raycast(fromPoint, tar - fromPoint, out hit, currDistance, obstaleMask))
                        {
                            if (result == null) result = new List<AgentHear>();
                            result.Add(agentHears[i]);
                            GameDebug.DrawLine(fromPoint, tar, Color.green, 3);
                        }
                        else
                        {
                            GameDebug.DrawLine(fromPoint, hit.point, Color.red, 3);
                        }
                    }
                }
            }

            return result;
        }
        
        [LabelText("额外听力范围")]
        public float hearRange;

        public void Sensor(Vector3 fromPoint, params object[] args)
        {
            onHearTarget?.Invoke(fromPoint, args);
        }
        
        public event Action<Vector3,object[]> onHearTarget;
        
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