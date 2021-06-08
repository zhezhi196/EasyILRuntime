using System;

namespace Module.SkillLoop
{
    public class TimeLoop : ISkillLoop
    {
        private Clock timeClock;
        private ISkill _skill;

        private Func<bool> _predicate;
        public float currLoop
        {
            get { return timeClock.currentTime; }
        }

        public float totalLoop
        {
            get { return timeClock.targetTime; }
        }

        public ISkill skill
        {
            get { return _skill; }
        }

        public event Action onStart;
        public event Action onEnd;
        public event Action onBreak;

        public TimeLoop()
        {
        }

        public TimeLoop(ISkill owner, float loop)
        {
            Init(owner, loop);
        }

        public void Init(ISkill owner, float loop)
        {
            timeClock = new Clock(loop);
            timeClock.owner = owner.owner;
            timeClock.onComplete += End;
            timeClock.autoKill = false;
            _skill = owner;
        }

        public void Start()
        {
            if (skill.isActive)
            {
                timeClock.Restart();
                onStart?.Invoke();
            }
        }

        public void End()
        {
            if (skill.isActive)
            {
                onEnd?.Invoke();
            }
        }

        public void Break()
        {
            timeClock.Stop();
            onBreak?.Invoke();
        }

        public void Pause()
        {
            if (skill.isActive)
            {
                timeClock.Pause();
            }
        }

        public void Continue()
        {
            if (skill.isActive)
            {
                timeClock.StartTick();
            }
        }

        public void Dispose()
        {
            timeClock.onComplete -= End;
        }

        public void SetListener(Func<bool> listener)
        {
            if (skill.isActive)
            {
                timeClock.SetListener(listener);
            }
        }
    }
}