using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Module;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class LockLight
{
    public GameObject light;
    public Image green;
    public GameObject greenGlowObj;
    public Image red;
    public GameObject redGlowObj;
    
}

public class OpenLockUILight : MonoBehaviour
{
    public HorizontalLayoutGroup layoutGroup;
    public List<LockLight> lights;
    public GameObject barGreenLight;
    public GameObject barRedLight;
    private int activeCount;
    
    public void OnRefresh(int count)
    {
        layoutGroup.spacing = count <= 3 ? 172 : 104;
        activeCount = count;
        for (int i = 0; i < lights.Count; i++)
        {
            lights[i].light.OnActive(i < count);
            lights[i].green.gameObject.OnActive(false);
            lights[i].red.gameObject.OnActive(false);
            lights[i].greenGlowObj.OnActive(false);
            lights[i].redGlowObj.OnActive(false);
        }
    }

    public async void Failed(Action callBack)
    {
        var seq = DOTween.Sequence();
        for (int i = 0; i < lights.Count; i++)
        {
            if (i < activeCount)
            {
                lights[i].green.gameObject.OnActive(false);
                lights[i].red.gameObject.OnActive(true);
                lights[i].redGlowObj.SetActive(true);
            }
        }
        barRedLight.OnActive(true);
        await Async.WaitforSecondsRealTime(1.2f);
        barRedLight.OnActive(false);
        for (int i = 0; i < lights.Count; i++)
        {
            if (i < activeCount)
            {
                lights[i].red.gameObject.OnActive(false);
                lights[i].redGlowObj.SetActive(false);
            }
        }
        callBack?.Invoke();
    }

    public async void Success(int successCount , Action callBack)
    {
        for (int i = 0; i < lights.Count; i++)
        {
            if (i < successCount)
            {
                lights[i].green.gameObject.OnActive(true);
                lights[i].greenGlowObj.SetActive(true);
            }
        }
        barGreenLight.OnActive(true);

        await Async.WaitforSecondsRealTime(1.2f);
        for (int i = 0; i < lights.Count; i++)
        {
            if (i < successCount)
            {
                lights[i].greenGlowObj.SetActive(false);
            }
        }
        barGreenLight.OnActive(false);
        callBack?.Invoke();
    }
}

