/*
 * 脚本名称：UIScrollBase
 * 项目名称：Bow
 * 脚本作者：黄哲智
 * 创建时间：2018-08-10 10:32:29
 * 脚本作用：
*/

using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Module
{
    public class UIScrollBase : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerDownHandler,IPointerUpHandler
    {
        public AnimationCurve delaCurve = new AnimationCurve(new Keyframe(0,0),new Keyframe(1,1));

        public bool isMutiTouch;
        public float Sensity = 0.1f;
        /// <summary>
        /// 当开始准备滑动的回调 参数:位置
        /// </summary>
        private Action<Vector2> onDragPrepare;

        private Action<Vector2> onPointUp;
        
        /// <summary>
        /// 当点击时的回调 1:位置 2:方向
        /// </summary>
        private Action<Vector2, Vector2> onDragDown;
        /// <summary>
        /// 当抬起时的回调  1:位置 2:方向
        /// </summary>
        private Action<Vector2, Vector2> onDragUp;
        /// <summary>
        /// 当一直按压时的回调,会返回一个当前已经按压多少秒的值,以及当前按压的坐标 1:位置 2:差值 3:时间
        /// </summary>
        private Action<Vector2, Vector2, float> onDrag;

        private Vector2 m_sensitivity;

        private Dictionary<PointerEventData, PointInfo> pointInfo = new Dictionary<PointerEventData, PointInfo>();
        private bool disableClear = false;
        private bool isDrag = false;//是否开始拖动了

        public void OnPointerDown(PointerEventData eventData)
        {
            PointInfo p = new PointInfo(eventData);
            bool result = (pointInfo.Count < 1) || (pointInfo.Count >= 1 && isMutiTouch);
            if (!result)
            {
                GameDebug.LogFormat("UIScrollBase:disableClear={0},isDrag={1}", disableClear, isDrag);
            }
            if (!pointInfo.ContainsKey(eventData) && result)
            {
                pointInfo.Add(eventData, p);
            }

            onDragPrepare?.Invoke(eventData.position);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!pointInfo.ContainsKey(eventData)) return;
            Vector2 dir = (eventData.position - pointInfo[eventData].lastPos).normalized;
            onDragDown?.Invoke(eventData.position, dir);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!pointInfo.ContainsKey(eventData)) return;
            isDrag = true;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!pointInfo.ContainsKey(eventData)) return;
            isDrag = false;
            Vector2 dir = (eventData.position - pointInfo[eventData].lastPos).normalized;
            pointInfo[eventData].SetDefault(eventData);
            onDragUp?.Invoke(eventData.position, dir);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!pointInfo.ContainsKey(eventData)) return;
            StartCoroutine(EndPointUp(eventData));
            onPointUp?.Invoke(eventData.position);
        }

        private IEnumerator EndPointUp(PointerEventData eventData)
        {
            yield return new WaitForEndOfFrame();
            if (pointInfo.ContainsKey(eventData))
                pointInfo.Remove(eventData);
        }

        void Update()
        {
            if (gameObject.activeInHierarchy)
            {
                if (disableClear)
                {
                    pointInfo.Clear();
                    disableClear = false;
                }
                if (isDrag)
                {
                    foreach (KeyValuePair<PointerEventData, PointInfo> info in pointInfo)
                    {
                        if (info.Value.isPointing)
                        {
                            //float scaleTime = Sensity * pointInfo.Count;
                            info.Value.time += TimeHelper.unscaledDeltaTimeIgnorePause;
                            Vector2 delePos = info.Key.position - info.Value.lastPos;

                            //DOTween.To(() => m_sensitivity, value => m_sensitivity = value, delePos, scaleTime).SetUpdate(!Player.player.battleRole.weapon.animatorCtrl.effectByTimescale).SetEase(delaCurve);
                            info.Value.lastPos = info.Key.position;
                            onDrag?.Invoke(info.Key.position, delePos, info.Value.time);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 外部调用停止拖动
        /// </summary>
        public void Stop()
        {
            if (pointInfo.Count > 0)
            {
                onDragUp?.Invoke(Vector2.zero, Vector2.zero);
                onPointUp?.Invoke(Vector2.zero);
                disableClear = true;
            }
        }

        private void OnDisable()
        {
            if (pointInfo.Count > 0)
            {
                onDragUp?.Invoke(Vector2.zero, Vector2.zero);
                onPointUp?.Invoke(Vector2.zero);
                disableClear = true;
            }
        }

        private void OnDestroy()
        {
            onDragPrepare = null;
            onDragDown = null;
            onDragUp = null;
            onDrag = null;
        }


        #region AddDragPrepare

        public void AddDragPrepare(Action<Vector2> callBack)
        {
            onDragPrepare += callBack;
        }

        public void RemoveDragPrepare(Action<Vector2> callBack)
        {
            onDragPrepare -= callBack;
        }

        public void AddDragPrepare<T>(Action<Vector2, T> callBack, T arg)
        {
            onDragPrepare += (pos) => { callBack(pos, arg); };
        }

        public void AddDragPrepare<T, K>(Action<Vector2, T, K> callBack, T arg1, K arg2)
        {
            onDragPrepare += (pos) => { callBack(pos, arg1, arg2); };
        }

        public void AddDragPrepare<T, K, L>(Action<Vector2, T, K, L> callBack, T arg1, K arg2, L arg3)
        {
            onDragPrepare += (pos) => { callBack(pos, arg1, arg2, arg3); };
        }

        public void AddDragPrepare<T, K, L, M>(Action<Vector2, T, K, L, M> callBack, T arg1, K arg2, L arg3, M arg4)
        {
            onDragPrepare += (pos) => { callBack(pos, arg1, arg2, arg3, arg4); };
        }

        #endregion
        
        #region AddPointUp

        public void AddPointUp(Action<Vector2> callBack)
        {
            onPointUp += (pos) => { callBack(pos); };
        }

        public void AddPointUp<T>(Action<Vector2, T> callBack, T arg)
        {
            onPointUp += (pos) => { callBack(pos, arg); };
        }

        public void AddPointUp<T, K>(Action<Vector2, T, K> callBack, T arg1, K arg2)
        {
            onPointUp += (pos) => { callBack(pos, arg1, arg2); };
        }

        public void AddPointUp<T, K, L>(Action<Vector2, T, K, L> callBack, T arg1, K arg2, L arg3)
        {
            onPointUp += (pos) => { callBack(pos, arg1, arg2, arg3); };
        }

        public void AddPointUp<T, K, L, M>(Action<Vector2, T, K, L, M> callBack, T arg1, K arg2, L arg3, M arg4)
        {
            onPointUp += (pos) => { callBack(pos, arg1, arg2, arg3, arg4); };
        }

        #endregion

        #region AddDragDown

        public void AddDragDown(Action<Vector2, Vector2> callBack)
        {
            onDragDown += (pos, dir) => { callBack(pos, dir); };
        }

        public void AddDragDown<T>(Action<Vector2, Vector2, T> callBack, T arg)
        {
            onDragDown += (pos, dir) => { callBack(pos, dir, arg); };
        }

        public void AddDragDown<T, K>(Action<Vector2, Vector2, T, K> callBack, T arg1, K arg2)
        {
            onDragDown += (pos, dir) => { callBack(pos, dir, arg1, arg2); };
        }

        public void AddDragDown<T, K, L>(Action<Vector2, Vector2, T, K, L> callBack, T arg1, K arg2, L arg3)
        {
            onDragDown += (pos, dir) => { callBack(pos, dir, arg1, arg2, arg3); };
        }

        public void AddDragDown<T, K, L, M>(Action<Vector2, Vector2, T, K, L, M> callBack, T arg1, K arg2, L arg3, M arg4)
        {
            onDragDown += (pos, dir) => { callBack(pos, dir, arg1, arg2, arg3, arg4); };
        }

        #endregion

        #region AddDragUp

        public void AddDragUp(Action<Vector2, Vector2> callBack)
        {
            onDragUp += (pos, dir) => { callBack(pos, dir); };
        }

        public void AddDragUp<T>(Action<Vector2, Vector2, T> callBack, T arg)
        {
            onDragUp += (pos, dir) => { callBack(pos, dir, arg); };
        }

        public void AddDragUp<T, K>(Action<Vector2, Vector2, T, K> callBack, T arg1, K arg2)
        {
            onDragUp += (pos, dir) => { callBack(pos, dir, arg1, arg2); };
        }

        public void AddDragUp<T, K, L>(Action<Vector2, Vector2, T, K, L> callBack, T arg1, K arg2, L arg3)
        {
            onDragUp += (pos, dir) => { callBack(pos, dir, arg1, arg2, arg3); };
        }

        public void AddDragUp<T, K, L, M>(Action<Vector2, Vector2, T, K, L, M> callBack, T arg1, K arg2, L arg3, M arg4)
        {
            onDragUp += (pos, dir) => { callBack(pos, dir, arg1, arg2, arg3, arg4); };
        }

        #endregion

        #region AddDrag

        public void AddDrag(Action<Vector2, Vector2, float> callBack)
        {
            onDrag += callBack;
        }

        public void AddDrag<T>(Action<Vector2, Vector2, float, T> callBack, T arg)
        {
            onDrag += (pos, dir, time) => { callBack(pos, dir, time, arg); };
        }

        public void AddDrag<T, K>(Action<Vector2, Vector2, float, T, K> callBack, T arg1, K arg2)
        {
            onDrag += (pos, dir, time) => { callBack(pos, dir, time, arg1, arg2); };
        }

        public void AddDrag<T, K, L>(Action<Vector2, Vector2, float, T, K, L> callBack, T arg1, K arg2, L arg3)
        {
            onDrag += (pos, dir, time) => { callBack(pos, dir, time, arg1, arg2, arg3); };
        }

        public void AddDrag<T, K, L, M>(Action<Vector2, Vector2, float, T, K, L, M> callBack, T arg1, K arg2, L arg3, M arg4)
        {
            onDrag += (pos, dir, time) => { callBack(pos, dir, time, arg1, arg2, arg3, arg4); };
        }

        #endregion

        public void RemoveDrag(Action<Vector2, Vector2, float> callBack)
        {
            onDrag -= callBack;
        }

        public void ClearDragDown()
        {
            onDragDown = null;
        }

        public void ClearDragUp(Action<Vector2, Vector2> callBack)
        {
            onDragUp = null;
        }

        public void ClearDrag(Action<Vector2, Vector2, float> callBack)
        {
            onDrag = null;
        }

        #region PointInfo

        private class PointInfo
        {
            public float time;
            public bool isPointing;
            public Vector2 lastPos;

            public PointInfo(PointerEventData eventData)
            {
                time = 0;
                isPointing = true;
                lastPos = eventData.position;
            }

            public void SetDefault(PointerEventData eventData)
            {
                time = 0;
                isPointing = false;
                lastPos = eventData.position;
            }
        }

        #endregion

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                onDragUp?.Invoke(Vector2.zero, Vector2.zero);
                onPointUp?.Invoke(Vector2.zero);
                pointInfo.Clear();
            }
        }
    }
}


