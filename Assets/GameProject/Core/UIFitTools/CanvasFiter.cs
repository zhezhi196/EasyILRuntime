using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasScaler))]
public class CanvasFiter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if ((float)Screen.width / (float)Screen.height > ((float)16 / (float)9))
        {
            transform.GetComponent<CanvasScaler>().matchWidthOrHeight = 1;
        }
        else
        {
            transform.GetComponent<CanvasScaler>().matchWidthOrHeight = 0;
        }
    }
}
