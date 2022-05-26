using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using System;
using UnityEngine.Events;

public class UISceneCtrl : MonoBehaviour
{
    public Camera sceneCamera;
    public FirstGameTimeline timeline;
    private UniversalAdditionalCameraData cameraData;
    private static UISceneCtrl _instance;

    public static UISceneCtrl Instance
    {
        get {
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }

    private void OnEnable()
    {
        if (cameraData == null)
        {
            cameraData = sceneCamera.GetComponent<UniversalAdditionalCameraData>();
        }
        cameraData.cameraStack.Add(CameraCtrl.Instance.uiCamera);
        CameraCtrl.HideMainCamera();
    }

    private void OnDisable()
    {
        CameraCtrl.ShowMainCamera();
    }

    public void PlayTimeline(UnityAction callBack)
    {
        timeline.Play(null, null, callBack,null);
    }
}
