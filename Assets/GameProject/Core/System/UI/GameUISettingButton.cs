using System;
using Module;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
[RequireComponent(typeof(CanvasGroup))]
public class GameUISettingButton : UILocationWrite
{
    public Image bgLight;

    public override void OnBeginDragItem(Vector2 eventData)
    {
        base.OnBeginDragItem(eventData);
        bgLight.gameObject.OnActive(true);
    }

    private void OnDisable()
    {
        bgLight.gameObject.OnActive(false);
    }
}