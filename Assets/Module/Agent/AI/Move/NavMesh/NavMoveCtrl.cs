using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

namespace Module
{
    public enum MoveStation
    {
        Idle,
        Moving,
    }

    public class NavMoveCtrl : IAgentCtrl
    {

        #region Private field

        private bool _isPause;
        private MoveStation _station = MoveStation.Idle;
        private NavMeshPathStatus _pathStatus;
        private NavMeshPath tempPath;
        private Action<NavMeshPathStatus, bool> callback;
        private bool _isStop;
        private bool isRotate;

        #endregion

        public bool isStop => isPause || _isStop || station == MoveStation.Idle || Math.Abs(owner.moveSpeed) < 0.0001f;
        public INavMoveObject owner { get; }
        public bool dynamicCallback { get; set; }
        public MoveStation station => _station;
        public bool isPause => _isPause;
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
        
        public event Action<Quaternion> onLookRotation;


        public NavMoveCtrl(INavMoveObject owner)
        {
            this.owner = owner;
            owner.onSwitchStation += OnOwnerSwitchStation;
            owner.navmesh.updateRotation = false;
            owner.navmesh.acceleration = float.MaxValue;
            owner.navmesh.enabled = true;
            OnOwnerSwitchStation();
        }
        
        void Rotate()
        {
            if (Mathf.Abs(owner.rotateSpeed) >= 0.0001f)
            {
                Vector3 normal = owner.terrainNormal;
                Vector3 direction = owner.moveDirection;
                direction.y = 0.0f;
                if (direction.magnitude > 0.1f && normal.magnitude > 0.1f)
                {
                    Quaternion qLook = Quaternion.LookRotation(direction, Vector3.up);
                    Quaternion qNorm = Quaternion.FromToRotation(Vector3.up, normal);
                    Quaternion lookRotation = qNorm * qLook;
                    var rotation = owner.transform.rotation;
                    rotation = Quaternion.Slerp(rotation, lookRotation, owner.GetDelatime(false) * owner.rotateSpeed);
                    owner.transform.rotation = rotation;
                    float angle2Target = Quaternion.Angle(rotation, lookRotation);
                    bool temp = angle2Target > owner.rotateToMove;
                    if (temp != isRotate)
                    {
                        isRotate = temp;
                        OnOwnerSwitchStation();
                    }
                    onLookRotation?.Invoke(lookRotation);
                }
            }
        }

        private void OnOwnerSwitchStation()
        {
            if (!owner.navmesh.isActiveAndEnabled) return;
            owner.navmesh.stoppingDistance = owner.stopMoveDistance;
            owner.navmesh.speed = isRotate ? 0 : owner.moveSpeed;
            owner.navmesh.isStopped = isStop;
            owner.navmesh.autoBraking = !isStop;
        }

        public void OnUpdate()
        {
            if (!isPause && station == MoveStation.Moving)
            {
                var path = owner.navmesh.path;
                if (dynamicCallback)
                {
                    var newStatus = path.status;
                    if (_pathStatus != newStatus)
                    {
                        callback?.Invoke(newStatus, false);
                        _pathStatus = newStatus;
                    }
                }
                if (isMoveEnd)
                {
                    _station = MoveStation.Idle;
                    callback?.Invoke(path.status, true);
                }
                
                Rotate();
            }
        }


        public void Pause()
        {
            _isPause = true;
            OnOwnerSwitchStation();
        }

        public void Continue()
        {
            _isPause = false;
            OnOwnerSwitchStation();
        }

        public void OnAgentDead()
        {
            OnOwnerSwitchStation();
            owner.navmesh.enabled = false;
        }

        public void OnDestroy()
        {
            owner.onSwitchStation -= OnOwnerSwitchStation;
        }

        private bool PrivateMoveTo(Vector3 position)
        {
            if (!owner.navmesh.enabled) return false;
            _station = MoveStation.Moving;
            _isStop = false;
            OnOwnerSwitchStation();
            owner.navmesh.SetDestination(position);
            callback?.Invoke(owner.navmesh.path.status, false);
            return true;
        }
        
        public bool MoveTo(Vector3 position, Action<NavMeshPathStatus, bool> callback)
        {
            this.callback = callback;
            return PrivateMoveTo(position);
        }
        
        public void StopMove()
        {
            _isStop = true;
            OnOwnerSwitchStation();
        }

        public T GetAgentCtrl<T>() where T : IAgentCtrl
        {
            return owner.GetAgentCtrl<T>();
        }

        public void EditorInit()
        {
            NavMeshAgent  _navmesh = owner.transform.gameObject.AddOrGetComponent<NavMeshAgent>();
            _navmesh.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
            _navmesh.enabled = false;
        }

        public bool CanMoveTo(Vector3 moveTarget)
        {
            return CalculatePath(moveTarget) && tempPath.status == NavMeshPathStatus.PathComplete;
        }

        private bool CalculatePath(Vector3 moveTarget)
        {
            if (tempPath == null)
            {
                tempPath = new NavMeshPath();
            }

            var temp = owner.navmesh.CalculatePath(moveTarget, tempPath);
            return temp;
        }
        
        public void OnDrawGizmos()
        {
            if (owner.isLog)
            {
                NavMeshPath path = owner.navmesh.path;
                if (path.corners.Length > 1)
                {
                    for (int i = 1; i < path.corners.Length; i++)
                    {
                        if (path.status == NavMeshPathStatus.PathComplete)
                        {
                            if (i < path.corners.Length - 1)
                            {
                                Gizmos.color = Color.yellow;
                            }
                            else
                            {
                                Gizmos.color = Color.green;
                            }
                        }
                        else
                        {
                            Gizmos.color = Color.gray;
                        }

                        Gizmos.DrawSphere(path.corners[i], 0.2f);
                    }
                }
            }
        }
    }
}