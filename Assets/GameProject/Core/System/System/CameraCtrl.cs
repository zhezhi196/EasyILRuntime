using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Module;

/// <summary>
/// 全局摄像机管理
/// </summary>
public class CameraCtrl : MonoBehaviour
{
    public Camera uiCamera;
    private static Camera baseCamera;
    public static Transform trans;//游戏内同步视角用
    private static UniversalAdditionalCameraData cameraData;
    private static CameraCtrl _instance;
    public static CameraCtrl Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        cameraData = this.GetComponent<UniversalAdditionalCameraData>();
        baseCamera = this.GetComponent<Camera>();
        trans = this.transform;
    }

    public static void AddCamera(Camera c,int index = 0)
    {
        cameraData.cameraStack.Insert(index, c);
    }

    public static void RemoveCamera(Camera c)
    {
        cameraData.cameraStack.Remove(c);
    }

    public static void Reset()
    {
        if (trans != null)
        {
            trans.rotation = Quaternion.identity;
            baseCamera.fieldOfView = 45f;
        }
    }

    public static void HideMainCamera()
    {
        baseCamera.gameObject.OnActive(false);
    }

    public static void ShowMainCamera()
    {
        if (baseCamera!=null && baseCamera.gameObject != null)
        {
            baseCamera.gameObject.OnActive(true);
        }
    }
}
