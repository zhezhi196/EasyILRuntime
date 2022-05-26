using Module;
using Sirenix.OdinInspector;
using UnityEngine;

public class WireConnectUIWireEditor : WireConnectUIItemEditor
{
    [TabGroup("电线")]
    public GameObject WireObject;

    public override void OnClickWire()
    {
        base.OnClickWire();
        
        var item = (slotItem as WireLineItem);

        item.SetEnabled(!item.enabled);
        WireObject.OnActive(item.enabled);
    }
}