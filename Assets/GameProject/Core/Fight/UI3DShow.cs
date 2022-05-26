using System.Collections.Generic;
using Module;
using UnityEngine;

public class UI3DShow : MonoSingleton<UI3DShow>
{
    public Dictionary<string, IModelObject> currModel = new Dictionary<string, IModelObject>();
    public Camera camera;
    public float rotateSpeed = 5;
    private IModelObject onLoadWatch;

    public void OnRotateModel(string key, Vector2 v)
    {
        if (currModel.TryGetValue(key, out IModelObject tar))
        {
            tar.GetModel(go =>
            {
                go.transform.Rotate(Vector3.up * (-v.x * rotateSpeed * TimeHelper.unscaledDeltaTimeIgnorePause),
                    Space.World);
            });
        }
    }

    public void OnShow(string key, IModelObject arg1)
    {
        OnShow(key, arg1, Vector3.zero);
    }

    public void OnShow(string key, IModelObject arg1, Vector3 rotation)
    {
        if (arg1 == null) return;
        
        UICommpont.FreezeUI(arg1.modelName);
        camera.gameObject.OnActive(true);
        if (currModel.TryGetValue(key, out IModelObject last))
        {
            last.GetModel(lastGo =>
            {
                lastGo.OnActive(false);
                LoadNew(key, arg1, rotation);
            });
        }
        else
        {
            LoadNew(key, arg1, rotation);
        }
    }

    private void LoadNew(string key,IModelObject arg1,Vector3 rotation)
    {
        currModel.SetOrAdd(key, arg1);
        onLoadWatch = arg1;
        arg1.GetModel(go =>
        {
            go.transform.SetParent(UICommpont.creatPoint3D);
            go.OnActive(onLoadWatch == arg1);
            go.transform.eulerAngles = rotation;
            onLoadWatch = null;
            UICommpont.UnFreezeUI(arg1.modelName);
        });
    }

    public void OnClose(string key)
    {
        if (currModel.TryGetValue(key, out IModelObject curr))
        {
            curr?.GetModel(go =>
            {
                go.OnActive(false);
                go.transform.eulerAngles = Vector3.zero;
            });
            currModel.Remove(key);
        }

        if (currModel.Count == 0)
        {
            camera.gameObject.OnActive(false);
        }
    }
}