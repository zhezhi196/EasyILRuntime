using System;
using UnityEngine;

namespace Module
{
    public class LineTrack : IMoveTrack
    {
        #region 字段

        private Transform moveTarget;
        private bool _isActive;
        private Vector3 _endPoint;

        /// <summary>
        /// 转置矩阵
        /// </summary>
        private Matrix4x4 _convertMatrix;

        private float _speed;
        private Vector3 _fromPoint;

        #endregion

        #region 属性

        /// <summary>
        /// 是否激活
        /// </summary>
        public bool isActive
        {
            get { return _isActive; }
        }

        public bool autoUpdate
        {
            get
            {
                return moveTarget != null;
            }
        }

        /// <summary>
        /// 经过若干次偏移到达终点
        /// </summary>
        public float loop { get; set; }

        /// <summary>
        /// 是否一直移动,无尽头
        /// </summary>
        public bool noEnd { get; set; }

        /// <summary>
        /// 开始的位置
        /// </summary>
        public Vector3 fromPoint
        {
            get { return _fromPoint; }
        }

        /// <summary>
        /// 结束的位置
        /// </summary>
        public Vector3 endPoint
        {
            get { return _endPoint; }
        }

        /// <summary>
        /// 当前移动的时间
        /// </summary>
        public float time { get; set; }

        /// <summary>
        /// 移动总时间
        /// </summary>
        public float duation { get; }

        /// <summary>
        /// 移动速度
        /// </summary>
        public float speed
        {
            get { return _speed; }
        }

        /// <summary>
        /// 当前移动的百分比
        /// </summary>
        public float percent
        {
            get
            {
                if (noEnd) return 0;
                return time / duation;
            }
        }
        
        /// <summary>
        /// 基点运动方程
        /// </summary>
        public IEquation equation { get; set; }

        /// <summary>
        /// 偏移
        /// </summary>
        public ITrackOffset offset { get; set; }

        /// <summary>
        /// 当到达终点时的回调
        /// </summary>
        public event Action onComplete;

        public event Action<float> onUpdate; 

        #endregion

        #region 构造函数

        public LineTrack(Vector3 fromPoint, Vector3 endPoint, float time)
        {
            _fromPoint = fromPoint;
            duation = Mathf.Abs(time);
            this.time = 0;
            _isActive = true;
            loop = 1;
            noEnd = false;
            ChangeEndValue(endPoint);
        }
        
        public LineTrack(Vector3 fromPoint, float time)
        {
            _fromPoint = fromPoint;
            duation = Mathf.Abs(time);
            this.time = 0;
            _isActive = true;
            loop = 1;
            noEnd = false;
        }

        #endregion

        #region 刷新获取位置

        public Vector3 UpdatePosition(float detaTime)
        {
            if (!isActive) return endPoint;
            time += detaTime;
            if (percent >= 1)
            {
                _isActive = false;
                onComplete?.Invoke();
                return endPoint;
            }
            else
            {
                float timeCell = duation / loop;
                float cellPercent = (time % timeCell) / timeCell;
                Vector3 resultPointWorld = (equation == null ? (time * speed) : equation.GetY(time)) * Vector3.forward + (offset == null ? Vector3.zero : offset.GetTrackOffset(cellPercent));
                Vector3 resultPointLocal = _convertMatrix.MultiplyPoint(resultPointWorld);
                GameDebug.DrawLine(this.fromPoint, this.endPoint, Color.blue);
                onUpdate?.Invoke(percent);
                return resultPointLocal;
            }
        }

        public Vector3 UpdatePosition(float detaTime, Vector3 endValue)
        {
            ChangeEndValue(endValue);
            return UpdatePosition(detaTime);
        }
        
        
        public void ChangeEndValue(Vector3 endValue)
        {
            if (endValue != fromPoint)
            {
                _endPoint = endValue;
                _speed = fromPoint.Distance(endValue) / duation;
                //获得转换矩阵,世界坐标乘以可以由转换矩阵变成本地坐标
                _convertMatrix = Matrix4x4.TRS(fromPoint, Quaternion.LookRotation(endValue - fromPoint), Vector3.one);
            }
        }

        #endregion
    }
}