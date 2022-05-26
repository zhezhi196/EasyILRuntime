using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Module;

public class GameUIEffect : MonoBehaviour,IPoolObject
{
    public ObjectPool pool { get; set; }

    public void OnGetObjectFromPool()
    {
    }

    public void ReturnToPool()
    {
        gameObject.OnActive(false);

    }

    public float time =1;
    private float tempTime = 0;
    public bool isContinue = false;

    private void Update()
    {
        if (tempTime > 0)
        {
            tempTime -= TimeHelper.deltaTime;
            if (tempTime <= 0)
            {
                gameObject.OnActive(false);
            }
        }
    }

    public void Show()
    {
        if (isContinue)
        {
            ContinueShow();
        }
        else {
            OnceShow();
        }
    }

    private void ContinueShow()
    {
        tempTime = time;
        if (!gameObject.activeSelf)
        {   
            gameObject.OnActive(true);
        }
    }

    private async void OnceShow()
    {
        gameObject.OnActive(true);
        await Async.WaitforSecondsRealTime(time, gameObject);
        ObjectPool.ReturnToPool(this);
    }

    private void OnDestroy()
    {
        Async.StopAsync(gameObject);
    }
}
