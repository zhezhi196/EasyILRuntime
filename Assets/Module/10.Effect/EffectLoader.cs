using UnityEngine;

namespace Module
{
    public static class EffectLoader
    {
        private const string effectPath = "Effect/{0}";

        public static EffectPlay Play(string name,Vector3 pos, Vector3 dir)
        {
            EffectPlay play = new EffectPlay();
            
            BundleManager.LoadGameoObject<EffectBase>(string.Format(effectPath, name), effect =>
            {
                effect.Play(play);
                effect.transform.position = pos;
                effect.transform.eulerAngles = dir;
            });
            
            return play;
        }
        
    }
}