using System;
using UnityEngine;

namespace Module
{
    public class CircleTrack : IMoveTrack
    {
        private bool _isActive;
        
        public bool isActive
        {
            get { return _isActive; }
        }

        public Vector3 center { get; }
        public float radius { get; }
        public float duation { get; set; }

        public float loop { get; set; }
        public bool noEnd { get; set; }
        public IEquation equation { get; set; }
        public ITrackOffset offset { get; set; }
        public event Action onComplete;

        public CircleTrack(Vector3 center, float radius, float time)
        {
            this.center = center;
            this.radius = radius;
            this.duation = time;
        }
        
        public Vector3 UpdatePosition(float detaTime)
        {
            return Vector3.zero;
        }

        public Vector3 UpdatePosition(float detaTime, Vector3 endValue)
        {
            return Vector3.zero;
        }

        public void ChangeEndValue(Vector3 endValue)
        {
        }

        public Vector3 GetPointInWorld()
        {
            return Vector3.zero;
        }
    }
}