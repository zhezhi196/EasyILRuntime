using Module;
using UnityEngine;

public class BodyPart : MonsterPart
{
    public override MonsterPartType partType
    {
        get { return MonsterPartType.Body; }
    }

}