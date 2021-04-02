using Module;
using UnityEngine;
using UnityEngine.UI;

public class FpsShow : MonoBehaviour
{
    private Text text;

    private void Awake()
    {
        text = GetComponent<Text>();
    }
    
    private void Update()
    {
        UpdateTick();
    }


#region 计算FPS
    private long mFrameCount = 0;
    private long mLastFrameTime = 0;
    static long mLastFps = 0;

    private void UpdateTick()
    {
        mFrameCount++;
        long nCurTime = TickToMilliSec(System.DateTime.Now.Ticks);
        if (mLastFrameTime == 0)
        {
            mLastFrameTime = TickToMilliSec(System.DateTime.Now.Ticks);
        }

        if ((nCurTime - mLastFrameTime) >= 1000)
        {
            long fps = (long) (mFrameCount * 1.0f / ((nCurTime - mLastFrameTime) / 1000.0f));

            mLastFps = fps;

            mFrameCount = 0;

            mLastFrameTime = nCurTime;
            text.text = fps.ToString();
        }
    }
    public static long TickToMilliSec(long tick)
    {
        return tick / (10 * 1000);
    }
    #endregion
}