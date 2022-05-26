using UnityEngine;
/// <summary>
/// Player中心点
/// </summary>
public class PlayerCenterPart : PlayerPart
{
    private void Awake()
    {
        partType = PlayerPartType.Player;
        player = transform.GetComponent<Player>();
    }

    public override Vector3 CheckPositon
    {
        get {
            return player.CenterPostion;
        }
    }
}
