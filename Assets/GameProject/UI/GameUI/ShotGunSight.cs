using UnityEngine;

/// <summary>
/// 霰弹枪准心
/// </summary>
public class ShotGunSight : WeaponSight
{
    public RectTransform[] crossLines;
    public float smoothTime = 0.3f;
    private float yVelocity = 0.0f;
    private float value = 25f;


    public override void ShowSight()
    {
        base.ShowSight();
        float f = Player.player.currentWeapon.GetAccurate;
        crossLines[0].anchoredPosition = new Vector2(-f, 0);
        crossLines[1].anchoredPosition = new Vector2(f, 0);
    }

    public void Update()
    {
        if (isShow)
        {
            value = Mathf.SmoothDamp(value, Player.player.currentWeapon.GetAccurate, ref yVelocity, smoothTime);
            crossLines[0].anchoredPosition = new Vector2(-value, 0);
            crossLines[1].anchoredPosition = new Vector2(value, 0);
        }
    }
}
