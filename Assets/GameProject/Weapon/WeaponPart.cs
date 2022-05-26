using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Data;

public class WeaponPart 
{
    public WeaponPartData data;
    public GameAttribute attribute;
    public WeaponPart(WeaponPartData data)
    {
        this.data = data;
        attribute = AttributeHelper.GetAttributeByType(data)[0];
    }
}
