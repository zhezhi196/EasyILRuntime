using System;
using DG.Tweening;

namespace Module
 {
     public interface ITimeCtrl
     {
         event Action<float> onTimescale;
         float timeScale { get; }
         float GetUnscaleDelatime(bool ignorePause);
         float GetDelatime(bool ignorePause);
         void SetTimescale(float timeScale);
         Tweener SetTimescale(float timeScale, float time);
     }
 }