using UnityEngine;

public class AEUICommpont
{
    private static Transform _creatPoint3d;
    public static Transform creatPoint3d
    {
        get
        {
            if (_creatPoint3d == null) _creatPoint3d = GameObject.Find("UIRoot/3DParent/prefab").transform;
            return _creatPoint3d;
        }
    }

    public static void NetError()
    {
    }

    public static void AdsError()
    {
        
    }
}