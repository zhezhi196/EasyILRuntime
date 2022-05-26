using Module;

public class MonsterSensorPoint : AgentSensorPoint
{
    public Monster monster;

    public override bool isSenserable
    {
        get { return monster.isAlive && monster.initStation == InitStation.Normal && gameObject.activeInHierarchy;; }
    }
}