using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Module
{
    public class Loading : MonoBehaviour
    {
        public Slider slider;
        public Text discription;
        public Dictionary<string, float> processDic = new Dictionary<string, float>();
        
        public void OpenLoading()
        {
            gameObject.SetActive(true);      
        }

        public void CloseLoading()
        {
            gameObject.SetActive(false);
        }

        public void AddLoading(string key, float valueProcessInAll)
        {
            processDic.Add(key,valueProcessInAll);
        }
        
        private Sequence loadingSequence;
        private Queue<string> discriptionQueue = new Queue<string>();
        
        public float SetLoading(float result, string discription, bool delay, float minScecond)
        {
            if (slider.value == 0)
            {
                loadingSequence = DOTween.Sequence();
                gameObject.SetActive(true);
            }
            discriptionQueue.Enqueue(discription);
            Tweener t = slider.DOValue(result, minScecond).SetEase(Ease.Linear).SetDelay(slider.value == 0 && delay ? 0.5f : 0).OnStart(() => { this.discription.text = discriptionQueue.Dequeue(); });
            loadingSequence.Append(t);

            loadingSequence.OnComplete(() =>
            {
                gameObject.SetActive(false);
                slider.value = 0;
            });


            if (result == 1)
            {
                loadingSequence.PlayForward();
            }


            return result;
        }
    }
}
