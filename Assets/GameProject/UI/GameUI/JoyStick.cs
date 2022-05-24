using System;
using Module;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoyStick : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public CanvasGroup canvasGroup;
    public Transform dir;
    public bool isDrag;
    public float canvasAlpha;

    private void Awake()
    {
        canvasAlpha = canvasGroup.alpha;
    }

    private void Update()
    {
        if (isDrag && Player.player.canCtrlPlayer)
        {
            Player.player.Move(-dir.transform.up);
        }
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        isDrag = true;
        canvasGroup.alpha = canvasGroup.alpha * 0.5f;
        dir.gameObject.OnActive(true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 handle;
        RectTransform rect=canvasGroup.transform as RectTransform;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, eventData.position, UICommpont.camera2D, out handle);
        Vector2 handleDir = handle - new Vector2(rect.localPosition.x, rect.localPosition.y);
        dir.up = -handleDir;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDrag = false;
        Player.player.StopMove();
        canvasGroup.alpha = canvasAlpha;
        dir.gameObject.OnActive(false);
    }
}