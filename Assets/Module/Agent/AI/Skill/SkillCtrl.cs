using System;
using System.Collections.Generic;
using DG.Tweening;
using Module.SkillAction;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Module
{
    [Flags]
    public enum BreakSkillFlag
    {
        WithAnimation = 1,
        BreakAction = 2,
    }
    public class SkillCtrl : IAgentCtrl
    {
        #region Private

        private IActiveSkill _currActive;
        private bool _isPause;

        #endregion

        #region 属性

        public bool isPause => _isPause;
        private List<Skill> _allSkill = new List<Skill>();
        public bool isBusy
        {
            get { return currActive != null; }
        }

        public List<Skill> allSkill
        {
            get
            {
                return _allSkill;
            }
        }

        public void SetSkill<T>(params T[] skill) where T: Skill
        {
            if (!allSkill.IsNullOrEmpty())
            {
                for (int i = 0; i < allSkill.Count; i++)
                {
                    allSkill[i].OnUnload();
                }

                allSkill.Clear();
            }

            AddSkill(skill);
        }

        public void AddSkill<T>(params T[] skill) where T: Skill
        {
            if (!skill.IsNullOrEmpty())
            {
                allSkill.AddRange(skill);
                    
                for (int i = 0; i < skill.Length; i++)
                {
                    skill[i].OnLoadToOwner(owner);
                    skill[i].isActive = true;
                }
            }
        }

        public ISkillObject owner { get; }

        /// <summary>
        /// 当前正在释放的技能
        /// </summary>
        public IActiveSkill currActive => _currActive;
        /// <summary>
        /// 想要释放的技能
        /// </summary>
        public IActiveSkill readyRelease;

        #endregion

        public SkillCtrl(ISkillObject owner)
        {
            this.owner = owner;
        }

        public void OnUpdate()
        {
            readyRelease = owner.RefreshReadySkill();
            for (int i = 0; i < allSkill.Count; i++)
            {
                allSkill[i].OnUpdate();
            }
        }

        public bool UpdateRelease(Action<bool> callback)
        {
            if (readyRelease != null)
            {
                if (!TryRelease(callback))
                {
                    UpdateCheck();
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        public void Pause()
        {
            if (isBusy && !isPause)
            {
                currActive.Pause();
            }
        }

        public void Continue()
        {
            if (isBusy)
            {
                currActive.Continue();
            }
        }

        public void OnAgentDead()
        {
        }

        public void OnDestroy()
        {
            allSkill.Clear();
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

        /// <summary>
        /// 打断技能
        /// </summary>
        /// <param name="withAnimation"></param>
        /// <returns></returns>
        public bool BreakSkill(BreakSkillFlag flag)
        {
            if (isBusy)
            {
                if (currActive.Break(flag))
                {
                    _currActive = null;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// 尝试触发被动
        /// </summary>
        /// <returns></returns>
        public bool TriggerPassive(params object[] args)
        {
            for (int i = 0; i < allSkill.Count; i++)
            {
                ISkill tarSkill = allSkill[i];
                if (tarSkill is IPassiveSkill passive && owner.canReleaseSkill && passive.isReadyRelease && passive.Trigger(args))
                {
                    owner.LogFormat("{0} 触发被动技能 {1}", owner, tarSkill);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 主动释放技能
        /// </summary>
        /// <param name="skill"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public bool TryRelease(IActiveSkill skill, Action<bool> callback)
        {
            if (CanReleaseSkill(skill))
            {
                skill.Release(complete =>
                {
                    callback?.Invoke(complete);
                    _currActive = null;
                });
                if (currActive != null)
                {
                    currActive.Break(BreakSkillFlag.BreakAction);
                }

                _currActive = skill;
                owner.OnReleasingSkill(skill);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 获取第一个序列的技能,尝试主动释放,注意这个函数需要再update中运行,当满足不了条件的时候,会每帧调用TryReleaeSkill
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public bool TryRelease(Action<bool> callback)
        {
            return TryRelease(readyRelease, callback);
        }

        public bool CanReleaseSkill(ISkill skill)
        {
            if (skill == null || !owner.canReleaseSkill || isBusy) return false;
            return skill.isReadyRelease;
        }

        public void UpdateCheck()
        {
            if (readyRelease != null)
            {
                readyRelease.UpdateTry();
            }
        }

        public void ClearCD()
        {
            for (int i = 0; i < allSkill.Count; i++)
            {
                if (allSkill[i].cd != null)
                {
                    allSkill[i].EnterReady();
                }
            }
        }

        public void ClearSkill()
        {
            _allSkill = null;
            _allSkill = new List<Skill>();
        }

        public void RemoveSkill(string name)
        {
            for (int i = 0; i < allSkill.Count; i++)
            {
                if (allSkill[i].name == name)
                {
                    allSkill[i].OnUnload();
                    allSkill.RemoveAt(i);
                    break;
                }
            }
        }

        public void ResetSkillCD()
        {
            for (int i = 0; i < allSkill.Count; i++)
            {
                if (allSkill[i].station == SkillStation.CD)
                {
                    allSkill[i].EnterReady();
                }
            }
        }
    }
}