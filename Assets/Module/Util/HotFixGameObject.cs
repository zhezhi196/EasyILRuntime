using System;
using System.Collections;
using System.Collections.Generic;
using Module;
using UnityEngine;

namespace HotFix
{
    public static class HotFixGameObject
    {
        public static GameObject Instantiate(GameObject prefab,Transform parent,Action<ViewReference[]> callBack)
        {
            GameObject go = GameObject.Instantiate(prefab,parent);
            ViewReference[] viewReferences = go.transform.GetComponentsInChildren<ViewReference>();
            callBack?.Invoke(viewReferences);

            return go;
        }
    }


}
