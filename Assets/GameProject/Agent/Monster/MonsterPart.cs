using UnityEngine;

public enum MonsterPartType
{
    Head,
    Body,
    Arm,
    Leg
}

public class MonsterPart : MonoBehaviour, IHurtObject
{
    public Monster monster;
    public MonsterPartType partType;

    public Damage OnHurt(Damage damage)
    {
        return monster.OnHurt(damage);
    }
}