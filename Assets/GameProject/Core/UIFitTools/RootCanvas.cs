using System;
using Module;
using UnityEngine;

public class RootCanvas : MonoBehaviour
{
    public Canvas CurrentCanvas;
    public static RootCanvas Instance
    {
        get; private set;
    }
    private RectTransform rectTransform;

    public float Width;
    public float Height;

    public float SafeWidth;

    void Awake()
    {
        Instance = this;
    }

    public void ResetCanvas()
    {
        var screenWidth = Screen.width;
        var screenHeight = Screen.height;
        if (screenWidth < screenHeight)
        {
            var temp = screenHeight;
            screenHeight = screenWidth;
            screenWidth = temp;
        }


        var screenSafeWidth = Screen.safeArea.width;
        var screenSafeHeight = Screen.safeArea.height;

        if ((float)screenWidth / screenHeight >= ((float)16 / (float)9))
        {
            Width = 1080 * ((float)screenWidth / (float)screenHeight);
            Height = 1080;
        }
        else
        {
            Width = 1920;
            Height = Width * ((float)screenHeight / (float)screenWidth);
        }

        if (float.IsInfinity(screenSafeWidth) || float.IsNaN(screenSafeWidth) || float.IsInfinity(screenSafeHeight) || float.IsNaN(screenSafeHeight))
        {
            SafeWidth = 1920;
        }
        else
        {
            if ((float)screenWidth / screenHeight >= ((float)16 / (float)9))
            {
                SafeWidth = (float)Math.Ceiling((Height * (screenSafeWidth / screenSafeHeight)));
            }
            else
            {
                SafeWidth = 1920;
            }
        }

    }

    void Start()
    {
        ResetCanvas();
    }


    public void InitSafeWidth()
    {
#if UNITY_ANDROID
        bool haveLiu = true;
#if !UNITY_EDITOR
       GameDebug.Log("尝试调用getbanginfo");
       //haveLiu= SDKMgr.GetInstance().MyCommon.GetBangInfo();
#endif
        if (haveLiu)
        {
            SafeWidth = Width - 174;
        }
#endif
    }
}
