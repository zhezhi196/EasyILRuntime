using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Module
{
    public class StateMachineCtrl : IAgentCtrl
    {
        public List<StateCell> cells = new List<StateCell>();
        private bool _isPause;
        public IStateMechineObject owner { get; }
        public bool isPause => _isPause;

        private List<StateMachinePlay> currPlay = new List<StateMachinePlay>();
        public Animator animator => owner.animator;

        public StateMachineCtrl(IStateMechineObject owner,params StateCell[] cell)
        {
            this.owner = owner;
            AddCell(cell);
        }

        public void BeforeChangeContrller()
        {
            for (int i = 0; i < currPlay.Count; i++)
            {
                currPlay[i].EndPlay(false);
            }
            currPlay.Clear();
        }

        public StateMachinePlay Play<T>() where T: StateCell
        {
            StateCell cell = cells.Find(fd => fd.GetType() == typeof(T));
            return PlayPrivate(cell);
        }

        public StateMachinePlay Play(string name)
        {
            StateCell cell = cells.Find(fd => fd.GetType().Name == name);
            return PlayPrivate(cell);
        }

        private StateMachinePlay PlayPrivate(StateCell cell)
        {
            if (cell != null)
            {
                owner.LogFormat("状态机动画 准备播放{0}",cell.name);
                var flagPlay = new StateMachinePlay(cell, owner);
                currPlay.Add(flagPlay);
                flagPlay.Play();
                return flagPlay;
            }
            GameDebug.LogError($"{cell}无法找到状态块");
            return null;
        }

        public void AddCell(params StateCell[] cell)
        {
            for (int i = 0; i < cell.Length; i++)
            {
                cell[i].ctrl = this;
                this.cells.Add(cell[i]);
            }
        }

        public bool OnUpdate()
        {
            if (currPlay != null)
            {
                for (int i = 0; i < currPlay.Count; i++)
                {
                    currPlay[i].OnUpdate();
                }
            }

            return true;
        }

        public void Pause()
        {
        }

        public void Continue()
        {
        }

        public void OnAgentDead()
        {
        }

        public void OnDestroy()
        {
        }

        public T GetAgentCtrl<T>() where T : IAgentCtrl
        {
            return owner.GetAgentCtrl<T>();
        }

        public void EditorInit()
        {
            
        }

        public void OnDrawGizmos()
        {
        }

        public void OnStateEnter(AnimatorStateInfo stateInfo, int layerIndex)
        {
            var temPlay = currPlay.Find(fd => fd.IsSame(animator, stateInfo.shortNameHash, layerIndex));
            if (temPlay != null)
            {
                temPlay.StartPlay(stateInfo);
            }
        }

        public void OnStateExit(AnimatorStateInfo stateInfo, int layerIndex)
        {
            var temPlay = currPlay.Find(fd => fd.IsSame(animator, stateInfo.shortNameHash, layerIndex));
            if (temPlay != null)
            {
                EndPlay(temPlay, true);
            }
        }

        public Tweener SetFloat(string name, float target, float time)
        {
            return DOTween
                .To(() => animator.GetFloat(name), (value) => { animator.SetFloat(name, value); }, target, time)
                .SetUpdate(true);
        }

        public void OnAnimatorDisable(int shortNameHash)
        {
            var @break = currPlay.Find(fd=>fd.IsSame(animator,shortNameHash));
            if (@break != null)
            {
                EndPlay(@break, false);
            }
        }

        public void EndPlay(StateMachinePlay play,bool complete)
        {
            play.EndPlay(complete);
            currPlay.Remove(play);
        }
    }
}