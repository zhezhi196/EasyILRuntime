using System;

public class PlayerSensorPoint : AgentSensorPoint
{
    public override bool isSenserable
    {
        get { return Player.player != null && Player.player.IsAlive && gameObject.activeInHierarchy; }
    }
}