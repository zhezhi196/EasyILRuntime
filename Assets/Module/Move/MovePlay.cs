using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Module
{
    public class MovePlay: IEnumerator
    {
        #region MoveArg

        private struct MoveArg<T>
        {
            public T value;
            public Func<bool> listener;
            public bool isChanged;

            public MoveArg(T value)
            {
                this.value = value;
                this.isChanged = false;
                listener = null;
            }

            public bool ChangeValue(T args, Func<bool> listener)
            {
                if (!this.value.Equals(args) || this.listener != listener)
                {
                    this.value = args;
                    isChanged = true;
                    this.listener = listener;
                    return true;
                }

                return false;
            }
        }

        #endregion

        public INavmeshMoveAgent meshAgent;

        private MoveArg<Vector3> target;
        private MoveArg<bool> isPause;
        private MoveArg<bool> isStop;
        private MoveArg<bool> autoBreaking;
        private MoveArg<float> speed;

        public bool isMoving
        {
            get { return !isPause.value && meshAgent.canMove && !isComplete&& !meshAgent.navmeshAgent.isStopped; }
        }

        public bool isComplete;
        private Action onComplete;
        private Action onStop;
        private Coroutine currCoroutin;

        public MovePlay(INavmeshMoveAgent meshAgent)
        {
            this.meshAgent = meshAgent;
            meshAgent.navmeshAgent.isStopped = false;
        }

        public Coroutine StartCoroutine(Vector3 target, Action callback)
        {
            meshAgent.navmeshAgent.SetDestination(target);
            currCoroutin = this.meshAgent.StartCoroutine(WaitEnd(callback));
            return currCoroutin;
        }

        public MovePlay SetTarget(Vector3 target, Func<bool> fun)
        {
            this.target.ChangeValue(target, fun);
            return this;
        }

        public void StopCoroutine()
        {
            if (!isComplete && currCoroutin != null)
            {
                meshAgent.navmeshAgent.isStopped = true;
                onStop?.Invoke();
                Reset();
                this.meshAgent.StopCoroutine(currCoroutin);
                currCoroutin = null;
            }
        }
        
        private IEnumerator WaitEnd(Action callback)
        {
            yield return this;
            Complete();
            callback?.Invoke();
        }

        public void Complete()
        {
            isComplete = true;
            onStop?.Invoke();
            onComplete?.Invoke();
            Reset();
        }

        public bool MoveNext()
        {
            if (isStop.isChanged && (isStop.listener == null || (isStop.listener != null && isStop.listener.Invoke())))
            {
                StopCoroutine();
                return false;
            }

            if (speed.isChanged)
            {
                meshAgent.navmeshAgent.speed = speed.value;
                speed.isChanged = false;
            }

            if (autoBreaking.isChanged && (autoBreaking.listener == null || (autoBreaking.listener != null && autoBreaking.listener.Invoke())))
            {
                meshAgent.navmeshAgent.autoBraking = autoBreaking.value;
                autoBreaking.isChanged = false;
            }
            
            if (isPause.isChanged && (isPause.listener == null || (isPause.listener != null && isPause.listener.Invoke())))
            {
                isPause.isChanged = false;
            }
            
            if (this.target.isChanged && (this.target.listener == null || (this.target.listener != null && this.target.listener.Invoke())))
            {
                meshAgent.navmeshAgent.SetDestination(this.target.value);
                this.target.isChanged = false;
            }

            meshAgent.navmeshAgent.isStopped = isPause.value || !meshAgent.canMove;
            return !IsMoveEnd();
        }
        
        public bool IsMoveEnd()
        {
            if (meshAgent.navmeshAgent.isActiveAndEnabled && !meshAgent.navmeshAgent.pathPending)
            {
                if (Mathf.Abs(meshAgent.navmeshAgent.remainingDistance) <= meshAgent.navmeshAgent.stoppingDistance)
                {
                    return true;
                }
            }
            return false;
        }
        
        public void Reset()
        {

        }

        public object Current { get; }

        public MovePlay Pause(Func<bool> listener = null)
        {
            isPause.ChangeValue(true, listener);
            return this;
        }

        public MovePlay Resume(Func<bool> listener = null)
        {
            isPause.ChangeValue(false, listener);
            return this;
        }

        public MovePlay Stop(Func<bool> listener = null)
        {
            if (listener != null)
            {
                isStop.ChangeValue(true, listener);
            }
            else
            {
                meshAgent.navmeshAgent.isStopped = true;
                StopCoroutine();
            }
            return this;
        }

        public MovePlay SetSpeed(float speed, Func<bool> listener = null)
        {
            this.speed.ChangeValue(speed, listener);
            return this;
        }

        public MovePlay OnComplete(Action callback)
        {
            this.onComplete = callback;
            return this;
        }
        public MovePlay OnStop(Action callback)
        {
            if (callback != null)
            {
                this.onStop += callback;
            }
            return this;
        }

        public MovePlay SetAutoBreaking(bool autoBreak, Func<bool> listener = null)
        {
            this.autoBreaking.ChangeValue(autoBreak, listener);
            return this;
        }
    }
}