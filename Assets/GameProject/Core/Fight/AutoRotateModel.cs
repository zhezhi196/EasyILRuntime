using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotateModel : MonoBehaviour
{
    public float speed = 20;
    void Update()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform temp = transform.GetChild(i);
            if (temp.gameObject.activeInHierarchy)
            {
                temp.Rotate(Vector3.up * -speed * Time.unscaledDeltaTime, Space.World);
            }
        }
    }
}
