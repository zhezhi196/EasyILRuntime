
using System;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityNavMeshAgent;
using UnityEngine;
using UnityEngine.AI;

namespace Module
{
    public enum MoveStation
    {
        Static,
        Moving,
        MoveComplete,
    }
    
    public class AgentMoveCtrl : IAgentCtrl
    {
        private List<object> _pauseList = new List<object>();
        private MoveStation _station = MoveStation.Static;
        private Vector3 _target;

        private Action callback;

        public IMoveObject owner { get; }

        public Vector3 target
        {
            get { return _target; }
        }

        public MoveStation station
        {
            get { return _station; }
            set
            {
                if (_station != value)
                {
                    _station = value;
                    onStationChange?.Invoke(_station);
                }
            }
        }

        public bool isPause
        {
            get { return _pauseList.Count > 0; }
        }

        public float moveParamater { get; set; } = 1;
        
        public bool isMoveEnd
        {
            get
            {
                if (owner.navmesh.isActiveAndEnabled && !owner.navmesh.pathPending)
                {
                    if (Mathf.Abs(owner.navmesh.remainingDistance) <= owner.navmesh.stoppingDistance)
                    {
                        return true;
                    }
                }

                return false;
            }
        }
        
        public event Action<MoveStation> onStationChange;
        public event Action<bool> onPause;
        
        public AgentMoveCtrl(IMoveObject owner)
        {
            this.owner = owner;
        }

        private void OnOwnerSwitchStation()
        {
            owner.navmesh.speed = owner.moveSpeed * moveParamater;
        }

        public void OnCreat()
        {
            owner.onSwitchStation += OnOwnerSwitchStation;
        }

        public void OnBorn()
        {
            owner.navmesh.enabled = true;
            owner.transform.position = owner.bornPoint.GetPostion();
            owner.transform.rotation = owner.bornPoint.GetRotation();
            owner.transform.localScale = owner.bornPoint.GetScale();
        }

        public void OnUpdate()
        {
            if (!isPause && station == MoveStation.Moving)
            {
                if (isMoveEnd)
                {
                    station = MoveStation.MoveComplete;
                    callback?.Invoke();
                }
            }
        }

        public void Pause(object key)
        {
            if (!_pauseList.Contains(key))
            {
                _pauseList.Add(key);
            }

            owner.navmesh.isStopped = true;
            onPause?.Invoke(true);
        }

        public void Continue(object key)
        {
            if (_pauseList.Contains(key))
            {
                _pauseList.Remove(key);
            }

            owner.navmesh.isStopped = false;
            onPause?.Invoke(false);
        }

        public void OnAgentDead()
        {
            owner.navmesh.enabled = false;
        }

        public void OnDestroy()
        {
            owner.onSwitchStation -= OnOwnerSwitchStation;
        }

        private void PrivateMoveTo(Vector3 position)
        {
            NavMeshHit hit;
            _target = position;
            owner.navmesh.speed = owner.moveSpeed * moveParamater;
            owner.navmesh.SetDestination(position);
            station = MoveStation.Moving;
        }

        public void MoveToTarget(Action callback)
        {
            MoveTo(owner.moveTarget, callback);
        }

        public void MoveTo(Vector3 position, Action callback)
        {
            PrivateMoveTo(position);
            this.callback = callback;
        }
        
        public bool CanMoveTo(Vector3 targetPoint)
        {
            var path = new NavMeshPath();
            NavMesh.CalculatePath(owner.navmesh.transform.position, targetPoint, NavMesh.AllAreas, path);
            return path.status == NavMeshPathStatus.PathComplete;
        }


        public T GetAgentCtrl<T>() where T : IAgentCtrl
        {
            return owner.GetAgentCtrl<T>();
        }
    }
}