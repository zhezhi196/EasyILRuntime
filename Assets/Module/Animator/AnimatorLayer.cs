using System;
using DG.Tweening;
using UnityEngine;

namespace Module
{
    public class AnimatorLayer
    {
        public int layer;
        private Tweener fadeIn;
        private Tweener fadeOut;
        public AnimatorPlay currPlay;

        public AnimatorLayer(int layer)
        {
            this.layer = layer;
        }

        public void FadeInLayer(Animator animator,  float time)
        {
            if (fadeIn != null)
            {
                fadeIn.Kill();
            }

            fadeIn = DOTween
                .To(() => animator.GetLayerWeight(layer), lay => animator.SetLayerWeight(layer, lay), 1f, time)
                .SetId(animator.gameObject);
        }

        public void FadeOutLayer(Animator animator, float time)
        {
            if (fadeOut != null)
            {
                fadeOut.Kill();
            }

            fadeOut = DOTween
                .To(() => animator.GetLayerWeight(layer), lay => animator.SetLayerWeight(layer, lay), 0f, time)
                .SetId(animator.gameObject);
        }

        public AnimatorPlay Play(AnimatorInfo info, Action<bool> onExitLayer)
        {
            if (currPlay != null && currPlay.isPlaying)
            {
                currPlay.SetStop();
            }

            info.Play(this);
            currPlay = new AnimatorPlay(info.owner, info);
            currPlay.StartCoroutine(() =>
            {
                currPlay = null;
                onExitLayer?.Invoke(true);
            });
            return currPlay;
        }

        public override string ToString()
        {
            return layer.ToString();
        }


    }
}