using Module;
using UnityEngine;

public class LegPart : MonsterPart
{
    public override MonsterPartType partType
    {
        get { return MonsterPartType.Leg; }
    }

}