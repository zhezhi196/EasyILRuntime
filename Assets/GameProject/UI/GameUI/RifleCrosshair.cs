using UnityEngine;

public class RifleCrosshair : WeaponSight
{
    public RectTransform[] crossLines;

    public float smoothTime = 0.3f;
    private float yVelocity = 0.0f;
    private float value = 25f;
    private float grow = 0;

    public override void ShowSight()
    {
        base.ShowSight();
        float f = Player.player.currentWeapon.GetAccurate;
        crossLines[0].anchoredPosition = new Vector2(-f, 0);
        crossLines[1].anchoredPosition = new Vector2(0, f);
        crossLines[2].anchoredPosition = new Vector2(f, 0);
        crossLines[3].anchoredPosition = new Vector2(0, -f);
    }

    public void Update()
    {
        if (isShow && Player.player)
        {
            value = Mathf.SmoothDamp(value, Player.player.currentWeapon.GetAccurate, ref yVelocity, smoothTime);
            crossLines[0].anchoredPosition = new Vector2(-value, 0);
            crossLines[1].anchoredPosition = new Vector2(0, value);
            crossLines[2].anchoredPosition = new Vector2(value, 0);
            crossLines[3].anchoredPosition = new Vector2(0, -value);
        }
    }
}

