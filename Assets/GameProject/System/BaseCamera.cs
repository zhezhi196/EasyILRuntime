using Module;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BaseCamera : MonoSingleton<BaseCamera>
{
    public Camera uiCamera;
    private static Camera baseCamera;
    public static Transform trans; //游戏内同步视角用
    private static UniversalAdditionalCameraData cameraData;

    void Start()
    {
        cameraData = this.GetComponent<UniversalAdditionalCameraData>();
        baseCamera = this.GetComponent<Camera>();
        trans = this.transform;
        AddCamera(uiCamera);
    }

    public static void AddCamera(Camera c, int index = 0)
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
        if (baseCamera != null && baseCamera.gameObject != null)
        {
            baseCamera.gameObject.OnActive(true);
        }
    }
}