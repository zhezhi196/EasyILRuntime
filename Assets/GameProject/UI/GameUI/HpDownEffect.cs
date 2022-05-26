using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Module;

public class HpDownEffect : MonoBehaviour,IPoolObject
{
    public Image effectImage;
    public float effectTime = 0.5f;
    public Vector3 toSize = Vector3.one;
    public Color toColor = Color.white;
    public AnimationCurve alphaCurve;
    public ObjectPool pool { get; set; }

    public void Play()
    {
        effectImage.transform.DOScale(toSize, effectTime).SetId(gameObject).SetUpdate(true);
        effectImage.DOColor(toColor, effectTime).OnComplete(OnComplete).SetId(gameObject).SetEase(alphaCurve).SetUpdate(true);
    }

    private void OnComplete()
    {
        ObjectPool.ReturnToPool(this);
    }

    private void OnDestroy()
    {
        DOTween.Kill(gameObject);
    }

    public void ReturnToPool()
    {
        effectImage.transform.localScale = Vector3.one;
        effectImage.color = Color.white;
        
    }

    public void OnGetObjectFromPool()
    {
        
    }
}
