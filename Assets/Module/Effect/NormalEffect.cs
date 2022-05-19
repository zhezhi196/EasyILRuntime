using UnityEngine;

namespace Module
{
    public class NormalEffect : EffectBase
    {
        public override void Stop()
        {
            play.SetComplete(true);
        }

        public override void ReturnToPool()
        {
            if (pool != null)
            {
                transform.localScale = Vector3.one;
            }
        }
    }
}