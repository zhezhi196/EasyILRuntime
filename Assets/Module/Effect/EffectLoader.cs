using UnityEngine;

namespace Module
{
    public static class EffectLoader
    {
        private const string effectPath = "Effect/{0}";

        public static EffectPlay Play(string name)
        {
            EffectPlay play = new EffectPlay();
            
            BundleManager.LoadGameoObject<EffectBase>(string.Format(effectPath, name), effect =>
            {
                effect.Play(play);
            });
            
            return play;
        }
    }
}