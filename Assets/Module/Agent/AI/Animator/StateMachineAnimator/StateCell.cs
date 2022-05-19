using UnityEngine;

namespace Module
{
    public abstract class StateCell
    {
        public string name
        {
            get { return this.GetType().Name; }
        }

        public int layer { get; }
        public StateMachineCtrl ctrl;
        public Animator animator => ctrl.owner.animator;
        public int shortName;
        public string firstStateName;

        public StateCell(int layer, string firstStateName)
        {
            this.layer = layer;
            this.firstStateName = firstStateName;
            this.shortName = Animator.StringToHash(firstStateName);
        }


        public abstract void Play();
        public abstract void Stop(bool completge);
        public override string ToString()
        {
            return name;
        }
    }
}