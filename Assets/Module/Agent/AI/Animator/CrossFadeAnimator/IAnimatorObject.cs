using UnityEngine;

namespace Module
{
    public interface IAnimatorObject : IAnimaotr
    {
        float animatorSpeed { get; }
        float GetTranslateTime(string name);
        float GetLayerFadeTime(int type, string name);
        float getBreakTranslation(int layer, string name);
        string GetLayerDefaultAnimation(int layer);
        void OnBreakAnamition(int layer, string name, bool sendEvent);
    }
}