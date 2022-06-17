using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Module
{
    public class AgentSee : AgentSensor, IEyeSight,ILogObject
    {
        public ISee owner;
        [SerializeField]
        private Vector3 _offset;
        [ShowInInspector] public List<ISensorTarget> onViewTarget = new List<ISensorTarget>();
        public Vector3 offset => _offset;
        public List<IEyeSight> includeSights { get; } = new List<IEyeSight>();
        public List<IEyeSight> excludeSights { get; } = new List<IEyeSight>();

        public Vector3 center
        {
            get { return transform.position+offset; }
        }

        private void Awake()
        {
            owner = transform.GetComponentInParent<ISee>();
        }

        public bool ContainPoint(int index,Vector3 point)
        {
            return includeSights[index].ContainPoint(point);
        }

        public bool ContainPoint(Vector3 point)
        {
            bool result = false;
            if (!includeSights.IsNullOrEmpty())
            {
                for (int i = 0; i < includeSights.Count; i++)
                {
                    result = result || includeSights[i].ContainPoint(point);
                }

                if (result)
                {
                    if (!excludeSights.IsNullOrEmpty())
                    {
                        for (int i = 0; i < excludeSights.Count; i++)
                        {
                            result = result && !excludeSights[i].ContainPoint(point);
                        }
                    }
                }
            }

            return result;
        }


        /// <summary>
        /// 当物体进入视野后的回调
        /// </summary>
        public event Action<ISensorTarget> onFousTarget;
        /// <summary>
        /// 当丢失掉物体视野后的回调
        /// </summary>
        public event Action<ISensorTarget> onLoseTarget;
        /// <summary>
        /// 当物体在视野内,每一帧会调这个视角用于监听视野内的物体的状态
        /// </summary>
        public event Action<ISensorTarget> onSenserTarget;

        private void Update()
        {
            if (owner != null)
            {
                var tempTarget = owner.canSeeTarget;
                for (int i = 0; i < tempTarget.Count; i++)
                {
                    var tar = tempTarget[i];
                    if (tar.isSenserable && SeeTarget(tar))
                    {
                        if (!onViewTarget.Contains(tar))
                        {
                            AddToTarget(tar);
                        }
                    }
                    else
                    {
                        if (onViewTarget.Contains(tar))
                        {
                            RemoveFromTarget(tar);
                            return;
                        }
                    }
                }

                for (int i = 0; i < onViewTarget.Count; i++)
                {
                    if (!onViewTarget[i].isSenserable)
                    {
                        onViewTarget.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        private void AddToTarget(ISensorTarget tar)
        {
            onFousTarget?.Invoke(tar);
            onViewTarget.Add(tar);

        }

        private void RemoveFromTarget(ISensorTarget tar)
        {
            onLoseTarget?.Invoke(tar);
            onViewTarget.Remove(tar);
        }

        private bool SeeTarget(ISensorTarget tar)
        {
            Vector3 tarPos = tar.transform.position;
            Vector3 thisPos = center;
            float distance = thisPos.Distance(tarPos);
            RaycastHit hit;
            if (!Physics.Raycast(thisPos, tarPos - thisPos, out hit, distance, layerMask))
            {
                if (ContainPoint(tarPos))
                {
                    onSenserTarget?.Invoke(tar);
                    return true;
                }
            }

            return false;
        }
        
        public void DrawGizmos(Color color)
        {
            for (int i = 0; i < includeSights.Count; i++)
            {
                includeSights[i].DrawGizmos(color);
            }
        }
        
#if UNITY_EDITOR
        #region EditorView

        public Color gimosColor = Color.green;
        public Color targetInView = Color.magenta;

        private void OnDrawGizmos()
        {
            if (isLog)
            {
                DrawGizmos(gimosColor);
                DrawTarget();
            }
        }

        private void DrawTarget()
        {
            Gizmos.color = targetInView;
            for (int i = 0; i < onViewTarget.Count; i++)
            {
                Gizmos.DrawSphere(onViewTarget[i].transform.position, 0.03f);
            }
        }

        #endregion
#endif
        public string logName
        {
            get
            {
                return name;
            }
        }

        public bool isLog { get; set; }
        public void LogFormat(string obj, params object[] args)
        {
        }
    }
}