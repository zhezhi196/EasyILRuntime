using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Module
{
    public class AgentSkillCtrl : IAgentCtrl
    {
        private List<object> _pauseList = new List<object>();
        private List<ISkill> _allSkill = new List<ISkill>();

        public AgentSkillCtrl(ISkillObject owner)
        {
            this.owner = owner;
        }
        
        public ISkillObject owner { get; }
        public ISkill currSkill { get; set; }
        
        public T GetAgentCtrl<T>() where T : IAgentCtrl
        {
            return owner.GetAgentCtrl<T>();
        }

        public void OnCreat()
        {
            _allSkill = owner.CreatSkill();
            owner.onSwitchStation += OnSwitchStation;
        }

        private void OnSwitchStation()
        {
            if (currSkill != null)
            {
                if (!currSkill.isActive)
                {
                    currSkill.Break(false);
                }
            }
        }

        public void OnBorn()
        {
            for (int i = 0; i < _allSkill.Count; i++)
            {
                _allSkill[i].SetActive(true);
            }
        }

        public void OnUpdate()
        {
        }

        public void Pause(object key)
        {
            if (!_pauseList.Contains(key))
            {
                _pauseList.Add(key);
            }

            if (currSkill != null)
            {
                currSkill.Pause();
            }
        }

        public void Continue(object key)
        {
            if (_pauseList.Contains(key))
            {
                _pauseList.Remove(key);
            }

            if (_pauseList.Count == 0)
            {
                if (currSkill != null)
                {
                    currSkill.Continue();
                }
            }
        }

        public void OnAgentDead()
        {
        }

        public void OnDestroy()
        {
            owner.onSwitchStation -= OnSwitchStation;
        }

        public bool BreakSkill(bool withAnimation)
        {
            if (currSkill != null)
            {
                return currSkill.Break(withAnimation);
            }

            return false;
        }

        public void ReleaseSkill(string name,Action callback)
        {
            for (int i = 0; i < _allSkill.Count; i++)
            {
                if (_allSkill[i].name == name)
                {
                    if (_allSkill[i].Release(() =>
                    {
                        callback?.Invoke();
                        currSkill = null;
                    }))
                    {
                        if (currSkill != null)
                        {
                            currSkill.Break(false);
                        }
                        currSkill = _allSkill[i];
                    }

                    break;
                }
            }
        }

        public ISkill TryLoopRelease(Action callback)
        {
            if (currSkill == null)
            {
                for (int i = 0; i < _allSkill.Count; i++)
                {
                    if (_allSkill[i].Release(() =>
                    {
                        callback?.Invoke();
                        currSkill = null;
                    }))
                    {
                        currSkill = _allSkill[i];
                        return currSkill;
                    }
                }
            }

            return null;
        }
    }
}