using System;
using Module;
using UnityEngine;

public class HeadPart : MonsterPart
{

    public override MonsterPartType partType
    {
        get { return MonsterPartType.Head; }
    }

    public float force;

    // Update is called once per frame

}