using System;
using System.Collections.Generic;

namespace Module.SkillAction
{

    public class MutiAction : ISkillAction
    {
        public enum MutiType
        {
            And,
            Or
        }
        private ISkillAction[] SkillActions;

        public bool isEnd
        {
            get
            {
                if (type == MutiType.And)
                {
                    for (int i = 0; i < SkillActions.Length; i++)
                    {
                        if (!SkillActions[i].isEnd)
                        {
                            return false;
                        }
                    }

                    return true;
                }
                else if (type == MutiType.Or)
                {
                    for (int i = 0; i < SkillActions.Length; i++)
                    {
                        if (SkillActions[i].isEnd)
                        {
                            return true;
                        }
                    }

                    return false;
                }

                return false;
            }
        }

        public float percent
        {
            get
            {
                float muti = 0;
                for (int i = 0; i < SkillActions.Length; i++)
                {
                    muti += SkillActions[i].percent;
                }

                return muti;
            }
        }

        public MutiType type { get; }

        public MutiAction(MutiType type,params ISkillAction[] actions)
        {
            this.type = type;
            this.SkillActions = actions;
        }
        
        public void OnStart()
        {
            for (int i = 0; i < SkillActions.Length; i++)
            {
                SkillActions[i].OnStart();
            }
        }

        public void OnEnd(bool complete)
        {
            for (int i = 0; i < SkillActions.Length; i++)
            {
                SkillActions[i].OnEnd(complete);
            }
        }

        public void OnPause()
        {
            for (int i = 0; i < SkillActions.Length; i++)
            {
                SkillActions[i].OnPause();
            }
        }

        public void OnContinue()
        {
            for (int i = 0; i < SkillActions.Length; i++)
            {
                SkillActions[i].OnContinue();
            }
        }

        public void Dispose()
        {
            for (int i = 0; i < SkillActions.Length; i++)
            {
                SkillActions[i].Dispose();
            }
        }

        public void OnUpdate()
        {
            for (int i = 0; i < SkillActions.Length; i++)
            {
                SkillActions[i].OnUpdate();
            }
        }
    }
}