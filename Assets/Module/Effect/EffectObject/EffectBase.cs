using UnityEngine;

namespace Module
{
    public class EffectBase : MonoBehaviour, IPoolObject
    {
        public string name { get; set; }
        public virtual bool isComplete { get; }
        public ObjectPool pool { get; set; }
        
        public virtual void Play(EffectPlay play)
        {
            play.Start(this);
        }
        public virtual void Restart(){}
        public virtual void Pause(){}
        public virtual void Stop(){}
        public virtual void Simulate(float time){}
        

        public virtual void OnGetObjectFromPool(){}
        public virtual void OnReturnToPool(){}
    }
}