using Module;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏内摄像机控制器
/// </summary>
public class GameCameraCtrl : MonoBehaviour
{
    Player _player;
    /// <summary>
    /// 环境摄像机
    /// </summary>
    public Camera evCamera;//渲染除玩家及武器外所有
    private Transform evCameraTrans;
    /// <summary>
    /// 玩家摄像机
    /// </summary>
    public Camera pCamera;//只渲染玩家模型和武器
    private Transform pCameraTrans;

    public float fallValue = 10;
    public float smoothTime = 0.01f;
    private float yVelocity = 0.0f;
    private float value = 0f;
    private float targetValue = 0;

    private void Start()
    {
        evCameraTrans = evCamera.transform;
        pCameraTrans = pCamera.transform;
        EventCenter.Register<Weapon>(EventKey.OnWeaponFire, OnFire);
    }

    private void OnDestroy()
    {
        EventCenter.UnRegister<Weapon>(EventKey.OnWeaponFire, OnFire);
    }

    public void Init(Player p)
    {
        _player = p;
    }

    private void Update()
    {
        UpdateRecoil();
    }

    private void LateUpdate()
    {
        transform.rotation = _player.cameraFllowTrans.rotation;
        transform.position = _player.cameraFllowTrans.position;
    }

    private void OnFire(Weapon weapon)
    {
        if (targetValue < weapon.weaponArgs.MaxRecoil && weapon.weaponArgs.Recoil>0)
        {
            targetValue += weapon.weaponArgs.Recoil;
            targetValue = targetValue.Clamp(0, weapon.weaponArgs.MaxRecoil);
        }
    }

    public void UpdateRecoil()
    {
        if (targetValue > 0)
        {
            targetValue -= TimeHelper.unscaledDeltaTime * fallValue;
        }
        targetValue = targetValue.Clamp(0, 60f);
        value = Mathf.SmoothDamp(value, targetValue, ref yVelocity, smoothTime);
        transform.parent.localEulerAngles = new Vector3(-value, 0, 0);
    }
}
