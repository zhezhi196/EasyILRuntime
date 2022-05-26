using Module;
using UnityEngine;

public class ArmPart : MonsterPart
{
    public override MonsterPartType partType
    {
        get { return MonsterPartType.Arm; }
    }

}