using System;
using Module;
using UnityEngine;
using UnityEngine.UI;

public class UILoading : MonoBehaviour
{
    public const string uiLoading = "uiLoading";
    private float updateTime;
    private CanvasGroup canvasGroup;
    public Text title;

    private void Awake()
    {
        Loading.openCallback += Open;
        Loading.closeCallback += Close;
        gameObject.OnActive(false);
    }

    private void OnDestroy()
    {
        Loading.openCallback -= Open;
        Loading.closeCallback -= Close;
    }

    private async void Open(string type,object[] args)
    {
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddOrGetComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 1;
        this.gameObject.OnActive(true);
        if (!args.IsNullOrEmpty())
        {
            SetText(args[0].ToString());
            await Async.WaitforSeconds(1.5f);
            SetText(args[1].ToString());
        }
    }

    private void Close(string type)
    {
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddOrGetComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 0;
        this.gameObject.OnActive(false);
        title.gameObject.OnActive(false);
    }

    public void SetText(string text)
    {
        this.title.gameObject.OnActive(true);
        this.title.text = text;
    }
}