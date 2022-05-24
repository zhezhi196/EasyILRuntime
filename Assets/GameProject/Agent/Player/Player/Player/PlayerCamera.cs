using System;
using System.Collections;
using DG.Tweening;
using Module;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Camera camera;

    public void ShakeCamera(Vector3 dir, float value)
    {
        
    }

    private void OnEnable()
    {
        BaseCamera.AddCamera(camera);
    }

    private void OnDisable()
    {
        BaseCamera.RemoveCamera(camera);
    }

    private void LateUpdate()
    {
        transform.position = Player.player.transform.position;
    }
}